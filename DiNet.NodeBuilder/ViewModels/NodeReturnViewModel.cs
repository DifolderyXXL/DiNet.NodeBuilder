using CommunityToolkit.Mvvm.ComponentModel;
using DiNet.NodeBuilder.Models;

namespace DiNet.NodeBuilder.ViewModels;

public partial class NodeReturnViewModel : ObservableObject
{
    [ObservableProperty] public partial ReturnMetadata? ReturnMetadata { get; set; }

    public NodeReturnViewModel(ReturnMetadata? returnMetadata)
    {
        ReturnMetadata = returnMetadata;
    }
}
