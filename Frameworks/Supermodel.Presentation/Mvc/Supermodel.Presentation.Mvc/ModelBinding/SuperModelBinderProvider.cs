using Microsoft.AspNetCore.Mvc.ModelBinding;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.ModelBinding;

public class SuperModelBinderProvider : IModelBinderProvider  
{  
    public IModelBinder? GetBinder(ModelBinderProviderContext context)  
    {  
        if (typeof(ISupermodelModelBinder).IsAssignableFrom(context.Metadata.ModelType)) return (ISupermodelModelBinder)ReflectionHelper.CreateType(context.Metadata.ModelType);  
  
        return null;  
    }  
}