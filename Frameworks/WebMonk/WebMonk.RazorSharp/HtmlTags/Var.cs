using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Var : InlineTag
{
    #region Constructors
    public Var(object? attributes = null, bool generateInline = false) : base("var", attributes, generateInline) { }
    #endregion
}