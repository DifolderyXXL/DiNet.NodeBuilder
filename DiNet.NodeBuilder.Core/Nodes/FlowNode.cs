using DiNet.NodeBuilder.Core.Nodes.Interfaces;

namespace DiNet.NodeBuilder.Core.Nodes;

public class FlowNode : Node, IFlowNode
{
    public FlowNode(NodeContainer container, int id) : base(container, id)
    {
    }

    public IEnterNode? NextNode { get; set; }
    public IFlowNode? PreviousNode { get; set; }
}
