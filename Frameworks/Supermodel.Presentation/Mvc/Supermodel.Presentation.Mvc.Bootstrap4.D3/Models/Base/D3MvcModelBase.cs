using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.ModelBinding;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.Models.Base;

public abstract class D3MvcModelBase : ISupermodelEditorTemplate, ISupermodelDisplayTemplate, ISupermodelHiddenTemplate, ISupermodelModelBinder
{
    #region Methods
    public abstract bool ContainsData();
    public abstract string GenerateD3Script(string containerId);
    public virtual string GenerateContainerTag(string containerId, AttributesDict attributesAsCaseInsensitiveDict)
    {
        return $"<span {UtilsLib.GenerateAttributesString(attributesAsCaseInsensitiveDict)}></span>";
    }
    #endregion

    #region ISupermodelEditorTemplate implementation
    public virtual IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        return DisplayTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
    }
    #endregion

    #region ISupermodelDisplayTemplate implementation
    public virtual IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (!ContainsData()) return "".ToHtmlEncodedHtmlString();

        //get the id for our property 
        var svgId = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName("").Replace(".", "_");

        //make a local, case insensitive version of the dict
        var svgTagAttributesDict = new AttributesDict(DivTagAttributesAsDict);
            
        //If id is not already there, we use one from our property name
        if (svgTagAttributesDict.TryGetValue("id", out var value)) svgId = value!;
        else svgTagAttributesDict["id"] = svgId;

        return new HtmlString($@"
                    {GenerateContainerTag(svgId, svgTagAttributesDict)}
                    {GenerateD3Script(svgId)}");
    }
    #endregion

    #region ISupermodelHiddenTemplate implementation
    public virtual IHtmlContent HiddenTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        throw new SupermodelException("D3MvcModel cannot be used as hidden.");
    }
    #endregion

    #region ISuperModelBinder implementation
    public virtual Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var originalValue = (D3MvcModelBase)bindingContext.Model!;
        bindingContext.Result = ModelBindingResult.Success(originalValue);
        return Task.CompletedTask;
    }
    #endregion

    #region Properties
    public object DivTagAttributesAsObj { set => DivTagAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
    public AttributesDict DivTagAttributesAsDict { get; set; } = new();
    #endregion
}