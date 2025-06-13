using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Results;

namespace WebMonk.Extensions;

public static class TagExt
{
    #region Methods
    public static HtmlResult ToHtmlResult(this IGenerateHtml tags)
    {
        return new HtmlResult(tags);
    }
    #endregion
}