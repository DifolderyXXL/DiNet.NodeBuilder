namespace DiNet.NodeBuilder.WPF.Views.Controls;
public class BranchContext
{
    public IEnumerable<NodeBranch> GetBranches()
            => Branches.Values.Distinct();
    public Dictionary<PortView, NodeBranch> Branches = [];
}
