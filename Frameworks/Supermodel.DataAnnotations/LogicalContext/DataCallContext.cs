using System.Collections.Concurrent;
using System.Threading;

namespace Supermodel.DataAnnotations.LogicalContext;

public static class DataCallContext
{
    #region Methods
    public static object? LogicalGetData(string key)
    {
        if (!_logicalContextDictionary.ContainsKey(key)) return null;
        return _logicalContextDictionary[key];
    }
    public static void LogicalSetData(string key, object? value)
    {
        _logicalContextDictionary[key] = value;
    }
    // ReSharper disable once InconsistentNaming
    private static ConcurrentDictionary<string, object?> _logicalContextDictionary => _logicalData.Value ?? (_logicalData.Value = new ConcurrentDictionary<string, object?>());

    #endregion

    #region Private Variables
    private static readonly AsyncLocal<ConcurrentDictionary<string, object?>> _logicalData = new();
    #endregion
}