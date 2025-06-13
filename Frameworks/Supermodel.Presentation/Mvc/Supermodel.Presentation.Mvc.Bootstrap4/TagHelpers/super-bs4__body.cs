using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers.Base;
using Supermodel.Presentation.Mvc.Extensions.Gateway;
using Supermodel.Presentation.Mvc.HtmlHelpers;

namespace Supermodel.Presentation.Mvc.Bootstrap4.TagHelpers;

[HtmlTargetElement("body", Attributes = "super-bs4-add-scripts")]
public class SuperBs4BodyTagHelper : TagHelperDerivedFromHtmlHelperBase
{
    #region Constructors
    public SuperBs4BodyTagHelper(IHtmlHelper<dynamic> htmlHelper) : base(htmlHelper){ }
    #endregion

    #region Overrides
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = await output.GetChildContentAsync();

        var urlHelper = _htmlHelper.Super().GetUrlHelper();

        // ReSharper disable Html.PathError
        output.Content.SetHtmlContent($@"
                {GetSupermodelSnippet(urlHelper)}
                {DialogsScript()}
                {childContent.GetContent()}
            ");
        // ReSharper restore Html.PathError

        RemoveMarkerAttribute(output);
    }
    #endregion

    #region Methods
    public virtual void RemoveMarkerAttribute(TagHelperOutput output)
    {
        output.Attributes.Remove(output.Attributes.Single(x => x.Name == "super-bs4-add-scripts"));
    }
    public virtual string GetSupermodelSnippet(IUrlHelper urlHelper)
    {
        return GetSupermodelSnippetStatic(urlHelper);
    }
    public static string GetSupermodelSnippetStatic(IUrlHelper urlHelper)
    {
        // ReSharper disable Html.PathError
        var result = $@"
                <script src=""{urlHelper.Content("~/static_web_files/jquery-3.6.0.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/bootstrap.bundle.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/jquery-ui.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/bootbox.all.min.js")}""></script>
                <script src=""{urlHelper.Content("~/static_web_files/super.bs4.js")}""></script>
            ";
        // ReSharper restore Html.PathError
        return result;
    }
    protected virtual string DialogsScript()
    {
        var sb = new StringBuilder();
        if (_htmlHelper.TempData.Super().NextPageStartupScript != null || 
            _htmlHelper.TempData.Super().NextPageAlertMessage != null || 
            _htmlHelper.TempData.Super().NextPageModalMessage != null)
        { 
            sb.AppendLine("<script>");
            sb.AppendLine("$(function () {");
            if (_htmlHelper.TempData.Super().NextPageStartupScript != null) sb.AppendLine(_htmlHelper.TempData.Super().NextPageStartupScript);
            if (_htmlHelper.TempData.Super().NextPageAlertMessage != null) sb.AppendLine("alert('" + HttpUtility.HtmlEncode(_htmlHelper.TempData.Super().NextPageAlertMessage!) + "');");
            if (_htmlHelper.TempData.Super().NextPageModalMessage != null) sb.AppendLine("bootbox.alert('" + _htmlHelper.TempData.Super().NextPageModalMessage + "'.replace(/\\n/g, '<br />'));");
            sb.AppendLine("});");
            sb.AppendLine("</script>");
        }
        return sb.ToString();
    }
    #endregion
}