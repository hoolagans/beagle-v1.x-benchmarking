using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Supermodel.Presentation.Mvc.ModelBinding;
using Supermodel.Presentation.Mvc.ModelBinding.Microsoft.AspNetCore.Mvc.ModelBinding;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Models.Api;

public abstract class SearchApiModel : ApiModel, ISupermodelModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        bindingContext.Model = ReflectionHelper.CreateType(GetType());
        foreach (var property in bindingContext.Model.GetType().GetProperties().Where(p => p.GetSetMethod() != null && p.GetGetMethod() != null && !p.HasAttribute<JsonIgnoreAttribute>()))
        {
            var valResult = bindingContext.ValueProvider.GetValue(property.Name);
            var valObj = SuperModelBindingHelper.ConvertTo(valResult.FirstValue, property.PropertyType, CultureInfo.CurrentCulture);
            if (valObj != null) bindingContext.Model.PropertySet(property.Name, valObj);
        }
        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
        return Task.CompletedTask;
    }
}