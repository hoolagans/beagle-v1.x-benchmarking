using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.WebMonk.Extensions;
using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Templates;
using WebMonk.Rendering.Views;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDSearchFormContainer : HtmlContainerSnippet
    {
        #region Constructors
        public CRUDSearchFormContainer(IEditorTemplate searchModel, string pageTitle, string? action, string? controller, bool resetButton, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors) : 
            this(searchModel, new Txt(pageTitle), action, controller, resetButton, validationSummaryVisible){ }
            
        public CRUDSearchFormContainer(IEditorTemplate searchModel, IGenerateHtml? pageTitle, string? action, string? controller, bool resetButton, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
        {
            action ??= "List";
            controller ??= HttpContext.Current.PrefixManager.CurrentContextControllerName;

            var url = Render.Helper.UrlForMvcAction(controller, action);
            AppendAndPush(new Form(new { id=ScaffoldingSettings.SearchFormId, action = url, method = "get" }));
            AppendAndPush(new Fieldset(new { id=ScaffoldingSettings.SearchFormFieldsetId } ));
            if (pageTitle != null) 
            {
                AppendAndPush(new H2(new { @class=ScaffoldingSettings.SearchTitleCssClass }));
                Append(pageTitle);
                Pop<H2>();
            }

            var showValidationSummary = ShowValidationSummaryHelper.ShouldShowValidationSummary(searchModel, validationSummaryVisible);
            if (showValidationSummary)
            {
                AppendAndPush(new Div(new { @class = $"col-sm-12 {ScaffoldingSettings.ValidationSummaryCssClass}" }));
                Append(Render.ValidationSummary());
                Pop<Div>();
            }

            Append(InnerContent = new Tags());

            var qs = HttpContext.Current.HttpListenerContext.Request.QueryString;
            Append(new Input(new { id="smSkip", name="smSkip", type="hidden", value = 0 }));
            Append(new Input(new { id="smTake", name="smTake", type="hidden", value = qs.GetTakeValue()?.ToString() ?? "" }));
            Append(new Input(new { id="smSortBy", name="smSortBy", type="hidden", value = qs.GetSortByValue() ?? "" }));

            AppendAndPush(new Div(new { @class="form-group row pt-2"}));
            Append(new Div(new { @class="col-sm-2"}));
            AppendAndPush(new Div(new { @class="col-sm-10"}));

            AppendAndPush(new Button(new { id=ScaffoldingSettings.FindButtonId, type="submit", @class= ScaffoldingSettings.FindButtonCssClass}));
            Append(new Span(new { @class="oi oi-magnifying-glass" }));
            Append(new Txt(" Find&nbsp;"));
            Pop<Button>();

            if (resetButton) 
            {
                AppendAndPush(new Button(new { id=ScaffoldingSettings.ResetButtonId, type="reset", @class= ScaffoldingSettings.ResetButtonCssClass}));
                Append(new Span(new { @class="oi oi-action-undo" }));
                Append(new Txt(" Reset&nbsp;"));
                Pop<Button>();
            }

            Pop<Div>();
            Pop<Div>();
            Pop<Fieldset>();
            Pop<Form>();
        }
        #endregion
    }
}