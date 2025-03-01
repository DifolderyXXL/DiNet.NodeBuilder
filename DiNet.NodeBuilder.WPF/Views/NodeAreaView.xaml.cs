using DiNet.NodeBuilder.WPF.ViewModels;
using DiNet.NodeBuilder.WPF.Views.Controls;
using DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiNet.NodeBuilder.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для NodeAreaView.xaml
    /// </summary>
    public partial class NodeAreaView : Grid, IMoveElement, IScaleElement
    {
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(Point), typeof(NodeAreaView), new PropertyMetadata(new Point()));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(NodeAreaView), new PropertyMetadata(1d));

        public double DeltaScale
        {
            get { return (double)GetValue(DeltaScaleProperty); }
            set { SetValue(DeltaScaleProperty, value); }
        }
        public static readonly DependencyProperty DeltaScaleProperty =
            DependencyProperty.Register(nameof(DeltaScale), typeof(double), typeof(NodeAreaView), new PropertyMetadata(0.1d));

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }
        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(NodeAreaView), new PropertyMetadata(0.15d));

        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }
        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(NodeAreaView), new PropertyMetadata(2d));

        public IEnumerable? Elements
        {
            get { return (IEnumerable)GetValue(ElementsProperty); }
            set { SetValue(ElementsProperty, value); }
        }
        public static readonly DependencyProperty ElementsProperty =
            DependencyProperty.Register(nameof(Elements), typeof(IEnumerable), typeof(NodeAreaView), new PropertyMetadata(null));

        private List<NodeView> _nodes = [];

        public ElementController Controller { get; }

        public NodeAreaView()
        {
            InitializeComponent();

            Controller = new();
            Controller.BeginScaling(this);
        }

        

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(Elements))
            {
                if (e.OldValue is ObservableCollection<NodeViewModel> oldCollection)
                    oldCollection.CollectionChanged -= OnContentChanged;
                if (e.NewValue is ObservableCollection<NodeViewModel> newCollection)
                    newCollection.CollectionChanged += OnContentChanged;

                if (e.NewValue is IEnumerable<NodeViewModel> enumerable)
                    SetContent(enumerable);
            }

            base.OnPropertyChanged(e);
        }

        private NodeView CreateNewNode(object? vm)
            => new NodeView(this) { DataContext = vm, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

        private void SetContent(IEnumerable<NodeViewModel>? nodeViewModels)
        {
            if (nodeViewModels is null)
                return;

            NodeContent.Children.Clear();
            _nodes.Clear();

            foreach (var item in nodeViewModels)
            {
                var node = CreateNewNode(item);

                NodeContent.Children.Add(node);
                _nodes.Add(node);
            }
        }

        private void OnContentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems!)
                    {
                        var node = CreateNewNode(item);

                        NodeContent.Children.Add(node);
                        _nodes.Add(node);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems!)
                    {
                        var node = _nodes.FirstOrDefault(x => x.DataContext == item);
                        if (node is null)
                            continue;

                        _nodes.Remove(node);
                        NodeContent.Children.Remove(node);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
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
            Controller.InvokeScale(e.MouseDevice.GetPosition(Content), e.Delta > 0 ? 1.1 : 0.9);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Controller.ContainsLineElement())
                Controller.UpdateLine(e.GetPosition(Content));

            if (e.LeftButton == MouseButtonState.Released
                && Controller.ContainsMoveElement()
                && !Controller.ContainsSpecificMoveElement(this))
                Controller.EndMovement();

            if (e.MiddleButton == MouseButtonState.Released
                && Controller.ContainsSpecificMoveElement(this))
                Controller.EndMovement();

            if (Controller.ContainsMoveElement())
            {
                Controller.InvokeMovement(
                    e.GetPosition(
                        Controller.ContainsSpecificMoveElement(this)
                        ? this
                        : Content));
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
            if (e.ChangedButton == MouseButton.Left
                && !Controller.ContainsSpecificMoveElement(this))
                Controller.EndMovement();

            if (e.ChangedButton == MouseButton.Middle
                && Controller.ContainsSpecificMoveElement(this))
            {
                Controller.EndMovement();
            }
        }
    }
}
