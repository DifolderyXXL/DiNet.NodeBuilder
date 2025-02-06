using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.ViewModels;
public partial class NodeWorldViewModel : ObservableObject
{
    public ObservableCollection<NodeViewModel> Nodes { get; } = [];

    public void AddNode(NodeViewModel node)
    {
        Nodes.Add(node);

        node.OnNodePressed += OnNodePressed;
    }

    public void RemoveNode(NodeViewModel node)
    {
        Nodes.Remove(node);

        node.OnNodePressed -= OnNodePressed;
    }

    private void OnNodePressed(object? sender, PointerEventArgs e, int id)
    {
        
    }
}

