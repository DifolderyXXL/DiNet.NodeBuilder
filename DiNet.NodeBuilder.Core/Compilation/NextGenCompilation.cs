using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Helpers;
using System.Linq.Expressions;

namespace DiNet.NodeBuilder.Core.Compilation;
public class NextGenCompilation
{
    Dictionary<int, ParameterExpression> _variables = [];
    private HashSet<int> _visited = [];
    private HashSet<int> _outputCached = [];

    private readonly LabelTarget _returnLabel = Expression.Label(typeof(ValueGroup));

    public Func<ValueGroup> Build(NodeContainer container, INode start)
    {
        foreach (var node in container.Nodes)
            _variables.Add(node.Id, Expression.Variable(typeof(ValueGroup), $"var{node.Id}"));

        var body = GetNodeBody(start);

        var nodes = container.Nodes
            .Where(x => !_visited.Contains(x.Id))
            .Where(x => x.InputPorts is null || x.InputPorts.Length == 0);

        var modifierExpressions = nodes.Select(x=>Expression.Block(GetModifierBody(x).Reverse()));

        var resBody = modifierExpressions.Concat(
            [
                body,
                Expression.Label(_returnLabel, ValueGroupHelper.GetVoidValueGrouper())
            ]);
        var lambda = Expression.Lambda<Func<ValueGroup>>(
            Expression.Block(_variables.Values, resBody));

        return lambda.Compile();
    }

    public Expression GetNodeBody(INode node)
    {
        var body = new List<Expression>();

        body.AddRange(GetModifierBody(node).Reverse());

        if (node is IBranchNode branch)
            body.AddRange(GetBranchBody(branch));
        else if (node is IFlowNode flow)
            body.AddRange(GetFlowBody(flow));
        else if (node is IReturnNode returnNode)
            body.AddRange(GetReturnBody(returnNode));

        return Expression.Block(body);
    }

    public IEnumerable<Expression> GetReturnBody(IReturnNode node)
    {
        yield return Expression.Return(_returnLabel, GetGroupInput(node.InputPorts));
    }

    public IEnumerable<Expression> GetModifierBody(INode node)
    {
        if (!_visited.Contains(node.Id))
        {
            _visited.Add(node.Id);
            
            if (node.OutputPorts is not null)
                foreach (var output in node.OutputPorts)
                {
                    if (output.ConnectedPort?.Parent is IModifierNode modifier)
                        foreach (var expression in GetModifierBody(modifier))
                            yield return expression;
                }

            if (node is IModifierNode && node.Command?.Func is not null && !_outputCached.Contains(node.Id))
            {
                _outputCached.Add(node.Id);
                yield return CacheOutput(node);
            }
        }
    }

    public IEnumerable<Expression> GetBranchBody(IBranchNode node)
    {
        yield return GetBranchExpression(node);
        if (node.NextNode is not null)
            yield return GetNodeBody(node.NextNode);
    }

    public IEnumerable<Expression> GetFlowBody(IFlowNode node)
    {
        yield return CacheOutput(node); 
        if (node.NextNode is not null)
            yield return GetNodeBody(node.NextNode);
    }


    public Expression GetBranchExpression(IBranchNode node)
    {
        var input = GetGroupInput(node.InputPorts);

        if (node.NextNodes is null)
            throw new NullReferenceException();

        var selectedBranch = Expression.Invoke(Expression.Constant(node.NodeSelectorFunc), input);

        var selectedBranchVar = Expression.Variable(typeof(int), "SelectedBranch");

        var cases = new List<Expression>();
        cases.Add(Expression.Assign(selectedBranchVar, selectedBranch));

        for (int i = 0; i < node.NextNodes.Length; i++)
        {
            if (node.NextNodes[i] is not null)
            {
                cases.Add(
                    Expression.IfThen(
                        Expression.Equal(selectedBranchVar, Expression.Constant(i)),
                        GetNodeBody(node.NextNodes[i])));
            }
        }

        return Expression.Block([selectedBranchVar], cases);
    }

    public Expression CacheOutput(INode node)
    {
        var input = GetGroupInput(node.InputPorts);

        var cache = GenerateCommandCall(node.Command, _variables[node.Id], input);

        return cache;
    }

    private Expression GetGroupInput(InputPort[] inputPorts)
    {
        var nodeInputs = new Expression[inputPorts.Length];
        foreach (var input in inputPorts)
        {
            if (input.ConnectedPort is null)
            {
                nodeInputs[input.Id] = Expression.Default(input.ValueType);
            }
            else
            {
                var outputIndex = input.ConnectedPort.Id;
                var valueBox = ValueGroupHelper.GetValueBoxByIndex(
                    _variables[input.ConnectedPort.Parent.Id], 
                    outputIndex);

                nodeInputs[input.Id] = Expression.Convert(
                    Expression.Field(valueBox, AccessHelper.ValueBoxObj.Value),
                    input.ValueType
                    );
            }
        }

        return ValueGroupHelper.GetValueGrouper(nodeInputs, inputPorts.Select(x=>x.ValueType).ToArray());
    }

    public Expression GenerateCommandCall(
        NodeCommand nodeCommand,
        ParameterExpression returnVar,
        Expression inputGroup)
    {
        var assignExpr = Expression.Assign(
            returnVar,
            Expression.Invoke(
                Expression.Constant(nodeCommand.Func),
                inputGroup));
        return assignExpr;
    }
}
