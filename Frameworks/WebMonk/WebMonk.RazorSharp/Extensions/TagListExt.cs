using System.Collections.Generic;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.Extensions;

public static class TagListExt
{
    public static IGenerateHtml ToIGenerateHtml(this List<Tag> me)
    {
        var tags = new Tags();
        tags.AddRange(me);
        return tags;
    }
}