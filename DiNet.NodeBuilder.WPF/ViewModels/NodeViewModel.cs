using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.ViewModels;
public partial class NodeViewModel : ObservableObject
{
    public int Id { get; }

    public event Action<object?, MouseButtonEventArgs, int>? OnNodePressed;

    [ObservableProperty] public partial float PositionX { get; set; }
    [ObservableProperty] public partial float PositionY { get; set; }

    public ObservableCollection<PortViewModel> InputPorts { get; } = [];
    public ObservableCollection<PortViewModel> OutputPorts { get; } = [];

    public NodeViewModel(int associatedId)
    {
        Id = associatedId;
    }

    public void OnPressed(object? sender, MouseButtonEventArgs pointerEventArgs)
    {
        OnNodePressed?.Invoke(sender, pointerEventArgs, Id);
    }
}
