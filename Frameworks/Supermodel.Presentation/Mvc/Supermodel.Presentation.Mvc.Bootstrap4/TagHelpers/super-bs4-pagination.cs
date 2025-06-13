using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-pagination", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4PaginationTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4PaginationTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().Pagination(VisiblePages, Skip, Take, TotalCount));
    }
    #endregion

    #region Properties
    public int? VisiblePages { get; set; } 
    public int? Skip { get; set; } 
    public int? Take { get; set; }
    public int? TotalCount { get; set; }
    #endregion
}