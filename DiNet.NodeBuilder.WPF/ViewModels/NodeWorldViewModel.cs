using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class NodeWorldViewModel : ObservableObject 
{
    public ObservableCollection<NodeViewModel> Nodes { get; } = [];
    public ObservableCollection<BranchViewModel> Branches { get; } = [];
}
