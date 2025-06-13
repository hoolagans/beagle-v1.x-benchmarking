using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Presentation.Mvc.Bootstrap4.Models;
using Supermodel.Presentation.Mvc.Bootstrap4.SuperHtmlHelpers;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.HtmlHelpers;
using Supermodel.Presentation.Mvc.Models;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Extensions;

public static class ObjectMultiColumnListExtensions
{
    #region Read Only
    public static StringBuilder ToReadOnlyHtmlTableHeader<TModel>(this object me, IHtmlHelper<TModel> html, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        var propertyInfos = me.GetType().GetListPropertyInfosInOrder().ToList();
        if (!propertyInfos.Any()) throw new SupermodelException("ListMvcModelT must have at least one property marked with [ListColumn] attribute");
        foreach (var propertyInfo in propertyInfos)
        {
            var listColumnAttr = propertyInfo.GetCustomAttribute<ListColumnAttribute>();
            var header = listColumnAttr!.Header ?? propertyInfo.Name.InsertSpacesBetweenWords(); //all properties we get from GetPropertiesInOrder will contain ListColumnAttribute
            var orderBy = listColumnAttr.OrderBy;
            var orderByDesc = listColumnAttr.OrderByDesc;
            var title = propertyInfo.GetAttribute<TooltipAttribute>()?.Tooltip;
            sb.Append(html.Super().Bs4().SortableColumnHeader(header, orderBy, orderByDesc, title).GetString());
        }
        return sb;
    }
    public static StringBuilder ToReadOnlyHtmlTableRow<TModel>(this object me, IHtmlHelper<TModel> html, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        var propertyInfos = me.GetType().GetListPropertyInfosInOrder().ToList();
        if (!propertyInfos.Any()) throw new SupermodelException("ListMvcModelT must have at least one property marked with [ListColumn] attribute");
        foreach (var propertyInfo in propertyInfos)
        {
            sb.Append("<td>");
            var propertyObj = me.PropertyGet(propertyInfo.Name);
            if (propertyObj != null)
            {
                //We do special handling for IHtmlContent type to allow for HTML context in columns while still being secure
                if (propertyObj is IHtmlContent htmlContent)
                {
                    sb.Append(htmlContent.GetString());
                }
                else
                {
                    if (propertyObj is ISupermodelDisplayTemplate) sb.Append(html.Super().Display(propertyInfo.Name).GetString());
                    else sb.Append(html.Encode(propertyObj.GetDescription()));
                }
            }
            sb.Append("</td>");
        }
        return sb;
    }
    #endregion

    #region Editable
    public static StringBuilder ToEditableHtmlTableHeader<TModel>(this IViewModelForEntity me, IHtmlHelper<TModel> html, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        var propertyInfos = me.GetType().GetDetailPropertyInfosInOrder().ToList();
        if (!propertyInfos.Any()) throw new SupermodelException("DetailMvcModelT must have at least one property if used with Editable List");
        foreach (var propertyInfo in propertyInfos)
        {
            string header;
            string? orderBy, orderByDesc;
            var listColumnAttr = propertyInfo.GetCustomAttribute<ListColumnAttribute>();
            if (listColumnAttr != null)
            {
                header = listColumnAttr.Header ?? me.GetType().GetDisplayNameForProperty(propertyInfo.Name);
                orderBy = listColumnAttr.OrderBy;
                orderByDesc = listColumnAttr.OrderByDesc;
            }
            else
            {
                header = me.GetType().GetDisplayNameForProperty(propertyInfo.Name);
                orderBy = null;
                orderByDesc = null;
            }

            var title = propertyInfo.GetAttribute<TooltipAttribute>()?.Tooltip;
            var requiredLabel = !propertyInfo.HasAttribute<NoRequiredLabelAttribute>() && (propertyInfo.HasAttribute<RequiredAttribute>() || propertyInfo.HasAttribute<ForceRequiredLabelAttribute>());
            sb.Append(html.Super().Bs4().SortableColumnHeader(header, orderBy, orderByDesc, title, requiredLabel).GetString());
        }
        return sb;
    }
    public static StringBuilder ToEditableHtmlTableRow<TModel>(this IViewModelForEntity me, IHtmlHelper<TModel> html, long? parentId, bool editable, bool selected, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        var propertyInfos = me.GetType().GetDetailPropertyInfosInOrder().ToList();
        if (!propertyInfos.Any()) throw new SupermodelException("DetailMvcModelT must have at least one property if used with Editable List");

        var isFirst = true;
        var showValidationSummary = !html.ViewData.ModelState.IsValid && selected;
        var validationSummaryGuidPlaceholder = Guid.NewGuid().ToString();
        foreach (var propertyInfo in propertyInfos)
        {
            var idAttr = selected ? "id = \"SelectedRow\"" : "";
            sb.Append($"<td {idAttr}>");
            if (editable && isFirst)
            {
                sb.AppendLine($"<input id='Id' name='Id' type='hidden' value='{me.Id}'/>".DisableAllControlsIf(!selected));
                if (parentId != null) sb.AppendLine($"<input id='{Config.InlinePrefix}_parentId' name='{Config.InlinePrefix}.parentId' type='hidden' value='{parentId}'/>".DisableAllControlsIf(!selected));
                sb.AppendLine("<input id='IsInline' name='IsInline' type='hidden' value='true'/>".DisableAllControlsIf(!selected));
                if (!me.IsNewModel()) sb.AppendLine(html.Super().HttpMethodOverride(HttpMethod.Put).GetString().DisableAllControlsIf(!selected));
            }
            var propertyObj = me.PropertyGet(propertyInfo.Name);
            if (propertyObj != null)
            {
                //We do special handling for IHtmlContent type to allow for HTML context in columns while still being secure
                if (propertyObj is IHtmlContent htmlContent)
                {
                    sb.Append(htmlContent.GetString());
                }
                else
                {
                    if (editable)
                    {
                        if (!propertyInfo.HasAttribute<DisplayOnlyAttribute>())
                        {
                            var text = html.Super().Editor(propertyInfo.Name).GetString().DisableAllControlsIf(!selected);
                            if (!selected) text = text.Replace("checked=\"checked\"", "data-checked=\"checked\"");
                            sb.Append(text);

                            if (selected && !html.ViewData.ModelState.IsValid)
                            {
                                //var msg = html.ValidationMessage($"{Settings.InlinePrefix}.{propertyInfo.Name}", null, new { @class=Bs4.ScaffoldingSettings.InlineValidationErrorCssClass }).GetString();
                                var msg = html.ValidationMessage(propertyInfo.Name, null, new { @class=Bs4.ScaffoldingSettings.InlineValidationErrorCssClass }).GetString();
                                if (!msg.Contains("></span>")) showValidationSummary = false;
                                msg = msg.Replace("<span ", "<div ").Replace("</span>", "</div>");
                                sb.Append(msg);
                                    
                                //var value = html.ViewData.ModelState.SingleOrDefault(x => x.Key == $"{Settings.InlinePrefix}.{propertyInfo.Name}").Value;
                                //if (value?.Errors != null && value.Errors.Any(x => !string.IsNullOrEmpty(x.ErrorMessage)))
                                //{
                                //    sb.Append($"<div class='{Bs4.ScaffoldingSettings.ValidationErrorCssClass}'>");
                                //    sb.AppendLine(value.Errors.First(x => !string.IsNullOrEmpty(x.ErrorMessage)).ErrorMessage);
                                //    sb.Append("</div>");
                                //}
                            }
                        }
                        else
                        {
                            sb.AppendLine(html.Super().Display(propertyInfo.Name).GetString());
                        }
                    }
                    else
                    {
                        if (propertyObj is ISupermodelDisplayTemplate) sb.Append(html.Super().Display(propertyInfo.Name).GetString());  
                        else sb.Append(html.Encode(propertyObj.GetDescription()));
                    }
                }
            }
            if (editable && isFirst)
            {
                sb.AppendLine(validationSummaryGuidPlaceholder);
                isFirst = false;
            }
                
            sb.Append("</td>");
        }

        if (showValidationSummary)
        {
            var validationSummarySb = new StringBuilder();
            validationSummarySb.AppendLine($"<div class='{Bs4.ScaffoldingSettings.ValidationSummaryCssClass}'>");
            validationSummarySb.AppendLine(html.ValidationSummary().GetString());
            validationSummarySb.AppendLine("</div>");
            sb = sb.Replace(validationSummaryGuidPlaceholder, validationSummarySb.ToString());
        }
        else
        {
            sb = sb.Replace(validationSummaryGuidPlaceholder, "");
        }

        return sb;
    }
    #endregion
}