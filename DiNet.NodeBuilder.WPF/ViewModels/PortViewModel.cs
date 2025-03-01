using CommunityToolkit.Mvvm.ComponentModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class PortViewModel : ObservableObject
{
    public int Id { get; }
    public PortViewModel(int id)
    {
        Id = id;
    }


}