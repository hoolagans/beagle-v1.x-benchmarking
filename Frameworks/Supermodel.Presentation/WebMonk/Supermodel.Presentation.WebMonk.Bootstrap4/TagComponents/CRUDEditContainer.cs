using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.WebMonk.Extensions;
using Supermodel.Presentation.WebMonk.Models;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDEditContainer : HtmlContainerSnippet
    {
        #region Constructors
        public CRUDEditContainer(IViewModelForEntity model, string pageTitle, bool readOnly = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors) :
            this(model, new Txt(pageTitle), readOnly, false, validationSummaryVisible)
        { }

        public CRUDEditContainer(IViewModelForEntity model, IGenerateHtml? pageTitle = null, bool readOnly = false, bool skipBackButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
        {
            //Start form
            var formAction = HttpContext.Current.RouteManager.LocalPathWithQueryStringMinusSelectedId;
            AppendAndPush(new Form(new { id=ScaffoldingSettings.EditFormId, action = formAction, method = "post", enctype = "multipart/form-data"}));

            if (pageTitle != null) Append(new H2(new { @class=ScaffoldingSettings.EditTitleCssClass }) { pageTitle });

            //Override Http Verb if needed (if the model is not new, we put, per REST)
            if (!model.IsNewModel()) Append(Render.HttpMethodOverride(HttpMethod.Put));

            var showValidationSummary = ShowValidationSummaryHelper.ShouldShowValidationSummary(model, validationSummaryVisible);
            if (showValidationSummary)
            {
                AppendAndPush(new Div(new { @class = $"col-sm-12 {ScaffoldingSettings.ValidationSummaryCssClass}" }));
                Append(Render.ValidationSummary());
                Pop<Div>();
            }

            InnerContent = Append(new Fieldset(new { id=ScaffoldingSettings.EditFormFieldsetId }));

            AppendAndPush(new Div(new { @class="form-group row pt-2" }));
            Append(new Div(new { @class="col-sm-2" }));
            AppendAndPush(new Div(new { @class="col-sm-10" }));
            if (!skipBackButton)
            {
                long? parentId = null;
                if (ReflectionHelper.IsClassADerivedFromClassB(model.GetType(), typeof(ChildMvcModelForEntity<,>))) parentId = (long?)model.PropertyGet("ParentId");

                //make sure we keep query string
                var qs = HttpContext.Current.HttpListenerContext.Request.QueryString;
                if (parentId != null) qs["parentId"] = parentId.ToString();
                qs.Remove("selectedId");

                //set up html attributes, linkLabel and controller
                var linkLabel = new Tags
                {
                    new Span(new { @class="oi oi-arrow-circle-left" }),
                    new Txt("&nbsp;&nbsp;Back")
                };
                var controller = HttpContext.Current.PrefixManager.CurrentContextControllerName;
                var htmlAttributes = new { id=ScaffoldingSettings.BackButtonId, @class=ScaffoldingSettings.BackButtonCssClass };
                Append(Render.ActionLink(linkLabel, controller, "List", null, qs, htmlAttributes));
            }
            if (!readOnly) 
            {
                Append(new Button(new { type="submit", id=ScaffoldingSettings.SaveButtonId, @class=ScaffoldingSettings.SaveButtonCssClass })
                {
                    new Span(new { @class="oi oi-circle-check"}),
                    new Txt("&nbsp;&nbsp;Save"),
                });
            }
            Pop<Div>();
            Pop<Div>();
            Pop<Form>();

            // ReSharper disable once VirtualMemberCallInConstructor
            DisableAllControlsIf(readOnly);
        }
        #endregion
    }
}