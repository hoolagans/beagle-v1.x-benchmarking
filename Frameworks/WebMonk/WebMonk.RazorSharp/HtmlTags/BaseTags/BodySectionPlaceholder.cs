namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class BodySectionPlaceholder : SectionPlaceholder
{
    #region Constructors
    public BodySectionPlaceholder() : base(BodySection)
    {
        UniqueIdentifier = BodySection;
    }
    #endregion

    #region Constants
    public const string BodySection = "@BodySection";
    #endregion
}