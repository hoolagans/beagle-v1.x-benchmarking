using System;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Html;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class StringAndStringBuilderExtensions
{
    #region Methods
    public static string RemoveControllerSuffix(this string me)
    {
        if (!me.EndsWith("Controller")) throw new ArgumentException("Must end with 'Controller'");
        return me[0..^10];
    }
    public static string ReplaceOrAddAttributeValue(this string me, string attributeName, string? newAttributeValue = null)
    {
        me = me.Trim();
        return me.RemoveAttribute(attributeName).AddAttribute(attributeName, newAttributeValue);
    }
    public static string RemoveAttribute(this string me, string attributeName)
    {
        me = me.Trim();
        var (startIdx, endIdx) = me.FindAttributeIndexes(attributeName);
        if (startIdx == -1 || endIdx == -1) return me;
        var firstPart = me.Substring(0, startIdx);
        var secondPart = me.Substring(endIdx + 1, me.Length - endIdx - 1);
        return $"{firstPart} {secondPart}";
    }
    public static string AddAttribute(this string me, string attributeName, string? newAttributeValue = null)
    {
        me = me.Trim();
        if (!me.StartsWith('<') || !me.EndsWith('>')) throw new ArgumentException("Must be a valid html tag", nameof(me));
        if (attributeName.Contains(" ")) throw new ArgumentException("Invalid attribute name", nameof(attributeName));

        // ReSharper disable once UseNullPropagation
        if (newAttributeValue != null) newAttributeValue = HttpUtility.HtmlEncode(newAttributeValue);
            
        string result;
        if (newAttributeValue != null)
        {
            if (me.EndsWith("/>")) result = $"{me.Substring(0, me.Length-2)} {attributeName}='{newAttributeValue}' />";
            else result = $"{me.Substring(0, me.Length-1)} {attributeName}='{newAttributeValue}' >";
        }
        else
        {
            if (me.EndsWith("/>")) result = $"{me.Substring(0, me.Length-2)} {attributeName}' />";
            else result = $"{me.Substring(0, me.Length-1)} {attributeName}' >";
        }

        return result;
    }
    public static (int, int) FindAttributeIndexes(this string me, string attributeName)
    {
        var attributeStartIndex = me.IndexOf(attributeName, StringComparison.OrdinalIgnoreCase);
        if (attributeStartIndex < 0) return (-1, -1);

        var idx = attributeStartIndex + attributeName.Length;
        idx = me.SkipWhiteSpace(idx);
            
        if (idx == me.Length || me[idx] != '=') return (attributeStartIndex, attributeStartIndex + attributeName.Length);
        idx++;

        idx = me.SkipWhiteSpace(idx);
        var quote = me[idx];
        if (quote != '"' && quote != '\\') throw new ArgumentException("Must be a valid html tag", nameof(me));
            
        var endQuoteIdx = me.IndexOf(quote, idx+1);
        if (endQuoteIdx == -1) throw new ArgumentException("Must be a valid html tag", nameof(me));

        return (attributeStartIndex, endQuoteIdx);
    }
    public static int SkipWhiteSpace(this string me, int start)
    {
        int i;
        for (i = start; i < me.Length; i++)
        {
            if (!char.IsWhiteSpace(me[i])) break;
        }
        return i;
    }
        
    public static IHtmlContent ToHtmlEncodedIHtmlContent(this StringBuilder sb)
    {
        return HttpUtility.HtmlEncode(sb.ToString()).ToHtmlString();
    }
    public static HtmlString ToHtmlEncodedHtmlString(this StringBuilder sb)
    {
        return new HtmlString(HttpUtility.HtmlEncode(sb.ToString()));
    }
    public static IHtmlContent ToHtmlEncodedIHtmlContent(this string str)
    {
        return HttpUtility.HtmlEncode(str).ToHtmlString();
    }
    public static HtmlString ToHtmlEncodedHtmlString(this string str)
    {
        return new HtmlString(HttpUtility.HtmlEncode(str));
    }
        
    public static IHtmlContent ToIHtmlContent(this StringBuilder sb)
    {
        return sb.ToHtmlString();
    }
    public static HtmlString ToHtmlString(this StringBuilder sb)
    {
        return new HtmlString(sb.ToString());
    }
    public static IHtmlContent ToIHtmlContent2(this string str)
    {
        return str.ToHtmlString();
    }
    public static HtmlString ToHtmlString(this string str)
    {
        return new HtmlString(str);
    }
        
    public static string DisableAllControls(this string str)
    {
        return str.Replace("<fieldset ", "<fieldset disabled ").Replace("<input ", "<input disabled ").Replace("<textarea ", "<textarea disabled ").Replace("<select ", "<select disabled ");
    }

    public static string DisableAllControlsIf(this string str, bool condition)
    {
        return condition ? str.DisableAllControls() : str;
    }

    public static string CapLength(this string? str, int len)
    {
        if (string.IsNullOrEmpty(str)) return "";
        if (str.Length <= len) return str;
        return str.Substring(0, len - 3) + "...";
    }

    public static string RemoveStartingFirst(this string str, string delimiter)
    {
        var dashIndex = str.IndexOf(delimiter, StringComparison.Ordinal);
        return dashIndex > 0 ? str.Substring(0, dashIndex) : str;
    }

    public static string RemoveStartingLast(this string str, string delimiter)
    {
        var dashIndex = str.LastIndexOf(delimiter, StringComparison.Ordinal);
        return dashIndex > 0 ? str.Substring(0, dashIndex) : str;
    }

    public static string ToStringHandleNull(this string? str)
    {
        return str ?? "";
    }
    #endregion
}