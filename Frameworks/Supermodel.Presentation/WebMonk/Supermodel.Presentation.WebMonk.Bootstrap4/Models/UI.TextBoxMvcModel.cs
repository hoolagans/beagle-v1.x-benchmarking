using Supermodel.ReflectionMapper;
using System;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Misc;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class TextBoxMvcModel : UIComponentBase
    {
        #region IRMapperCustom implemtation
        public override Task MapFromCustomAsync<T>(T other)
        {
            Value = (other != null ? other.ToString() : "")!;
            InitFor<T>();
            return Task.CompletedTask;
        }
        // ReSharper disable once RedundantAssignment
        public override Task<T> MapToCustomAsync<T>(T other)
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
                Pattern ??= "";
                Type ??= "text";
                Step ??= "";
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
                Pattern ??= ""; //"[0-9]*";
                Type ??= "number";
                Step ??= "1";
            }
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?) ||
                     typeof(T) == typeof(float) ||  typeof(T) == typeof(float?) ||
                     typeof(T) == typeof(decimal) ||  typeof(T) == typeof(decimal?))
            {
                Pattern ??= ""; //"[+-]?([0-9]*[.])?[0-9]+";
                Type ??= "number";
                Step ??= "any";
            }
            else 
            {
                throw new Exception($"TextBoxMvcModel InitFor<T>: Unknown type {typeof(T).GetTypeFriendlyDescription()}");
            }
                
            //this is for fluent initialization
            return this; 
        }
        #endregion

        #region IEditorTemplate implemtation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            var attributesDict = new AttributesDict(HtmlAttributesAsDict);
            attributesDict.Add("type", Type);
            attributesDict.Add("class", "form-control");
                
            if (Pattern != "") attributesDict.Add("pattern", Pattern);
            if (Step != "") attributesDict.Add("step", Step);
                
            //If this is numeric, remove $ and ,
            var value = Type == "number" ? Value.Replace("$", "").Replace(",", "") : Value;
                
            var inputTag = Render.TextBoxForModel(value, attributesDict);
            inputTag.AddOrUpdateAttr(attributes);

            return inputTag;
        }
        #endregion

        #region IDisplayTemplate implemtation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            var value = Value;
            if (DisplayNumericFormat != null && !string.IsNullOrEmpty(Value)) value = decimal.Parse(Value).ToString(DisplayNumericFormat);
            return new Txt(value);
        }
        #endregion

        #region IComparable implementation
        public override int CompareTo(object? obj)
        {
            if (obj == null) return 1;
                
            if (Type == "number")
            {
                var valueToCompareWith = ((TextBoxMvcModel)obj).DecimalValue;
                if (DecimalValue == null && valueToCompareWith == null) return 0;
                if (DecimalValue == null || valueToCompareWith == null) return 1;
                return decimal.Compare(DecimalValue.Value, valueToCompareWith.Value);
            }

            return base.CompareTo(obj);
        }
        #endregion

        #region IUIComponentWithValue implementation
        public override string ComponentValue 
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

        public string? Type { get; set; }
        public string? Pattern { get; set; }
        public string? Step { get; set; }

        public object HtmlAttributesAsObj { set => HtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict HtmlAttributesAsDict { get; set; } = new();

        public string? DisplayNumericFormat { get; set; }
        #endregion
    }
}