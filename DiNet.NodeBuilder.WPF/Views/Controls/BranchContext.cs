using DiNet.NodeBuilder.Common.Helpers;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DiNet.NodeBuilder.WPF.Views.Controls;
public class BranchContext
{

    private Dictionary<PortView, NodeBranch> _branches = [];

    private Panel _container;

    public BranchContext(Panel container)
    {
        _container = container;
    }

    public bool Contains(Line line)
        => _branches.Values.Any(x => x.line == line);

    public bool Contains(PortView port)
        => _branches.ContainsKey(port);

    public void RemoveByLine(Line line)
    {
        while (TryGetBranch(line, out var branch))
            Remove(branch.port);
        _container.Children.Remove(line);
    }

    public bool TryGetBranch(PortView port, out NodeBranch result)
    {
        if(_branches.ContainsKey(port))
        {
            result = _branches[port];
            return true;
        }
        result = default!;
        return false;
    }

    public bool TryGetBranch(Line line, out NodeBranch result)
    {
        if (_branches.Values.TryFind(x => x.line == line, out var branch))
        {
            result = branch;
            return true;
        }
        result = default!;
        return false;
    }

    public IEnumerable<NodeBranch> GetAllBranches(NodeView parent)
        => _branches.Values.Where(x => x.portParent == parent);

    public void Add(NodeBranch branch)
    {
        if(!Contains(branch.line))
            _container.Children.Add(branch.line);
        _branches.Add(branch.port, branch);
    }
    public void Remove(PortView port)
    {
        _branches.Remove(port);
    }
}
public record NodeBranch(Line line, PortView port, NodeView portParent, bool isFirstCoord);
