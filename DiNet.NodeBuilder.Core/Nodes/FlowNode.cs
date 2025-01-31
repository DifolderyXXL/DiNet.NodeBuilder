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

public class ReturnNode : Node, IReturnNode
{
    public ReturnNode(NodeContainer container, int id) : base(container, id)
    {
        
    }

    public IFlowNode? PreviousNode { get; set; }

    public void SetReturnTypes(Type[] types)
    {
        InputPorts = Enumerable.Range(0, types.Length).Select(x => new InputPort(x, this, types[x])).ToArray();
        Command = new(x=>x, types, []);
    }
}