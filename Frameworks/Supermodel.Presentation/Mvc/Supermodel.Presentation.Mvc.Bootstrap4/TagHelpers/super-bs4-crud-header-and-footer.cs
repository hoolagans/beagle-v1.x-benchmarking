using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-crud-header-and-footer")]
public class SuperBs4CRUDHeaderAndFooterTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4CRUDHeaderAndFooterTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Overrides
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var childContent = await output.GetChildContentAsync();

        if (PageTitle != null)
        {
            output.Content.SetHtmlContent($@"
                    {_htmlHelper.Super().Bs4().CRUDEditHeader(PageTitle, ReadOnly, ValidationSummaryVisible)}
                    {childContent.GetContent()}
                    {_htmlHelper.Super().Bs4().CRUDEditFooter(ReadOnly, SkipBackButton)}
                ");
        }
        else
        {
            output.Content.SetHtmlContent($@"
                    {_htmlHelper.Super().Bs4().CRUDEditHeader((IHtmlContent?)null, ReadOnly, ValidationSummaryVisible)}
                    {childContent.GetContent()}
                    {_htmlHelper.Super().Bs4().CRUDEditFooter(ReadOnly, SkipBackButton)}
                ");
        }
    }
    #endregion

    #region Properties
    public string? PageTitle { get; set; } = null;
    public bool ReadOnly {get; set; } = false;
    public bool SkipBackButton { get; set; } = false;
    public ValidationSummaryVisible ValidationSummaryVisible { get; set; } = ValidationSummaryVisible.IfNoVisibleErrors;
    #endregion
}