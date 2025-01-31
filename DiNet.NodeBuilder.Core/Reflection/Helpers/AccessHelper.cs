using DiNet.NodeBuilder.Core.Primitives;
using System.Reflection;

namespace DiNet.NodeBuilder.Core.Reflection.Helpers;

public static class AccessHelper
{
    public static readonly Lazy<ConstructorInfo> ValueGroupConstructor =
        new(() => typeof(ValueGroup).GetConstructor([typeof(ValueBox[])])!);
    public static readonly Lazy<ConstructorInfo> ValueGroupEmptyConstructor =
        new(() => typeof(ValueGroup).GetConstructor([])!);

    public static readonly Lazy<ConstructorInfo> ValueBoxConstructor =
        new(() => typeof(ValueBox).GetConstructor([typeof(object), typeof(Type)])!);

    public static readonly Lazy<PropertyInfo> GroupProperty =
        new(() => typeof(ValueGroup).GetProperty("Group")!);

    public static readonly Lazy<FieldInfo> ValueBoxObj =
        new(() => typeof(ValueBox).GetField("obj")!);


    public static readonly Lazy<MethodInfo> HelperTupleIndexAccess =
        new(() => typeof(Helper).GetMethod(nameof(Helper.GetTupleValue))!);
}