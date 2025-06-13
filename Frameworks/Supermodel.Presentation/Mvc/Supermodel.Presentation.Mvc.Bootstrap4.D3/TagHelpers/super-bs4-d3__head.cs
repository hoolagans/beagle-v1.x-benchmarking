using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.TagHelpers;

[HtmlTargetElement("head", Attributes = "super-bs4-d3-add-meta-and-links")]
public class SuperBs4D3HeadTagHelper : SuperBs4HeadTagHelper
{
    #region Constructors
    public SuperBs4D3HeadTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper) { }
    #endregion

    #region Methods
    public override void RemoveMarkerAttribute(TagHelperOutput output)
    {
        output.Attributes.Remove(new TagHelperAttribute("super-bs4-d3-add-meta-and-links"));
    }
    public override string GetSupermodelSnippet(IUrlHelper urlHelper)
    {
        return GetSupermodelSnippetStatic(urlHelper);
    }
    public new static string GetSupermodelSnippetStatic(IUrlHelper urlHelper)
    {
        // ReSharper disable Html.PathError
        var result = $@"
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/bootstrap.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/open_iconic/font/css/open-iconic-bootstrap.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/jquery-ui.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/britecharts.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/super.bs4.css")}"" />
            ";
        // ReSharper restore Html.PathError
        return result;
    }
    #endregion
}