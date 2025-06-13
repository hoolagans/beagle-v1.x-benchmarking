using System.Net;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.Results;

public class HtmlResult : TextResult
{
    #region Constructors
    public HtmlResult(IGenerateHtml tags) : base(HttpStatusCode.OK, tags.ToHtml().ToString(), "text/html") {}
    #endregion

    //#region Helper Methods
    //private static string GetString(IEnumerable<Tag> tags)
    //{
    //    var sb = new StringBuilderWithIndents();
    //    foreach (var tag in tags) sb = tag.ToHtml(sb);
    //    return sb.ToString();
    //}
    //#endregion
}