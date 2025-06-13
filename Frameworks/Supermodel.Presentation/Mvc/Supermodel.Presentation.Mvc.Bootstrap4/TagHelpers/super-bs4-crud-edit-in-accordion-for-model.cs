using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.Mvc.Bootstrap4.Models;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-edit-in-accordion-for-model", TagStructure = TagStructure.WithoutEndTag, Attributes = "accordion-id, panels")]
public class SuperBs4CRUDEditInAccordionForModelTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDEditInAccordionForModelTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDEditInAccordion(AccordionId, Panels, PageTitle, ReadOnly, SkipBackButton));
    }
    #endregion

    #region Properties
    public string AccordionId { get; set; } = "";
    public IEnumerable<Bs4.AccordionPanel> Panels { get; set; } = new List<Bs4.AccordionPanel>();     
        
    public string PageTitle { get; set; } = "";
    public bool ReadOnly {get; set; } = false;
    public bool SkipBackButton { get; set; } = false;
    public ValidationSummaryVisible ValidationSummaryVisible { get; set; } = ValidationSummaryVisible.Always;
    #endregion

}