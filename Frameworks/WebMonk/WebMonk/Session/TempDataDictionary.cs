using System.Collections.Concurrent;
using System.Collections.Generic;
using WebMonk.Exceptions;

namespace WebMonk.Session;

public class TempDataDictionary
{
    #region Methods
    internal void MoveFutureIntoCurrent(string sessionId)
    {
        _currentDict = _futureDict;
        _futureDict = null;
    }
    internal void MergeCurrentIntoFuture(string sessionId)
    {
        foreach (var key in CurrentDict.Keys) 
        {
            if (!FutureDict.ContainsKey(key)) FutureDict[key] = CurrentDict[key];
        }
    }

    public bool Contains(string key)
    {
        return CurrentDict.ContainsKey(key);
    }
    public bool Remove(string key)
    {
        FutureDict.Remove(key, out _);
        return CurrentDict.Remove(key, out _);
    }
    public bool TryGetValue(string key, out object? value)
    {
        return CurrentDict.TryGetValue(key, out value);
    }
    public object? this[string key]
    {
        get
        {
            if (CurrentDict.TryGetValue(key, out var result)) return result;
            else return null;
        }
        set
        {
            if (value != null) 
            {
                CurrentDict[key] = FutureDict[key] = value;
            }
            else 
            {
                FutureDict.Remove(key, out _);
                CurrentDict.Remove(key, out _);
            }
        }
    }
    #endregion

    #region Properties
    protected ConcurrentDictionary<string, object> CurrentDict
    { 
        get 
        { 
            if (_currentDict == null) _currentDict = new ConcurrentDictionary<string, object>();
            return _currentDict ?? throw new WebMonkException("TempData: issue with concurrency.");
        } 
    }
    private ConcurrentDictionary<string, object>? _currentDict;

    protected ConcurrentDictionary<string, object> FutureDict
    { 
        get 
        { 
            if (_futureDict == null) _futureDict = new ConcurrentDictionary<string, object>();
            return _futureDict ?? throw new WebMonkException("TempData: issue with concurrency.");
        } 
    }
    private ConcurrentDictionary<string, object>? _futureDict;
    #endregion
}