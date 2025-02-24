using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.Behaviors;
public class ContentTransformBehavior : Grid
{
    public Point Position
    {
        get { return (Point)GetValue(PositionProperty); }
        set { SetValue(PositionProperty, value); }
    }
    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register(nameof(Position), typeof(Point), typeof(ContentTransformBehavior), new PropertyMetadata(new Point()));

    public double Scale
    {
        get { return (double)GetValue(ScaleProperty); }
        set { SetValue(ScaleProperty, value); }
    }
    public static readonly DependencyProperty ScaleProperty =
        DependencyProperty.Register(nameof(Scale), typeof(double), typeof(ContentTransformBehavior), new PropertyMetadata(1d));

    public double DeltaScale
    {
        get { return (double)GetValue(DeltaScaleProperty); }
        set { SetValue(DeltaScaleProperty, value); }
    }
    public static readonly DependencyProperty DeltaScaleProperty =
        DependencyProperty.Register(nameof(DeltaScale), typeof(double), typeof(ContentTransformBehavior), new PropertyMetadata(0.1d));

    public double MinScale
    {
        get { return (double)GetValue(MinScaleProperty); }
        set { SetValue(MinScaleProperty, value); }
    }
    public static readonly DependencyProperty MinScaleProperty =
        DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(ContentTransformBehavior), new PropertyMetadata(0.5d));

    public double MaxScale
    {
        get { return (double)GetValue(MaxScaleProperty); }
        set { SetValue(MaxScaleProperty, value); }
    }
    public static readonly DependencyProperty MaxScaleProperty =
        DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(ContentTransformBehavior), new PropertyMetadata(2d));

    public ContentTransformBehavior() : base()
    {
        

        this.MouseUp += OnMouseUp;
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseWheel += OnMouseWheel;
    }

    private void ScaleWorld(Point relativePoint, int dir)
    {
        var scaleAdd = DeltaScale * dir;
        if (Scale + scaleAdd > MaxScale || Scale + scaleAdd < MinScale)
            return;

        Position = new Point(
            Position.X - (relativePoint.X / Scale - relativePoint.X / (Scale + scaleAdd)), 
            Position.Y - (relativePoint.Y / Scale - relativePoint.Y / (Scale + scaleAdd)));

        Scale += scaleAdd;
    }

    private bool _isDragging;
    private Point _startMousePoint;
    private Point _startBodyPoint;

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScaleWorld(e.MouseDevice.GetPosition(this), e.Delta / 120);
    }

    private bool UpdateDrag(MouseEventArgs args)
    {
        if (!_isDragging) return false;
        return _isDragging = args.MiddleButton == MouseButtonState.Pressed;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        Debug.WriteLine(UpdateDrag(e));
        if (!UpdateDrag(e)) // middle mouse check is not working
                return;

        var curPoint = e.GetPosition(this);

        var delta = new Point((curPoint.X - _startMousePoint.X) / Scale, (curPoint.Y - _startMousePoint.Y / Scale));
        Position = new(_startBodyPoint.X + delta.X, _startBodyPoint.Y + delta.Y);
        
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            if (UpdateDrag(e))
            {
                _startMousePoint = e.GetPosition(this);
                _startBodyPoint = Position;
            }
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            _isDragging = e.MiddleButton == MouseButtonState.Pressed;
        }
    }
}

