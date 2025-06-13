using System;
using System.Collections.Generic;
using System.Linq;
using WebMonk.Exceptions;

namespace WebMonk.ValueProviders;

public static class ValueProviderListExt
{
    #region Methods
    public static List<string>? GetIndexesWithValue(this List<IValueProvider> me, string key)
    {
        foreach (var valueProvider in me)
        {
            var value = valueProvider.GetIndexesWithValue(key);
            if (value != null) return value;
        }
        return null;
    }

#nullable disable
    public static IValueProvider.Result GetValueOrDefault<T>(this List<IValueProvider> me, string key)
    {
        return me.GetValueOrDefault(key, typeof(T));
    }
#nullable enable

    public static IValueProvider.Result GetValueOrDefault(this List<IValueProvider> me, string key, Type type)
    {
        foreach (var valueProvider in me)
        {
            var result = valueProvider.GetValueOrDefault(key, type);
            if (!result.ValueMissing) return result;
        }
        return new IValueProvider.Result(null, true);
    }
    public static IValueProvider.Result GetValueOrDefault(this List<IValueProvider> me, string key)
    {
        foreach (var valueProvider in me)
        {
            var result = valueProvider.GetValueOrDefault(key);
            if (!result.ValueMissing) return result;
        }
        return new IValueProvider.Result(null, true);
    }

    public static void ReplaceOrAppendValueProvider<TValueProvider>(this List<IValueProvider> me, TValueProvider newValueProvider) where TValueProvider: class, IValueProvider
    {
        if (!me.TryReplaceValueProvider(newValueProvider)) me.Add(newValueProvider);
    }
    public static void ReplaceOrInsertValueProvider<TValueProvider>(this List<IValueProvider> me, TValueProvider newValueProvider, int insertAt) where TValueProvider: class, IValueProvider
    {
        if (!me.TryReplaceValueProvider(newValueProvider)) me.Insert(insertAt, newValueProvider);
    }
    public static bool TryReplaceValueProvider<TValueProvider>(this List<IValueProvider> me, TValueProvider newValueProvider) where TValueProvider: class, IValueProvider
    {
        me.ValidateValueProviders();
        for(var i = 0; i < me.Count; i++)
        {
            if (me[i].GetType() == typeof(TValueProvider))
            {
                me[i] = newValueProvider;
                return true;
            }
        }
        return false;
    }
    public static TValueProvider? GetFirstOrDefaultValueProviderOfType<TValueProvider>(this List<IValueProvider> me) where TValueProvider: class, IValueProvider
    {
        return (TValueProvider?)me.FirstOrDefault(x => x.GetType() == typeof(TValueProvider));
    }

    public static void ValidateValueProviders(this List<IValueProvider> me)
    {
        if (me.GroupBy(x => x.GetType()).Any(g => g.Count() > 1)) throw new WebMonkException("Duplicate ValueProvider types");
    }
    #endregion
}