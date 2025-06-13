using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("head", Attributes = "super-bs4-add-meta-and-links")]
public class SuperBs4HeadTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4HeadTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){}
    #endregion
        
    #region Overrides
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = await output.GetChildContentAsync();

        var urlHelper = _htmlHelper.Super().GetUrlHelper();

        // ReSharper disable Html.PathError
        output.Content.SetHtmlContent($@"
                {GetSupermodelSnippet(urlHelper)}
                {childContent.GetContent()}
            ");
        // ReSharper restore Html.PathError

        RemoveMarkerAttribute(output);
    }
    #endregion

    #region Methods
    public virtual void RemoveMarkerAttribute(TagHelperOutput output)
    {
        output.Attributes.Remove(output.Attributes.Single(x => x.Name == "super-bs4-add-meta-and-links"));
    }
    public virtual string GetSupermodelSnippet(IUrlHelper urlHelper)
    {
        return GetSupermodelSnippetStatic(urlHelper);
    }
    public static string GetSupermodelSnippetStatic(IUrlHelper urlHelper)
    {
        // ReSharper disable Html.PathError
        var result = $@"
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/bootstrap.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/open_iconic/font/css/open-iconic-bootstrap.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/jquery-ui.min.css")}"" />
                <link rel=""stylesheet"" href=""{urlHelper.Content("~/static_web_files/super.bs4.css")}"" />
            ";
        // ReSharper restore Html.PathError
        return result;
    }
    #endregion
}