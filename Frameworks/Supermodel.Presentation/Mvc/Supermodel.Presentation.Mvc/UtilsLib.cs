using System.Text;
using Microsoft.AspNetCore.Html;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Extensions;

namespace Supermodel.Presentation.Mvc;

public static class UtilsLib
{
    public static bool IsNullOrEmpty(IHtmlContent? content)
    {
        return content == null || string.IsNullOrEmpty(content.GetString());
    }
        
    public static string GenerateAttributesString(AttributesDict? dict)
    {
        if (dict == null) return " ";
        var sb = new StringBuilder();
        foreach (var pair in dict) 
        {
            if (pair.Value != null) sb.Append($"{pair.Key}=\"{pair.Value?.Replace("\"", "&quot;")}\"");
        }
        return $" {sb.ToString().Trim()} ";
    }

    public static HtmlString MakeIdAndClassAttributes(string? id, string? cssClass)
    {
        return (MakeIdAttribute(id).GetString() + MakeClassAttribute(cssClass)).ToHtmlString();
    }

    public static HtmlString MakeIdAttribute(string? id)
    {
        return (string.IsNullOrEmpty(id) ? "" : $" Id='{id}' ").ToHtmlString();
    }

    public static HtmlString MakeClassAttribute(string? cssClass)
    {
        return (string.IsNullOrEmpty(cssClass) ? "" : $" class='{cssClass}' ").ToHtmlString();
    }
}