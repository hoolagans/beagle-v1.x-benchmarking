using System;
using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.DataContext.Core;

public interface ICachedDataContext
{
    int CacheAgeToleranceInSeconds { get; set; }
    Task PurgeCacheAsync(int? cacheExpirationAgeInSeconds = null, Type modelType = null);
}