using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Async;
using Supermodel.DataAnnotations.Enums;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.Mvc.Bootstrap4.Extensions;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;
using Supermodel.Presentation.Mvc.Context;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.Presentation.Mvc.Models;
using Supermodel.Presentation.Mvc.Models.Mvc;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;
using Supermodel.ReflectionMapper;
using static Supermodel.Presentation.Mvc.Bootstrap4.Models.Bs4;

namespace Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;

public class SuperBs4HtmlHelper<TModel>
{
    #region Constructors
    public SuperBs4HtmlHelper(SuperHtmlHelper<TModel> superHtml)
    {
        SuperHtml = superHtml;
    }
    #endregion

    #region Login Form
    public IHtmlContent LoginFormForModel(string? fromAction = null)
    {
        var result = new StringBuilder();
        if (Html.ViewData.Model == null) throw new Exception("Model is null");

        var action = fromAction == null ? "" : $"action = '{fromAction}'";
        result.AppendLine($"<form {UtilsLib.MakeIdAttribute(ScaffoldingSettings.LoginFormId)} {action} method='{HtmlHelper.GetFormMethodString(FormMethod.Post)}' enctype='multipart/form-data'>");
        result.AppendLine(Html.Super().EditorForModel().GetString());
        result.AppendLine("<input name='submit-button' class='btn btn-primary' type='submit' value='Log In' />");
        result.AppendLine("</form>");

        return result.ToString().ToHtmlString();
    }
    #endregion

    #region CRUD Search Form Helpers
    public IHtmlContent CRUDSearchFormForModel(string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDSearchFormForModel(pageTitle.ToHtmlEncodedHtmlString(), action, controller, resetButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchFormForModel(IHtmlContent? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDSearchFormHelper((Expression<Func<TModel, TModel>>?)null, pageTitle, controller, action, resetButton, validationSummaryVisible);
    }

    public IHtmlContent CRUDSearchFormFor<TValue>(Expression<Func<TModel, TValue>> searchByModelExpression, string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors) where TValue : MvcModel
    {
        return CRUDSearchFormFor(searchByModelExpression, pageTitle.ToHtmlEncodedHtmlString(), action, controller, resetButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchFormFor<TValue>(Expression<Func<TModel, TValue>> searchByModelExpression, IHtmlContent? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors) where TValue : MvcModel
    {
        return CRUDSearchFormHelper(searchByModelExpression, pageTitle, controller, action, resetButton, validationSummaryVisible);
    }

    public IHtmlContent CRUDSearchForm(string searchByModelExpression, string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDSearchForm(searchByModelExpression, pageTitle.ToHtmlEncodedHtmlString(), action, controller, resetButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchForm(string searchByModelExpression, IHtmlContent? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDSearchFormHelper(searchByModelExpression, pageTitle, controller, action, resetButton, validationSummaryVisible);
    }

    //these two methods are exactly identical CreateModelExpression has overloads for both string and expression
    private IHtmlContent CRUDSearchFormHelper(string searchByModelExpression, IHtmlContent? pageTitle, string? controller, string? action, bool resetButton, ValidationSummaryVisible validationSummaryVisible)
    {
        if (Html.ViewData.Model == null) throw new ArgumentException("Model == null");
            
        var modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
            
        //If expression result is null, create a default one and reset cache
        if (modelExpression.Model == null) 
        {
            Html.ViewData.Model.PropertySet(modelExpression.Name, ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType));
            modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
        }

        if (!(modelExpression.Model is ISupermodelEditorTemplate)) throw new SupermodelException("Search Model must implement ISupermodelEditorTemplate");
        var templatedModel = (ISupermodelEditorTemplate) modelExpression.Model;
            
        var result = new StringBuilder();
            
        result.AppendLine(CRUDSearchFormHeader(templatedModel, pageTitle, action, controller, validationSummaryVisible).GetString());
            
        var innerHtml = SuperHtml.MakeInnerHtmlHelper(modelExpression, true);
        result.AppendLine(templatedModel.EditorTemplate(innerHtml).GetString());

        result.AppendLine(CRUDSearchFormFooter(resetButton).GetString());

        return result.ToHtmlString();
    }
    private IHtmlContent CRUDSearchFormHelper<TValue>(Expression<Func<TModel, TValue>>? searchByModelExpression, IHtmlContent? pageTitle, string? controller, string? action, bool resetButton, ValidationSummaryVisible validationSummaryVisible)
    {
        if (Html.ViewData.Model == null) throw new ArgumentException("Model == null");
            
        var result = new StringBuilder();

        if (searchByModelExpression != null)
        {
            var modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
            var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
                
            //If expression result is null, create a default one and reset cache
            if (modelExpression.Model == null) 
            {
                Html.ViewData.Model.PropertySet(modelExpression.Name, ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType));
                modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
                modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
            }

            if (!(modelExpression.Model is ISupermodelEditorTemplate)) throw new SupermodelException("Search Model must implement ISupermodelEditorTemplate");
            var templatedModel = (ISupermodelEditorTemplate) modelExpression.Model;
                
            result.AppendLine(CRUDSearchFormHeader(templatedModel, pageTitle, action, controller, validationSummaryVisible).GetString());
                
            var innerHtml = SuperHtml.MakeInnerHtmlHelper(modelExpression, true);
            result.AppendLine(templatedModel.EditorTemplate(innerHtml).GetString());
        }
        else
        {
            if (!(Html.ViewData.Model is ISupermodelEditorTemplate supermodelModelEditorTemplate)) throw new Exception("Model must implement ISupermodelEditorTemplate");
                
            result.AppendLine(CRUDSearchFormHeader(supermodelModelEditorTemplate, pageTitle, action, controller, validationSummaryVisible).GetString());
            result.AppendLine(SuperHtml.EditorForModel().GetString());
        }

        result.AppendLine(CRUDSearchFormFooter(resetButton).GetString());

        return result.ToHtmlString();
    }
    #endregion

    #region CRUD Search Form In Accordion Helpers
    public IHtmlContent CRUDSearchFormInAccordionForModel(string accordionElementId, IEnumerable<AccordionPanel> panels, string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
    {
        return CRUDSearchFormInAccordionForModel(accordionElementId, panels, pageTitle.ToHtmlEncodedHtmlString(), action, controller, resetButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchFormInAccordionForModel(string accordionElementId, IEnumerable<AccordionPanel> panels, IHtmlContent? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
    {
        return CRUDSearchFormInAccordionHelper((Expression<Func<TModel, TModel>>?)null, accordionElementId, panels, pageTitle, controller, action, resetButton, validationSummaryVisible);
    }
        
    public IHtmlContent CRUDSearchFormInAccordionFor<TValue>(Expression<Func<TModel, TValue>> searchByModelExpression, string accordionId, IEnumerable<AccordionPanel> panels, string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always) where TValue : MvcModel
    {
        return CRUDSearchFormInAccordionFor(searchByModelExpression, accordionId, panels, pageTitle.ToHtmlEncodedHtmlString(), action, controller, resetButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchFormInAccordionFor<TValue>(Expression<Func<TModel, TValue>> searchByModelExpression, string accordionId, IEnumerable<AccordionPanel> panels, IHtmlContent? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always) where TValue : MvcModel
    {
        return CRUDSearchFormInAccordionHelper(searchByModelExpression, accordionId, panels, pageTitle, controller, action, resetButton, validationSummaryVisible);
    }

    public IHtmlContent CRUDSearchFormInAccordion(string searchByModelExpression, string accordionId, IEnumerable<AccordionPanel> panels, string pageTitle, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
    {
        return CRUDSearchFormInAccordion(searchByModelExpression, accordionId, panels, pageTitle.ToHtmlEncodedHtmlString(), action, controller, resetButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchFormInAccordion(string searchByModelExpression, string accordionId, IEnumerable<AccordionPanel> panels, IHtmlContent? pageTitle = null, string? action = null, string? controller = null, bool resetButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
    {
        return CRUDSearchFormInAccordionHelper(searchByModelExpression, accordionId, panels, pageTitle, controller, action, resetButton, validationSummaryVisible);
    }

    //these two methods are exactly identical CreateModelExpression has overloads for both string and expression
    private IHtmlContent CRUDSearchFormInAccordionHelper(string searchByModelExpression, string accordionId, IEnumerable<AccordionPanel> panels, IHtmlContent? pageTitle, string? controller, string? action, bool resetButton, ValidationSummaryVisible validationSummaryVisible)
    {
        if (Html.ViewData.Model == null) throw new ArgumentException("Model == null");
            
        var modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
            
        //If expression result is null, create a default one and reset cache
        if (modelExpression.Model == null) 
        {
            Html.ViewData.Model.PropertySet(modelExpression.Name, ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType));
            modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
        }

        if (!(modelExpression.Model is ISupermodelEditorTemplate)) throw new SupermodelException("Model must implement ISupermodelEditorTemplate");
        var templatedModel = (ISupermodelEditorTemplate) modelExpression.Model;
            
        var result = new StringBuilder();

        result.AppendLine(CRUDSearchFormHeader(templatedModel, pageTitle, action, controller, validationSummaryVisible).GetString());
        result.AppendLine($"<div class='accordion' id='{accordionId}'>");

        var innerHtml = SuperHtml.MakeInnerHtmlHelper(modelExpression, true);
        foreach (var panel in panels)
        {
            var body = templatedModel.EditorTemplate(innerHtml, panel.ScreenOrderFrom, panel.ScreenOrderTo).GetString();
            result.AppendLine(GetAccordionSection(accordionId, panel, body));
        }

        result.AppendLine("</div>");
        result.AppendLine(CRUDSearchFormFooter(resetButton).GetString());

        return result.ToHtmlString();
    }
    private IHtmlContent CRUDSearchFormInAccordionHelper<TValue>(Expression<Func<TModel, TValue>>? searchByModelExpression, string accordionId, IEnumerable<AccordionPanel> panels, IHtmlContent? pageTitle, string? controller, string? action, bool resetButton, ValidationSummaryVisible validationSummaryVisible)
    {
        if (Html.ViewData.Model == null) throw new ArgumentException("Model == null");
            
        var result = new StringBuilder();

        if (searchByModelExpression != null)
        {
            var modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
            var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
                
            //If expression result is null, create a default one and reset cache
            if (modelExpression.Model == null) 
            {
                Html.ViewData.Model.PropertySet(modelExpression.Name, ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType));
                modelExpressionProvider = new ModelExpressionProvider(Html.MetadataProvider);
                modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, searchByModelExpression);
            }

            if (!(modelExpression.Model is ISupermodelEditorTemplate)) throw new SupermodelException("Model must implement ISupermodelEditorTemplate");
            var templatedModel = (ISupermodelEditorTemplate) modelExpression.Model;
                
            result.AppendLine(CRUDSearchFormHeader(templatedModel, pageTitle, action, controller, validationSummaryVisible).GetString());
            result.AppendLine($"<div class='accordion' id='{accordionId}'>");

            var innerHtml = SuperHtml.MakeInnerHtmlHelper(modelExpression, true);
            foreach (var panel in panels)
            {
                var body = templatedModel.EditorTemplate(innerHtml, panel.ScreenOrderFrom, panel.ScreenOrderTo).GetString();
                result.AppendLine(GetAccordionSection(accordionId, panel, body));
            }
        }
        else
        {
            if (!(Html.ViewData.Model is ISupermodelEditorTemplate supermodelModelEditorTemplate)) throw new Exception("Model must implement ISupermodelEditorTemplate");

            result.AppendLine(CRUDSearchFormHeader(supermodelModelEditorTemplate, pageTitle, action, controller, validationSummaryVisible).GetString());
            result.AppendLine($"<div class='accordion' id='{accordionId}'>");

            foreach (var panel in panels)
            {
                var body = supermodelModelEditorTemplate.EditorTemplate(Html, panel.ScreenOrderFrom, panel.ScreenOrderTo).GetString();
                result.AppendLine(GetAccordionSection(accordionId, panel, body));
            }
        }
        result.AppendLine("</div>");

        result.AppendLine(CRUDSearchFormFooter(resetButton).GetString());

        return result.ToHtmlString();
    }
    #endregion

    #region CRUD Search Form Header and Footer Helpers
    public IHtmlContent CRUDSearchFormHeader(ISupermodelEditorTemplate searchModel, string pageTitle, string? action, string? controller, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDSearchFormHeader(searchModel, pageTitle.ToHtmlEncodedHtmlString(), action, controller, validationSummaryVisible);
    }
    public IHtmlContent CRUDSearchFormHeader(ISupermodelEditorTemplate searchModel, IHtmlContent? pageTitle, string? action, string? controller, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        var result = new StringBuilder();

        action ??= "List";
        controller ??= Html.ViewContext.RouteData.Values["controller"]!.ToString();

        var url = SuperHtml.GenerateUrl(action, controller);
        result.AppendLine($"<form {UtilsLib.MakeIdAttribute(ScaffoldingSettings.SearchFormId)} action='{url}' method='{HtmlHelper.GetFormMethodString(FormMethod.Get)}'>");
            
        var showValidationSummary = ShouldShowValidationSummary(searchModel, validationSummaryVisible);
        if (showValidationSummary)
        {
            result.AppendLine($"<div class='{ScaffoldingSettings.ValidationSummaryCssClass}'>");
            result.AppendLine(Html.ValidationSummary().GetString());
            result.AppendLine("</div>");
        }
            
        result.AppendLine($"<fieldset {UtilsLib.MakeIdAttribute(ScaffoldingSettings.SearchFormFieldsetId)}>");

        if (!UtilsLib.IsNullOrEmpty(pageTitle)) result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.SearchTitleCssClass) + ">" + pageTitle!.GetString() + "</h2>");

        return result.ToHtmlString();
    }

    public IHtmlContent CRUDSearchFormFooter(bool resetButton)
    {
        var result = new StringBuilder();

        result.AppendLine("<input id='smSkip' name='smSkip' type='hidden' value='0'>");
        result.AppendLine(Html.Hidden("smTake", Html.ViewContext.HttpContext.Request.Query.GetTakeValue()).GetString());
        result.AppendLine(Html.Hidden("smSortBy", Html.ViewContext.HttpContext.Request.Query.GetSortByValue()).GetString());

        result.AppendLine("<div class='form-group row pt-2'>");
        result.AppendLine("<div class='col-sm-2'></div>");
        result.AppendLine("<div class='col-sm-10'>");

        result.AppendLine("<button type='submit' " + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.FindButtonId, ScaffoldingSettings.FindButtonCssClass) + "><span class='oi oi-magnifying-glass'></span> Find&nbsp;</button>");
        if (resetButton) result.AppendLine("<button type='reset' " + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.ResetButtonId, ScaffoldingSettings.ResetButtonCssClass) + "><span class='oi oi-action-undo'></span> Reset&nbsp;</button>");

        //result.AppendLine("</div>");
        result.AppendLine("</div>");
        result.AppendLine("</div>");
        result.AppendLine("</fieldset>");
        result.AppendLine("</form>");

        return result.ToHtmlString();
    }
    #endregion

    #region CRUD List Helpers
    public IHtmlContent CRUDList(IEnumerable<MvcModelForEntityCore> items, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDListHelper(items, null, pageTitle.ToHtmlEncodedHtmlString(), skipAddNew, skipDelete, viewOnly, null);
    }
    public IHtmlContent CRUDList(IEnumerable<MvcModelForEntityCore> items, IHtmlContent? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDListHelper(items, null, pageTitle, skipAddNew, skipDelete, viewOnly, null);
    }
    public IHtmlContent CRUDChildrenList(IEnumerable<MvcModelForEntityCore> items, Type childControllerType, long parentId, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDListHelper(items, parentId, pageTitle.ToHtmlEncodedHtmlString(), skipAddNew, skipDelete, viewOnly, childControllerType);
    }
    public IHtmlContent CRUDChildrenList(IEnumerable<MvcModelForEntityCore> items, Type childControllerType, long parentId, IHtmlContent? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDListHelper(items, parentId, pageTitle, skipAddNew, skipDelete, viewOnly, childControllerType);
    }
        
    private IHtmlContent CRUDListHelper(IEnumerable<MvcModelForEntityCore> items, long? parentId, IHtmlContent? pageTitle, bool skipAddNew, bool skipDelete, bool viewOnly, Type? controllerType)
    {
        var controllerName = controllerType != null ?
            controllerType.GetControllerName() :
            Html.ViewContext.RouteData.Values["controller"]!.ToString();
        if (controllerName == null) throw new SupermodelException("controllerName == null. this should never happen");

        var result = new StringBuilder();
        if (parentId == null || parentId > 0)
        {
            if (!UtilsLib.IsNullOrEmpty(pageTitle))
            {
                if (parentId == null) result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.ListTitleCssClass) + ">" + pageTitle!.GetString() + "</h2>");
                else result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.ChildListTitleCssClass) + ">" + pageTitle!.GetString() + "</h2>");
            }
            result.AppendLine("<div" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTopDivId, ScaffoldingSettings.CRUDListTopDivCssClass) + ">");
            if (!skipAddNew)
            {
                //make sure we keep query string
                var routeValues = SuperHtml.QueryStringRouteValues();
                var newRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = (long) 0, parentId });
                routeValues.AddOrUpdateWith(newRouteValues);

                //set up html attributes
                var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListAddNewCssClass });
                result.AppendLine("<p>" + SuperHtml.ActionLinkHtmlContent("<span class='oi oi-plus'></span>".ToHtmlString(), "Detail", controllerName, routeValues, htmlAttributes) + "</p>");
            }
            result.AppendLine("<table" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTableId, ScaffoldingSettings.CRUDListTableCssClass) + ">");
            result.AppendLine("<thead>");
            result.AppendLine("<tr>");
            result.AppendLine("<th scope='col'>Name</th>");
            result.AppendLine("<th scope='col'> Actions </th>");
            result.AppendLine("</tr>");
            result.AppendLine("</thead>");
            result.AppendLine("<tbody>");
            foreach (var item in items)
            {
                result.AppendLine("<tr>");
                result.AppendLine("<td>" + item.Label + "</td>");

                //make sure we keep query string
                var routeValues = SuperHtml.QueryStringRouteValues();
                var newRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = item.Id, parentId });
                routeValues.AddOrUpdateWith(newRouteValues);

                result.AppendLine("<td>");
                if (!skipDelete) result.AppendLine("<div class='btn-group'>");
                    
                if (viewOnly) result.AppendLine(SuperHtml.ActionLinkHtmlContent("<span class='oi oi-eye'></span>".ToHtmlString(), "Detail", controllerName, routeValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListEditCssClass })).GetString());
                else result.AppendLine(SuperHtml.ActionLinkHtmlContent("<span class='oi oi-pencil'></span>".ToHtmlString(), "Detail", controllerName, routeValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListEditCssClass })).GetString());

                if (!skipDelete)
                {
                    result.AppendLine(SuperHtml.RESTfulActionLinkHtmlContent(HttpMethod.Delete, "<span class='oi oi-trash'></span>".ToHtmlString(), "Detail", controllerName, routeValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListDeleteCssClass }), "Are you sure?").GetString());
                    result.AppendLine("</div>");
                }
                result.AppendLine("</td>");

                result.AppendLine("</tr>");
            }
            result.AppendLine("</tbody>");
            result.AppendLine("</table>");
            result.AppendLine("</div>");
        }
        return result.ToHtmlString();
    }
    #endregion
        
    #region CRUD Edit Helpers
    public IHtmlContent CRUDEdit(string pageTitle, bool readOnly = false, bool skipBackButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDEdit(pageTitle.ToHtmlEncodedHtmlString(), readOnly, skipBackButton, validationSummaryVisible);
    }
    public IHtmlContent CRUDEdit(IHtmlContent? pageTitle = null, bool readOnly = false, bool skipBackButton = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        var result = new StringBuilder();

        result.AppendLine(CRUDEditHeader(pageTitle, readOnly, validationSummaryVisible).GetString());
        result.AppendLine(SuperHtml.EditorForModel().GetString().DisableAllControlsIf(readOnly));
        result.AppendLine(CRUDEditFooter(readOnly, skipBackButton).GetString());
        result.AppendLine();

        return result.ToHtmlString();
    }
    #endregion

    #region CRUD Edit Header & Footer Helpers
    public IHtmlContent CRUDEditHeader(string pageTitle, bool readOnly = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        return CRUDEditHeader(pageTitle.ToHtmlEncodedHtmlString(), readOnly, validationSummaryVisible);
    }
    public IHtmlContent CRUDEditHeader(IHtmlContent? pageTitle = null, bool readOnly = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.IfNoVisibleErrors)
    {
        var result = new StringBuilder();
        if (Html.ViewData.Model == null) throw new Exception("Model is null");
        var model = (IViewModelForEntity)Html.ViewData.Model;

        //Start form
        result.AppendLine($"<form {UtilsLib.MakeIdAttribute(ScaffoldingSettings.EditFormId)} action='{Html.ViewContext.HttpContext.Request.GetEncodedPathAndQueryMinusSelectedId()}' method='{HtmlHelper.GetFormMethodString(FormMethod.Post)}' enctype='multipart/form-data'>");
            
        var showValidationSummary = ShouldShowValidationSummary(model, validationSummaryVisible);
        if (showValidationSummary)
        {
            result.AppendLine($"<div class='{ScaffoldingSettings.ValidationSummaryCssClass}'>");
            result.AppendLine(Html.ValidationSummary().GetString());
            result.AppendLine("</div>");
        }

        result.AppendLine($"<fieldset {UtilsLib.MakeIdAttribute(ScaffoldingSettings.EditFormFieldsetId)}>");
            
        if (!UtilsLib.IsNullOrEmpty(pageTitle)) result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.EditTitleCssClass) + ">" + pageTitle + "</h2>");

        //Override Http Verb if needed (if the model is not new, we put, per REST)
        if (!model.IsNewModel()) result.AppendLine(SuperHtml.HttpMethodOverride(HttpMethod.Put).GetString());

        return (readOnly ? result.ToString().DisableAllControls() : result.ToString()).ToHtmlString();
    }

    public IHtmlContent CRUDEditFooter(bool readOnly = false, bool skipBackButton = false)
    {
        var result = new StringBuilder();
        if (Html.ViewData.Model == null) throw new Exception("Model is null");
        var model = (IViewModelForEntity)Html.ViewData.Model;

        result.AppendLine("</fieldset>");
        result.AppendLine("<div class='form-group row pt-2'>");
        result.AppendLine("<div class='col-sm-2'></div>");
        result.AppendLine("<div class='col-sm-10'>");
        if (!skipBackButton)
        {
            long? parentId = null;
            if (ReflectionHelper.IsClassADerivedFromClassB(model.GetType(), typeof(ChildMvcModelForEntity<,>))) parentId = (long?)model.PropertyGet("ParentId");

            //make sure we keep query string
            var routeValues = SuperHtml.QueryStringRouteValues();
            var newRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { parentId });
            routeValues.AddOrUpdateWith(newRouteValues);
            routeValues.Remove("selectedId");

            //set up html attributes
            var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = ScaffoldingSettings.BackButtonId, @class = ScaffoldingSettings.BackButtonCssClass });
            result.AppendLine(SuperHtml.ActionLinkHtmlContent("<span class='oi oi-arrow-circle-left'></span>&nbsp;&nbsp;Back".ToHtmlString(), "List", routeValues, htmlAttributes).GetString());
        }
        if (!readOnly) result.AppendLine("<button type='submit' " + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.SaveButtonId, ScaffoldingSettings.SaveButtonCssClass) + "><span class='oi oi-circle-check'></span>&nbsp;&nbsp;Save</button>");
        result.AppendLine("</div>");
        result.AppendLine("</div>");
        result.AppendLine("</form>");

        return (readOnly ? result.ToString().DisableAllControls() : result.ToString()).ToHtmlString();
    }
    #endregion

    #region CRUD Edit In Accordion Helpers
    public IHtmlContent CRUDEditInAccordion(string accordionId, IEnumerable<AccordionPanel> panels, string pageTitle, bool readOnly = false, bool skipBackButton = false, bool skipHeaderAndFooter = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
    {
        return CRUDEditInAccordion(accordionId, panels, pageTitle.ToHtmlEncodedHtmlString(), readOnly, skipBackButton, skipHeaderAndFooter, validationSummaryVisible);
    }
    public IHtmlContent CRUDEditInAccordion(string accordionId, IEnumerable<AccordionPanel> panels, IHtmlContent? pageTitle = null, bool readOnly = false, bool skipBackButton = false, bool skipHeaderAndFooter = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
    {
        var result = new StringBuilder();

        if (!skipHeaderAndFooter) result.AppendLine(CRUDEditHeader(pageTitle, readOnly, validationSummaryVisible).GetString());

        result.AppendLine($"<div id='{accordionId}'>");
        foreach (var panel in panels)
        {
            if (!(Html.ViewData.Model is ISupermodelEditorTemplate supermodelModelEditorTemplate)) throw new Exception("Model must implement ISupermodelEditorTemplate");
            var body = supermodelModelEditorTemplate.EditorTemplate(Html, panel.ScreenOrderFrom, panel.ScreenOrderTo).GetString().DisableAllControlsIf(readOnly);
            result.AppendLine(GetAccordionSection(accordionId, panel, body));
        }
        result.AppendLine("</div>");

        if (!skipHeaderAndFooter) result.AppendLine(CRUDEditFooter(readOnly, skipBackButton).GetString());
        result.AppendLine();

        return result.ToHtmlString();
    }
    #endregion

    #region CRUD MuiliColumn List Helpers
    public IHtmlContent CRUDMultiColumnList(IEnumerable<IViewModelForEntity> items, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDMultiColumnListHelper(items, null, pageTitle.ToHtmlEncodedHtmlString(), null, skipAddNew, skipDelete, viewOnly);
    }
    public IHtmlContent CRUDMultiColumnList(IEnumerable<IViewModelForEntity> items, IHtmlContent? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDMultiColumnListHelper(items, null, pageTitle, null, skipAddNew, skipDelete, viewOnly);
    }
        
    public IHtmlContent CRUDMultiColumnChildrenList(IEnumerable<IChildMvcModelForEntity> items, Type childControllerType, long parentId, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDMultiColumnListHelper(items, childControllerType, pageTitle.ToHtmlEncodedHtmlString(), parentId, skipAddNew, skipDelete, viewOnly);
    }
    public IHtmlContent CRUDMultiColumnChildrenList(IEnumerable<IChildMvcModelForEntity> items, Type childControllerType, long parentId, IHtmlContent? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false)
    {
        return CRUDMultiColumnListHelper(items, childControllerType, pageTitle, parentId, skipAddNew, skipDelete, viewOnly);
    }

    private IHtmlContent CRUDMultiColumnListHelper(IEnumerable<IViewModelForEntity> items, Type? detailControllerType, IHtmlContent? pageTitle, long? parentId, bool skipAddNew, bool skipDelete, bool viewOnly)
    {
        var controllerName = detailControllerType != null ?
            detailControllerType.GetControllerName() :
            Html.ViewContext.RouteData.Values["controller"]!.ToString();
        if (controllerName == null) throw new SupermodelException("controllerName == null. this should never happen");

        var result = new StringBuilder();
        if (parentId == null || parentId > 0)
        {
            if (!UtilsLib.IsNullOrEmpty(pageTitle))
            {
                if (parentId == null) result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.ListTitleCssClass) + ">" + pageTitle + "</h2>");
                else result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.ChildListTitleCssClass) + ">" + pageTitle + "</h2>");
            }
            result.AppendLine("<div" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTopDivId, ScaffoldingSettings.CRUDListTopDivCssClass) + ">");
                
            if (!skipAddNew)
            {
                //make sure we keep query string
                var addRouteValues = SuperHtml.QueryStringRouteValues();
                var newAddRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = 0, parentId });
                addRouteValues.AddOrUpdateWith(newAddRouteValues);

                result.AppendLine("<p>" + SuperHtml.ActionLinkHtmlContent("<span class='oi oi-plus'></span>".ToHtmlString(), "Detail", controllerName, addRouteValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListAddNewCssClass })).GetString() + "</p>");
            }
                
            result.AppendLine("<table" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTableId, ScaffoldingSettings.CRUDListTableCssClass) + ">");
            result.AppendLine("<thead>");
            result.AppendLine("<tr>");
                
            //Create header using reflection
            var mvcModelType = items.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).First();
            var mvcModelForHeader = ReflectionHelper.CreateType(mvcModelType);                
            result = mvcModelForHeader.ToReadOnlyHtmlTableHeader(Html, result);

            result.AppendLine("<th scope='col'> Actions </th>");
            result.AppendLine("</tr>");
            result.AppendLine("</thead>");
            result.AppendLine("<tbody>");
            foreach (var item in items)
            {
                result.AppendLine("<tr>");

                //Render list columns using reflection
                result = item.ToReadOnlyHtmlTableRow(Html.Super().MakeInnerHtmlHelper(item), result);

                //make sure we keep query string
                var editViewDeleteRouteValues = SuperHtml.QueryStringRouteValues();
                var newEditViewDeleteRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = item.Id, parentId });
                editViewDeleteRouteValues.AddOrUpdateWith(newEditViewDeleteRouteValues);

                if (viewOnly)
                {
                    result.AppendLine("<td>" + SuperHtml.ActionLinkHtmlContent("<span class='oi oi-eye'></span>".ToHtmlString(), "Detail", controllerName, editViewDeleteRouteValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListEditCssClass })).GetString() + "</td>");
                }
                else
                {
                    result.AppendLine("<td>");
                    if (!skipDelete) result.AppendLine("<div class='btn-group'>");
                    result.AppendLine(SuperHtml.ActionLinkHtmlContent("<span class='oi oi-pencil'></span>".ToHtmlString(), "Detail", controllerName, editViewDeleteRouteValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListEditCssClass })).GetString());
                    if (!skipDelete)
                    {
                        result.AppendLine(SuperHtml.RESTfulActionLinkHtmlContent(HttpMethod.Delete, "<span class='oi oi-trash'></span>".ToHtmlString(), "Detail", controllerName, editViewDeleteRouteValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListDeleteCssClass }), "Are you sure?").GetString());
                        result.AppendLine("</div>");
                    }
                    result.AppendLine("</td>");
                }
                result.AppendLine("</tr>");
            }
            result.AppendLine("</tbody>");
            result.AppendLine("</table>");
            result.AppendLine("</div>");
        }
        return result.ToHtmlString();
    }
    #endregion

    #region CRUD MuiliColumn List NoActions Helpers
    public IHtmlContent CRUDMultiColumnListNoActions(IEnumerable<IMvcModel> items, string pageTitle)
    {
        return CRUDMultiColumnListNoActionsHelper(items, pageTitle.ToHtmlEncodedHtmlString());
    }
    public IHtmlContent CRUDMultiColumnListNoActions(IEnumerable<IMvcModel> items, IHtmlContent? pageTitle = null)
    {
        return CRUDMultiColumnListNoActionsHelper(items, pageTitle);
    }
        
    public IHtmlContent CRUDMultiColumnChildrenListNoActions(IEnumerable<IChildMvcModelForEntity> items, string pageTitle)
    {
        return CRUDMultiColumnListNoActionsHelper(items, pageTitle.ToHtmlEncodedHtmlString());
    }
    public IHtmlContent CRUDMultiColumnChildrenListNoActions(IEnumerable<IChildMvcModelForEntity> items, IHtmlContent? pageTitle = null)
    {
        return CRUDMultiColumnListNoActionsHelper(items, pageTitle);
    }
        
    private IHtmlContent CRUDMultiColumnListNoActionsHelper(IEnumerable<IMvcModel> items, IHtmlContent? pageTitle)
    {
        var result = new StringBuilder();
        if (!UtilsLib.IsNullOrEmpty(pageTitle)) result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.ListTitleCssClass) + ">" + pageTitle + "</h2>");
        result.AppendLine("<div" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTopDivId, ScaffoldingSettings.CRUDListTopDivCssClass) + ">");
        result.AppendLine("<table" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTableId, ScaffoldingSettings.CRUDListTableCssClass) + ">");
        result.AppendLine("<thead>");
        result.AppendLine("<tr>");
            
        //Create header using reflection
        var mvcModelType = items.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).First();
        var mvcModelForHeader = ReflectionHelper.CreateType(mvcModelType);                
        result = mvcModelForHeader.ToReadOnlyHtmlTableHeader(Html, result);

        result.AppendLine("</tr>");
        result.AppendLine("</thead>");
        result.AppendLine("<tbody>");
        foreach (var item in items)
        {
            result.AppendLine("<tr>");

            //Render list columns using reflection
            result = item.ToReadOnlyHtmlTableRow(Html.Super().MakeInnerHtmlHelper(item), result);

            result.AppendLine("</tr>");
        }
        result.AppendLine("</tbody>");
        result.AppendLine("</table>");
        result.AppendLine("</div>");
        return result.ToHtmlString();
    }
    #endregion

    #region CRUD Multicolumn Editable Lists Helpers
    public IHtmlContent CRUDMultiColumnChildrenEditableList(IEnumerable<IChildMvcModelForEntity> items, Type dataContextType, Type childControllerType, long parentId, string pageTitle, bool skipAddNew = false, bool skipDelete = false)
    {
        return CRUDMultiColumnEditableListHelper(items, dataContextType, childControllerType, pageTitle.ToHtmlEncodedHtmlString(), parentId, skipAddNew, skipDelete);
    }
    public IHtmlContent CRUDMultiColumnChildrenEditableList(IEnumerable<IChildMvcModelForEntity> items, Type dataContextType, Type childControllerType, long parentId, IHtmlContent? pageTitle = null, bool skipAddNew = false, bool skipDelete = false)
    {
        return CRUDMultiColumnEditableListHelper(items, dataContextType, childControllerType, pageTitle, parentId, skipAddNew, skipDelete);
    }       
        
    public IHtmlContent CRUDMultiColumnEditableList(IEnumerable<IViewModelForEntity> items, Type dataContextType, string pageTitle, bool skipAddNew = false, bool skipDelete = false)
    {
        return CRUDMultiColumnEditableListHelper(items, dataContextType, null, pageTitle.ToHtmlEncodedHtmlString(), null, skipAddNew, skipDelete);
    }
    public IHtmlContent CRUDMultiColumnEditableList(IEnumerable<IViewModelForEntity> items, Type dataContextType, IHtmlContent? pageTitle = null, bool skipAddNew = false, bool skipDelete = false)
    {
        return CRUDMultiColumnEditableListHelper(items, dataContextType, null, pageTitle, null, skipAddNew, skipDelete);
    }

    private IHtmlContent CRUDMultiColumnEditableListHelper(IEnumerable<IViewModelForEntity> items, Type dataContextType, Type? detailControllerType, IHtmlContent? pageTitle, long? parentId, bool skipAddNew, bool skipDelete)
    {
        var controllerName = detailControllerType != null ?
            detailControllerType.GetControllerName() :
            Html.ViewContext.RouteData.Values["controller"]!.ToString();
        if (controllerName == null) throw new SupermodelException("controllerName == null. this should never happen");


        var result = new StringBuilder();
        if (parentId == null || parentId > 0)
        { 
            if (!UtilsLib.IsNullOrEmpty(pageTitle)) result.AppendLine("<h2 " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.ListTitleCssClass) + ">" + pageTitle + "</h2>");
            
            var query = Html.ViewContext.HttpContext.Request.Query;
            var routeValues = query.ToRouteValueDictionary();
            routeValues.Remove("selectedId");
            routeValues.AddOrUpdateWith("parentId", parentId);

            result.AppendLine($"<form {UtilsLib.MakeIdAttribute(ScaffoldingSettings.EditFormId)} action='{Html.Super().GenerateUrl("Detail", controllerName, routeValues)}' method='{HtmlHelper.GetFormMethodString(FormMethod.Post)}' enctype='multipart/form-data'>");
            result.AppendLine($"<fieldset {UtilsLib.MakeIdAttribute(ScaffoldingSettings.EditFormFieldsetId)}>");
            result.AppendLine("<div" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTopDivId, ScaffoldingSettings.CRUDListTopDivCssClass) + ">");
                
            //var selectedId = (long?)Html.ViewBag.SelectedId ?? ParseNullableLong(Html.ViewContext.HttpContext.Request.Query["selectedId"]);
            var selectedId = ParseNullableLong(Html.ViewContext.HttpContext.Request.Query["selectedId"]);
                
            //This could be a potential scalability issue but I can't figure out how to solve it for now
            var newItem = AsyncHelper.RunSync(() => GetNewItemAsync(items.GetType(), dataContextType));
                
            // ReSharper disable once PossibleMultipleEnumeration
            var selectedItem = selectedId == 0 ? newItem : items.SingleOrDefault(x => x.Id == selectedId);
            var anySelected = selectedItem != null;

            if (!skipAddNew)
            {
                //make sure we keep query string
                //var addRouteValues = SuperHtml.QueryStringRouteValues();
                //var newAddRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = 0 });
                //addRouteValues.AddOrUpdateWith(newAddRouteValues);

                if (anySelected) result.AppendLine("<p><button type='button' disabled data-open-new-for-edit " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListAddNewCssClass) + "><span class='oi oi-plus'></span></button></p>");     
                else result.AppendLine("<p><button type='button' data-open-new-for-edit " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListAddNewCssClass) + "><span class='oi oi-plus'></span></button></p>");     
            }
                
            result.AppendLine("<table" + UtilsLib.MakeIdAndClassAttributes(ScaffoldingSettings.CRUDListTableId, ScaffoldingSettings.CRUDListTableCssClass) + ">");
            result.AppendLine("<thead>");
            result.AppendLine("<tr>");
            result = newItem.ToEditableHtmlTableHeader(Html, result);
            result.AppendLine("<th scope='col'> Actions </th>");
            result.AppendLine("</tr>");
            result.AppendLine("</thead>");
            result.AppendLine("<tbody>");

            //do html for selected item so that we could clear the ModelState, so that other forms do not pick up the data from ModelState
            string? selectedItemTrHtml = null; 
            if (selectedItem != null)
            {
                var itemInnerHtml = Html.Super().MakeInnerHtmlHelper(selectedItem, Config.InlinePrefix);
                selectedItemTrHtml = selectedId == 0 ? 
                    MakeNewItemEditableTr(selectedItem, itemInnerHtml, parentId, true) : 
                    MakeEditableTr(selectedItem, itemInnerHtml, parentId, true);
            }
            Html.ViewData.ModelState.Clear();
                
            //new item 
            var selected = newItem.Id == selectedId;
            if (selected)
            {
                result.Append(selectedItemTrHtml);
            }
            else
            {
                var itemInnerHtml = Html.Super().MakeInnerHtmlHelper(newItem, Config.InlinePrefix);
                result.Append(MakeNewItemEditableTr(newItem, itemInnerHtml, parentId, false));
            }

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                //are we dealing with a selected item?
                selected = item.Id == selectedId;
                    
                //make sure we keep query string
                var editViewDeleteRouteValues = SuperHtml.QueryStringRouteValues();
                var newEditViewDeleteRouteValues = HtmlHelper.AnonymousObjectToHtmlAttributes(new { id = item.Id });
                editViewDeleteRouteValues.AddOrUpdateWith(newEditViewDeleteRouteValues);
                    
                //Get inner html for the item
                var itemInnerHtml = Html.Super().MakeInnerHtmlHelper(item, Config.InlinePrefix, item.Id);
                    
                result = result.Append(MakeReadOnlyTr(item, itemInnerHtml, parentId, selected, anySelected, skipDelete, controllerName, editViewDeleteRouteValues));

                if (selected) result.Append(selectedItemTrHtml);
                else result.Append(MakeEditableTr(item, itemInnerHtml, parentId, false));
            }
            result.AppendLine("</tbody>");
            result.AppendLine("</table>");
            result.AppendLine("</div>");
            result.AppendLine("</fieldset>");
            result.AppendLine("</form>");
        }

        return result.ToHtmlString();
    }
        
    protected string MakeReadOnlyTr(IViewModelForEntity item, IHtmlHelper<dynamic> itemInnerHtml, long? parentId, bool selected, bool anySelected, bool skipDelete, string controllerName, RouteValueDictionary editViewDeleteRouteValues)
    {
        var sb = new StringBuilder();
        if (selected) sb.AppendLine($"<tr id='{item.Id}' class='d-none'>");
        else sb.AppendLine($"<tr id='{item.Id}'>");
        sb = item.ToEditableHtmlTableRow(itemInnerHtml, parentId, false, false, sb);
        sb.AppendLine("<td>");
        if (anySelected) sb.AppendLine("<div class='btn-group d-none' data-read-only-tr-buttons>");
        else sb.AppendLine("<div class='btn-group' data-read-only-tr-buttons>");
        sb.AppendLine("<button type='button' data-open-for-edit " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListEditCssClass) + "><span class='oi oi-pencil'></span></button>");     
        if (!skipDelete) sb.AppendLine(SuperHtml.RESTfulActionLinkHtmlContent(HttpMethod.Delete, "<span class='oi oi-trash'></span>".ToHtmlString(), "Detail", controllerName, editViewDeleteRouteValues, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = ScaffoldingSettings.CRUDListDeleteCssClass, data_delete = "data-delete" }), "Are you sure?").GetString());
        sb.AppendLine("</div>");
        sb.AppendLine("</td>");
        sb.AppendLine("</tr>");
        return sb.ToString();
    }
    protected string MakeEditableTr(IViewModelForEntity item, IHtmlHelper<dynamic> itemInnerHtml, long? parentId, bool selected)
    {
        var sb = new StringBuilder();
        if (selected) sb.AppendLine($"<tr id='{-item.Id}' class='table-primary'>");
        else sb.AppendLine($"<tr id='{-item.Id}' class='table-primary d-none'>");
        sb = item.ToEditableHtmlTableRow(itemInnerHtml, parentId, true, selected, sb);
        sb.AppendLine("<td><div class='btn-group'>");
        sb.AppendLine("<button type='submit' " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListSaveCssClass) + " data_save_edit='data-save-edit'><span class='oi oi-circle-check'></span></button>");     
        sb.AppendLine("<button type='button' data-cancel-edit " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListCancelCssClass) + "><span class='oi oi-action-undo'></span></button>");     
        sb.AppendLine("</div></td>");
        sb.AppendLine("</tr>");
        return sb.ToString();
    }
    protected string MakeNewItemEditableTr(IViewModelForEntity newItem, IHtmlHelper<dynamic> itemInnerHtml, long? parentId, bool selected)
    {
        var sb = new StringBuilder();
        if (selected) sb.AppendLine($"<tr id='{newItem.Id}' class='table-primary'>");
        else sb.AppendLine($"<tr id='{newItem.Id}' class='table-primary d-none'>");
        itemInnerHtml.ViewContext.RouteData.Values["id"] = 0;
        sb = newItem.ToEditableHtmlTableRow(itemInnerHtml, parentId, true, selected, sb);
        sb.AppendLine("<td><div class='btn-group'>");
        sb.AppendLine("<button type='submit' " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListSaveCssClass) + " data-save-new-edit='data-save-new-edit'><span class='oi oi-circle-check'></span></button>");     
        sb.AppendLine("<button type='button' data-cancel-new-edit " + UtilsLib.MakeClassAttribute(ScaffoldingSettings.CRUDListCancelCssClass) + "><span class='oi oi-action-undo'></span></button>");     
        sb.AppendLine("</div></td>");
        sb.AppendLine("</tr>");
        return sb.ToString();
    }
    protected virtual async Task<IViewModelForEntity> GetNewItemAsync(Type iEnumerableType, Type dataContextType)
    {
        await using((IAsyncDisposable)ReflectionHelper.CreateGenericType(typeof(UnitOfWork<>), dataContextType, ReadOnly.Yes))
        {
            if (!iEnumerableType.IsGenericType) throw new SupermodelException("!iEnumerableType.IsGenericType");

            var mvcModelItemType = iEnumerableType.GenericTypeArguments.First();
            var newMvcModelItem = (IViewModelForEntity)ReflectionHelper.CreateType(mvcModelItemType);

            //Init mvc model if it requires async initialization
            if (newMvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
            
            var newEntityItem = newMvcModelItem.CreateEntity();
            newMvcModelItem = await newMvcModelItem.MapFromAsync(newEntityItem);
            return newMvcModelItem;
        }
    }

    //private Type? GetBaseGenericChildCRUDControllerType(Type me)
    //{
    //    Type? toCheck = me;
    //    while (toCheck != null && toCheck != typeof(object))
    //    {
    //        var curType = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
    //        if (typeof(ChildCRUDController<,,,,>) == curType) return toCheck;
    //        toCheck = toCheck.BaseType;
    //    }
    //    return null;
    //}
    #endregion

    #region New Search Action Link Helpers
    public IHtmlContent NewSearchActionLink(string? linkLabel = null)
    {
        return NewSearchActionLink(linkLabel?.ToHtmlEncodedIHtmlContent());
    }
    public IHtmlContent NewSearchActionLink(IHtmlContent? linkLabel)
    {
        linkLabel ??= "<span class='oi oi-magnifying-glass'></span>&nbsp;New Search".ToHtmlString();
        var routeValues = SuperHtml.QueryStringRouteValues();
        var controller = Html.ViewContext.RouteData.Values["controller"]!.ToString();
        var htmlAttributesDict = HtmlHelper.AnonymousObjectToHtmlAttributes(new {id = ScaffoldingSettings.NewSearchButtonId, @class = ScaffoldingSettings.NewSearchButtonCssClass});
        return SuperHtml.ActionLinkHtmlContent(linkLabel, "Search", controller!, routeValues, htmlAttributesDict);
    }
    #endregion

    #region CRUD Pagination Helpers
    public IHtmlContent Pagination(int? visiblePages = null, int? skip = null, int? take = null, int? totalCount = null)
    {
        visiblePages ??= 11;
        if (visiblePages < 3 || visiblePages % 2 == 0) throw new ArgumentException("Must be >=3 and odd", nameof(visiblePages));
            
        var query =  Html.ViewContext.HttpContext.Request.Query;
        skip ??= query.GetSkipValue() ?? 0;
        take ??= query.GetTakeValue();
        totalCount ??= (int?)Html.ViewBag.SupermodelTotalCount;

        var result = new StringBuilder();
        if (take != null && take < totalCount) //skip is never null
        {
            result.AppendLine("<nav>");
            result.AppendLine($"<ul class='pagination {ScaffoldingSettings.PaginationCssClass}'>");

            var currentPage = skip.Value / take.Value + 1;

            var firstPage = currentPage - visiblePages.Value / 2;
            if (firstPage < 1) firstPage = 1;

            var lastPage = firstPage + visiblePages.Value - 1;
            if (lastPage > totalCount / take)
            {
                firstPage -= lastPage - totalCount.Value / take.Value;
                if (firstPage < 1) firstPage = 1;
                lastPage = (int)Math.Ceiling((double)totalCount / take.Value);
            }

            //Prev page
            if (currentPage > 1) result.AppendLine("<li class='page-item'>" + GetPageActionLink("«", currentPage - 1, take.Value) + "</li>");
            else result.AppendLine("<li class='page-item disabled'><a href='#' class='page-link' tabindex='-1'>«</a></li>");

            //Neighboring pages
            for (var page = firstPage; page <= lastPage; page++)
            {
                var linkStr = GetPageActionLink(page.ToString(CultureInfo.InvariantCulture), page, take.Value);
                if (page == currentPage) result.AppendLine("<li class='page-item active'>" + linkStr + "</li>");
                else result.AppendLine("<li class='page-item'>" + linkStr + "</li>");
            }

            //Next page
            if (currentPage < lastPage) result.AppendLine("<li class='page-item'>" + GetPageActionLink("»", currentPage + 1, take.Value) + "</li>");
            else result.AppendLine("<li class='page-item disabled'><a href='#' class='page-link' tabindex='-1'>»</a></li>");

            result.AppendLine("</ul>");
            result.AppendLine("</nav>");
        }
        return result.ToHtmlString();
    }
    public int PaginationTotalRecordsCount()
    {
        var totalCount = Html.ViewBag.SupermodelTotalCount;
        if (totalCount == null) throw new ArgumentNullException();
        return totalCount;
    }
    private string GetPageActionLink(string linkText, int pageNum, int pageSize)
    {
        return Html.ActionLink(linkText, (string)Html.ViewContext.RouteData.Values["action"]!, Html.Super().QueryStringRouteValues().AddOrUpdateWith("smSkip", (pageNum - 1) * pageSize), new { @class = "page-link"}).GetString();
    }
    #endregion

    #region SortBy Helpers
    public IHtmlContent SortByDropdownForm(SortByOptions sortByOptions, object? htmlAttributes = null)
    {
        var htmlAttributesDict = AttributesDict.FromAnonymousObject(htmlAttributes);

        var result = new StringBuilder();

        //begin form
        result.AppendLine($"<form {UtilsLib.MakeIdAttribute(ScaffoldingSettings.SortByDropdownFormId)} method='{HtmlHelper.GetFormMethodString(FormMethod.Get)}'>");
        result.AppendLine($"<fieldset {UtilsLib.MakeIdAttribute(ScaffoldingSettings.SortByDropdownFieldsetId)}>");

        //Get all the query string params, except the dropdown
        var query = Html.ViewContext.HttpContext.Request.Query;
        foreach (var queryStringKey in query.Keys)
        {
            switch (queryStringKey)
            {
                case "smSortBy": 
                    continue;

                case "smSkip":
                    result.AppendLine("<input id='smSkip' name='smSkip' type='hidden' value='0' />");
                    break;

                case "_":
                    result.AppendLine($"<input name='_' type='hidden' value='{DateTime.Now.Ticks}'");
                    break;

                default:
                    result.AppendLine(Html.Hidden(queryStringKey, query[queryStringKey]).GetString());
                    break;
            }
        }

        //Get the dropdown
        var selectedValue = RequestHttpContext.Current.Request.Query["smSortBy"];
        var sortBySelectList = new List<SelectListItem> { new() { Value = "", Text = "Select Sort Order" } };
        foreach (var sortByOption in sortByOptions) 
        {
            sortBySelectList.Add(new SelectListItem { Value = sortByOption.Value, Text = "Sort By: " + sortByOption.Key, Selected = sortByOption.Value == selectedValue});
        }

        //Create an empty dict or a copy of it
        htmlAttributesDict = new AttributesDict(htmlAttributesDict);
            
        htmlAttributesDict["onchange"] = "this.form.submit();";
        htmlAttributesDict.AddOrAppendCssClass("form-control");

        result.AppendLine(Html.DropDownList("smSortBy", sortBySelectList, htmlAttributesDict.ToMvcDictionary()).GetString());

        //end fieldset and form
        result.AppendLine("</fieldset>");
        result.AppendLine("</form>");

        return result.ToHtmlString();
    }
    public IHtmlContent SortableColumnHeader(string headerName, string? orderBy, string? orderByDesc, string? tooltip = null, bool requiredLabel = false, IHtmlContent? sortedHtml = null, IHtmlContent? sortedHtmlDesc = null, object? htmlAttributes = null)
    {
        //var htmlAttributesDict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        var htmlAttributesDict = AttributesDict.FromAnonymousObject(htmlAttributes);
        
        if (!string.IsNullOrEmpty(tooltip))
        {
            htmlAttributesDict.Add("data-toggle", "tooltip");
            htmlAttributesDict.Add("title", tooltip);
            headerName += " \u24d8";
        }

        sortedHtml ??= "▲".ToHtmlString();
        sortedHtmlDesc ??= "▼".ToHtmlString();
            
        var result = new StringBuilder();
        result.AppendLine("<th>");

        var query = Html.ViewContext.HttpContext.Request.Query;
        var currentSortByValue = (query["smSortBy"].FirstOrDefault() ?? "").Trim();
        var action = (string)Html.ViewContext.RouteData.Values["action"]!;

        var routeValues = query.ToRouteValueDictionary().
            AddOrUpdateWith("smSkip", 0).
            AddOrUpdateWith("smSortBy", orderBy);

        var routeValuesDesc = query.ToRouteValueDictionary().
            AddOrUpdateWith("smSkip", 0).
            AddOrUpdateWith("smSortBy", orderByDesc);

        var requiredMark = "";
        if (requiredLabel) requiredMark = $"<sup><em class='text-danger font-weight-bold {ScaffoldingSettings.RequiredAsteriskCssClass}'>*</em></sup>";

        if (currentSortByValue == orderBy)
        {
            if (!string.IsNullOrEmpty(orderByDesc)) result.AppendLine(Html.ActionLink(headerName, action, routeValuesDesc, htmlAttributesDict).GetString() + requiredMark + sortedHtml);
            else if (!string.IsNullOrEmpty(orderBy)) result.AppendLine(Html.ActionLink(headerName, action, routeValues, htmlAttributesDict).GetString() + requiredMark + sortedHtml);
            else result.AppendLine(headerName + requiredMark);
        }
        else if (currentSortByValue == orderByDesc)
        {
            if (!string.IsNullOrEmpty(orderBy)) result.AppendLine(Html.ActionLink(headerName, action, routeValues, htmlAttributesDict).GetString() + requiredMark + sortedHtmlDesc);
            else if (!string.IsNullOrEmpty(orderByDesc)) result.AppendLine(Html.ActionLink(headerName, action, routeValuesDesc, htmlAttributesDict).GetString() + requiredMark + sortedHtmlDesc);
            else result.AppendLine(headerName + requiredMark);
        }
        else
        {
            if (!string.IsNullOrEmpty(orderBy)) result.AppendLine(Html.ActionLink(headerName, action, routeValues, htmlAttributesDict).GetString() + requiredMark);
            else if (!string.IsNullOrEmpty(orderByDesc)) result.AppendLine(Html.ActionLink(headerName, action, routeValuesDesc, htmlAttributesDict).GetString() + requiredMark);
            else result.AppendLine($"<span {UtilsLib.GenerateAttributesString(htmlAttributesDict)}>{headerName}</span>{requiredMark}");
        }
        result.AppendLine("</th>");
        return result.ToHtmlString();
    }
    #endregion

    #region Private Helpers
    protected string GetAccordionSection(string accordionId, AccordionPanel panel, string body)
    {
        return $@"
                    <div class=""card"">
                        <div class=""card-header"" id=""heading_{panel.ElementId}"">
                            <h5 class=""mb-0"">
                                <button type=""button"" class=""btn btn-link"" data-toggle=""collapse"" data-target=""#collapse_{panel.ElementId}"" aria-expanded=""{panel.Expanded}"" aria-controls=""collapse_{panel.ElementId}"">
                                    <h5 {UtilsLib.MakeClassAttribute(ScaffoldingSettings.AccordionSectionTitleCss)}>{panel.Title}</h5>
                                </button>
                            </h5>
                        </div>

                        <div id=""collapse_{panel.ElementId}"" class=""collapse {(panel.Expanded? "show" : "")}"" aria-labelledby=""heading_{panel.ElementId}"" data-parent=""#{accordionId}"">
                            <div class=""card-body"">{body}</div>
                        </div>
                    </div>
                    ";
    }
    protected bool ShouldShowValidationSummary(object model, ValidationSummaryVisible validationSummaryVisible)
    {
        switch (validationSummaryVisible)
        {
            case ValidationSummaryVisible.IfNoVisibleErrors:
            {
                var selectedId = ParseNullableLong(Html.ViewContext.HttpContext.Request.Query["selectedId"]);
                var showValidationSummary = !Html.ViewData.ModelState.IsValid && selectedId == null;
                foreach (var propertyInfo in model.GetType().GetDetailPropertyInfosInOrder())
                {
                    var msg = Html.ValidationMessage(propertyInfo.Name).GetString();
                    if (!msg.Contains("></span>")) showValidationSummary = false;
                }
                return showValidationSummary;
            }
            case ValidationSummaryVisible.Always: 
            { 
                return true;
            }
            case ValidationSummaryVisible.Never:
            {
                return false;
            } 
            default:
            {
                throw new SupermodelException($"Invalid ValidationSummaryVisible value {validationSummaryVisible}");
            }
        }
    }
    protected static long? ParseNullableLong(string? str)
    {
        if (long.TryParse(str, out var result)) return result;
        return null;
    }
    #endregion

    #region Properties
    public SuperHtmlHelper<TModel> SuperHtml { get; }
    public IHtmlHelper<TModel> Html => SuperHtml.Html;
    #endregion
}