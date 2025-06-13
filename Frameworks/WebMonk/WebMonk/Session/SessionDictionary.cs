using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WebMonk.Session;

public class SessionDictionary
{
    #region Methods
    public bool Contains(string key)
    {
        return Dict.ContainsKey(key);
    }
    public bool Remove(string key)
    {
        return Dict.Remove(key, out _);
    }
    public bool TryGetValue(string key, out object? value)
    {
        return Dict.TryGetValue(key, out value);
    }
    public object? this[string key]
    {
        get
        {
            if (Dict.TryGetValue(key, out var result)) return result;
            else return null;
        }
        set
        {
            if (value != null) Dict[key] = value;
            else Dict.Remove(key, out _);
        }
    }
    #endregion

    #region Properties
    protected ConcurrentDictionary<string, object> Dict{ get; } = new();
    #endregion
}