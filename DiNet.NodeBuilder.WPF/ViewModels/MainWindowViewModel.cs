using CommunityToolkit.Mvvm.ComponentModel;
using DiNet.NodeBuilder.Core.Nodes.Interfaces;
using System.Configuration;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public NodeViewModel Node { get; } = new(0, "NODE");
    public CanvasViewModel CanvasView { get; } = new();
}
