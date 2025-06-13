using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class SectionPlaceholder : Tag
{
    #region Constructors
    public SectionPlaceholder(string uniqueIdentifier) : base("SectionPlaceholder") 
    { 
        UniqueIdentifier = uniqueIdentifier;
    }
    #endregion

    #region Override
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        throw new SupermodelException("All SectionPlaceholders must be removed before calling ToHtml() method");
    }
    #endregion

    #region Properties
    public string UniqueIdentifier { get; protected set; }
    #endregion
}