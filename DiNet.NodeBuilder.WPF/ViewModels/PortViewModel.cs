namespace DiNet.NodeBuilder.WPF.ViewModels;

public partial class PortViewModel : TransformViewModel
{
    public int Id { get; }
    public PortViewModel(int id)
    {
        Id = id;
    }
}