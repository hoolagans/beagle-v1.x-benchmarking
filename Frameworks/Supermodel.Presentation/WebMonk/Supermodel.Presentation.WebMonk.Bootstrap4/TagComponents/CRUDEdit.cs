using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.WebMonk.Models;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class CRUDEdit : HtmlSnippet
    {
        #region Constructors
        public CRUDEdit(IViewModelForEntity model, string pageTitle, bool readOnly = false, bool skipBackButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors) :
            this(model, new Txt(pageTitle), readOnly, skipBackButton, validationSummaryVisible)
        { }

        public CRUDEdit(IViewModelForEntity model, IGenerateHtml? pageTitle = null, bool readOnly = false, bool skipBackButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
        {
            AppendAndPush(new CRUDEditContainer(model, pageTitle, readOnly, skipBackButton, validationSummaryVisible));
            Append(Render.EditorForModel(model).DisableAllControlsIf(readOnly));
            Pop<CRUDEditContainer>();
        }
        #endregion
    }
}