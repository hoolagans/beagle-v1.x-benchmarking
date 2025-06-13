using Supermodel.DataAnnotations;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Html : Tag
{
    #region Constructors
    public Html(object? attributes = null) : base("html", attributes) { }
    #endregion

    #region Overrides
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        sb.AppendLine($"<!doctype {Doctype}>");
        return base.ToHtml(sb);
    }
    #endregion

    #region Properties
    public string Doctype { get; set; } = "html";
    #endregion
}