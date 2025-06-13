using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.Presentation.Mvc.Models.Mvc;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-multi-column-list", Attributes = "items", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDMultiColumnListTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDMultiColumnListTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        if (PageTitle != null) output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnList(Items!, PageTitle, SkipAddNew, SkipDelete, ViewOnly));
        else output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnList(Items!, (IHtmlContent?)null, SkipAddNew, SkipDelete, ViewOnly));
    }
    #endregion

    #region Properties
    public IEnumerable<IMvcModelForEntity>? Items { get; set; }

    public string? PageTitle { get; set; } 
    public bool SkipAddNew { get; set; }
    public bool SkipDelete { get; set; }
    public bool ViewOnly { get; set; }
    #endregion
}