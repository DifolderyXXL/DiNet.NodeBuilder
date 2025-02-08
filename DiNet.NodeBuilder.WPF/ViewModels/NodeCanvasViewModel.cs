using CommunityToolkit.Mvvm.ComponentModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;
public partial class NodeCanvasViewModel : ObservableObject
{
    [ObservableProperty] public partial float PositionX { get; set; }
    [ObservableProperty] public partial float PositionY { get; set; }

    [ObservableProperty] public partial float Scale { get; set; }
}
