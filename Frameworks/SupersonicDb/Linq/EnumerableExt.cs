using System;
using System.Collections.Generic;
using Supersonic.GC;

namespace Supersonic.Linq;

public static class EnumerableExt
{
    //TODO: Add this back later if neeeded
    //public static ConcurrentSupersonicList<ItemT> ToConcurrentSupersonicList<ItemT>(this IEnumerable<ItemT> me) where ItemT : class
    //{
    //    using (new SustainedLowLatencyGC())
    //    {
    //        return new ConcurrentSupersonicList<ItemT>(me);
    //    }
    //}

    public static SupersonicList<TItem> ToSupersonicList<TItem>(this IEnumerable<TItem> me) where TItem : class
    {
        using (new SustainedLowLatencyGC())
        {
            return new SupersonicList<TItem>(me);
        }
    }

    internal static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var retVal = 0;
        foreach (var item in items)
        {
            if (predicate(item)) return retVal;
            retVal++;
        }
        return -1;
    }
    internal static int IndexOf<T>(this IEnumerable<T> items, T item)
    {
        return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
    }
}