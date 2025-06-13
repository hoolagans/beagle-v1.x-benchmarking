using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Keygen : SelfClosingTag
{
    #region Constructors
    public Keygen(object? attributes = null) : base("keygen", attributes) { }
    #endregion
}