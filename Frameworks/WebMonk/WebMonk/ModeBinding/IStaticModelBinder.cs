using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMonk.ValueProviders;

namespace WebMonk.ModeBinding;

public interface IStaticModelBinder
{
    Task<object?> BindNewModelAsync(Type rootType, Type modelType, List<IValueProvider> valueProviders, bool ignoreRootObjectIModelBinder = false);
    Task<object?> BindExistingModelAsync(Type rootType, Type modelType, object? model, List<IValueProvider> valueProviders, bool ignoreRootObjectISelfModelBinder = false);
}