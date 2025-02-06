using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.ViewModels;
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

    private void OnNodePressed(object? sender, MouseButtonEventArgs args, int id)
    {
        
    }
}

