using System.Web;

namespace WebMonk.RazorSharp.Extensions;

public static class StringExt
{
    #region Methods
    //public static Txt Txt(this string str, bool generateInline = false)
    //{
    //    return new Txt(str, generateInline);
    //}
    
    public static string JavaScriptStringEncode(this string str)
    {
        return HttpUtility.JavaScriptStringEncode(str);
    }
    public static string HtmlAttributeEncode(this string str)
    {
        return HttpUtility.HtmlAttributeEncode(str);
    }

    public static string HtmlEncode(this string str)
    {
        return HttpUtility.HtmlEncode(str);
    }
    public static string HtmlDecode(this string str)
    {
        return HttpUtility.HtmlDecode(str);
    }

    public static string UrlEncode(this string str)
    {
        return HttpUtility.UrlEncode(str);
    }
    public static string UrlDecode(this string str)
    {
        return HttpUtility.UrlDecode(str);
    }
    #endregion
}