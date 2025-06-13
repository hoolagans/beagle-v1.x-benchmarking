using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.Presentation.Mvc.Models.Mvc;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-multi-column-children-list-no-actions", Attributes = "items", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4CRUDMultiColumnChildrenListNoActionsTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDMultiColumnChildrenListNoActionsTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (PageTitle != null) output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnChildrenListNoActions(Items!, PageTitle));
        else output.Content.SetHtmlContent(_htmlHelper.Super().Bs4().CRUDMultiColumnChildrenListNoActions(Items!));
    }
    #endregion

    #region Properties
    public IEnumerable<IChildMvcModelForEntity>? Items { get; set; }
    public string? PageTitle { get; set; } 
    #endregion
}