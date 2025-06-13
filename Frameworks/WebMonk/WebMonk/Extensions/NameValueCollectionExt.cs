using System.Collections.Generic;
using System.Collections.Specialized;
using WebMonk.Misc;

namespace WebMonk.Extensions;

public static class NameValueCollectionExt
{
    public static Dictionary<string, object> ToValueProviderDictionary(this NameValueCollection me)
    {
        var values = new Dictionary<string, object>();
        foreach (string? key in me.Keys)
        {
            if (key == null) continue;

            if (values.ContainsKey(key))
            {
                var currentDictValue = values[key];
                if (currentDictValue is IList<string> list) list.Add(me[key]);
                else values[key] = new List<string> { (string)currentDictValue, me[key] };
            }
            else
            {
                values.Add(key, me[key]);
            }
        }
        return values;
    }
    public static QueryStringDict ToQueryStringDictionary(this NameValueCollection me)
    {
        var values = new QueryStringDict();
        foreach (string key in me.Keys)
        {
            values.TryAdd(key, me[key]);
        }
        return values;
    }
}