using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-hidden-for-model", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4HiddenForModelTagHelper : TemplateForModelSuperBs4TagHelperBase
{
    #region Constructors
    public SuperBs4HiddenForModelTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override IHtmlContent TemplateForModel(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        return _htmlHelper.Super().HiddenForModel(screenOrderFrom, screenOrderTo, markerAttribute);
    }
    #endregion
}