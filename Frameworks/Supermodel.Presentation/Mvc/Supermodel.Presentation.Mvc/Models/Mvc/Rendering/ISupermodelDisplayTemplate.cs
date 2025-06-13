using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Supermodel.Presentation.Mvc.Models.Mvc.Rendering;

public interface ISupermodelDisplayTemplate
{
    IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null);    
}