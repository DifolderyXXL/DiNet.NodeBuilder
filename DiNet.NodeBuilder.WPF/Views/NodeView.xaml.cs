using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.Views;
/// <summary>
/// Логика взаимодействия для NodeView.xaml
/// </summary>
public partial class NodeView : UserControl
{
    public event MouseButtonEventHandler? OnMouseDownEvent;

    public static readonly RoutedEvent OnHeaderPressedEvent =
        EventManager.RegisterRoutedEvent(
            "OnHeaderPressed", RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(NodeView));

    public event MouseButtonEventHandler OnHeaderPressed
    {
        add { AddHandler(OnHeaderPressedEvent, value); }
        remove { RemoveHandler(OnHeaderPressedEvent, value); }
    }



    public static readonly DependencyProperty OnDragStartedProperty =
        DependencyProperty.Register("OnDragStartedCommand", typeof(ICommand), typeof(NodeView), new PropertyMetadata(null));
    public ICommand? OnDragStartedCommand
    {
        get { return (ICommand?)GetValue(OnDragStartedProperty); }
        set { SetValue(OnDragStartedProperty, value); }
    }

    public NodeView()
    {
        InitializeComponent();
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (OnDragStartedCommand?.CanExecute(DataContext) ?? false)
            OnDragStartedCommand.Execute(DataContext);

        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = OnHeaderPressedEvent
        });

        OnMouseDownEvent?.Invoke(this, e);
    }
}
