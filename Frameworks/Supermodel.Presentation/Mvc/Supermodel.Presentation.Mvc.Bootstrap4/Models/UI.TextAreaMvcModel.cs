using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Extensions;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class TextAreaMvcModel : TextBoxMvcModel
    {
        #region Constructors
        public TextAreaMvcModel()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            InitFor<string>();
        }
        #endregion
            
        #region IRMapperCustom implemtation
        public override Task MapFromCustomAsync<T>(T other)
        {
            if (typeof(T) != typeof(string)) throw new ArgumentException("other must be of string type", nameof(other));
            InitFor<string>();
            Value = (other != null ? other.ToString() : "")!;
            return Task.CompletedTask;
        }
        // ReSharper disable once RedundantAssignment
        public override Task<T> MapToCustomAsync<T>(T other)
        {
            if (typeof(T) != typeof(string)) throw new ArgumentException("other must be of string type", nameof(other));

            other = (T)(object)Value;
            return Task.FromResult(other);
        }
        #endregion

        #region ISupermodelEditorTemplate implemtation
        public override IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            var htmlAttributes = new AttributesDict(HtmlAttributesAsDict);
            htmlAttributes.Add("type", Type);
            htmlAttributes.Add("rows", Rows.ToString());
            htmlAttributes.Add("class", "form-control");
                
            if (Pattern != "") htmlAttributes.Add("pattern", Pattern);
            if (Step != "") htmlAttributes.Add("step", Step);
                
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            var text = html.TextArea("", Value ?? "", htmlAttributes.ToMvcDictionary()).GetString();
            text = text.Replace("/>", $"{markerAttribute} />");
            return text.ToHtmlString();
        }
        #endregion
            
        #region ISupermodelDisplayTemplate implementation
        public override IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            var value = Value;
            if (DisplayNumericFormat != null && !string.IsNullOrEmpty(Value)) value = decimal.Parse(Value).ToString(DisplayNumericFormat);
            if (DisplayCapLengthAt != null && value.Length > DisplayCapLengthAt) value = $"{value.CapLength(DisplayCapLengthAt.Value)}...";
            if (DisplayShowLineBreaks) 
            {
                value = HttpUtility.HtmlEncode(value).Replace("\n", "<br />");
                return value.ToHtmlString();
            }
            else
            {
                return value.ToHtmlEncodedHtmlString();
            }
        }
        #endregion

        #region Properties
        public int Rows { get; set; } = 3;
        public int? DisplayCapLengthAt { get; set; }
        public bool DisplayShowLineBreaks { get; set; } = false;
        #endregion
    }
}