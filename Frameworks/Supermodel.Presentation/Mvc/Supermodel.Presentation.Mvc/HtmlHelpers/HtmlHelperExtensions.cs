using Microsoft.AspNetCore.Mvc.Rendering;

namespace Supermodel.Presentation.Mvc.HtmlHelpers;

public static class HtmlHelperExtensions
{
    #region Methods
    public static SuperHtmlHelper<TModel> Super<TModel>(this IHtmlHelper<TModel> html)
    {
        return new SuperHtmlHelper<TModel>(html);
    }
    public static SuperHtmlHelper<dynamic> Super(this IHtmlHelper<dynamic> html)
    {
        return new SuperHtmlHelper<dynamic>(html);
    }
    public static SuperHtmlHelper Super(this IHtmlHelper html)
    {
        return new SuperHtmlHelper(html);
    }
    #endregion
}