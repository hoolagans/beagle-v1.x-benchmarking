using WebMonk.RazorSharp.Html2RazorSharp;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

//Use with caution: improper use may create XSS vulnerability
public class TagsFromHtmlString : Tags
{
    #region Constructors
    public TagsFromHtmlString(string htmlString)
    {
        Add(TranslatorBase.CreateMnemonic(htmlString, false, true).ToRazorSharp());
    }
    #endregion
}