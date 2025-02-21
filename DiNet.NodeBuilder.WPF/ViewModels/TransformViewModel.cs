using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class TransformViewModel : ObservableObject
{
    [ObservableProperty] public partial Point Offset { get; set; }
}
