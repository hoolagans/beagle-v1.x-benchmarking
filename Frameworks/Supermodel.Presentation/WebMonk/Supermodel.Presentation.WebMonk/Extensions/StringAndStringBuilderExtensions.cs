using System;

namespace Supermodel.Presentation.WebMonk.Extensions;

public static class StringAndStringBuilderExtensions
{
    #region Methods
    public static int SkipWhiteSpace(this string me, int start)
    {
        int i;
        for (i = start; i < me.Length; i++)
        {
            if (!char.IsWhiteSpace(me[i])) break;
        }
        return i;
    }
        
    public static string CapLength(this string? str, int len)
    {
        if (str == null) return "";
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