using System;
using System.Collections.Generic;
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

namespace DiNet.NodeBuilder.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для CanvasPositionUpdater.xaml
    /// </summary>
    public partial class CanvasPositionUpdater : UserControl
    {
        public static readonly DependencyProperty HotspotProperty =
        DependencyProperty.Register(nameof(Hotspot), typeof(Point), typeof(CanvasPositionUpdater));
        public Point Hotspot
        {
            get => (Point)GetValue(HotspotProperty);
            set => SetValue(HotspotProperty, value);
        }

        public static readonly DependencyProperty AncestorProperty =
            DependencyProperty.Register(nameof(Ancestor),
            typeof(FrameworkElement), typeof(CanvasPositionUpdater),
                    new FrameworkPropertyMetadata(Ancestor_PropertyChanged));
        public FrameworkElement Ancestor
        {
            get => (FrameworkElement)GetValue(AncestorProperty);
            set => SetValue(AncestorProperty, value);
        }

        private static void Ancestor_PropertyChanged
            (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (CanvasPositionUpdater)d;
            c.UpdateHotspot();
        }

        public CanvasPositionUpdater()
        {
            InitializeComponent();

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

            var center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

            var centerRelativeToAncestor = this.TransformToAncestor(this.Ancestor).Transform(center);

            this.Hotspot = centerRelativeToAncestor;
        }
    }
}
