using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class CanvasViewModel : TransformViewModel
{
    [ObservableProperty] public partial double Scale { get; set; }
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        Debug.WriteLine(e.PropertyName);
    }
}
