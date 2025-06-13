using System.Collections.Generic;
using System.Threading.Tasks;
using WebMonk.ValueProviders;

namespace WebMonk.Context;

public interface IValueProviderManager
{
    Task<List<IValueProvider>> GetValueProvidersListAsync();
    List<IValueProvider>? GetCachedValueProvidersList();
}