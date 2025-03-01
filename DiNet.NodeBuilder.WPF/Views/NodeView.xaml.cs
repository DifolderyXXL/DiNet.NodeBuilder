using DiNet.NodeBuilder.WPF.ViewModels;
using DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiNet.NodeBuilder.WPF.Views;
/// <summary>
/// Логика взаимодействия для NodeView.xaml
/// </summary>
public partial class NodeView : Grid, IMoveElement
{
    private NodeAreaView _nodeArea;

    private Line _lineFirst;
    private Line _lineSecond;
    //TODO: Bind Position

    public NodeView(NodeAreaView nodeArea)
    {
        _nodeArea = nodeArea;

        InitializeComponent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(DataContext))
            MoveElement((Vector)((DataContext as NodeViewModel)!.Offset));

        base.OnPropertyChanged(e);
    }

    public void MoveElement(Vector offset)
    {
        if (matrixTransform is null) return;

        var matrix = matrixTransform.Matrix;
        offset.Negate();


        if (_lineFirst is not null)
        {
            _lineFirst.X1 += offset.X;
            _lineFirst.Y1 += offset.Y;
        }
        if (_lineSecond is not null)
        {
            _lineSecond.X2 += offset.X;
            _lineSecond.Y2 += offset.Y;
        }
        
        matrix.Translate(offset.X, offset.Y);
        matrixTransform.Matrix = matrix;
    }

    private Point GetCenterPosition(FrameworkElement element, UIElement parent)
    {
        var center = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
        return element.TransformToAncestor(parent).Transform(center);
    }

    private void PortView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        var line = new Line() { StrokeThickness = 3, Stroke = new SolidColorBrush(Colors.Red) , IsHitTestVisible = false};
        _nodeArea.BranchContent.Children.Add(line);
        _lineFirst = line;

        var pos = Mouse.GetPosition(_nodeArea.Content);

        var centerRelativeToAncestor = GetCenterPosition(NextPort, _nodeArea.Content);

        line.X1 = centerRelativeToAncestor.X;
        line.Y1 = centerRelativeToAncestor.Y;

        line.X2 = pos.X;
        line.Y2 = pos.Y;

        _nodeArea.Controller.BeginLineMove(line);
    }

    private void PortView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_nodeArea.Controller.ContainsLineElement())
        {
            _lineSecond = _nodeArea.Controller.CurrentLine!;

            var centerRelativeToAncestor = GetCenterPosition(PreviousPort, _nodeArea.Content);

            _lineSecond.X2 = centerRelativeToAncestor.X;
            _lineSecond.Y2 = centerRelativeToAncestor.Y;

            _nodeArea.Controller.EndLineMove();
        }
    }

    protected static void CreateBinding(DependencyObject target, object src, DependencyProperty property, string path, BindingMode mode = BindingMode.TwoWay)
    {
        var bind = new Binding(path)
        {
            Source = src,
            Mode = mode,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
        BindingOperations.SetBinding(target, property, bind);
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Handled)
            return;

        _nodeArea.Controller.BeginMovement(e.GetPosition(_nodeArea.Content), this);
    }

    private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Handled)
            return;

        _nodeArea.Controller.EndMovement();
    }
}
