using DiNet.NodeBuilder.Core.Primitives;

namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface INode
{
    public int Id { get; }

    public InputPort[]? InputPorts { get; }
    public OutputPort[]? OutputPorts { get; }

    public NodeCommand? Command { get; }

    public void SetCommand(NodeCommand command);
}
