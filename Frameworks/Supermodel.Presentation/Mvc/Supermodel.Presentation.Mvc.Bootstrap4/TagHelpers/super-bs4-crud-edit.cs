using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-edit", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDEditTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDEditTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output) 
    {
        output.TagName = null;
        output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDEdit(PageTitle, ReadOnly, SkipBackButton, ValidationSummaryVisible));
    }
    #endregion

    #region Properties
    public string PageTitle { get; set; } = "";
    public bool ReadOnly {get; set; } = false;
    public bool SkipBackButton { get; set; } = false;
    public ValidationSummaryVisible ValidationSummaryVisible { get; set; } = ValidationSummaryVisible.IfNoVisibleErrors;
    #endregion

}