namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface IEnterNode : INode
{
    public IFlowNode? PreviousNode { get; set; }
}
