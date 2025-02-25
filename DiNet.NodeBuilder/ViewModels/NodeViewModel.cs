﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.ViewModels;
public partial class NodeViewModel : ObservableObject
{
    public int Id { get; }

    public event Action<object?, PointerEventArgs, int>? OnNodePressed;

    [ObservableProperty] public partial float PositionX { get; set; }
    [ObservableProperty] public partial float PositionY { get; set; }

    public ObservableCollection<PortViewModel> InputPorts { get; } = [];
    public ObservableCollection<PortViewModel> OutputPorts { get; } = [];

    public NodeViewModel(int associatedId)
    {
        Id = associatedId;
    }

    public void OnPressed(object? sender, PointerEventArgs pointerEventArgs)
    {
        OnNodePressed?.Invoke(sender, pointerEventArgs, Id);
    }
}
