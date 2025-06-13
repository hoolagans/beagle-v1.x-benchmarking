using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-new-search-action-link", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4NewSearchActionLink : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4NewSearchActionLink(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().NewSearchActionLink());
    }
    #endregion
}