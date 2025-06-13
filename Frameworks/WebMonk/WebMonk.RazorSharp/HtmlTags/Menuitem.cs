using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Menuitem : SelfClosingTag
{
    #region Constructors
    public Menuitem(object? attributes = null) : base("menuitem", attributes) { }
    #endregion
}