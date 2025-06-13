using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use <ul> instead.")]
public class Dir : Tag
{
    #region Constructors
    public Dir(object? attributes = null) : base("dir", attributes) { }
    #endregion
}