namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface IFlowNode : IEnterNode
{
    public IEnterNode? NextNode { get; set; }
}
