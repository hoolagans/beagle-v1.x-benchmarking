using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Table : Tag
{
    #region Constructors
    public Table(object? attributes = null) : base("table", attributes) { }
    #endregion
}