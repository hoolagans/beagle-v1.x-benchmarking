using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.Models;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class PasswordTextBoxMvcModel : TextBoxMvcModel
    {
        #region Embedded Types
        public enum PlaceholderBehaviorEnum { Default, ForceNoPlaceholder, ForceDotDotDotPlaceholder }
        #endregion
            
        #region Constructors
        public PasswordTextBoxMvcModel()
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

            Value = "";
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
                
            Type = "password";

            var outerHtml = (IHtmlHelper)html.ViewContext.ViewData["OuterHtml"]!;
            if (outerHtml.ViewData.Model is IViewModelForEntity model && !model.IsNewModel() && PlaceholderBehavior != PlaceholderBehaviorEnum.ForceNoPlaceholder || PlaceholderBehavior == PlaceholderBehaviorEnum.ForceDotDotDotPlaceholder)  htmlAttributes.Add("placeholder", "•••••••");
            else htmlAttributes.Remove("placeholder");

            htmlAttributes.Add("type", Type);
            htmlAttributes.Add("class", "form-control");
                
            if (Pattern != "") htmlAttributes.Add("pattern", Pattern);
            if (Step != "") htmlAttributes.Add("step", Step);
                
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            var text = html.TextBox("", Value ?? "", htmlAttributes.ToMvcDictionary()).GetString();
            text = text.Replace("/>", $"{markerAttribute} />");
            return text.ToHtmlString();
        }
        #endregion

        #region ISupermodelDisplayTemplate implementation
        public override IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return "•••••••".ToHtmlString();
        }
        #endregion

        #region ISupermodelHiddenTemplate implemtation
        public override IHtmlContent HiddenTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            throw new InvalidOperationException("HiddenTemplate() is invalid for PasswordTextBoxMvcModel");
        }
        #endregion

        #region Properties
        public PlaceholderBehaviorEnum PlaceholderBehavior { get; set; } = PlaceholderBehaviorEnum.Default;
        #endregion
    }
}