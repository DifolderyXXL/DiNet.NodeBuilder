using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Helpers;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DiNet.NodeBuilder.Core.Reflection.Generators;
public class NodeCommandGenerator
{
    public static readonly NodeCommandGenerator Shared = new();

    private List<IGrouperGenerator> _generators = 
        [ 
        new TupleGrouperGenerator(),
        new ObjectGrouperGenerator(),
        ];

    public static Expression LambdaCall<T>(T lambda, Expression[]? parameters)
        => Expression.Invoke(Expression.Constant(lambda, typeof(T)), parameters);
    
    public Func<ValueGroup, ValueGroup> GenerateLambda(object instance, Type instType, MethodInfo method)
    {
        var inputParam = Expression.Parameter(typeof(ValueGroup));
        var parametersLocal = GetFuncParameters(inputParam, method);

        var inst = Expression.Constant(instance, instType);
        var result = Expression.Call(inst, method, parametersLocal);

        var body = new List<Expression>();

        var grouperLambda = _generators.First(x => method.ReturnType.IsAssignableTo(x.BasicType))
            .GetOrGenerateGrouper(method.ReturnType);

        if(method.ReturnType.IsEquivalentTo(typeof(void)))
            body.AddRange(result, ValueGroupHelper.GetVoidValueGrouper());
        else
            body.Add(Expression.Convert(
                LambdaCall(
                    grouperLambda,
                    [Expression.Convert(result, typeof(object))]), typeof(ValueGroup)));
        
        var lambda = Expression.Lambda<Func<ValueGroup, ValueGroup>>(
            Expression.Block(body), [inputParam]);

        return lambda.Compile();
    }

    public static Type[] GetInputTypes(MethodInfo method)
    {
        return method.GetParameters().Select(x => x.ParameterType).ToArray();
    }
    public static Type[] GetOutputTypes(MethodInfo method)
    {
        if (method.ReturnType.IsAssignableTo(typeof(ITuple)))
            return method.ReturnType.GenericTypeArguments;

        return [method.ReturnType];
    }

    private static Expression[] GetFuncParameters(ParameterExpression inputParam, MethodInfo method)
    {
        var groupProp = Expression.Property(inputParam, AccessHelper.GroupProperty.Value);

        var parameters = method.GetParameters();

        var parametersLocal = new Expression[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var vb = Expression.ArrayIndex(groupProp, Expression.Constant(i));

            parametersLocal[i] = Expression.Convert(
                Expression.Field(vb, AccessHelper.ValueBoxObj.Value),
                parameters[i].ParameterType);
        }

        return parametersLocal;
    }
}
