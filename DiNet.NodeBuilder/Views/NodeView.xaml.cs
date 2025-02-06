using DiNet.NodeBuilder.ViewModels;

namespace DiNet.NodeBuilder.Views;

public partial class NodeView : ContentView
{
	public NodeView()
	{
		InitializeComponent();
	}

    private void PointerGestureRecognizer_PointerPressed(object sender, PointerEventArgs e)
    {
		(BindingContext as NodeViewModel)!.OnPressed(sender, e);
    }
}