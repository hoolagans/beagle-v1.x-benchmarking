using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.Presentation.Mvc.Models.Mvc;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-multi-column-editable-list", Attributes = "items, type-of-data-context", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDMultiColumnEditableListTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDMultiColumnEditableListTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        if (PageTitle != null) output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnEditableList(Items!, TypeOfDataContext!, PageTitle, SkipAddNew, SkipDelete));
        else output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnEditableList(Items!, TypeOfDataContext!, (IHtmlContent?)null, SkipAddNew, SkipDelete));
    }
    #endregion

    #region Properties
    public IEnumerable<IMvcModelForEntity>? Items { get; set; }
    public Type? TypeOfDataContext { get; set; }

    public string? PageTitle { get; set; } 
    public bool SkipAddNew { get; set; }
    public bool SkipDelete { get; set; }
    #endregion
}