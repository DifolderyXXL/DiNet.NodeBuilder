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

    public static IEnterNode CreateConstantModifiedToNode(NodeContainer container)
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

    public static IEnterNode CreateNodeToNode(NodeContainer container)
    {
        var openVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("OpenVector");
        var packVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("PackVector");
        var getPowerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");

        var packNode = container.CreateFlowNode(packVectorCommand);

        var constant = container.CreateConstantNode(new Vector2(12, 13));
        var unpackModifier = container.CreateModifierNode(openVectorCommand);

        //var powerX = container.CreateModifierNode(getPowerCommand);
        var powerY = container.CreateModifierNode(getPowerCommand);

        if (!container.Link(constant.OutputPorts[0], unpackModifier.InputPorts[0])) throw new Exception();

        //if (!container.Link(unpackModifier.OutputPorts[0], powerX.InputPorts[0])) throw new Exception();
        if (!container.Link(unpackModifier.OutputPorts[1], powerY.InputPorts[0])) throw new Exception();

        //if (!container.Link(powerX.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();
        if (!container.Link(powerY.OutputPorts[0], packNode.InputPorts[1])) throw new Exception();


        var constNode = CreateConstToNode(container);

        if (!container.Link(constNode.OutputPorts[0], packNode.InputPorts[0])) throw new Exception();


        if (!container.LinkNode(constNode, packNode)) throw new Exception();


        var returnNode = container.CreateReturnNode(typeof(Vector2));

        if (!container.Link(packNode.OutputPorts[0], returnNode.InputPorts[0])) throw new Exception();
        if (!container.LinkNode(packNode, returnNode)) throw new Exception();

        return packNode;
    }

    public static IEnterNode CreateNodeToNodeSharedConstant(NodeContainer container)
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


    public static IEnterNode CreateBranches(NodeContainer container)
    {
        var openVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("OpenVector");
        var packVectorCommand = NodeCommandContext.Shared.GetFuncDataByName("PackVector");
        var getPowerCommand = NodeCommandContext.Shared.GetFuncDataByName("GetPower");
        var isPositiveCommand = NodeCommandContext.Shared.GetFuncDataByName("IsPositive");


        var constNode = container.CreateConstantNode(new Vector2(1, 2));
        var openVec = container.CreateModifierNode(openVectorCommand);

        container.Link(constNode.OutputPorts[0], openVec.InputPorts[0]);

        var posCheckMod = container.CreateModifierNode(isPositiveCommand);

        container.Link(openVec.OutputPorts[0], posCheckMod.InputPorts[0]);

        var ifNode = container.CreateIfElseNode();

        container.Link(posCheckMod.OutputPorts[0], ifNode.InputPorts[0]);

        var powerTrue = container.CreateFlowNode(getPowerCommand);
        var powerFalse = container.CreateFlowNode(getPowerCommand);

        container.LinkNode(ifNode, 0, powerTrue);
        container.LinkNode(ifNode, 1, powerFalse);

        container.Link(openVec.OutputPorts[1], powerFalse.InputPorts[0]);

        var returnP1 = container.CreateReturnNode(typeof(float));
        var returnP2 = container.CreateReturnNode(typeof(float));
        var return0 = container.CreateReturnNode(typeof(float));

        container.Link(powerTrue.OutputPorts[0], returnP1.InputPorts[0]);
        container.Link(powerFalse.OutputPorts[0], returnP2.InputPorts[0]);

        container.LinkNode(powerTrue, returnP1);
        container.LinkNode(powerFalse, returnP2);

        container.LinkNode(ifNode, return0);


        return ifNode;
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
        var lambda = compiler.BetterCompileNode(node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }


    public static void ConstantToNode(NodeContainer container)
    {
        var node = CreateConstToNode(container) as IFlowNode;



        var compiler = new NextGenCompilation();
        var lambda = compiler.Build(container, node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }

    public static void ConstantModifiedToNode(NodeContainer container)
    {
        var node = CreateConstantModifiedToNode(container);

        var compiler = new NextGenCompilation();
        var lambda = compiler.Build(container, node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }

    public static void NodeToNode(NodeContainer container)
    {
        var node = CreateNodeToNode(container);

        while (node.PreviousNode != null)
            node = node.PreviousNode;

        var compiler = new NextGenCompilation();
        var lambda = compiler.Build(container, node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }

    public static void Branches(NodeContainer container)
    {
        var node = CreateBranches(container);

        var compiler = new NextGenCompilation();
        var lambda = compiler.Build(container, node);

        var output = lambda.Invoke();

        Console.WriteLine(output.Group[0].obj);
    }
}
