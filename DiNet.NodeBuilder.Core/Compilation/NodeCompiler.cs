using DiNet.NodeBuilder.Core.Nodes;
using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace DiNet.NodeBuilder.Core.Compilation;
public class NodeCompiler
{
    private Dictionary<int, CachedNodeCompile> _nodeCache = [];

    private List<int> _buildSequence = [];


    public BlockCache BuildBlock(INode root)
    {
        if (root is IFlowNode flowNode)
            return BuildBlock(flowNode);
        else if (root is IBranchNode branchNode)
            return BuildBlock(branchNode);
        else return new([], []);
    }

    public BlockCache BuildBlock(IFlowNode root)
    {
        var cache = CallForCacheNode(root);

        var cur = new BlockCache([], [cache.assignation]);
        if (root.NextNode is not null)
        {
            var block = BuildBlock(root.NextNode);
            cur.variables.AddRange(block.variables);
            cur.body.AddRange(block.body);
        }
       
        return cur;
    }

    public BlockCache BuildBlock(IBranchNode root)
    {
        var input = GetNodeInput(root);

        if (root.NextNodes is null)
            throw new NullReferenceException();

        var selectedBranch = Expression.Invoke(Expression.Constant(root.NodeSelectorFunc), input);

        var cases = new List<SwitchCase>();
        for (int i = 0; i < root.NextNodes.Length; i++)
        {
            if (root.NextNodes[i] is not null)
            {
                var block = BuildBlock(root.NextNodes[i]!);
                cases.Add(
                    Expression.SwitchCase(
                        Expression.Block(block.variables, block.body),
                        Expression.Constant(i)));
            }
        }

        if (cases.Count == 0)
            return new([], [Expression.Empty()]);
        else
            return new([], [Expression.Switch(selectedBranch, cases.ToArray())]);
    }


    public Func<ValueGroup> CompileNode(IFlowNode node)
    {
        //getting root
        var startNode = node;
        while (startNode.PreviousNode is not null)
            startNode = startNode.PreviousNode;

        //compiling
        CallForCacheNode(startNode);
        var lastNode = startNode;
        while(lastNode.NextNode is not null)
        {
            var next = lastNode.NextNode;

            if (next is IFlowNode flowNode)
                lastNode = flowNode;
            else if(next is IBranchNode branchNode)


            CallForCacheNode(lastNode);
        }
        
        var seq = _buildSequence.Select(x => _nodeCache[x]);
        var block = Expression.Block(seq.Select(x => x.resultVariable), seq.Select(x => x.assignation));

        var lambda = Expression.Lambda<Func<ValueGroup>>(block, []);
        return lambda.Compile();
    }

    public Func<ValueGroup> CompileNode(INode node)
    {
        CallForCacheNode(node);
        var seq = _buildSequence.Select(x => _nodeCache[x]);
        var block = Expression.Block(seq.Select(x => x.resultVariable), seq.Select(x => x.assignation));

        var lambda = Expression.Lambda<Func<ValueGroup>>(block, []);
        return lambda.Compile();
    }

    public CachedNodeCompile CallForCacheNode(INode node)
    {
        if (!_nodeCache.ContainsKey(node.Id))
        {
            GenerateExpression(node);

            //before all is generated

            _buildSequence.Add(node.Id);
        }

        return _nodeCache[node.Id];
    }

    public Expression GetNodeInput(INode node)
    {
        if (node.Command is null)
            throw new NullReferenceException($"node.Command is null");
        if (node.InputPorts is null)
            throw new NullReferenceException($"node.InputPorts is null");

        var nodeInputs = new Expression[node.Command.InputTypes.Length];
        foreach (var input in node.InputPorts)
        {
            if(input.ConnectedPort is null)
            {
                nodeInputs[input.Id] = Expression.Default(input.ValueType);
                continue;
            }

            var connectedNode = input.ConnectedPort.Parent;
            var resVar = CallForCacheNode(connectedNode).resultVariable;

            var outputIndex = input.ConnectedPort.Id;

            var valueBox = ValueGroupHelper.GetValueBoxByIndex(resVar, outputIndex);
            nodeInputs[input.Id] = Expression.Convert(
                Expression.Field(valueBox, AccessHelper.ValueBoxObj.Value),
                input.ValueType
                );
        }

        return ValueGroupHelper.GetValueGrouper(nodeInputs, node.Command.InputTypes);
    }

    public void GenerateExpression(INode node)
    {
        var returnVar = Expression.Variable(typeof(ValueGroup), $"var{node.Id}");
        var nodeCommand = node.Command;

        var assignExpr = Expression.Assign(
            returnVar, 
            Expression.Invoke(
                Expression.Constant(nodeCommand.Func), 
                GetNodeInput(node)));

        _nodeCache.Add(node.Id, new(returnVar, assignExpr));
    }
}

public record CachedNodeCompile(ParameterExpression resultVariable, Expression assignation);


public record BlockCache(List<ParameterExpression> variables, List<Expression> body);