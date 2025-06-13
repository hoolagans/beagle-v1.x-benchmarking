using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.Mvc.Bootstrap4.Models;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-search-form-in-accordion", Attributes = "for, panels", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDSearchFormInAccordionTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDSearchFormInAccordionTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        if (PageTitle != null) output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDSearchFormInAccordion(Expression!.Name, AccordionId, Panels!, PageTitle, Action, Controller, ResetButton, ValidationSummaryVisible));
        else output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDSearchFormInAccordion(Expression!.Name, AccordionId, Panels!, (IHtmlContent?)null, Action, Controller, ResetButton, ValidationSummaryVisible));
    }
    #endregion

    #region Properties
    [HtmlAttributeName("for")] public ModelExpression? Expression { get; set; }
    [HtmlAttributeName("Id")] public string AccordionId { get; set; } = "accordion";
    public IEnumerable<Bs4.AccordionPanel>? Panels { get; set; }

    public string? PageTitle { get; set; } = null;
    public string? Controller {get; set; } = null;
    public string? Action { get; set; } = null;
    public bool ResetButton { get; set; } = false;
    public ValidationSummaryVisible ValidationSummaryVisible { get; set; } = ValidationSummaryVisible.Always;
    #endregion
}