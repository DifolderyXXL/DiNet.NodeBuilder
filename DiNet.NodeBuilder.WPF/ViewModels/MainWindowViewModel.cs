using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<NodeViewModel> Nodes { get; }
    public MainWindowViewModel()
    {
        Nodes = [new(0), new(1)];
    }
}
