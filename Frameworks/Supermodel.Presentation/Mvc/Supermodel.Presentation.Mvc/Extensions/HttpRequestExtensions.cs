using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class HttpRequestExtensions
{
    public static string GetEncodedPathAndQueryMinusSelectedId(this HttpRequest request)
    {
        var url = request.GetEncodedPathAndQuery();
            
        var selectedIdStartIndex = url.IndexOf("selectedId", StringComparison.InvariantCultureIgnoreCase);
        if (selectedIdStartIndex < 0) return url;
            
        var selectedIdEndIndex = GetNextAmpersandIndex(url, selectedIdStartIndex) ?? url.Length;
            
        var newUrl = url[..selectedIdStartIndex] + url[selectedIdEndIndex..];
        return newUrl;
    }

    private static int? GetNextAmpersandIndex(string str, int startIndex)
    {
        for(var i = startIndex + "selectedId".Length; i < str.Length; i++)
        {
            if (str[i] == '&') return i+1;
        }
        return null;
    }
}