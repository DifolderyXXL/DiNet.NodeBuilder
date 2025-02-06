using DiNet.NodeBuilder.Core;
using DiNet.NodeBuilder.Core.Attributes;
using DiNet.NodeBuilder.Core.Compilation;
using DiNet.NodeBuilder.Core.Nodes;
using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection;
using DiNet.NodeBuilder.Core.Reflection.Generators;
using DiNet.NodeBuilder.Tests;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;





NodeCommandContext.Shared.LoadNodeCommands();


CompilerTester.Branches(new());


ValueGroup Run(Node node, ValueGroup input)
{
    var result = node.Command.Func(input);

    var next = new ValueGroup(node.InputPorts.Length);
    foreach(var output in node.OutputPorts)
    {
        if(output.ConnectedPort != null)
            next.Group[output.ConnectedPort.Id] = result.Group[output.Id];
    }

    return node.OutputPorts.First(x=>x.ConnectedPort != null).ConnectedPort.Parent.Command.Func(next);
}
