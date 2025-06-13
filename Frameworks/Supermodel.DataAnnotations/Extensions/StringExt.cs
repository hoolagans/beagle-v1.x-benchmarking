using System.Text;

namespace Supermodel.DataAnnotations.Extensions;

public static class StringExt
{
    public static string HttpHeaderEncode(this string me)
    {
        const string validChars = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.~";

        var sb = new StringBuilder();
        foreach (var chr in me)
        {
            if (validChars.Contains(chr)) sb.Append(chr);
            else sb.Append($"%{(int)chr:x2}");
        }
        return sb.ToString();
    }
}