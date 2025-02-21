using CommunityToolkit.Mvvm.ComponentModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class CanvasViewModel : TransformViewModel
{
    [ObservableProperty] public partial double Scale { get; set; }
}
