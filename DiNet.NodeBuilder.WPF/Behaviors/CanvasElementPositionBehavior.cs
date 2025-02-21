using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DiNet.NodeBuilder.WPF.Behaviors
{
    class CanvasElementPositionBehavior : UIElement
    {
        public static readonly DependencyProperty HotspotProperty =
        DependencyProperty.Register(nameof(Hotspot), typeof(Point), typeof(CanvasElementPositionBehavior));
        public Point Hotspot
        {
            get => (Point)GetValue(HotspotProperty);
            set => SetValue(HotspotProperty, value);
        }

        public static readonly DependencyProperty AncestorProperty =
            DependencyProperty.Register(nameof(Ancestor),
            typeof(FrameworkElement), typeof(CanvasElementPositionBehavior),
                    new FrameworkPropertyMetadata(Ancestor_PropertyChanged));
        public FrameworkElement Ancestor
        {
            get => (FrameworkElement)GetValue(AncestorProperty);
            set => SetValue(AncestorProperty, value);
        }

        private static void Ancestor_PropertyChanged
            (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (CanvasElementPositionBehavior)d;
            c.UpdateHotspot();
        }

        public CanvasElementPositionBehavior()
        {
            this.LayoutUpdated += new EventHandler(ConnectorItem_LayoutUpdated);
        }

        private void ConnectorItem_LayoutUpdated(object? sender, EventArgs e)
        {
            UpdateHotspot();
        }

        private void UpdateHotspot()
        {
            if (this.Ancestor == null)
                return;

            //var center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            var center = new Point(0, 0);

            var centerRelativeToAncestor = this.TransformToAncestor(this.Ancestor).Transform(center);

            this.Hotspot = centerRelativeToAncestor;
        }
    }
}
