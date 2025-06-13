using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;

public abstract class TemplateForModelSuperBs4TagHelperBase : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    protected TemplateForModelSuperBs4TagHelperBase(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion

    #region Abstract Methods
    public abstract IHtmlContent TemplateForModel(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null);
    #endregion

    #region Methods
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var sb = new StringBuilder();
        foreach (var outputAttribute in output.Attributes) sb.Append($"{outputAttribute.Name}='{outputAttribute.Value}' ");
        var markerAttribute = sb.ToString().Trim();

        var htmlHelperOutput = TemplateForModel(ScreenOrderFrom, ScreenOrderTo, markerAttribute);

        output.Content.SetHtmlContent(htmlHelperOutput);
    }
    #endregion

    #region Properties
    public int ScreenOrderFrom { get; set; } = int.MinValue;
    public int ScreenOrderTo { get; set; } = int.MaxValue;
    #endregion
}