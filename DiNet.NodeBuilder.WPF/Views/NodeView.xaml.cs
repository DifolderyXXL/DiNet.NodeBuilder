using DiNet.NodeBuilder.WPF.Helpers;
using DiNet.NodeBuilder.WPF.ViewModels;
using DiNet.NodeBuilder.WPF.Views.Controls;
using DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;
using System.Windows;
using System.Windows.Controls;
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
    private BranchContext _branchContext => _nodeArea.BranchContext;

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

        var branches = _branchContext.GetAllBranches(this);
        foreach (var item in branches.Where(x => x.isFirstCoord))
            item.line.AddFirst(offset);

        foreach (var item in branches.Where(x => !x.isFirstCoord))
            item.line.AddSecond(offset);

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

        var portView = (sender as PortView)!;

        var pos = Mouse.GetPosition(_nodeArea.Content);

        if (_branchContext.TryGetBranch(portView, out var outBranch))
        {
            _nodeArea.Controller.BeginLineMove(outBranch.line, outBranch.isFirstCoord);
            _branchContext.Remove(outBranch.port);
        }
        else
        {
            var line = new Line() { StrokeThickness = 3, Stroke = new SolidColorBrush(Colors.Red), IsHitTestVisible = false };

            _branchContext.Add(new(line, portView, this, true));

            var centerRelativeToAncestor = GetCenterPosition(portView, _nodeArea.Content);

            line.SetFirst(centerRelativeToAncestor);
            line.SetSecond(pos);

            _nodeArea.Controller.BeginLineMove(line);
        }
    }

    private void PortView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var portView = (sender as PortView)!;

        if (_nodeArea.Controller.ContainsLineElement())
        {
            var currentLine = _nodeArea.Controller.CurrentLine!;

            if (_branchContext.Contains(portView))
            {
                _branchContext.RemoveByLine(currentLine);
            }
            else
            {
                bool isFirst = false;
                if (_branchContext.TryGetBranch(currentLine, out var branch))
                    isFirst = !branch.isFirstCoord;

                _branchContext.Add(new(currentLine, portView, this, isFirst));

                var centerRelativeToAncestor = GetCenterPosition((sender as PortView)!, _nodeArea.Content);

                if (!isFirst)
                    currentLine.SetSecond(centerRelativeToAncestor);
                else
                    currentLine.SetFirst(centerRelativeToAncestor);
            }

            _nodeArea.Controller.EndLineMove();
        }
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
