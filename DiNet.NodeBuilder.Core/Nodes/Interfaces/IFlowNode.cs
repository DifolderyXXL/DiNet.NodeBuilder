namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface IFlowNode : IEnterNode
{
    public IEnterNode? NextNode { get; set; }
}

public interface IBranchNode : IEnterNode
{
    public IEnterNode?[]? NextNodes { get; set; }

    public Func<object, int> NodeSelectorFunc { get; }

    public void SetSelectionFunc(Func<object, int> func);
}