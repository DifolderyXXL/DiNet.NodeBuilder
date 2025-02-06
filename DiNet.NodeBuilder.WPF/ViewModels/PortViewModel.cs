using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiNet.NodeBuilder.WPF.Models;
using System.Diagnostics;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.ViewModels;
public partial class PortViewModel : ObservableObject
{
    public event Action<object?, MouseButtonEventArgs, int>? OnPortPressed;
    public event Action<object?, MouseButtonEventArgs, int>? OnPortReleased;

    public int Id { get; init; }

    [ObservableProperty] public partial PortMetadata? PortMetadata { get; set; }

    public PortViewModel(int portId, PortMetadata? inputMetadata)
    {
        PortMetadata = inputMetadata;

        Id = portId;
    }

    public void OnPressed(object? sender, MouseButtonEventArgs pointerEventArgs)
    {
        OnPortPressed?.Invoke(sender, pointerEventArgs, Id);

        Debug.WriteLine("PRESSED");
    }
    public void OnReleased(object? sender, MouseButtonEventArgs pointerEventArgs)
    {
        OnPortReleased?.Invoke(sender, pointerEventArgs, Id);

        Debug.WriteLine("RELEASED");
    }
}
