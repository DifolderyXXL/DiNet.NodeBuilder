using DiNet.NodeBuilder.WPF.Logging;
using DiNet.NodeBuilder.WPF.Views.Controls.Interfaces;
using System.Windows;
using System.Windows.Shapes;

namespace DiNet.NodeBuilder.WPF.Views.Controls;
public class ElementController(ILogger? c_logger = null)
{
    private IMoveElement? _moveElement;
    private IScaleElement? _scaleElement;

    private Point _oldMovementPosition;

    private bool _isMovement = false;
    private bool _isScaling = false;

    private ILogger? _logger = c_logger;

    private Line? _line;

    public bool ContainsSpecificMoveElement(IMoveElement element)
        => _moveElement is not null && _moveElement == element;

    public bool ContainsMoveElement()
        => _moveElement is not null;

    public bool ContainsScaleElement()
        => _scaleElement is not null;

    public Line? CurrentLine => _line;
    public bool ContainsLineElement()
        => _line is not null;

    public void BeginLineMove(Line line)
    {
        _line = line;
    }
    public void UpdateLine(Point position)
    {
        if (_line is null)
            return;

        _line.X2 = position.X;
        _line.Y2 = position.Y;
    }
    public void EndLineMove()
    {
        _line = null;
    }


    public void BeginMovement(Point position, IMoveElement element)
    {
        if(_isMovement)
        {
            _logger?.Log("Movement already started;");
            return;
        }

        _moveElement = element;
        _oldMovementPosition = position;
        _isMovement = true;
    }

    public void EndMovement()
    {
        _moveElement = null;
        _isMovement = false;

        _logger?.Log("Movement ended;");
    }

    public void BeginScaling(IScaleElement element)
    {
        if (_isScaling)
        {
            _logger?.Log("Scaling already started;");
            return;
        }

        _scaleElement = element;
        _isScaling = true;
    }

    public void EndScaling()
    {
        _scaleElement = null;
        _isScaling = false;
    }

    public void InvokeMovement(Point position)
    {
        if (!_isMovement)
            return;

        var offset = position - _oldMovementPosition;
        _oldMovementPosition = position;
        offset.Negate();

        _moveElement?.MoveElement(offset);
    }

    public void InvokeScale(Point position, double delta)
    {
        if (!_isScaling)
            return;

        if (delta > 1 && _scaleElement!.IsMaximumScaled)
            return;
        if (delta < 1 && _scaleElement!.IsMinimumScaled)
            return;

        _scaleElement?.ScaleElement(position, delta);
    }
}

