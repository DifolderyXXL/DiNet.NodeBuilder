using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiNet.NodeBuilder.WPF.Models;
using System.Diagnostics;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.ViewModels;
public partial class PortViewModel : ObservableObject
{
    public int Id { get; init; }

    [ObservableProperty] public partial PortMetadata? PortMetadata { get; set; }


    public PortViewModel(int portId, PortMetadata? inputMetadata)
    {
        PortMetadata = inputMetadata;

        Id = portId;
    }
}

public partial class BranchViewModel : ObservableObject
{
    public int Id { get; init; }

    [ObservableProperty] public partial PortMetadata? PortMetadata { get; set; }

    public BranchViewModel(int portId)
    {
        Id = portId;
    }
}