using System.Collections.Concurrent;
using System.Threading;

namespace Supermodel.DataAnnotations.LogicalContext;

public static class HttpCallContext
{
    //Code is taken from https://www.cazzulino.com/callcontext-netstandard-netcore.html and slightly modified

    #region Methods
    public static void LogicalSetData(string key, object? data)
    {
        _state.GetOrAdd(key, _ => new AsyncLocal<object?>()).Value = data;
    }
    public static object? LogicalGetData(string key)
    {
        return _state.TryGetValue(key, out AsyncLocal<object?> data) ? data.Value : null;
    }
    #endregion

    #region Private Fields
    private static readonly ConcurrentDictionary<string, AsyncLocal<object?>> _state = new();
    #endregion
}