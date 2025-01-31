using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;

namespace DiNet.NodeBuilder.Core.Nodes;
public class Node : INode
{
    public int Id { get; }

    public InputPort[]? InputPorts { get; protected set; }
    public OutputPort[]? OutputPorts { get; protected set; }

    public NodeCommand? Command { get; protected set; }

    private readonly NodeContainer _container;

    public Node(NodeContainer container, int id)
    {
        _container = container;

        Id = id;
    }

    public void SetCommand(NodeCommand command)
    {
        Command = command;

        InputPorts = NodeHelper.GenerateInputPorts(this, command.InputTypes, InputPorts,
            x => _container.Unlink((x.ConnectedPort as OutputPort)!, x));
        OutputPorts = NodeHelper.GenerateOutputPorts(this, command.OutputTypes, OutputPorts,
            x => _container.Unlink(x, (x.ConnectedPort as InputPort)!));
    }
}

public class IfElseNode : FlowNode, IBranchNode
{
    public IfElseNode(NodeContainer container, int id) : base(container, id)
    {
        InputPorts = [new(0, this, typeof(bool))];
        Command = new(null, [typeof(bool)], []);
    }

    public IEnterNode?[]? NextNodes { get; set; } = new IEnterNode[2];

    public Func<ValueGroup, int> NodeSelectorFunc => (xg) => 
    {
        var x = xg.Group[0].obj;
        Console.WriteLine($"Select IF {x}");

        if (x is bool b)
            return b ? 0 : 1;
        if (x is int i)
            return i > 0 ? 0 : 1;
        if (x is long l)
            return l > 0 ? 0 : 1;
        return -1;
    };

    public void SetSelectionFunc(Func<object, int> func)
    {
        
    }
}

public static class NodeHelper
{
    public static InputPort[] GenerateInputPorts(
        Node node, Type[] types, InputPort[]? previous, Action<InputPort>? dispose)
    {
        return Enumerable
            .Range(0, types.Length)
            .Select(x =>
            {
                if (previous is not null && x < previous.Length)
                    if (types[x].IsEquivalentTo(previous[x].ValueType))
                        return previous[x];
                    else
                        dispose?.Invoke(previous[x]);

                return new InputPort(x, node, types[x]);
            }).ToArray();
    }

    public static OutputPort[] GenerateOutputPorts(
        Node node, Type[] types, OutputPort[]? previous, Action<OutputPort>? dispose)
    {
        return Enumerable
            .Range(0, types.Length)
            .Select(x =>
            {
                if (previous is not null && x < previous.Length)
                    if (types[x].IsEquivalentTo(previous[x].ValueType))
                        return previous[x];
                    else
                        dispose?.Invoke(previous[x]);

                return new OutputPort(x, node, types[x]);
            }).ToArray();
    }
}