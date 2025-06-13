using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Fieldset : Tag
{
    #region Constructors
    public Fieldset(object? attributes = null) : base("fieldset", attributes) { }
    #endregion
}