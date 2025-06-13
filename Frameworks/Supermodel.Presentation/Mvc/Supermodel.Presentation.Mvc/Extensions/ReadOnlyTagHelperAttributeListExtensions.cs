using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.DataAnnotations.Misc;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class ReadOnlyTagHelperAttributeListExtensions
{
    public static AttributesDict ToAttributesDictionary(this ReadOnlyTagHelperAttributeList me, params string[] skipAttributes)
    {
        var skipAttributesList = skipAttributes.ToList();
        var dict = new AttributesDict();
        foreach (var pair in me) 
        {
            if (skipAttributesList.All(x => x != pair.Name)) dict.Add(pair.Name, pair.Value.ToString());
        }
        return dict;
    }
}