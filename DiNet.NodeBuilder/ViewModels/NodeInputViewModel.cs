using CommunityToolkit.Mvvm.ComponentModel;
using DiNet.NodeBuilder.Models;

namespace DiNet.NodeBuilder.ViewModels;

public partial class NodeInputViewModel : ObservableObject
{
    [ObservableProperty] public partial InputMetadata? InputMetadata { get; set; }

    public NodeInputViewModel(InputMetadata? inputMetadata)
    {
        InputMetadata = inputMetadata;
    }
}
