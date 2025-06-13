using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-login-form-for-model", TagStructure = TagStructure.WithoutEndTag)]
public class SuperBs4LoginFormForModel : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4LoginFormForModel(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        var htmlHelperOutput = _htmlHelper.Super().Bs4().LoginFormForModel(FormAction);
        output.Content.SetHtmlContent(htmlHelperOutput);
    }
    #endregion

    #region Properties
    public string? FormAction { get; set; }
    #endregion
}