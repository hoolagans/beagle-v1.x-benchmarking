using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.ModelBinding;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;

public abstract class SingleSelectMvcModel : IComparable, ISupermodelModelBinder, ISupermodelEditorTemplate, ISupermodelDisplayTemplate, ISupermodelHiddenTemplate, IUIComponentWithValue
{
    #region Option nested class
    public class Option
    {
        public Option(string value, string label, bool isDisabled = false)
        {
            Value = value;
            Label = label;
            IsDisabled = isDisabled;
        }
        public string Value { get; }
        public string Label { get; }
        public bool IsDisabled { get; }
    }
    public List<Option> Options { get; protected set; } = new();
    #endregion

    #region Static Dropdown and Radio helpers
    public virtual IHtmlContent CommonDropdownEditorTemplate<T>(IHtmlHelper<T> html, AttributesDict? htmlAttributesAsDict)
    {
        if (html.ViewData.Model == null) throw new NullReferenceException($"{ReflectionHelper.GetCurrentContext()} is called for a model that is null");
        if (!(html.ViewData.Model is SingleSelectMvcModel)) throw new InvalidCastException($"{ReflectionHelper.GetCurrentContext()} is called for a model of type different from SingleSelectMvcModelBase.");

        var singleSelect = (SingleSelectMvcModel)(object)html.ViewData.Model;

        htmlAttributesAsDict ??= new AttributesDict();
        htmlAttributesAsDict.AddOrAppendCssClass("form-control");

        var selectListItemList = new List<SelectListItem> { new() { Value = "", Text = "" } };
        foreach (var option in singleSelect.Options)
        {
            var isSelectedOption = singleSelect.SelectedValue != null && string.CompareOrdinal(singleSelect.SelectedValue, option.Value) == 0;
            if (isSelectedOption || !option.IsDisabled)
            {
                var item = new SelectListItem { Value = option.Value, Text = !option.IsDisabled ? option.Label : option.Label + DisabledSuffix, Selected = isSelectedOption };
                selectListItemList.Add(item);
            }
        }

        var htmlContent = html.DropDownList("", selectListItemList, htmlAttributesAsDict.ToMvcDictionary());
        return htmlContent;
    }
    public virtual IHtmlContent CommonRadioEditorTemplate<T>(IHtmlHelper<T> html, AttributesDict? divHtmlAttributesAsDict, AttributesDict? inputHtmlAttributesAsDict, AttributesDict? labelHtmlAttributesAsDict)
    {
        if (html.ViewData.Model == null) throw new NullReferenceException($"{ReflectionHelper.GetCurrentContext()} is called for a model that is null");
        if (!(html.ViewData.Model is SingleSelectMvcModel)) throw new InvalidCastException($"{ReflectionHelper.GetCurrentContext()} is called for a model of type different from SingleSelectMvcModelBase.");

        var singleSelect = (SingleSelectMvcModel)(object)html.ViewData.Model;
        var result = new StringBuilder();

        //Set up class attributes
        divHtmlAttributesAsDict ??= new AttributesDict();
        divHtmlAttributesAsDict.AddOrAppendCssClass("form-check");

        inputHtmlAttributesAsDict ??= new AttributesDict();
        inputHtmlAttributesAsDict.AddOrAppendCssClass("form-check-input");

        labelHtmlAttributesAsDict ??= new AttributesDict();
        labelHtmlAttributesAsDict.AddOrAppendCssClass("form-check-label");

        foreach (var option in singleSelect.Options)
        {
            result.AppendLine("<div " + UtilsLib.GenerateAttributesString(divHtmlAttributesAsDict) + ">");
            var isSelectedOption = singleSelect.SelectedValue != null && string.CompareOrdinal(singleSelect.SelectedValue, option.Value) == 0;
            if (isSelectedOption || !option.IsDisabled)
            {
                result.AppendLine(html.RadioButton("", option.Value, singleSelect.SelectedValue == option.Value, inputHtmlAttributesAsDict.ToMvcDictionary()).GetString());
                result.AppendLine(html.Label("", !option.IsDisabled ? option.Label : option.Label + DisabledSuffix, labelHtmlAttributesAsDict.ToMvcDictionary()).GetString());
            }
            result.AppendLine("</div>");
        }
        result.AppendLine(string.Format("<input id='{0}' name='{0}' type='hidden' value=''>", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName("")));
            
        return result.ToHtmlString();        
    }
    #endregion

    #region ISuperModelBinder implemtation
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        bool success = true;
            
        var key = bindingContext.ModelName;
        var val = bindingContext.ValueProvider.GetValue(key);
        string attemptedValue;
        if (string.IsNullOrEmpty(val.FirstValue))
        {
            if (bindingContext.IsPropertyRequired()) 
            {
                var displayName = bindingContext.ModelMetadata.ContainerType!.GetDisplayNameForProperty(bindingContext.ModelMetadata.PropertyName!);
                bindingContext.ModelState.AddModelError(key, $"The {displayName} field is required");
                success = false;
            }
            attemptedValue = "";
        }
        else
        {
            attemptedValue = val.FirstValue.Replace(",", "").Trim();
        }

        bindingContext.ModelState.SetModelValue(key, val);

        SelectedValue = attemptedValue;

        //if (bindingContext.Model == null) bindingContext.Model = this;
        var existingModel = (SingleSelectMvcModel)bindingContext.Model!;
        existingModel.SelectedValue = SelectedValue;

        if (success) bindingContext.Result = ModelBindingResult.Success(existingModel);  
        else bindingContext.Result = ModelBindingResult.Failed(); 

        return Task.CompletedTask;
    }
    #endregion

    #region ISupermodelEditorTemplate implemtation
    public abstract IHtmlContent EditorTemplate<T>(IHtmlHelper<T> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null);
    #endregion

    #region ISupermodelDipslayTemplate implementation
    public virtual IHtmlContent DisplayTemplate<T>(IHtmlHelper<T> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        //return EditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute).DisableAllControls();
        return (SelectedLabel ?? "").ToHtmlEncodedHtmlString();
    }
    #endregion

    #region ISupermodelHIddenTemplate implemtation
    public virtual IHtmlContent HiddenTemplate<T>(IHtmlHelper<T> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
    {
        return html.Hidden("", SelectedValue);
    }
    #endregion

    #region IComparable implementation
    public virtual int CompareTo(object? obj)
    {
        if (obj == null) return 1;
                    
        var valueToCompareWith = ((SingleSelectMvcModel)obj).SelectedValue;
        if (SelectedValue == null && valueToCompareWith == null) return 0;
        if (SelectedValue == null || valueToCompareWith == null) return 1;
        return string.Compare(SelectedValue, valueToCompareWith, StringComparison.InvariantCulture);
    }
    #endregion

    #region IUIComponentWithValue implementation
    public virtual string ComponentValue 
    {
        get => SelectedValue ?? "";
        set => SelectedValue = value;
    }
    #endregion

    #region ToString override
    public override string ToString()
    {
        return SelectedValue ?? "";
    }
    #endregion

    #region Properties
    public string? SelectedValue { get; set; }
    public string? SelectedLabel
    {
        get
        {
            var selectedOption = Options.FirstOrDefault(x => x.Value == SelectedValue);
            return selectedOption?.Label;
        }
    }
    public bool IsEmpty => string.IsNullOrEmpty(SelectedValue);
    public string DisabledSuffix { get; set; } = " [DISABLED]";
    #endregion
}