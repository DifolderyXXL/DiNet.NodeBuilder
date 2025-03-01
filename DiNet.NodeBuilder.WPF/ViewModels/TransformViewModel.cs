using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class TransformViewModel : ObservableObject
{
    [ObservableProperty] public partial Point Offset { get; set; }
#if DEBUG
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"{e.PropertyName} {Offset}");
        base.OnPropertyChanged(e);
    }
#endif
}
