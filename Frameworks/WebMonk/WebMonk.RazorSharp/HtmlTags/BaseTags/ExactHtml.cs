using System.Collections.Generic;
using WebMonk.RazorSharp.Exceptions;
using WebMonk.RazorSharp.Html2RazorSharp;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

//Use with caution: improper use may create XSS vulnerability
public class ExactHtml : Tags
{
    #region Constructors
    public ExactHtml(string htmlString)
    {
        Add(new ExactHtmlInternal(htmlString));
    }
    #endregion

    #region Overrides
    public override List<Tag> NormalizeAndFlatten()
    {
        if (Count != 1) throw new RazorSharpException("ExactHtml contains more or less than one ExactHtmlInternal tag");
        var exactHtmlTag = (ExactHtmlInternal)this[0];

        Clear();
        AddRange(TranslatorBase.CreateMnemonic(exactHtmlTag.HtmlString, false, true).ToRazorSharp().RootTags);
        return base.NormalizeAndFlatten();  
    }
    #endregion
}