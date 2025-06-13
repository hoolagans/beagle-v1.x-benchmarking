using Supermodel.DataAnnotations.Enums;
using Supermodel.DataAnnotations.Exceptions;
using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Extensions;

public static class ShowValidationSummaryHelper
{
    public static bool ShouldShowValidationSummary(object model, ValidationSummaryVisible validationSummaryVisible)
    {
        switch (validationSummaryVisible)
        {
            case ValidationSummaryVisible.IfNoVisibleErrors:
            {
                var selectedId = ParseNullableLong(HttpContext.Current.HttpListenerContext.Request.QueryString["selectedId"]!);
                var showValidationSummary = !HttpContext.Current.ValidationResultList.IsValid && selectedId == null;
                foreach (var propertyInfo in model.GetType().GetDetailPropertyInfosInOrder())
                {
                    var msg = Render.ValidationMessage(model, propertyInfo.Name);
                    if (!(msg is Tags tags && tags.Count == 0)) showValidationSummary = false;
                }
                return showValidationSummary;
            }
            case ValidationSummaryVisible.Always: 
            { 
                return true;
            }
            case ValidationSummaryVisible.Never:
            {
                return false;
            } 
            default:
            {
                throw new SupermodelException($"Invalid ValidationSummaryVisible value {validationSummaryVisible}");
            }
        }
    }

    #region Helper Methods
    public static long? ParseNullableLong(string str)
    {
        if (long.TryParse(str, out var result)) return result;
        return null;
    }
    #endregion
}