using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;

public abstract class TagHelperDerivedFromHtmlHelperBase : TagHelper
{
    #region Constructors
    protected TagHelperDerivedFromHtmlHelperBase(IHtmlHelper<dynamic> htmlHelper)
    {
        _htmlHelper = htmlHelper;
    }
    #endregion

    #region Overrides
    public override void Init(TagHelperContext context)
    {
        base.Init(context);
        if (!(_htmlHelper is IViewContextAware viewContextAware)) throw new Exception($"{nameof(TagHelperDerivedFromHtmlHelperBase)}.{nameof(Process)}(): viewContextAware == null");
        viewContextAware.Contextualize(ViewContext);
    }
    #endregion

    #region Properties
    [ViewContext, HtmlAttributeNotBound] public ViewContext? ViewContext { get; set; }
    protected readonly IHtmlHelper<dynamic> _htmlHelper;
    #endregion
}