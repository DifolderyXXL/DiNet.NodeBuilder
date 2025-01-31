namespace DiNet.NodeBuilder.Core.Primitives;
public class NodeCommand
{
    public Func<ValueGroup, ValueGroup> Func { get; }

    public Type[] InputTypes { get; }
    public Type[] OutputTypes { get; }

    public NodeCommand(Func<ValueGroup, ValueGroup> func, 
        Type[] inputTypes, 
        Type[] outputTypes)
    {
        Func = func;
        InputTypes = inputTypes;
        OutputTypes = outputTypes;
    }
}
