
using DiNet.NodeBuilder.Core.Nodes;

namespace DiNet.NodeBuilder.Core;

public class InputPort : PortBase
{
    public InputPort(int id, Node parent, Type type) : base(id, parent, type)
    {
    }
}
