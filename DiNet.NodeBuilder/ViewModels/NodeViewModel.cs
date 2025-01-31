using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.ViewModels;
public partial class NodeViewModel : ObservableObject
{
    public ObservableCollection<NodeInputViewModel> Inputs { get; } = new();
    public ObservableCollection<NodeReturnViewModel> Returns { get; } = new();
}
