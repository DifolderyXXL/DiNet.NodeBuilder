using DiNet.NodeBuilder.Core.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DiNet.NodeBuilder.Core.Reflection.Helpers;
public static class Helper
{
    public static object? GetTupleValue(ITuple tuple, int index)
    {
        return tuple[index];
    }

    public static IEnumerable<Type> GetAllTypes()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
                yield return type;
    }

    public static IEnumerable<Type> GetNodeCommandTypes()
    {
        foreach (var type in GetAllTypes())
            if (GetNodeMethods(type).Any())
                yield return type;
    }

    public static IEnumerable<MethodInfo> GetNodeMethods(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

        foreach (var method in methods)
            if (method.GetCustomAttribute<NodeMethodAttribute>() != null)
                yield return method;
    }
}
