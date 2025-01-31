using DiNet.NodeBuilder.Core.Compilation;
using DiNet.NodeBuilder.Core.Reflection;
using DiNet.NodeBuilder.Core;
using System.Numerics;
using DiNet.NodeBuilder.Core.Nodes;
using System.ComponentModel;
using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Nodes.Interfaces;

namespace DiNet.NodeBuilder.Tests;
public static class CompilerTester
{
    public static FlowNode CreateConstToNode(NodeContainer container)
    {
        var powerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");

        var node = container.CreateFlowNode(powerCommand);
        var constant = container.CreateConstantNode(15f);

        if (!container.Link(constant.OutputPorts[0], node.InputPorts[0]))
            throw new Exception();

        return node;
    }

    public static FlowNode CreateConstantModifiedToNode(NodeContainer container)
    {
        var openVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("OpenVector");
        var packVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("PackVector");
        var getPowerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");

        var packNode = container.CreateFlowNode(packVectorCommand);

        var constant = container.CreateConstantNode(new Vector2(12, 13));
        var unpackModifier = container.CreateModifierNode(openVectorCommand);

        var powerX = container.CreateModifierNode(getPowerCommand);
        var powerY = container.CreateModifierNode(getPowerCommand);

        if (!container.Link(constant.OutputPorts[0], unpackModifier.InputPorts[0])) throw new Exception();

        if (!container.Link(unpackModifier.OutputPorts[0], powerX.InputPorts[0])) throw new Exception();
        if (!container.Link(unpackModifier.OutputPorts[1], powerY.InputPorts[0])) throw new Exception();

        if (!container.Link(powerX.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();
        if (!container.Link(powerY.OutputPorts[0], packNode.InputPorts[1])) throw new Exception();

        return packNode;
    }

    public static FlowNode CreateNodeToNode(NodeContainer container)
    {
        var openVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("OpenVector");
        var packVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("PackVector");
        var getPowerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");

        var packNode = container.CreateFlowNode(packVectorCommand);

        var constant = container.CreateConstantNode(new Vector2(12, 13));
        var unpackModifier = container.CreateModifierNode(openVectorCommand);

        var powerX = container.CreateModifierNode(getPowerCommand);
        var powerY = container.CreateModifierNode(getPowerCommand);

        if (!container.Link(constant.OutputPorts[0], unpackModifier.InputPorts[0])) throw new Exception();

        if (!container.Link(unpackModifier.OutputPorts[0], powerX.InputPorts[0])) throw new Exception();
        if (!container.Link(unpackModifier.OutputPorts[1], powerY.InputPorts[0])) throw new Exception();

        //if (!container.Link(powerX.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();
        if (!container.Link(powerY.OutputPorts[0], packNode.InputPorts[1])) throw new Exception();


        var constNode = CreateConstToNode(container);

        if (!container.Link(constNode.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();


        if (!container.LinkNode(constNode, packNode)) throw new Exception();

        return packNode;
    }

    public static FlowNode CreateNodeToNodeSharedConstant(NodeContainer container)
    {
        var openVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("OpenVector");
        var packVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("PackVector");
        var getPowerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");

        var packNode = container.CreateFlowNode(packVectorCommand);

        var constant = container.CreateConstantNode(new Vector2(12, 13));
        var unpackModifier = container.CreateModifierNode(openVectorCommand);

        var powerX = container.CreateModifierNode(getPowerCommand);
        var powerY = container.CreateModifierNode(getPowerCommand);

        if (!container.Link(constant.OutputPorts[0], unpackModifier.InputPorts[0])) throw new Exception();

        //if (!container.Link(unpackModifier.OutputPorts[0], powerX.InputPorts[0])) throw new Exception();
        if (!container.Link(unpackModifier.OutputPorts[1], powerY.InputPorts[0])) throw new Exception();

        //if (!container.Link(powerX.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();
        if (!container.Link(powerY.OutputPorts[0], packNode.InputPorts[1])) throw new Exception();


        var constNode = container.CreateFlowNode(getPowerCommand);

        if (!container.Link(unpackModifier.OutputPorts[0], constNode.InputPorts[0])) throw new Exception();

        if (!container.Link(constNode.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();

        if (!container.LinkNode(constNode, packNode)) throw new Exception();

        return packNode;
    }


    public static INode CreateConstSeq(NodeContainer container)
    {
        var powerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");

        var node = container.CreateModifierNode(powerCommand);
        var constant = container.CreateConstantNode(15f);

        if (!container.Link(constant.OutputPorts[0], node.InputPorts[0]))
            throw new Exception();

        return node;
    }
    public static void ConstantSeq(NodeContainer container)
    {
        var node = CreateConstSeq(container);

        var compiler = new NodeCompiler();
        var lambda = compiler.CompileNode(node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }


    public static void ConstantToNode(NodeContainer container)
    {
        var node = CreateConstToNode(container);

        var compiler = new NodeCompiler();
        var lambda = compiler.CompileNode(node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }

    public static void ConstantModifiedToNode(NodeContainer container)
    {
        var node = CreateConstantModifiedToNode(container);

        var compiler = new NodeCompiler();
        var lambda = compiler.CompileNode(node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }

    public static void NodeToNode(NodeContainer container)
    {
        var node = CreateNodeToNode(container);

        var compiler = new NodeCompiler();
        var lambda = compiler.CompileNode(node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }


}
