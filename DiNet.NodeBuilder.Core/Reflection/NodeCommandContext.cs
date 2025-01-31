using DiNet.NodeBuilder.Core.Primitives;
using DiNet.NodeBuilder.Core.Reflection.Generators;
using DiNet.NodeBuilder.Core.Reflection.Helpers;
using System.Reflection;

namespace DiNet.NodeBuilder.Core.Reflection;
public class NodeCommandContext
{
    public static readonly NodeCommandContext Shared = new();

    private Dictionary<Type, CachedNodeCommandData> _cache = [];

    private Dictionary<string, (Type type, MethodInfo method)> _methodInstances = [];

    public void LoadNodeCommands()
    {
        foreach(var type in Helper.GetNodeCommandTypes())
            TryGenerateType(type);
    }

    public void TryGenerateType(Type type)
    {
        if (!_cache.ContainsKey(type))
            _cache.Add(type, new(Activator.CreateInstance(type)!, []));

        TryGenerateMethods(type);
    }

    public void TryGenerateMethods(Type type)
    {
        var data = _cache[type];
        foreach (var method in Helper.GetNodeMethods(type))
        {
            if (!data.funcs.ContainsKey(method))
                data.funcs.Add(method, new(
                    NodeCommandGenerator.Shared.GenerateLambda(data.instance, type, method),
                    NodeCommandGenerator.GetInputTypes(method),
                    NodeCommandGenerator.GetOutputTypes(method)));

            if (!_methodInstances.ContainsKey(method.Name))
                _methodInstances.Add(method.Name, (type, method));
        }
    }

    public NodeCommand GetFuncDataByName(string name)
    {
        if (!_methodInstances.ContainsKey(name))
            throw new ArgumentException();

        var association = _methodInstances[name];
        return _cache[association.type].funcs[association.method];
    }
}

public record struct CachedNodeCommandData(
    object instance, 
    Dictionary<MethodInfo, NodeCommand> funcs);
