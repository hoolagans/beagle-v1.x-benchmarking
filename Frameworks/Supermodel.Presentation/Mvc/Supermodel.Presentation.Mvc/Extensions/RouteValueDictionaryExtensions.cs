using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class RouteValueDictionaryExtensions
{
    public static RouteValueDictionary AddOrUpdateWith(this RouteValueDictionary rvDict, IDictionary<string, object> other)
    {
        foreach (var pair in other) rvDict[pair.Key] = pair.Value;
        return rvDict;
    }

    public static RouteValueDictionary AddOrUpdateWith(this RouteValueDictionary rvDict, RouteValueDictionary other)
    {
        foreach (var pair in other) rvDict[pair.Key] = pair.Value;
        return rvDict;
    }
            
    public static RouteValueDictionary AddOrUpdateWith(this RouteValueDictionary rvDict, string key, object? value)
    {
        rvDict[key] = value;
        return rvDict;
    }

    public static RouteValueDictionary RemoveKey(this RouteValueDictionary rvDict, string key)
    {
        rvDict.Remove(key);
        return rvDict;
    }

    public static RouteValueDictionary RemoveKeys(this RouteValueDictionary rvDict, params string[] keys)
    {
        foreach (var key in keys) rvDict.Remove(key);
        return rvDict;
    }
}