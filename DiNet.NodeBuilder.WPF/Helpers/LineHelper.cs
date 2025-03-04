using System.Windows;
using System.Windows.Shapes;

namespace DiNet.NodeBuilder.WPF.Helpers;
public static class LineHelper
{
    public static void AddFirst(this Line point, Vector offset)
    {
        point.X1 += offset.X;
        point.Y1 += offset.Y;
    }
    public static void AddSecond(this Line point, Vector offset)
    {
        point.X2 += offset.X;
        point.Y2 += offset.Y;
    }

    public static void SetFirst(this Line point, Vector pos)
    {
        point.X1 = pos.X;
        point.Y1 = pos.Y;
    }
    public static void SetSecond(this Line point, Vector pos)
    {
        point.X2 = pos.X;
        point.Y2 = pos.Y;
    }

    public static void SetFirst(this Line point, Point pos)
    {
        point.X1 = pos.X;
        point.Y1 = pos.Y;
    }
    public static void SetSecond(this Line point, Point pos)
    {
        point.X2 = pos.X;
        point.Y2 = pos.Y;
    }
}