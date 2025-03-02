using DiNet.NodeBuilder.Common.Helpers;
using DiNet.NodeBuilder.Core;
using DiNet.NodeBuilder.WPF.ViewModels;
using DiNet.NodeBuilder.WPF.Views.Controls;
using DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiNet.NodeBuilder.WPF.Views;

public record NodeBranch(Line line, PortView port, NodeView portParent, bool isFirstCoord);

/// <summary>
/// Логика взаимодействия для NodeView.xaml
/// </summary>
public partial class NodeView : Grid, IMoveElement
{
    private NodeAreaView _nodeArea;


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

        var branches = _nodeArea.GetBranches();
        foreach (var item in branches.Where(x=>x.isFirstCoord && x.portParent == this))
        {
            item.line.X1 += offset.X;
            item.line.Y1 += offset.Y;
        }
        foreach (var item in branches.Where(x => !x.isFirstCoord && x.portParent == this))
        {
            item.line.X2 += offset.X;
            item.line.Y2 += offset.Y;
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

        var portView = (sender as PortView)!;

        var pos = Mouse.GetPosition(_nodeArea.Content);

        var branches = _nodeArea.Branches.Values;
        if (branches.TryFind(x=>x.port == portView, out var outBranch))
        {
            _nodeArea.Controller.BeginLineMove(outBranch.line, outBranch.isFirstCoord);
            _nodeArea.Branches.Remove(outBranch.port);
        }
        else
        {
            var line = new Line() { StrokeThickness = 3, Stroke = new SolidColorBrush(Colors.Red), IsHitTestVisible = false };
            _nodeArea.BranchContent.Children.Add(line);

            _nodeArea.Branches.Add(portView, new(line, portView, this, true));

            var centerRelativeToAncestor = GetCenterPosition(portView, _nodeArea.Content);

            line.X1 = centerRelativeToAncestor.X;
            line.Y1 = centerRelativeToAncestor.Y;

            line.X2 = pos.X;
            line.Y2 = pos.Y;

            _nodeArea.Controller.BeginLineMove(line);
        }

        
    }

    private void PortView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var portView = (sender as PortView)!;

        if (_nodeArea.Controller.ContainsLineElement())
        {
            var currentLine = _nodeArea.Controller.CurrentLine!;

            var branches = _nodeArea.Branches.Values;
            if (branches.TryFind(x => x.port == portView, out var outBranch))
            {
                if (outBranch.line == currentLine)
                {
                    _nodeArea.Branches.Remove(outBranch.port);
                    _nodeArea.BranchContent.Children.Remove(outBranch.line);
                }
                else
                {
                    if (branches.TryFind(x => x.line == currentLine, out var branch))
                        _nodeArea.Branches.Remove(branch.port);
                    _nodeArea.BranchContent.Children.Remove(currentLine);
                }
            }
            else
            {
                bool isFirst = false;
                if(_nodeArea.Branches.Values.TryFind(x => x.line == currentLine, out var branch))
                    isFirst = !branch.isFirstCoord;
                
                _nodeArea.Branches.Add(portView, new(currentLine, portView, this, isFirst));

                var centerRelativeToAncestor = GetCenterPosition((sender as PortView)!, _nodeArea.Content);

                if(!isFirst)
                {
                    currentLine.X2 = centerRelativeToAncestor.X;
                    currentLine.Y2 = centerRelativeToAncestor.Y;

                }
                else
                {
                    currentLine.X1 = centerRelativeToAncestor.X;
                    currentLine.Y1 = centerRelativeToAncestor.Y;
                }
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
