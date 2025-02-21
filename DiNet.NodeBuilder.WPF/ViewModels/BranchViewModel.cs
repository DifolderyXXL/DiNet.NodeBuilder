using CommunityToolkit.Mvvm.ComponentModel;

namespace DiNet.NodeBuilder.WPF.ViewModels;

public class BranchViewModel : ObservableObject
{
    public PortViewModel OutputPort { get; }
    public PortViewModel InputPort { get; }

    public BranchViewModel(PortViewModel outputPort, PortViewModel inputPort)
    {
        OutputPort = outputPort;
        InputPort = inputPort;
    }
}
