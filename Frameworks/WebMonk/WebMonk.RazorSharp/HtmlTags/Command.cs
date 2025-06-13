using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Command : SelfClosingTag
{
    #region Constructors
    public Command(object? attributes = null) : base("command", attributes) { }
    #endregion
}