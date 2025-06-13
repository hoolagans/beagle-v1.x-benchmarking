using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Output : InlineTag
{
    #region Constructors
    public Output(object? attributes = null, bool generateInline = false) : base("output", attributes, generateInline) { }
    #endregion
}