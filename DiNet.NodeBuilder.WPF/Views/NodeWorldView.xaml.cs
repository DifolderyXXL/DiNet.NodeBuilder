using CommunityToolkit.Mvvm.ComponentModel;
using DiNet.NodeBuilder.WPF.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiNet.NodeBuilder.WPF.Views;
/// <summary>
/// Логика взаимодействия для NodeWorldView.xaml
/// </summary>
public partial class NodeWorldView : UserControl
{
    public double LocalScale => NodeCanvas.LocalScale;

    private NodeDragMachine? _dragNode;
    private NodeDragMachine? _dragPort;

    public NodeWorldView()
    {
        InitializeComponent();

        DataContext = new NodeWorldViewModel();
    }

    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _dragNode?.MouseUp(sender, e);
    }

    private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        _dragNode?.MouseMove(sender, e);
    }

    private void NodeView_OnHeaderPressed(object sender, MouseButtonEventArgs e)
    {
        var vm = ((sender as NodeView)!.DataContext as NodeViewModel)!;
        _dragNode = new(
            this,
            () => new(vm.PositionX, vm.PositionY),
            x => { vm.PositionX = x.X; vm.PositionY = x.Y; },
            ToWorldCursor);

        _dragNode?.MouseDown(sender, e);
    }

    private void Port_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var port = (sender as PortView)!;
        var vm = (port.DataContext as PortViewModel)!;

        
    }
    private void Port_MouseUp(object sender, MouseButtonEventArgs e)
    {
        
    }

    private Point ToWorldCursor(Point mousePos)
    {
        mousePos.X /= LocalScale;
        mousePos.Y /= LocalScale;
        return mousePos;
    }
}

public class NodeDragMachine(
    UIElement c_parent, 
    Func<Point> c_getPosition,
    Action<Point> c_setPosition,
    Func<Point, Point> c_convertToWorldCoordinates)
{
    private bool _mousePressed;
    private Point _pressPoint;
    private Point _nodePressPoint;

    private UIElement _parent = c_parent;
    private Func<Point, Point> _convertToWorldCoordinate = c_convertToWorldCoordinates;

    private Func<Point> _getPosition = c_getPosition;
    private Action<Point> _setPosition = c_setPosition;

    public void MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_mousePressed)
        {
            _pressPoint = _convertToWorldCoordinate(e.GetPosition(_parent));

            _nodePressPoint = _getPosition.Invoke();
            _mousePressed = true;
        }
    }

    public void MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
            _mousePressed = false;
        if (_mousePressed)
        {
            var pos = _convertToWorldCoordinate(e.GetPosition(_parent));

            _setPosition.Invoke(new(
                _nodePressPoint.X + pos.X - _pressPoint.X, 
                _nodePressPoint.Y + pos.Y - _pressPoint.Y
                ));
        }
    }

    public void MouseUp(object sender, MouseButtonEventArgs e)
    {
        _mousePressed = false;
    }
}
