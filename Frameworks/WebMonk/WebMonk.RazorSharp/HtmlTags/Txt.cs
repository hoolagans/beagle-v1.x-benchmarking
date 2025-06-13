using System.Collections.Generic;
using System.Web;
using Supermodel.DataAnnotations;
using WebMonk.RazorSharp.Extensions;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Txt : InlineTag
{
    #region Constructors
    public Txt(string innerText, bool generateInline = false) : base(null, null, generateInline) 
    {
        InnerText = innerText;
    }
    #endregion

    #region Implicit Conversion from string
    public static implicit operator Txt(string s) => new(s);
    #endregion

    #region Overrides
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        if (GenerateInline) 
        {
            sb.TrimEndWhitespace();
            sb.Append(WhitespaceTranslatedInnerText.HtmlEncode());
        }
        else
        {
            sb.AppendLine(WhitespaceTranslatedInnerText.HtmlEncode());
        }
        return sb;
    }
    public StringBuilderWithIndents ToHtmlNoHtmlEncode(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        if (GenerateInline) 
        {
            sb.TrimEndWhitespace();
            sb.Append(InnerText);
        }
        else
        {
            sb.AppendLine(InnerText);
        }
        return sb;
    }
    public StringBuilderWithIndents ToHtmlNoNewLineAtTheEnd(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        if (GenerateInline) 
        {
            sb.TrimEndWhitespace();
            sb.Append(WhitespaceTranslatedInnerText.HtmlEncode());
        }
        else
        {
            sb.Append(WhitespaceTranslatedInnerText.HtmlEncode());
        }
        return sb;
    }
    #endregion

    #region Properties
    public string InnerText { get; set; }
    public string WhitespaceTranslatedInnerText 
    { 
        get 
        {
            if (string.IsNullOrEmpty(InnerText)) return InnerText;

            var result = InnerText;
            foreach (var pair in WhitespaceCodeTranslationDict) result = result.Replace(pair.Key, pair.Value);
            return result;
        } 
    }

    public static string Nbsp { get; } = HttpUtility.HtmlDecode("&nbsp;");
    public static string Ensp { get; } = HttpUtility.HtmlDecode("&ensp;");
    public static string Emsp { get; } = HttpUtility.HtmlDecode("&emsp;");
    public static string Emsp13 { get; } = HttpUtility.HtmlDecode("&emsp13;");
    public static string Emsp14 { get; } = HttpUtility.HtmlDecode("&emsp14;");
    public static string Numsp { get; } = HttpUtility.HtmlDecode("&numsp;");
    public static string Puncsp { get; } = HttpUtility.HtmlDecode("&puncsp;");
    public static string Thinsp { get; } = HttpUtility.HtmlDecode("&Thinsp;");
    public static string Hairsp { get; } = HttpUtility.HtmlDecode("&hairsp;");

    public static Dictionary<string, string> WhitespaceCodeTranslationDict { get; } = new()
    {
        { "&nbsp;", Nbsp },
        { "&ensp;", Ensp },
        { "&emsp;", Emsp },
        { "&emsp13;", Emsp13 },
        { "&emsp14;", Emsp14 },
        { "&numsp;", Numsp },
        { "&puncsp;", Puncsp },
        { "&thinsp;", Thinsp },
        { "&hairsp;", Hairsp },
    };
    #endregion
}