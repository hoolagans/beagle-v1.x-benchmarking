using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.ModeBinding;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Models.Api;

public abstract class SearchApiModel : ApiModel, ISelfModelBinder
{
    public async Task<object?> BindMeAsync(Type rootType, List<IValueProvider>? valueProviders = null)
    {
        foreach (var property in GetType().GetProperties().Where(p => p.GetSetMethod() != null && p.GetGetMethod() != null && !p.HasAttribute<JsonIgnoreAttribute>()))
        {
            valueProviders ??= await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);
            var queryStringValueProvider = valueProviders.GetFirstOrDefaultValueProviderOfType<QueryStringValueProvider>();
            if (queryStringValueProvider == null) throw new SupermodelException("queryStringValueProvider == null");

            var valResult = queryStringValueProvider.GetValueOrDefault(property.Name, property.PropertyType);
            if (!valResult.ValueMissing) this.PropertySet(property.Name, valResult.Value);
        }
        return this;
    }
}