using CommunityToolkit.Mvvm.ComponentModel;
using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using System.Collections.ObjectModel;
using System.Configuration;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<NodeViewModel> Nodes { get; } =  [
        new(0, "NODE1") { Offset = new(0, 10), PreviousPort=new(0), NextPort=new(1) },
        new(1, "NODE2") { Offset = new(0, 100), PreviousPort=new(0), NextPort=new(1) },
        ];
    public CanvasViewModel Canvas { get; } = new() { Offset =new(10, 0), Scale=1};
}
