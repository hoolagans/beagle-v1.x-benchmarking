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

[HtmlTargetElement("super-bs4-crud-multi-column-children-editable-list", Attributes = "items, type-of-data-context, child-controller-type, parent-id", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDMultiColumnChildrenEditableListTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDMultiColumnChildrenEditableListTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        if (Title != null) output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnChildrenEditableList(Items!, TypeOfDataContext!, ChildControllerType!, ParentId, Title, SkipAddNew, SkipDelete));
        else output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnChildrenEditableList(Items!, TypeOfDataContext!, ChildControllerType!, ParentId, (IHtmlContent?)null, SkipAddNew, SkipDelete));
    }
    #endregion

    #region Properties
    public IEnumerable<IChildMvcModelForEntity>? Items { get; set; }
    public Type? TypeOfDataContext { get; set; }
    public Type? ChildControllerType { get; set; } 
    public long ParentId { get; set; }

    public string? Title { get; set; } 
    public bool SkipAddNew { get; set; }
    public bool SkipDelete { get; set; }
    #endregion
}