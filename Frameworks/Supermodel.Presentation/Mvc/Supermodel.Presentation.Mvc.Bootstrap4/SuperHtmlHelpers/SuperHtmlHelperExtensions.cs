namespace Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;

public static class SuperHtmlHelperExtensions
{
    #region Methods
    public static SuperBs4HtmlHelper<TModel> Bs4<TModel>(this HtmlHelpers.SuperHtmlHelper<TModel> superHtml)
    {
        return new SuperBs4HtmlHelper<TModel>(superHtml);
    }
    public static SuperBs4HtmlHelper<dynamic> Bs4(this HtmlHelpers.SuperHtmlHelper<dynamic> superHtml)
    {
        return new SuperBs4HtmlHelper<dynamic>(superHtml);
    }
    #endregion
}