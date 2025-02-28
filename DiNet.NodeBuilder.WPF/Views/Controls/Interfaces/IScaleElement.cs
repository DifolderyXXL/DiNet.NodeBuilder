using System.Windows;

namespace DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;

public interface IScaleElement
{

    public double MaxScale { get; set; }

    public double MinScale { get; set; }

    public double Scale { get; }

    public bool IsMaximumScaled => Scale >= MaxScale;
    public bool IsMinimumScaled => Scale <= MinScale;
    public void ScaleElement(Point position, double delta);
}