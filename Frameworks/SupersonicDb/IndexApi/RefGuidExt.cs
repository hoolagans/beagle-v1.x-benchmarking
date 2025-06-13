using System;
using System.Runtime.CompilerServices;

namespace Supersonic.IndexApi;

public static class RefGuidExt
{
    #region Embedded Types
    // ReSharper disable once ClassNeverInstantiated.Local
    private class RefGuid
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
    #endregion

    #region Methods
    public static Guid GetRefGuid<T>(this T obj) where T : class
    {
        if (obj == null) return default(Guid);
        return _ids.GetOrCreateValue(obj).Id;
    }
    #endregion

    #region Properties
    private static readonly ConditionalWeakTable<object, RefGuid> _ids = new();
    #endregion
}