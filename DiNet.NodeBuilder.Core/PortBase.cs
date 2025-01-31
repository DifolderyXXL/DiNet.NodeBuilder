using DiNet.NodeBuilder.Core.Nodes;

namespace DiNet.NodeBuilder.Core;

public abstract class PortBase
{
    public int Id { get; }
    public Node Parent { get; }
    public Type ValueType { get; }
    public PortBase? ConnectedPort { get; internal set; }

    protected PortBase(int id, Node parent, Type type)
    {
        Id = id;
        Parent = parent;
        ValueType = type;
    }
}
