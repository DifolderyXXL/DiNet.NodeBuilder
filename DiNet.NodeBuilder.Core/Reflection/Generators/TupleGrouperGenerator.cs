using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Helpers;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DiNet.NodeBuilder.Core.Reflection.Generators;

public delegate ValueGroup GrouperDelegate(object value);
public interface IGrouperGenerator
{
    public Type BasicType { get; }
    public GrouperDelegate GetOrGenerateGrouper(Type type);
}

public class TupleGrouperGenerator : IGrouperGenerator
{
    public Type BasicType => typeof(ITuple);

    private Dictionary<Type, GrouperDelegate> _valueGroupCache = [];

    public GrouperDelegate GetOrGenerateGrouper(Type type)
    {
        if(_valueGroupCache.ContainsKey(type)) 
            return _valueGroupCache[type];

        if (!type.IsAssignableTo(BasicType))
            throw new ArgumentException();

        var instParam = Expression.Parameter(typeof(object));
        var tupleParam = Expression.Convert(instParam, BasicType);

        var types = type.GenericTypeArguments;

        var resultExpr = new Expression[types.Length];
        for (int i = 0; i < types.Length; i++)
            resultExpr[i] =
                Expression.Call(
                    AccessHelper.HelperTupleIndexAccess.Value,
                    tupleParam,
                    Expression.Constant(i));

        var lambda = Expression.Lambda<GrouperDelegate>(
            Expression.Block(ValueGroupHelper.GetValueGrouper(resultExpr, types)),
            [instParam]);

        _valueGroupCache.Add(type, lambda.Compile());

        return _valueGroupCache[type];
    }
}

public class ObjectGrouperGenerator : IGrouperGenerator
{
    public Type BasicType => typeof(object);

    private Dictionary<Type, GrouperDelegate> _valueGroupCache = [];

    public GrouperDelegate GetOrGenerateGrouper(Type type)
    {
        if (_valueGroupCache.ContainsKey(type))
            return _valueGroupCache[type];

        if (!type.IsAssignableTo(BasicType))
            throw new ArgumentException();

        var instParam = Expression.Parameter(typeof(object));

        var lambda = Expression.Lambda<GrouperDelegate>(
            Expression.Block(ValueGroupHelper.GetValueGrouper([instParam], [type])),
            [instParam]);

        _valueGroupCache.Add(type, lambda.Compile());

        return _valueGroupCache[type];
    }
}