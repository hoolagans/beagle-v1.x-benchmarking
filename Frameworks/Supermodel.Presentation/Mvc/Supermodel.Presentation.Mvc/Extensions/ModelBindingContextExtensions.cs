using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class ModelBindingContextExtensions
{
    public static bool IsPropertyRequired(this ModelBindingContext bindingContext)
    {
        var propInfo = bindingContext.ModelMetadata.ContainerType!.GetProperty(bindingContext.ModelMetadata.PropertyName!);
        if (propInfo == null) throw new NoNullAllowedException("propInfo == null");
        return Attribute.IsDefined(propInfo, typeof(RequiredAttribute));
    }
}