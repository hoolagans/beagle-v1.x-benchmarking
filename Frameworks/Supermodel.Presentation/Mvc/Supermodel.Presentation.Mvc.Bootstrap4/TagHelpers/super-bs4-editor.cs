using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("super-bs4-editor", Attributes = "for", TagStructure = TagStructure.WithoutEndTag)]
public class EditorSuperBs4TagHelper : TemplateSuperBs4TagHelperBase
{
    #region Constructors
    public EditorSuperBs4TagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Overrides
    public override IHtmlContent Template(string expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        var htmlContent = _htmlHelper.Super().Editor(expression, screenOrderFrom, screenOrderTo, markerAttribute);
        if (ReadOnly) htmlContent = htmlContent.GetString().DisableAllControls().ToHtmlString();
        return htmlContent;
    }
    #endregion

    #region Properties
    public bool ReadOnly { get; set; } = false;
    #endregion
}