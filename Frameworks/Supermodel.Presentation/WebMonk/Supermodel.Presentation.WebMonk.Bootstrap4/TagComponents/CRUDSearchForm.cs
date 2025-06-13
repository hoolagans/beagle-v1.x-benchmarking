using System;
using Supermodel.DataAnnotations.Enums;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Templates;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDSearchForm : HtmlSnippet
    {
        #region Constructors
        public CRUDSearchForm(IEditorTemplate searchModel, string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors) :
            this(searchModel, new Txt(pageTitle), action, controller, resetButton, validationSummaryVisible)
        { }

        public CRUDSearchForm(IEditorTemplate searchModel, IGenerateHtml? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
        {
            if (searchModel == null) throw new ArgumentException(nameof(searchModel));
            
            AppendAndPush(new CRUDSearchFormContainer(searchModel, pageTitle, action, controller, resetButton, validationSummaryVisible));
            Append(searchModel.EditorTemplate());
            Pop<CRUDSearchFormContainer>();
        }
        #endregion
    }
}