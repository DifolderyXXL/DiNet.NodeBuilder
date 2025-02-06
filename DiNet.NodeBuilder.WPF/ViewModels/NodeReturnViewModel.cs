using CommunityToolkit.Mvvm.ComponentModel;
using DiNet.NodeBuilder.WPF.Models;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class NodeReturnViewModel : ObservableObject
{
    [ObservableProperty] public partial ReturnMetadata? ReturnMetadata { get; set; }

    public NodeReturnViewModel(ReturnMetadata? returnMetadata)
    {
        ReturnMetadata = returnMetadata;
    }
}
