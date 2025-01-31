using DiNet.NodeBuilder.Core.Exceptions;

namespace DiNet.NodeBuilder.Core.Primitives;
public class ValueGroup
{
    public ValueBox[] Group { get; }
    public int Count => Group.Length;

    public ValueGroup(params ValueBox[] group)
    {
        Group = group;
    }

    public ValueGroup(int count)
    {
        Group = new ValueBox[count];
    }

    public ValueGroup()
    {
        Group = [];
    }
}

public struct ValueBox
{
    public object? obj;
    public Type? type;
    public ValueBox(object? obj, Type type)
    {
        this.obj = obj;
        this.type = type;
    }

    public static ValueBox Create<T>(T value)
    {
        return new(value, typeof(T));
    }

    public T As<T>()
    {
        if (!type.IsAssignableTo(typeof(T))) 
            throw new TypeMismatchException();
        return (T)obj!;
    }
}