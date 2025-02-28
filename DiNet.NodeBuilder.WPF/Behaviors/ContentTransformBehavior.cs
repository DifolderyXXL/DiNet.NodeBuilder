using DiNet.NodeBuilder.WPF.Views.Controls;
using DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiNet.NodeBuilder.WPF.Behaviors;
public class ContentTransformBehavior : Grid, IMoveElement, IScaleElement
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
        DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(ContentTransformBehavior), new PropertyMetadata(0.15d));

    public double MaxScale
    {
        get { return (double)GetValue(MaxScaleProperty); }
        set { SetValue(MaxScaleProperty, value); }
    }

    public static readonly DependencyProperty MaxScaleProperty =
        DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(ContentTransformBehavior), new PropertyMetadata(2d));


    private MatrixTransform? matrixTransform;

    public ElementController Controller { get; }

    public ContentTransformBehavior()
    {
        Controller = new();
        Controller.BeginScaling(this);

        this.MouseUp += OnMouseUp;
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseWheel += OnMouseWheel;
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        matrixTransform = new MatrixTransform();
        this.Children[0].RenderTransform = matrixTransform;
    }

    public void MoveElement(Vector offset)
    {
        if (matrixTransform is null) return;

        var matrix = matrixTransform.Matrix;
        offset.Negate();
        matrix.Translate(offset.X, offset.Y);
        matrixTransform.Matrix = matrix;
        Position = new(matrix.OffsetX, matrix.OffsetY);
    }

    public void ScaleElement(Point position, double delta)
    {
        if (matrixTransform is null) return;

        var matrix = matrixTransform.Matrix;

        matrix.ScaleAtPrepend(delta, delta, position.X, position.Y);
        matrixTransform.Matrix = matrix;
        Scale = matrix.M11;
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        Controller.InvokeScale(e.MouseDevice.GetPosition(this.Children[0]), e.Delta > 0 ? 1.1 : 0.9);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Released)
            Controller.EndMovement();

        if (Controller.ContainsMoveElement())
        {
            Controller.InvokeMovement(e.GetPosition(this));
        }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            Controller.BeginMovement(e.GetPosition(this), this);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            Controller.EndMovement();
        }
    }
}

