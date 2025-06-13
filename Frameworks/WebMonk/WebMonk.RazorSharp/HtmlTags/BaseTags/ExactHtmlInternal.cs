using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

internal class ExactHtmlInternal : Tag
{
    #region Constructors
    internal ExactHtmlInternal(string htmlString) : base(null)
    {
        HtmlString = htmlString;
    }
    #endregion
        
    #region Overrides
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        sb.AppendLine(HtmlString);
        return sb;
    }
    #endregion

    #region Properties
    public string HtmlString { get; set; }
    #endregion
}