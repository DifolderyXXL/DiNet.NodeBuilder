using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;
public partial class NodeWorldViewModel : ObservableObject
{
    [ObservableProperty] public partial NodeCanvasViewModel CanvasViewModel { get; set; }
    public ObservableCollection<NodeViewModel> Nodes { get; } = [new(0), new(1)];

    public NodeViewModel? DraggingNode { get; private set; }

    public NodeWorldViewModel()
    {
        Nodes = [new(0), new(1)];
    }

    public void AddNode(NodeViewModel node)
    {
        Nodes.Add(node);
    }

    public void RemoveNode(NodeViewModel node)
    {
        Nodes.Remove(node);
    }

    [RelayCommand]
    private void OnNodeDragStarted(NodeViewModel vm)
    {
        DraggingNode = vm;
    }
}
