using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class HtmlContentExtensions
{
    #region Methods
    public static HtmlString Replace(this IHtmlContent content, string str1, string str2)
    {
        return content.GetString().Replace(str1, str2).ToHtmlString();
    }
    public static string GetString(this IHtmlContent content)
    {
        using (var writer = new System.IO.StringWriter())
        {        
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        } 
    }   
    public static HtmlString DisableAllControls(this IHtmlContent htmlStr)
    {
        return htmlStr.ToStringHandleNull().DisableAllControls().ToHtmlString();
    }
    public static HtmlString DisableAllControlsIf(this IHtmlContent htmlStr, bool condition)
    {
        return htmlStr.ToStringHandleNull().DisableAllControlsIf(condition).ToHtmlString();
    }
    public static string ToStringHandleNull(this IHtmlContent? htmlStr)
    {
        return htmlStr != null ? htmlStr.GetString() : "";
    }
    #endregion
}