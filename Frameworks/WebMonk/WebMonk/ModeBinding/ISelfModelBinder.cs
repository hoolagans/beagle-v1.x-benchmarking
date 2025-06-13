using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMonk.ValueProviders;

namespace WebMonk.ModeBinding;

public interface ISelfModelBinder
{
    Task<object?> BindMeAsync(Type rootType, List<IValueProvider> valueProviders);
}