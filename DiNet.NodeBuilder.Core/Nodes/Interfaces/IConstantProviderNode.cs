﻿namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface IConstantProviderNode : IModifierNode
{
    public object? Value { get; }
    public Type? Type { get; }

    public void SetValue(object? value, Type type);
}
