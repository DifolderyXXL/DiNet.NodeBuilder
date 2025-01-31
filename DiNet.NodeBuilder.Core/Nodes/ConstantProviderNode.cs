using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Generators;

namespace DiNet.NodeBuilder.Core.Nodes;

public class ConstantProviderNode : Node, IConstantProviderNode
{
    public ConstantProviderNode(NodeContainer container, int id) : base(container, id)
    {
    }

    public object? Value { get; private set; }
    public Type? Type { get; private set; }

    public void SetValue(object? value, Type type)
    {
        Value = value;
        Type = type;

        SetCommand(new(
            x=>new ValueGroup(new ValueBox(value, type)),
            [],
            [type]
            ));
    }
}

public sealed class ModifierNode : Node, IModifierNode
{
    public ModifierNode(NodeContainer container, int id) : base(container, id)
    {
    }
}