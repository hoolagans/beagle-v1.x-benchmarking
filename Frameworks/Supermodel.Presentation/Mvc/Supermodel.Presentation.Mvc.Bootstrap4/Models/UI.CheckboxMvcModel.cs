using System;
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

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class CheckboxMvcModel : IRMapperCustom, ISupermodelEditorTemplate, ISupermodelDisplayTemplate, ISupermodelHiddenTemplate, ISupermodelModelBinder, IComparable, IUIComponentWithValue
    {
        #region IRMapperCustom implemtation
        public virtual Task MapFromCustomAsync<T>(T other)
        {
            if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?)) throw new ArgumentException("other must be of bool type", nameof(other));

            Value = (other != null ? other.ToString() : false.ToString())!;
            return Task.CompletedTask;
        }
        // ReSharper disable once RedundantAssignment
        public virtual Task<T> MapToCustomAsync<T>(T other)
        {
            if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?)) throw new ArgumentException("other must be of bool type", nameof(other));
                
            other = (T)(object)bool.Parse(Value);
            return Task.FromResult(other);
        }
        #endregion

        #region ISupermodelEditorTemplate implemtation
        public virtual IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            var htmlAttributes = new AttributesDict(HtmlAttributesAsDict);
            htmlAttributes.Add("type", Type);
            htmlAttributes.Add("class", "form-check-input");
                
            var sb = new StringBuilder();
            sb.AppendLine("<div class='form-check py-2'>");
                
            var text = html.CheckBox("", ValueBool, htmlAttributes.ToMvcDictionary()).GetString();
            text = text.Replace("/>", $"{markerAttribute} />");
            sb.AppendLine(text);

            sb.AppendLine("</div>");
            return sb.ToHtmlString();
        }
        #endregion

        #region ISupermodelDisplayTemplate implementation
        public virtual IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return ValueBool.ToYesNo().ToHtmlEncodedHtmlString();
        }
        #endregion

        #region ISupermodelHiddenTemplate implemtation
        public virtual IHtmlContent HiddenTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            return html.Hidden("", Value ?? false.ToString());
        }
        #endregion

        #region ISuperModelBinder implemtation
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));  

            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
                
            string attemptedValue;
            if (string.IsNullOrEmpty(val.FirstValue)) attemptedValue = false.ToString();
            else attemptedValue = val.FirstValue;

            if (bool.TryParse(attemptedValue, out var boolResult)) Value = boolResult.ToString();
            else Value = false.ToString();
                
            bindingContext.ModelState.SetModelValue(key, val);

            //if (bindingContext.Model == null) bindingContext.Model = this;
            var existingModel = (CheckboxMvcModel)bindingContext.Model!;
            existingModel.Value = Value;

            bindingContext.Result = ModelBindingResult.Success(existingModel);  

            return Task.CompletedTask;
        }
        #endregion

        #region IComparable implementation
        public virtual int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            var valueToCompareWith = ((TextBoxMvcModel)obj).Value;
            // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Value == null && valueToCompareWith == null) return 0;
            if (Value == null || valueToCompareWith == null) return 1;
            // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            return string.Compare(Value, valueToCompareWith, StringComparison.InvariantCulture);
        }
        #endregion

        #region IUIComponentWithValue implementation
        public virtual string ComponentValue 
        {
            get => Value;
            set => Value = value;
        }
        #endregion

        #region ToString override
        public override string ToString()
        {
            return Value;
        }
        #endregion

        #region Properies
        public string Value { get; set; } = false.ToString();
        public bool ValueBool
        {
            get
            {
                if (string.IsNullOrEmpty(Value)) return false;
                if (bool.TryParse(Value, out var boolean)) return boolean;
                return false;
            }
            set => Value = value.ToString();
        }

        public string Type { get; set; } = "checkbox";

        public object HtmlAttributesAsObj { set => HtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict HtmlAttributesAsDict { get; set; } = new();
        #endregion
    }        
}