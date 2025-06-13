using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-display", Attributes = "for", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4DisplayTagHelper : TemplateSuperBs4TagHelperBase
{
    #region Constructors
    public SuperBs4DisplayTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override IHtmlContent Template(string expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        return _htmlHelper.Super().Display(expression, screenOrderFrom, screenOrderTo, markerAttribute);
    }
    #endregion
}