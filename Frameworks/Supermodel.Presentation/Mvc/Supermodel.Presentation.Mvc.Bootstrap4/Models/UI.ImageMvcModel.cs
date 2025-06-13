using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class ImageMvcModel : BinaryFileMvcModel
    {
        #region IRMCustomMapper implementation
        public override Task<T> MapToCustomAsync<T>(T other)
        {
            return Task.FromResult(other); //do nothing
        }
        #endregion

        #region ISupermodelEditorTemplate implementation
        public override IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return DisplayTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
        }
        #endregion

        #region ISupermodelDisplayTemplate implementation
        public override IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            if (!(html.ViewData.Model is ImageMvcModel)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type different from ImageMvcModel.");
            var model = (ImageMvcModel)(object)html.ViewData.Model;

            //if (string.IsNullOrEmpty(model.FileName) || !html.ViewData.ModelState.IsValid) return HtmlString.Empty;
            if (model.IsEmpty) return HtmlString.Empty;

            var urlHelper = html.Super().GetUrlHelper();                    
            var imageLink = urlHelper.Content($"~/{html.ViewContext.RouteData.Values["Controller"]}/BinaryFile/{html.ViewContext.RouteData.Values["id"]}?pn={html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix.Replace($"{Config.InlinePrefix}.", "")}");
            return new HtmlString($"<div><img src='{imageLink}' {UtilsLib.GenerateAttributesString(HtmlAttributesAsDict)}/></div>");
        }
        #endregion

        #region ISupermodelModelBinder implementation
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var originalValue = (BinaryFileMvcModel)bindingContext.Model!;
            bindingContext.Result = ModelBindingResult.Success(originalValue);
            return Task.CompletedTask;
        }
        #endregion
    }
}