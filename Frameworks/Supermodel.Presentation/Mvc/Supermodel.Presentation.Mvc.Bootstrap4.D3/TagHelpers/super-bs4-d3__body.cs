using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.TagHelpers;

[HtmlTargetElement("body", Attributes = "super-bs4-d3-add-scripts")]
public class SuperBs4D3BodyTagHelper : SuperBs4BodyTagHelper
{
    #region Constructors
    public SuperBs4D3BodyTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper) { }
    #endregion

    #region Overrides
    public override void RemoveMarkerAttribute(TagHelperOutput output)
    {
        output.Attributes.Remove(new TagHelperAttribute("super-bs4-d3-add-scripts"));
    }
    public override string GetSupermodelSnippet(IUrlHelper urlHelper)
    {
        return GetSupermodelSnippetStatic(urlHelper);
    }
    public new static string GetSupermodelSnippetStatic(IUrlHelper urlHelper)
    {
        // ReSharper disable Html.PathError
        var result = $@"
                <script src=""{urlHelper.Content("~/static_web_files/jquery-3.6.0.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/bootstrap.bundle.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/jquery-ui.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/d3.v5.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/britecharts.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/bootbox.all.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/super.bs4.js")}""></script>
            ";
        // ReSharper restore Html.PathError
        return result;
    }
    #endregion
}