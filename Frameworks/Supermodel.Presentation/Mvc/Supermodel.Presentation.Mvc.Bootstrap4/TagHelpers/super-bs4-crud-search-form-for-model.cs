using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-search-form-for-model", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDSearchFormForModelTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDSearchFormForModelTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        if (PageTitle != null) output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDSearchFormForModel(PageTitle, Action, Controller, ResetButton, ValidationSummaryVisible));
        else output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDSearchFormForModel((IHtmlContent?)null, Action, Controller, ResetButton, ValidationSummaryVisible));
    }
    #endregion

    #region Properties
    public string? PageTitle { get; set; } = null;
    public string? Controller {get; set; } = null;
    public string? Action { get; set; } = null;
    public bool ResetButton { get; set; } = false;
    public ValidationSummaryVisible ValidationSummaryVisible { get; set; } = ValidationSummaryVisible.IfNoVisibleErrors;
    #endregion
}