using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.Models.Mvc;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.HtmlHelpers;

public class SuperHtmlHelper<TModel> : SuperHtmlHelper
{
    #region Constructors
    public SuperHtmlHelper(IHtmlHelper<TModel> html) : base(html){}
    #endregion

    #region MvcModel Display & Editor & Hidden & Label Helpers
    public IHtmlContent Editor(string expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        //If expression result is null, create a default one and reset cache
        var model = modelExpression.Model; //NotTypedHtml.ViewData.Model.PropertyGet(modelExpression.Name);
        if (model == null) 
        {
            model = ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType);
            NotTypedHtml.ViewData.Model.PropertySet(modelExpression.Name, model);
            modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
        }

        if (model is ISupermodelEditorTemplate templatedModel) 
        {
            var innerHtml = MakeInnerHtmlHelper(modelExpression);
            if (NotTypedHtml.ViewData.Model is IMvcModelForEntity iEntityModel) innerHtml.ViewContext.RouteData.Values["id"] = iEntityModel.Id;
            return templatedModel.EditorTemplate(innerHtml, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Special handling for HtmlString
        if (typeof(HtmlString).IsAssignableFrom(modelExpression.Metadata.ModelType)) return (HtmlString)model;

        return NotTypedHtml.Editor(expression);
    }
    public IHtmlContent EditorFor<TValue>(Expression<Func<TModel, TValue>> expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");
            
        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
            
        //If expression result is null, create a default one and reset cache
        var model = modelExpression.Model; //NotTypedHtml.ViewData.Model.PropertyGet(modelExpression.Name);
        if (model == null) 
        {
            model = ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType);
            NotTypedHtml.ViewData.Model.PropertySet(modelExpression.Name, model);
            modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
        }

        if (model is ISupermodelEditorTemplate templatedModel) 
        {
            var innerHtml = MakeInnerHtmlHelper(modelExpression);
            if (NotTypedHtml.ViewData.Model is IMvcModelForEntity iEntityModel) innerHtml.ViewContext.RouteData.Values["id"] = iEntityModel.Id;
            return templatedModel.EditorTemplate(innerHtml, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Special handling for HtmlString
        if (typeof(HtmlString).IsAssignableFrom(modelExpression.Metadata.ModelType)) return (HtmlString)model;

        return Html.EditorFor(expression);
    }
    public IHtmlContent EditorForModel(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        if (NotTypedHtml.ViewData.Model is ISupermodelEditorTemplate templatedModel)
        {
            return templatedModel.EditorTemplate(Html, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Special handling for HtmlString
        if (NotTypedHtml.ViewData.Model is HtmlString htmlStringModel) return htmlStringModel;

        return NotTypedHtml.EditorForModel();
    }

    public IHtmlContent Display(string expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        //If expression result is null, create a default one and reset cache
        var model = modelExpression.Model; //NotTypedHtml.ViewData.Model.PropertyGet(modelExpression.Name);
        if (model == null) 
        {
            model = ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType);
            NotTypedHtml.ViewData.Model.PropertySet(modelExpression.Name, model);
            modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
        }

        if (model is ISupermodelDisplayTemplate templatedModel) 
        {
            var innerHtml = MakeInnerHtmlHelper(modelExpression);
            if (NotTypedHtml.ViewData.Model is IMvcModelForEntity iEntityModel) innerHtml.ViewContext.RouteData.Values["id"] = iEntityModel.Id;
            return templatedModel.DisplayTemplate(innerHtml, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Special handling for MvcHtmlString
        if (typeof(HtmlString).IsAssignableFrom(modelExpression.Metadata.ModelType)) return (HtmlString)model;

        return NotTypedHtml.Display(expression);
    }
    public IHtmlContent DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        //If expression result is null, create a default one and reset cache
        var model = modelExpression.Model; //NotTypedHtml.ViewData.Model.PropertyGet(modelExpression.Name);
        if (model == null) 
        {
            model = ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType);
            NotTypedHtml.ViewData.Model.PropertySet(modelExpression.Name, model);
            modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
        }

        if (model is ISupermodelDisplayTemplate templatedModel) 
        {
            var innerHtml = MakeInnerHtmlHelper(modelExpression);
            if (NotTypedHtml.ViewData.Model is IMvcModelForEntity iEntityModel) innerHtml.ViewContext.RouteData.Values["id"] = iEntityModel.Id;
            return templatedModel.DisplayTemplate(innerHtml, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Special handling for MvcHtmlString
        if (typeof(HtmlString).IsAssignableFrom(modelExpression.Metadata.ModelType)) return (HtmlString)model;

        return Html.DisplayFor(expression);
    }
    public IHtmlContent DisplayForModel(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        if (NotTypedHtml.ViewData.Model is ISupermodelDisplayTemplate templatedModel)
        {
            return templatedModel.DisplayTemplate(Html, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Special handling for HtmlString
        if (NotTypedHtml.ViewData.Model is HtmlString htmlStringModel) return htmlStringModel;
            
        return NotTypedHtml.DisplayForModel();
    }

    public IHtmlContent Hidden(string expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        //If expression result is null, create a default one and reset cache
        var model = modelExpression.Model; //NotTypedHtml.ViewData.Model.PropertyGet(modelExpression.Name);
        if (model == null) 
        {
            model = ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType);
            NotTypedHtml.ViewData.Model.PropertySet(modelExpression.Name, model);
            modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
        }

        if (model is ISupermodelHiddenTemplate templatedModel)
        {
            var innerHtml = MakeInnerHtmlHelper(modelExpression);
            if (NotTypedHtml.ViewData.Model is IMvcModelForEntity iEntityModel) innerHtml.ViewContext.RouteData.Values["id"] = iEntityModel.Id;
            return templatedModel.HiddenTemplate(innerHtml, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        return NotTypedHtml.Hidden(expression);
    }
    public IHtmlContent HiddenFor<TValue>(Expression<Func<TModel, TValue>> expression, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        //If expression result is null, create a default one and reset cache
        var model = modelExpression.Model; //NotTypedHtml.ViewData.Model.PropertyGet(modelExpression.Name);
        if (model == null) 
        {
            model = ReflectionHelper.CreateType(modelExpression.ModelExplorer.ModelType);
            NotTypedHtml.ViewData.Model.PropertySet(modelExpression.Name, model);
            modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
            modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);
        }

        if (model is ISupermodelHiddenTemplate templatedModel)
        { 
            var innerHtml = MakeInnerHtmlHelper(modelExpression);
            if (NotTypedHtml.ViewData.Model is IMvcModelForEntity iEntityModel) innerHtml.ViewContext.RouteData.Values["id"] = iEntityModel.Id;
            return templatedModel.HiddenTemplate(innerHtml, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        return Html.HiddenFor(expression);
    }
    public IHtmlContent HiddenForModel(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        if (NotTypedHtml.ViewData.Model is ISupermodelHiddenTemplate templatedModel)
        {
            return templatedModel.HiddenTemplate(Html, screenOrderFrom, screenOrderTo, markerAttribute);
        }

        //Html.HiddenForModel does not exist in MVC so we do custom one here
        var result = new StringBuilder();
        if (NotTypedHtml.ViewData.TemplateInfo.TemplateDepth <= 1)
        {
            var properties = NotTypedHtml.ViewData.ModelMetadata.Properties.Where(pm => pm.ShowForEdit);
            foreach (var prop in properties)
            {
                result.AppendLine(Html.Super().Hidden(prop.PropertyName!).GetString());
            }
        }

        return result.ToHtmlString();
    }
        
    public IHtmlContent LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, object? htmlAttributes = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");
            
        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        return Html.LabelFor(expression, GetDisplayName(modelExpression.Name), htmlAttributes);
    }
    public IHtmlContent Label(string expression, object? htmlAttributes = null)
    {
        if (NotTypedHtml.ViewData.Model == null) throw new ArgumentException("Model == null");

        var modelExpressionProvider = new ModelExpressionProvider(NotTypedHtml.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(Html.ViewData, expression);

        return NotTypedHtml.Label(expression, GetDisplayName(modelExpression.Name), htmlAttributes);
    }
    #endregion

    #region HttpMethodOverride Helpers
    public IHtmlContent HttpMethodOverride(HttpMethod httpMethod)
    {
        string httpMethodStr;
        switch (httpMethod)
        {
            case HttpMethod.Put:
                httpMethodStr = "PUT";
                break;
            case HttpMethod.Delete:
                httpMethodStr = "DELETE";
                break;
            default:
                throw new ArgumentException("Invalid Http Verb", nameof(httpMethod));
        }
        return HttpMethodOverride(httpMethodStr);
    }
    public IHtmlContent HttpMethodOverride(string httpMethod)
    {
        if (string.IsNullOrEmpty(httpMethod)) throw new ArgumentNullException(nameof(httpMethod));
            
        if (string.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase) || 
            string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Invalid Http Method", nameof(httpMethod));

        return new TagBuilder("input")
        {
            Attributes = {
                ["type"] = "hidden",
                ["name"] = "X-HTTP-Method-Override",
                ["value"] = httpMethod
            }
        };
    }
    #endregion

    #region ActionLinkHtmlContent Helpers
    public IHtmlContent ActionLinkHtmlContent(IHtmlContent linkText, string actionName)
    {
        return Html.ActionLink(MarkerText, actionName).Replace(MarkerText, linkText.GetString());
    }
    public IHtmlContent ActionLinkHtmlContent(IHtmlContent linkText, string actionName, object routeValues)
    {
        return Html.ActionLink(MarkerText, actionName, routeValues).Replace(MarkerText, linkText.GetString());
    }
    public IHtmlContent ActionLinkHtmlContent(IHtmlContent linkText, string actionName, object routeValues, object htmlAttributes)
    {
        return Html.ActionLink(MarkerText, actionName, routeValues, htmlAttributes).Replace(MarkerText, linkText.GetString());
    }
    public IHtmlContent ActionLinkHtmlContent(IHtmlContent linkText, string actionName, string controllerName)
    {
        return Html.ActionLink(MarkerText, actionName, controllerName).Replace(MarkerText, linkText.GetString());
    }
    public IHtmlContent ActionLinkHtmlContent(IHtmlContent linkText, string actionName, string controllerName, object routeValues)
    {
        return Html.ActionLink(MarkerText, actionName, controllerName, routeValues).Replace(MarkerText, linkText.GetString());
    }
    public IHtmlContent ActionLinkHtmlContent(IHtmlContent linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
    {
        return Html.ActionLink(MarkerText, actionName, controllerName, routeValues, htmlAttributes).Replace(MarkerText, linkText.GetString());
    }
    #endregion

    #region RESTful ActionLinkHtmlContent helpers
    public IHtmlContent RESTfulActionLinkHtmlContent(HttpMethod httpMethod, IHtmlContent linkHtml, object htmlAttributes, string actionName, object routeValues, string confMsg, bool isButton = true)
    {
        var url = GenerateUrl(actionName, null, routeValues);
        return RESTfulActionLinkHtmlContent(linkHtml, url, httpMethod, htmlAttributes, confMsg, isButton);
    }
    public IHtmlContent RESTfulActionLinkHtmlContent(HttpMethod httpMethod, IHtmlContent linkHtml, object htmlAttributes, string actionName, object routeValues, string controllerName, string confMsg, bool isButton = true)
    {
        var url = GenerateUrl(actionName, controllerName, routeValues);
        return RESTfulActionLinkHtmlContent(linkHtml, url, httpMethod, htmlAttributes, confMsg, isButton);
    }
    public IHtmlContent RESTfulActionLinkHtmlContent(HttpMethod httpMethod, IHtmlContent linkHtml, string actionName, RouteValueDictionary routeValues, object? htmlAttributes, string confMsg, bool isButton = true)
    {
        var url = GenerateUrl(actionName, null, routeValues);
        return RESTfulActionLinkHtmlContent(linkHtml, url, httpMethod, htmlAttributes, confMsg, isButton);
    }
    public IHtmlContent RESTfulActionLinkHtmlContent(HttpMethod httpMethod, IHtmlContent linkHtml, string actionName, string controllerName, RouteValueDictionary? routeValues, object? htmlAttributes, string? confMsg, bool isButton = true)
    {
        var url = GenerateUrl(actionName, controllerName, routeValues);
        return RESTfulActionLinkHtmlContent(linkHtml, url, httpMethod, htmlAttributes, confMsg, isButton);
    }

    private IHtmlContent RESTfulActionLinkHtmlContent(IHtmlContent linkHtml, string url, HttpMethod httpMethod, object? htmlAttributes, string? confMsg, bool isButton)
    {
        var htmlAttributesDict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            
        string httpMethodOverride;
        switch (httpMethod)
        {
            case HttpMethod.Delete:
                httpMethodOverride = "Delete";
                break;
            case HttpMethod.Head:
                httpMethodOverride = "Head";
                break;
            case HttpMethod.Put:
                httpMethodOverride = "Put";
                break;
            case HttpMethod.Post:
                httpMethodOverride = "Post";
                break;
            default:
                throw new SupermodelException("Unsupported HttpVerb in ActionLinkFormContent");
        }

        TagBuilder tag;
        if (isButton)
        {
            tag = new TagBuilder("button");
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            tag.InnerHtml.AppendHtml(linkHtml.GetString() ?? "");
            tag.MergeAttribute("type", "button");
        }
        else
        {
            tag = new TagBuilder("a");
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            tag.InnerHtml.AppendHtml(linkHtml.GetString() ?? "");
            tag.MergeAttribute("href", "#");
        }

        if (confMsg != null) tag.MergeAttribute("onclick", "supermodel_restfulLinkToUrlWithConfirmation('" + url + "', '" + httpMethodOverride + "', '" + confMsg + "')");
        else tag.MergeAttribute("onclick", "supermodel_restfulLinkToUrlWithConfirmation('" + url + "', '" + httpMethodOverride + "')");
            
        tag.MergeAttributes(htmlAttributesDict);

        return tag;
    }
    #endregion

    #region Query String Route Values
    public RouteValueDictionary QueryStringRouteValues()
    {
        return Html.ViewContext.HttpContext.Request.Query.ToRouteValueDictionary();
    }
    #endregion

    #region UrlHelper
    public IUrlHelper GetUrlHelper()
    {
        var urlHelperFactory = Html.ViewContext!.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
        var urlHelper = urlHelperFactory.GetUrlHelper(Html.ViewContext); 
        return urlHelper;
    }
    public string GenerateUrl(string? actionName, string? controllerName = null, object? routeValues = null)
    {
        var urlHelper = Html.Super().GetUrlHelper();
        var actionContext = new UrlActionContext 
        {
            Action = actionName,
            Controller = controllerName,
            Values = routeValues
        };
        var url = urlHelper.Action(actionContext);
        return url!;
    }
    #endregion

    #region Properties
    public IHtmlHelper<TModel> Html => (IHtmlHelper<TModel>)NotTypedHtml;
    #endregion
}
    
public class SuperHtmlHelper
{
    #region Constructor
    public SuperHtmlHelper(IHtmlHelper html)
    {
        NotTypedHtml = html;
    }
    #endregion

    #region Methods
    public string GetDisplayName(string propertyName)
    {
        return NotTypedHtml.ViewData.ModelMetadata.ModelType.GetDisplayNameForProperty(propertyName);
    }
    public IHtmlHelper<dynamic> MakeInnerHtmlHelper(ModelExpression modelExpression, bool forceNoPrefix = false)
    {
        var innerHtml = NotTypedHtml.ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlHelper<dynamic>>();
        var viewData = new ViewDataDictionary<dynamic>(NotTypedHtml.ViewData, modelExpression.Model);
        var viewDataDictionary = new ViewDataDictionary(viewData) 
        { 
            Model = modelExpression.Model, 
            TemplateInfo = 
            { 
                FormattedModelValue = modelExpression.Model, 
                HtmlFieldPrefix = forceNoPrefix ? "" : NotTypedHtml.ViewData.TemplateInfo.GetFullHtmlFieldName(modelExpression.Name)
            }
        };
        var viewContext = new ViewContext(NotTypedHtml.ViewContext, NotTypedHtml.ViewContext.View, viewDataDictionary, NotTypedHtml.ViewContext.Writer);
        (innerHtml as IViewContextAware)?.Contextualize(viewContext);
        innerHtml.ViewContext.ViewData["OuterHtml"] = NotTypedHtml;

        return innerHtml;
    }
    public IHtmlHelper<dynamic> MakeInnerHtmlHelper(dynamic model, string htmlFieldPrefix = "", long? id = null)
    {
        var innerHtml = NotTypedHtml.ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlHelper<dynamic>>();
        var viewData = new ViewDataDictionary<dynamic>(NotTypedHtml.ViewData, model);
        var viewDataDictionary = new ViewDataDictionary(viewData) 
        { 
            Model = model,
            TemplateInfo = 
            { 
                FormattedModelValue = model, 
                HtmlFieldPrefix = htmlFieldPrefix
            }
        };
        var viewContext = new ViewContext(NotTypedHtml.ViewContext, NotTypedHtml.ViewContext.View, viewDataDictionary, NotTypedHtml.ViewContext.Writer);
        (innerHtml as IViewContextAware)?.Contextualize(viewContext);
        innerHtml.ViewContext.ViewData["OuterHtml"] = NotTypedHtml;

        if (id.HasValue) innerHtml.ViewContext.RouteData.Values["id"] = id.Value;

        return innerHtml;
    }
    #endregion

    #region Properties
    public const string MarkerText = "][%$!)]^[(!$%][";
    public IHtmlHelper NotTypedHtml { get; }
    #endregion
}