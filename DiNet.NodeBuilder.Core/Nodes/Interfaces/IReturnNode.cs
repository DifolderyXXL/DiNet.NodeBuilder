namespace DiNet.NodeBuilder.Core.Nodes.Interfaces;

public interface IReturnNode : IEnterNode
{
    public void SetReturnTypes(Type[] types);
}