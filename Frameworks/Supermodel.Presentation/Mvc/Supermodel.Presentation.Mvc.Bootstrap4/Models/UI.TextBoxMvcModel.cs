using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.DataAnnotations.Misc;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.ModelBinding;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class TextBoxMvcModel : IRMapperCustom, ISupermodelEditorTemplate, ISupermodelDisplayTemplate, ISupermodelHiddenTemplate, ISupermodelModelBinder, IComparable, IUIComponentWithValue
    {
        #region IRMapperCustom implemtation
        public virtual Task MapFromCustomAsync<T>(T other)
        {
            Value = (other != null ? other.ToString() : "")!;
            InitFor<T>();
            return Task.CompletedTask;
        }
        // ReSharper disable once RedundantAssignment
        public virtual Task<T> MapToCustomAsync<T>(T other)
        {
            if (typeof(T) == typeof(string)) 
            {
                other = (T)(object)Value;
                return Task.FromResult(other);
            }
                
            if (!string.IsNullOrEmpty(Value))
            {
                if (typeof(T) == typeof(int) || typeof(T) == typeof(int?)) other = (T)(object)int.Parse(Value);
                else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?)) other = (T)(object)uint.Parse(Value);
                else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?)) other = (T)(object)long.Parse(Value);
                else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?)) other = (T)(object)ulong.Parse(Value);
                else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?)) other = (T)(object)short.Parse(Value);
                else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?)) other = (T)(object)ushort.Parse(Value);
                else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?)) other = (T)(object)byte.Parse(Value);
                else if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?)) other = (T)(object)sbyte.Parse(Value);
                
                else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?)) other = (T)(object)double.Parse(Value);
                else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?)) other = (T)(object)float.Parse(Value);
                else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?)) other = (T)(object)decimal.Parse(Value);
                else throw new Exception($"TextBoxMvcModel.MapToCustom: Unknown type {typeof(T).GetTypeFriendlyDescription()}");
            }
            else
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)) other = default!;
                else throw new ValidationResultException($"Cannot parse blank string into {typeof(T).GetTypeFriendlyDescription()}");
            }

            return Task.FromResult(other);
        }
        public virtual TextBoxMvcModel InitFor<T>()
        {
            if (typeof(T) == typeof(string))
            {
                Pattern = "";
                Type = "text";
                Step ="";
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?) ||
                     typeof(T) == typeof(uint) || typeof(T) == typeof(uint?) ||
                     typeof(T) == typeof(long) ||  typeof(T) == typeof(long?) ||
                     typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?) ||
                     typeof(T) == typeof(short) ||  typeof(T) == typeof(short?) ||
                     typeof(T) == typeof(ushort) ||  typeof(T) == typeof(ushort?) ||
                     typeof(T) == typeof(byte) || typeof(T) == typeof(byte?) ||
                     typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
            {
                //Pattern = "[0-9]*";
                Type = "number";
                Step ="1";
            }
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?) ||
                     typeof(T) == typeof(float) ||  typeof(T) == typeof(float?) ||
                     typeof(T) == typeof(decimal) ||  typeof(T) == typeof(decimal?))
            {
                //Pattern = "[+-]?([0-9]*[.])?[0-9]+";
                Type = "number";
                Step ="any";
            }
            else 
            {
                throw new Exception($"TextBoxMvcModel.InitFor<T>: Unknown type {typeof(T).GetTypeFriendlyDescription()}");
            }
                
            //this is for fluent initialization
            return this; 
        }
        #endregion

        #region ISupermodelEditorTemplate implemtation
        public virtual IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            var htmlAttributes = new AttributesDict(HtmlAttributesAsDict);
            htmlAttributes.Add("type", Type);
            htmlAttributes.Add("class", "form-control");
                
            if (Pattern != "") htmlAttributes.Add("pattern", Pattern);
            if (Step != "") htmlAttributes.Add("step", Step);
                
            //If this is numeric, remove $ and ,
            var value = Type == "number" ? Value.Replace("$", "").Replace(",", "") : Value;
                
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            var text = html.TextBox("", value ?? "", htmlAttributes.ToMvcDictionary()).GetString();
            text = text.Replace("/>", $"{markerAttribute} />");
            return text.ToHtmlString();
        }
        #endregion

        #region ISupermodelDisplayTemplate implementation
        public virtual IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            var value = Value;
            if (DisplayNumericFormat != null && !string.IsNullOrEmpty(Value)) value = decimal.Parse(Value).ToString(DisplayNumericFormat);
            return value.ToHtmlEncodedHtmlString();
        }
        #endregion

        #region ISupermodelHiddenTemplate implemtation
        public virtual IHtmlContent HiddenTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            return html.Hidden("", Value ?? "");
        }
        #endregion

        #region ISuperModelBinder implemtation
        public virtual Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));  

            var success = true;
                
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
            string? displayName = null;
                
            string attemptedValue;
            if (string.IsNullOrEmpty(val.FirstValue))
            {
                displayName = bindingContext.ModelMetadata.ContainerType!.GetDisplayNameForProperty(bindingContext.ModelMetadata.PropertyName!);
                    
                if (bindingContext.IsPropertyRequired())
                {
                    bindingContext.ModelState.AddModelError(key, $"The {displayName} field is required");
                    success = false; 
                }
                attemptedValue = "";
            }
            else
            {
                attemptedValue = val.FirstValue;
            }

            try
            {
                Value = attemptedValue;
            }
            catch (FormatException)
            {
                Value = "";
                if (displayName == null) displayName = bindingContext.ModelMetadata.ContainerType!.GetDisplayNameForProperty(bindingContext.ModelMetadata.PropertyName!);
                bindingContext.ModelState.AddModelError(key, $"The field {displayName} is invalid");
                success = false;
            }

            bindingContext.ModelState.SetModelValue(key, val);

            //if (bindingContext.Model == null) bindingContext.Model = this;
            var existingModel = (TextBoxMvcModel)bindingContext.Model!;
            existingModel.Value = Value;

            if (success) bindingContext.Result = ModelBindingResult.Success(existingModel);  
            else bindingContext.Result = ModelBindingResult.Failed(); 

            return Task.CompletedTask;
        }
        #endregion

        #region IComparable implementation
        public virtual int CompareTo(object? obj)
        {
            if (obj == null) return 1;
                
            if (Type == "number")
            {
                var valueToCompareWith = ((TextBoxMvcModel)obj).DecimalValue;
                if (DecimalValue == null && valueToCompareWith == null) return 0;
                if (DecimalValue == null || valueToCompareWith == null) return 1;
                return decimal.Compare(DecimalValue.Value, valueToCompareWith.Value);
            }
            else
            {
                var valueToCompareWith = ((TextBoxMvcModel)obj).Value;
                // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (Value == null && valueToCompareWith == null) return 0;
                if (Value == null || valueToCompareWith == null) return 1;
                // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                return string.Compare(Value, valueToCompareWith, StringComparison.InvariantCulture);
            }
        }
        #endregion

        #region ToString override
        public override string ToString()
        {
            return Value;
        }
        #endregion

        #region IUIComponentWithValue implementation
        public virtual string ComponentValue 
        {
            get => Value;
            set => Value = value;
        }
        #endregion

        #region Properies
        public string Value { get; set; } = "";
        public int? IntValue 
        { 
            get 
            {
                if (int.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public uint? UIntValue 
        { 
            get 
            {
                if (uint.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public long? LongValue 
        { 
            get 
            {
                if (long.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public ulong? ULongValue 
        { 
            get 
            {
                if (ulong.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public short? ShortValue 
        { 
            get 
            {
                if (short.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public ushort? UShortValue 
        { 
            get 
            {
                if (ushort.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public byte? ByteValue 
        { 
            get 
            {
                if (byte.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public sbyte? SByteValue 
        { 
            get 
            {
                if (sbyte.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public double? DoubleValue 
        { 
            get 
            {
                if (double.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public float? FloatValue 
        { 
            get 
            {
                if (float.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }
        public decimal? DecimalValue 
        { 
            get 
            {
                if (decimal.TryParse(Value, out var val)) return val;
                return null;
            }
            set => Value = value?.ToString() ?? "";
        }

        public string Type { get; set; } = "text";
        public string Pattern { get; set; } = "";
        public string Step { get; set; } = "";

        public object HtmlAttributesAsObj { set => HtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict HtmlAttributesAsDict { get; set; } = new();

        public string? DisplayNumericFormat { get; set; }
        #endregion
    }
}