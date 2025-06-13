using System.Collections;
using System.Collections.Generic;

namespace Supermodel.ReflectionMapper;

public static class CollectionExtensions
{
    public static void ClearCollection(this ICollection me)
    {
        me.ExecuteMethod(nameof(ICollection<object?>.Clear));
    }
    public static void AddToCollection(this ICollection me, object? item)
    {
        me.ExecuteMethod(nameof(ICollection<object?>.Add), item);
    }
    public static void RemoveFromCollection(this ICollection me, object? item)
    {
        me.ExecuteMethod(nameof(ICollection<object?>.Remove), item);
    }
}