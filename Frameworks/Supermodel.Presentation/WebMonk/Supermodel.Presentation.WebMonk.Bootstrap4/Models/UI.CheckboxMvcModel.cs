using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using Supermodel.Presentation.WebMonk.Extensions;
using WebMonk.Context;
using WebMonk.Exceptions;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class CheckboxMvcModel : UIComponentBase
    {
        #region IRMapperCustom implemtation
        public override Task MapFromCustomAsync<T>(T other)
        {
            if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?)) throw new ArgumentException("other must be of bool type", nameof(other));

            Value = (other != null ? other.ToString() : false.ToString())!;
            return Task.CompletedTask;
        }
        // ReSharper disable once RedundantAssignment
        public override Task<T> MapToCustomAsync<T>(T other)
        {
            if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?)) throw new ArgumentException("other must be of bool type", nameof(other));

            other = (T)(object)bool.Parse(Value);
            return Task.FromResult(other);
        }
        #endregion

        #region IEditorTemplate implemtation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            var htmlAttributes = new AttributesDict(HtmlAttributesAsDict);
            htmlAttributes.Add("type", Type);
            htmlAttributes.Add("class", "form-check-input");

            var div = new HtmlStack();
            div.AppendAndPush(new Div(new { @class="form-check py-2" }));

            var checkBoxTags = div.Append(Render.CheckBoxForModel(ValueBool, htmlAttributes));
            var checkboxInputTag = (Input)checkBoxTags[0];
            if (checkboxInputTag.Attributes["type"] != "checkbox") throw new SupermodelException("checkboxInputTag.Attributes[\"type\"] != \"checkbox\": this should never happen");
            checkboxInputTag.AddOrUpdateAttr(attributes);
                
            div.Pop<Div>();
            return div;
        }
        #endregion

        #region IDisplayTemplate implementation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return new Txt(ValueBool.ToYesNo());
        }
        #endregion

        #region ISelfModelBinder implementation
        public override Task<object?> BindMeAsync(Type rootType, List<IValueProvider> valueProviders)
        {
            var prefix = HttpContext.Current.PrefixManager.CurrentPrefix;

            if (string.IsNullOrEmpty(prefix)) throw new WebMonkException("prefix is not set");
            var name = prefix.ToHtmlName();

            ValueBool = valueProviders.GetValueOrDefault<bool>(name).UpdateInternal(ValueBool);

            return Task.FromResult((object?)this);
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