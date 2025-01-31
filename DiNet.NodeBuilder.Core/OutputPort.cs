
using DiNet.NodeBuilder.Core.Nodes;

namespace DiNet.NodeBuilder.Core;

public class OutputPort : PortBase
{
    public OutputPort(int id, Node parent, Type type) : base(id, parent, type)
    {
    }
}
