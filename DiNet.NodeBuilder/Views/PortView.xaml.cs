using DiNet.NodeBuilder.ViewModels;

namespace DiNet.NodeBuilder.Views;

public partial class PortView : ContentView
{
	public PortView()
	{
		InitializeComponent();

        BindingContext = new PortViewModel(0, null);
    }

    private void PointerGestureRecognizer_PointerPressed(object sender, PointerEventArgs e)
    {
        (BindingContext as PortViewModel)!.OnPressed(sender, e);
    }

    private void PointerGestureRecognizer_PointerReleased(object sender, PointerEventArgs e)
    {
        (BindingContext as PortViewModel)!.OnReleased(sender, e);
    }
}