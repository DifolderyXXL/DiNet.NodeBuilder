using DiNet.NodeBuilder.Core.Attributes;
using System.Numerics;

namespace DiNet.NodeBuilder.Tests;

class NodeMethodCommands
{
    [NodeMethod]
    public (float, float) OpenVector(Vector2 vector)
    {
        return (vector.X, vector.Y);
    }

    [NodeMethod]
    public Vector2 PackVector(float x, float y)
    {
        return new(x ,y);
    }

    [NodeMethod]
    public float GetPower(float value)
    {
        return value * value;
    }
}
