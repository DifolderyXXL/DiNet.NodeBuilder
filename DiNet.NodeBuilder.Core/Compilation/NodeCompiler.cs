using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Helpers;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DiNet.NodeBuilder.Core.Compilation;
public class NodeCompiler
{
    private Dictionary<int, CachedNodeCompile> _nodeCache = [];

    private readonly LabelTarget _returnLabel = Expression.Label(typeof(ValueGroup));

    public Func<ValueGroup> BetterCompileNode(IEnterNode node)
    {
        //getting root
        var startNode = node;
        while (startNode.PreviousNode is not null)
            startNode = startNode.PreviousNode;

        var cache = BuildBlock(startNode);

        cache.body.Add(Expression.Label(_returnLabel, ValueGroupHelper.GetVoidValueGrouper()));
        var block = Expression.Block(cache.variables, cache.body);

        var lambda = Expression.Lambda<Func<ValueGroup>>(block, []);
        return lambda.Compile();
    }

    public Func<ValueGroup> BetterCompileNode(INode node)
    {
        var cache = Cache(node);

        var block = Expression.Block(cache.Item2.variables, cache.Item2.body);

        var lambda = Expression.Lambda<Func<ValueGroup>>(block, []);
        return lambda.Compile();
    }

    public BlockCache BuildBlock(INode root)
    {
        if (root is IBranchNode branchNode)
            return BuildBlock(branchNode);
        else if (root is IFlowNode flowNode)
            return BuildBlock(flowNode);
        else if (root is IReturnNode returnNode)
            return BuildBlock(returnNode);

        else return new([], []);
    }

    public BlockCache BuildBlock(IReturnNode root)
    {
        var cur = Cache(root);

        var input = GetNodeInput(root);

        cur.Item2.variables.AddRange(input.Item2.variables);
        cur.Item2.body.AddRange(input.Item2.body);

        cur.Item2.body.Add(Expression.Return(_returnLabel, input.Item1));

        return cur.Item2;
    }

    public BlockCache BuildBlock(IFlowNode root)
    {
        var cur = Cache(root);
        if (root.NextNode is not null)
        {
            var block = BuildBlock(root.NextNode);
            cur.Item2.variables.AddRange(block.variables);
            cur.Item2.body.AddRange(block.body);
        }

        return cur.Item2;
    }

    public BlockCache BuildBlock(IBranchNode root)
    {
        var input = GetNodeInput(root);

        if (root.NextNodes is null)
            throw new NullReferenceException();

        var selectedBranch = Expression.Invoke(Expression.Constant(root.NodeSelectorFunc), input.Item1);

        var selectedBranchVar = Expression.Variable(typeof(int), "SelectedBranch");


        var cases = new List<Expression>();
        cases.Add(Expression.Assign(selectedBranchVar, selectedBranch));


        for (int i = 0; i < root.NextNodes.Length; i++)
        {
            if (root.NextNodes[i] is not null)
            {
                var block = BuildBlock(root.NextNodes[i]!);
                cases.Add(
                    Expression.IfThen(
                        Expression.Equal(Expression.Constant(i), selectedBranchVar),
                        Expression.Block(block.variables, block.body)));
            }
        }

        var flowBlock = BuildBlock(root as IFlowNode);

        if (cases.Count != 0)
        {
            input.Item2.body.Add(Expression.Block([selectedBranchVar], cases));

            input.Item2.body.AddRange(flowBlock.body);
            input.Item2.variables.AddRange(flowBlock.variables);
            return input.Item2;
        }

        return flowBlock;
    }

    public (CachedNodeCompile, BlockCache) Cache(INode node)
    {
        if (!_nodeCache.ContainsKey(node.Id))
        {
            var cache = GenerateExpression(node);

            //before all is generated
            return (_nodeCache[node.Id], cache);
        }

        return (_nodeCache[node.Id], new([], []));
    }

    public (Expression, BlockCache) GetNodeInput(INode node)
    {
        if (node.Command is null)
            throw new NullReferenceException($"node.Command is null");
        if (node.InputPorts is null)
            throw new NullReferenceException($"node.InputPorts is null");


        var blockCache = new BlockCache([], []);

        var nodeInputs = new Expression[node.Command.InputTypes.Length];
        foreach (var input in node.InputPorts)
        {
            if (input.ConnectedPort is null)
            {
                nodeInputs[input.Id] = Expression.Default(input.ValueType);
                continue;
            }

            var connectedNode = input.ConnectedPort.Parent;
            var cache = Cache(connectedNode);
            var resVar = cache.Item1.resultVariable;

            blockCache.variables.AddRange(cache.Item2.variables);
            blockCache.body.AddRange(cache.Item2.body);

            var outputIndex = input.ConnectedPort.Id;

            var valueBox = ValueGroupHelper.GetValueBoxByIndex(resVar, outputIndex);
            nodeInputs[input.Id] = Expression.Convert(
                Expression.Field(valueBox, AccessHelper.ValueBoxObj.Value),
                input.ValueType
                );
        }

        return (ValueGroupHelper.GetValueGrouper(nodeInputs, node.Command.InputTypes), blockCache);
    }

    public BlockCache GenerateExpression(INode node)
    {
        var returnVar = Expression.Variable(typeof(ValueGroup), $"var{node.Id}");
        var nodeCommand = node.Command;


        var inp = GetNodeInput(node);

        if (nodeCommand?.Func is not null)
        {
            var assignExpr = Expression.Assign(
                returnVar,
                Expression.Invoke(
                    Expression.Constant(nodeCommand.Func),
                    inp.Item1));

            inp.Item2.body.Add(assignExpr);
        }
        else
            Debug.WriteLine($"Null func in {node}");

        inp.Item2.variables.Add(returnVar);


        _nodeCache.Add(node.Id, new(returnVar));
        return inp.Item2;
    }
}

public record CachedNodeCompile(ParameterExpression resultVariable);


public record BlockCache(List<ParameterExpression> variables, List<Expression> body);