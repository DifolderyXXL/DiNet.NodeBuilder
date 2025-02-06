using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiNet.NodeBuilder.Models;
using System.Diagnostics;

namespace DiNet.NodeBuilder.ViewModels;

public partial class PortViewModel : ObservableObject
{
    public event Action<object?, PointerEventArgs, int>? OnPortPressed;
    public event Action<object?, PointerEventArgs, int>? OnPortReleased;

    public int Id { get; init; }

    [ObservableProperty] public partial PortMetadata? PortMetadata { get; set; }

    public PortViewModel(int portId, PortMetadata? inputMetadata)
    {
        PortMetadata = inputMetadata;

        Id = portId;
    }

    public void OnPressed(object? sender, PointerEventArgs pointerEventArgs)
    {
        OnPortPressed?.Invoke(sender, pointerEventArgs, Id);

        Debug.WriteLine("PRESSED");
    }
    public void OnReleased(object? sender, PointerEventArgs pointerEventArgs)
    {
        OnPortReleased?.Invoke(sender, pointerEventArgs, Id);

        Debug.WriteLine("RELEASED");
    }
}
