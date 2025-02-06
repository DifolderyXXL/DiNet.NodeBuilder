using System.ComponentModel;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace DiNet.NodeBuilder.WPF.Views;
/// <summary>
/// Логика взаимодействия для NodeWorldView.xaml
/// </summary>
public partial class NodeWorldView : UserControl
{
    public static readonly DependencyProperty LocalPositionXProperty = DependencyProperty.Register(
        "LocalPositionX", typeof(double),
        typeof(NodeWorldView), new(0d)
        );
    public double LocalPositionX
    {
        get => (double)GetValue(LocalPositionXProperty);
        set => SetValue(LocalPositionXProperty, value);
    }

    public static readonly DependencyProperty LocalPositionYProperty = DependencyProperty.Register(
        "LocalPositionY", typeof(double),
        typeof(NodeWorldView), new(0d)
        );
    public double LocalPositionY
    {
        get => (double)GetValue(LocalPositionYProperty);
        set => SetValue(LocalPositionYProperty, value);
    }


    public static readonly DependencyProperty LocalScaleProperty = DependencyProperty.Register(
        "LocalScale", typeof(double),
        typeof(NodeWorldView), new(1d)
        );
    public double LocalScale
    {
        get => (double)GetValue(LocalScaleProperty);
        set => SetValue(LocalScaleProperty, Math.Clamp(value, MinScale, MaxScale));
    }


    public static readonly DependencyProperty MinScaleProperty =
        DependencyProperty.Register("MinScale", typeof(double), typeof(NodeWorldView), new PropertyMetadata(0.5d));
    public double MinScale
    {
        get { return (double)GetValue(MinScaleProperty); }
        set { SetValue(MinScaleProperty, value); }
    }

    public static readonly DependencyProperty MaxScaleProperty =
        DependencyProperty.Register("MaxScale", typeof(double), typeof(NodeWorldView), new PropertyMetadata(2d));
    public double MaxScale
    {
        get { return (double)GetValue(MaxScaleProperty); }
        set { SetValue(MaxScaleProperty, value); }
    }


    public NodeWorldView()
    {
        InitializeComponent();

        _worldTransform.Children.Add(_worldTranslateTransform);
        _worldTransform.Children.Add(_worldScaleTransform);

        WorldGrid.RenderTransform = _worldTransform;

        DependencyPropertyDescriptor.FromProperty(LocalPositionXProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnLocalPositionXChanged);
        DependencyPropertyDescriptor.FromProperty(LocalPositionYProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnLocalPositionYChanged);

        DependencyPropertyDescriptor.FromProperty(MinScaleProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnMinScaleChanged);
        DependencyPropertyDescriptor.FromProperty(MaxScaleProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnMaxScaleChanged);

        DependencyPropertyDescriptor.FromProperty(LocalScaleProperty, typeof(NodeWorldView))
            .AddValueChanged(this, OnLocalScaleChanged);
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

    private void OnLocalScaleChanged(object? sender, EventArgs e)
    {
        _worldScaleTransform.ScaleX = LocalScale;
        _worldScaleTransform.ScaleY = LocalScale;
    }

    private void OnLocalPositionXChanged(object? sender, EventArgs e)
    {
        _worldTranslateTransform.X = LocalPositionX;
    }
    private void OnLocalPositionYChanged(object? sender, EventArgs e)
    {
        _worldTranslateTransform.Y = LocalPositionY;
    }

    private bool _middleMousePressed;
    private Point _pressPoint;
    private Point _localWorldPositionOnDown;

    private TranslateTransform _worldTranslateTransform = new();
    private ScaleTransform _worldScaleTransform = new();
    private TransformGroup _worldTransform = new();

    private void WorldGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton is MouseButtonState.Pressed)
        {
            _localWorldPositionOnDown = new(LocalPositionX, LocalPositionY);

            _pressPoint = GetMouseRelativePosition(e.MouseDevice);
            _middleMousePressed = true;
        }
    }
    private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        UpdateIfMiddleUp(e);

        if (_middleMousePressed)
        {
            UpdateWorldPosition(e.MouseDevice);
        }
    }
    private void UpdateIfMiddleUp(MouseEventArgs e)
    {
        if (e.MiddleButton is MouseButtonState.Released)
        {
            _middleMousePressed = false;
        }
    }
    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        UpdateIfMiddleUp(e);
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

    private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScaleWorld(e.MouseDevice.GetPosition(this), e.Delta/120);
    }
}
