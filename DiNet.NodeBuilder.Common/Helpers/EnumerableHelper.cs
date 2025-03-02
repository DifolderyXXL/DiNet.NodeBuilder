namespace DiNet.NodeBuilder.Common.Helpers;
public static class EnumerableHelper
{
    public static bool TryFind<T>(this IEnumerable<T> collection, Func<T, bool> match, out T result)
    {
        var res = collection.FirstOrDefault(match);
        result = (res ?? default)!;
        return res is not null;
    }
}
