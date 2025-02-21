using System.Collections.ObjectModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class NodeViewModel : TransformViewModel
{
    public int Id { get; }
    public string? Title { get; }

    public PortViewModel? PreviousPort { get; }
    public PortViewModel? NextPort { get; }

    public ObservableCollection<PortViewModel> Input { get; } = [];
    public ObservableCollection<PortViewModel> Output { get; } = [];

    public NodeViewModel(int id, string? title)
    {
        Id = id;
        Title = title;
    }
}
