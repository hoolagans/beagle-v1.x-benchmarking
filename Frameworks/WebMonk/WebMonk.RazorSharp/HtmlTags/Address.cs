using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Address : Tag
{
    #region Constructors
    public Address(object? attributes = null) : base("address", attributes) { }
    #endregion
}