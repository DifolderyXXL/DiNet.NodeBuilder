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

namespace DiNet.NodeBuilder.WPF.Views;
/// <summary>
/// Логика взаимодействия для CanvasView.xaml
/// </summary>
public partial class CanvasView : UserControl
{
    public object InnerContent
    {
        get { return GetValue(InnerContentProperty); }
        set { SetValue(InnerContentProperty, value); }
    }

    public static readonly DependencyProperty InnerContentProperty =
        DependencyProperty.Register("InnerContent", typeof(object), typeof(CanvasView), new PropertyMetadata(null));

    public CanvasView()
    {
        InitializeComponent();
    }
}
