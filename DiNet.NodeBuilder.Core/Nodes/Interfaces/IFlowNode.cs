using DiNet.NodeBuilder.Core.Primitives;

namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface IFlowNode : IEnterNode
{
    public IEnterNode? NextNode { get; set; }
}

public interface IBranchNode : IFlowNode
{
    public IEnterNode?[]? NextNodes { get; set; }

    public Func<ValueGroup, int> NodeSelectorFunc { get; }

    public void SetSelectionFunc(Func<object, int> func);
}