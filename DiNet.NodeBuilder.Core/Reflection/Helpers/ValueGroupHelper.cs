using DiNet.NodeBuilder.Core.Primitives;
using System.Linq.Expressions;

namespace DiNet.NodeBuilder.Core.Reflection.Helpers;

public static class ValueGroupHelper
{
    public static Expression GetVoidValueGrouper()
    { 
        return Expression.New(
            AccessHelper.ValueGroupEmptyConstructor.Value);
    }

    public static Expression GetValueGrouper(Expression[] values, Type[] types)
    {
        if (values.Length == 0)
            return GetVoidValueGrouper();

        var body = new Expression[values.Length];

        for (int i = 0; i < values.Length; i++)
            body[i] = GetValueBox(Expression.Convert(values[i], typeof(object)), types[i]);

        return Expression.New(
            AccessHelper.ValueGroupConstructor.Value,
            Expression.NewArrayInit(typeof(ValueBox), body));
    }

    public static Expression GetValueBox(Expression value, Type type)
    {
        return Expression.New(
            AccessHelper.ValueBoxConstructor.Value,
            value,
            Expression.Constant(type));
    }

    public static Expression GetValueBoxByIndex(Expression valueGroup, int index)
    {
        var prop = Expression.Property(valueGroup, AccessHelper.GroupProperty.Value);
        return Expression.ArrayIndex(prop, Expression.Constant(index));
    }
}

