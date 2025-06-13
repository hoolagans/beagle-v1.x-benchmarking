using System;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;

public abstract class TemplateSuperBs4TagHelperBase : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    protected TemplateSuperBs4TagHelperBase(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Abstract Methods
    public abstract IHtmlContent Template(string expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null);
    #endregion

    #region Methods
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Expression == null || string.IsNullOrEmpty(Expression.Name)) throw new Exception($"{nameof(TemplateSuperBs4TagHelperBase)}.{nameof(Process)}(): for is a required attribute");

        output.TagName = null;
            
        var sb = new StringBuilder();
        foreach (var outputAttribute in output.Attributes) sb.Append($"{outputAttribute.Name}='{outputAttribute.Value}' ");
        var markerAttribute = sb.ToString().Trim();
            
        var htmlHelperOutput = Template(Expression.Name, ScreenOrderFrom, ScreenOrderTo, markerAttribute);

        output.Content.SetHtmlContent(htmlHelperOutput);
    }
    #endregion

    #region Properties
    [HtmlAttributeName("for")] public ModelExpression? Expression { get; set; }
    public int ScreenOrderFrom { get; set; } = int.MinValue;
    public int ScreenOrderTo { get; set; } = int.MaxValue;
    #endregion
}