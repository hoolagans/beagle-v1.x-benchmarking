using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use CSS instead.")]
public class Big : InlineTag
{
    #region Constructors
    public Big(object? attributes = null, bool generateInline = false) : base("big", attributes, generateInline) { }
    #endregion
}