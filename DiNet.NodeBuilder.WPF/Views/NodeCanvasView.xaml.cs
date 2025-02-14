using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiNet.NodeBuilder.WPF.Views;
/// <summary>
/// Логика взаимодействия для NodeCanvasView.xaml
/// </summary>
public partial class NodeCanvasView : UserControl
{
    public static readonly DependencyProperty LocalPositionXProperty = DependencyProperty.Register(
    "LocalPositionX", typeof(double),
    typeof(NodeCanvasView), new(0d)
    );
    public double LocalPositionX
    {
        get => (double)GetValue(LocalPositionXProperty);
        set => SetValue(LocalPositionXProperty, value);
    }

    public static readonly DependencyProperty LocalPositionYProperty = DependencyProperty.Register(
        "LocalPositionY", typeof(double),
        typeof(NodeCanvasView), new(0d)
        );
    public double LocalPositionY
    {
        get => (double)GetValue(LocalPositionYProperty);
        set => SetValue(LocalPositionYProperty, value);
    }


    public static readonly DependencyProperty LocalScaleProperty = DependencyProperty.Register(
        "LocalScale", typeof(double),
        typeof(NodeCanvasView), new(1d)
        );
    public double LocalScale
    {
        get => (double)GetValue(LocalScaleProperty);
        set => SetValue(LocalScaleProperty, Math.Clamp(value, MinScale, MaxScale));
    }

    public static readonly DependencyProperty MinScaleProperty =
        DependencyProperty.Register("MinScale", typeof(double), typeof(NodeCanvasView), new PropertyMetadata(0.5d));
    public double MinScale
    {
        get { return (double)GetValue(MinScaleProperty); }
        set { SetValue(MinScaleProperty, value); }
    }

    public static readonly DependencyProperty MaxScaleProperty =
        DependencyProperty.Register("MaxScale", typeof(double), typeof(NodeCanvasView), new PropertyMetadata(2d));
    public double MaxScale
    {
        get { return (double)GetValue(MaxScaleProperty); }
        set { SetValue(MaxScaleProperty, value); }
    }


    public NodeCanvasView()
    {
        InitializeComponent();

        DependencyPropertyDescriptor.FromProperty(MinScaleProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnMinScaleChanged);
        DependencyPropertyDescriptor.FromProperty(MaxScaleProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnMaxScaleChanged);
    }

    private void OnMaxScaleChanged(object? sender, EventArgs e)
    {
        if (LocalScale > MaxScale)
            LocalScale = MaxScale;
    }
    private void OnMinScaleChanged(object? sender, EventArgs e)
    {
        if (LocalScale < MinScale)
            LocalScale = MinScale;
    }


    private bool _middleMousePressed;
    private Point _pressPoint;
    private Point _localWorldPositionOnDown;

    private void WorldGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton is MouseButtonState.Pressed)
        {
            _localWorldPositionOnDown = new(LocalPositionX, LocalPositionY);

            _pressPoint = GetMouseRelativePosition(e.MouseDevice);
            _middleMousePressed = true;
        }
    }
    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        UpdateIfMiddleUp(e);
    }

    private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        UpdateIfMiddleUp(e);

        if (_middleMousePressed)
        {
            UpdateWorldPosition(e.MouseDevice);
        }
    }
    private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScaleWorld(e.MouseDevice.GetPosition(this), e.Delta / 120);
    }


    private void UpdateIfMiddleUp(MouseEventArgs e)
    {
        if (e.MiddleButton is MouseButtonState.Released)
        {
            _middleMousePressed = false;
        }
    }

    private Point ToWorldCursor(Point mousePos)
    {
        mousePos.X /= LocalScale;
        mousePos.Y /= LocalScale;
        return mousePos;
    }
    private Point GetMouseRelativePosition(MouseDevice e)
    {
        var pos = e.GetPosition(this);

        return ToWorldCursor(pos);
    }

    private void UpdateWorldPosition(MouseDevice mouseDevice)
    {
        var curPoint = GetMouseRelativePosition(mouseDevice);

        UpdatePositionRelative(_localWorldPositionOnDown, new(curPoint.X - _pressPoint.X, curPoint.Y - _pressPoint.Y));
    }
    private Point UpdatePositionRelative(Point startPoint, Point offset)
    {
        var result = new Point(
            startPoint.X + offset.X,
            startPoint.Y + offset.Y
            );

        LocalPositionX = result.X;
        LocalPositionY = result.Y;

        return result;
    }

    private void ScaleWorld(Point relativePoint, int scaleDelta)
    {
        var scaleAdd = scaleDelta * 0.1f;

        if (LocalScale + scaleAdd > MaxScale || LocalScale + scaleAdd < MinScale)
            return;

        LocalPositionX -= (relativePoint.X / LocalScale - relativePoint.X / (LocalScale + scaleAdd));
        LocalPositionY -= (relativePoint.Y / LocalScale - relativePoint.Y / (LocalScale + scaleAdd));

        LocalScale += scaleAdd;
    }
}
