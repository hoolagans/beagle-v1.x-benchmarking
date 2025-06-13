using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.Models;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-sort-by-dropdown-form", Attributes = "sort-by-options", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4SortByDropdownFormTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4SortByDropdownFormTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().SortByDropdownForm(SortByOptions!, context.AllAttributes.ToAttributesDictionary("sort-by-options")));
    }
    #endregion

    #region Properties
    public Bs4.SortByOptions? SortByOptions { get; set; }
    #endregion
}