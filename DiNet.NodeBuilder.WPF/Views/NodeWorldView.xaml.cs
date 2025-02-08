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


    public readonly static DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(NodeWorldView), new PropertyMetadata(null));
    public IEnumerable? ItemsSource
    {
        get { return GetValue(ItemsSourceProperty) as IEnumerable; }
        set { SetValue(ItemsSourceProperty, value); }
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

    private NodeDragMachine? _drag;

    public NodeWorldView()
    {
        InitializeComponent();

        DataContext = new NodeWorldViewModel();

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

        DependencyPropertyDescriptor.FromProperty(LocalScaleProperty, typeof(NodeWorldView)).AddValueChanged(this, OnLocalScaleChanged);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        
        base.OnPropertyChanged(e);
        return;
        switch (e.Property.Name)
        {
            case nameof(ItemsSource):
                if (e.OldValue is not null)
                {
                    ClearItemsSource(e.OldValue as ObservableCollection<NodeViewModel>);
                    (e.OldValue as ObservableCollection<NodeViewModel>)!.CollectionChanged -= ItemsSourceChanged;
                }

                var collection = (ItemsSource as ObservableCollection<NodeViewModel>)!;
                
                InitItemsSource(collection);

                collection.CollectionChanged += ItemsSourceChanged;
                break;
        }

        
    }

    private Dictionary<int, NodeView> _childNodes = [];

    private void ItemsSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        
        if(e.OldItems is not null)
        foreach (var node in e.OldItems) 
        {
            var vm = node as NodeViewModel;
            if (vm is not null)
                RemoveNode(vm);
        }

        if (e.NewItems is not null)
        foreach (var node in e.NewItems)
        {
            var vm = node as NodeViewModel;
            if (vm is not null)
                AddNewNode(vm);
        }
    }

    private void RemoveNode(NodeViewModel vm)
    {

        //WorldGrid.Children.Remove(_childNodes[vm.Id]);
        _childNodes.Remove(vm.Id);
    }

    private void AddNewNode(NodeViewModel vm)
    {
        var instance = new NodeView() { DataContext = vm, VerticalAlignment=VerticalAlignment.Top, HorizontalAlignment=HorizontalAlignment.Left };
        //WorldGrid.Children.Add(instance);
        _childNodes.Add(vm.Id, instance);
    }

    private void ClearItemsSource(ObservableCollection<NodeViewModel> source)
    {
        foreach (var element in source)
            RemoveNode(element);
    }

    private void InitItemsSource(ObservableCollection<NodeViewModel> source)
    {
        foreach (var elements in source)
            AddNewNode(elements);
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
        _drag?.MouseMove(sender, e);
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
        _drag?.MouseUp(sender, e);
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
        ScaleWorld(e.MouseDevice.GetPosition(this), e.Delta / 120);
    }

    private void NodeView_OnHeaderPressed(object sender, MouseButtonEventArgs e)
    {
        _drag = new(sender as NodeView, this, ToWorldCursor);
        _drag.MouseDown(sender, e);
    }
}

public class NodeDragMachine(NodeView c_target, UIElement c_parent, Func<Point, Point> c_convertToWorldCoordinates)
{
    private bool _mousePressed;
    private Point _pressPoint;
    private Point _nodePressPoint;

    private NodeView _target = c_target;
    private UIElement _parent = c_parent;
    private Func<Point, Point> _convertToWorldCoordinate = c_convertToWorldCoordinates;

    public void MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_mousePressed)
        {
            _pressPoint = _convertToWorldCoordinate(e.GetPosition(_parent));

            var dc = (_target.DataContext as NodeViewModel)!;
            _nodePressPoint = new(dc.PositionX, dc.PositionY);
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

            var dc = (_target.DataContext as NodeViewModel)!;
            dc.PositionX = (float)(_nodePressPoint.X + pos.X - _pressPoint.X);
            dc.PositionY = (float)(_nodePressPoint.Y + pos.Y - _pressPoint.Y);
        }
    }

    public void MouseUp(object sender, MouseButtonEventArgs e)
    {
        _mousePressed = false;
    }
}
