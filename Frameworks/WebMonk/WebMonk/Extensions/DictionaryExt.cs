using System.Text;
using System.Web;
using WebMonk.Misc;

namespace WebMonk.Extensions;

public static class DictionaryExt
{
    public static string ToUrlEncodedNameValuePairs(this QueryStringDict me)
    {
        var sb = new StringBuilder();
        var first = true;
        foreach (var nvp in me)
        {
            if (nvp.Value != null)
            {
                if (first)
                {
                    first = false;
                    sb.Append($"?{HttpUtility.UrlEncode(nvp.Key)}={HttpUtility.UrlEncode(nvp.Value)}");
                }
                else
                {
                    sb.Append($"&{HttpUtility.UrlEncode(nvp.Key)}={HttpUtility.UrlEncode(nvp.Value)}");
                }
            }
        }
        return sb.ToString();
    }
}