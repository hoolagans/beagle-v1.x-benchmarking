using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Menu : Tag
{
    #region Constructors
    public Menu(object? attributes = null) : base("Menu", attributes) { }
    #endregion
}