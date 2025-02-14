﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.ViewModels;
public partial class NodeViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty] public partial double PositionX { get; set; }
    [ObservableProperty] public partial double PositionY { get; set; }

    public ObservableCollection<PortViewModel> InputPorts { get; } = [];
    public ObservableCollection<PortViewModel> OutputPorts { get; } = [];

    public NodeViewModel(int associatedId)
    {
        Id = associatedId;

        PositionX = Random.Shared.NextSingle() * 600;
        PositionY = Random.Shared.NextSingle() * 600;
    }
}
