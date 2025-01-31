using DiNet.NodeBuilder.Core.Nodes;
using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;

namespace DiNet.NodeBuilder.Core;
public class NodeContainer
{
    public List<Node> Nodes { get; } = [];

    private int _index = 0;

    private int ProvideNewIndex()
        => _index++;

    public T CreateEmptyNode<T>()
        where T : Node
    {
        var node = (Activator.CreateInstance(typeof(T), this, ProvideNewIndex()) as T)!;
        Nodes.Add(node);
        return node;
    }

    public Node CreateSpecificNode(NodeCommand command)
    {
        var node = CreateEmptyNode<Node>();
        node.SetCommand(command);
        return node;
    }


    public Node CreateConstantNode<T>(T value)
    {
        var node = CreateEmptyNode<ConstantProviderNode>();
        node.SetValue(value, typeof(T));
        return node;
    }

    public FlowNode CreateFlowNode(NodeCommand command)
    {
        var node = CreateEmptyNode<FlowNode>();
        node.SetCommand(command);
        return node;
    }
    
    public ModifierNode CreateModifierNode(NodeCommand command)
    {
        var node = CreateEmptyNode<ModifierNode>();
        node.SetCommand(command);
        return node;
    }

    public IfElseNode CreateIfElseNode()
    {
        var node = CreateEmptyNode<IfElseNode>();
        return node;
    }

    public ReturnNode CreateReturnNode(params Type[]? types)
    {
        var node = CreateEmptyNode<ReturnNode>();
        node.SetReturnTypes(types ?? []);
        return node;
    }

    public void RemoveNode(Node node)
    {
        UnlinkOutput(node);
        UnlinkInput(node);

        Nodes.Remove(node);
    }

    public void UnlinkOutput(Node node)
    {
        if (node.InputPorts != null)
            foreach (var input in node.InputPorts)
                if (input.ConnectedPort != null)
                    Unlink((input.ConnectedPort as OutputPort)!, input);
    }

    public void UnlinkInput(Node node)
    {
        if (node.OutputPorts != null)
            foreach (var output in node.OutputPorts)
                if (output.ConnectedPort != null)
                    Unlink(output, (output.ConnectedPort as InputPort)!);

    }

    public bool LinkNode(IBranchNode from, int index, IEnterNode to)
    {
        if (from.NextNodes[index] is not null) return false;
        if (to.PreviousNode is not null) return false;

        from.NextNodes[index] = to;
        to.PreviousNode = from;
        return true;
    }

    public bool LinkNode(FlowNode from, IEnterNode to)
    {
        if (from.NextNode is not null) return false;
        if (to.PreviousNode is not null) return false;

        from.NextNode = to;
        to.PreviousNode = from;
        return true;
    }

    public bool UnlinkNode(FlowNode from, IEnterNode to)
    {
        if (from.NextNode is null) return false;
        if (to.PreviousNode is null) return false;
        if (to.PreviousNode.Id != from.NextNode.Id) return false;

        from.NextNode = null;
        to.PreviousNode = null;
        return true;
    }



    public bool Link(OutputPort output, InputPort input)
    {
        if (input.Parent.Id == output.Parent.Id) return false;
        if (input.ValueType != output.ValueType) return false;
        if (input.ConnectedPort != null) return false;

        input.ConnectedPort = output;
        output.ConnectedPort = input;

        return true;
    }

    public bool Unlink(OutputPort output, InputPort input)
    {
        if (input.ConnectedPort is null) return false;
        if (output.ConnectedPort is null) return false;

        if (output.ConnectedPort.Parent.Id != input.Parent.Id) return false;
        if (output.ConnectedPort.Id != input.Id) return false;

        input.ConnectedPort = null;
        output.ConnectedPort = null;

        return true;
    }

    public bool MultiLink(OutputPort[] outputPorts, InputPort[] inputPorts)
    {
        bool verdict = true;
        for (int i = 0; i < outputPorts.Length; i++)
            verdict &= Link(outputPorts[i], inputPorts[i]);
        return verdict;
    }

    public bool MultiLink((OutputPort, InputPort)[] outputPorts)
    {
        bool verdict = true;
        for (int i = 0; i < outputPorts.Length; i++)
            verdict &= Link(outputPorts[i].Item1, outputPorts[i].Item2);
        return verdict;
    }
}