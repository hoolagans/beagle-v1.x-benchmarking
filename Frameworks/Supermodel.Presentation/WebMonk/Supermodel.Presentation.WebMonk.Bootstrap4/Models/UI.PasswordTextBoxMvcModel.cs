using Supermodel.Presentation.WebMonk.Models;
using System;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Misc;
using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

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
            
        #region IEditorTemplate implemtation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            var attributesDict = new AttributesDict(HtmlAttributesAsDict);
                
            Type = "password";

            if (HttpContext.Current.PrefixManager.RootParent is IViewModelForEntity model && !model.IsNewModel() && PlaceholderBehavior != PlaceholderBehaviorEnum.ForceNoPlaceholder || PlaceholderBehavior == PlaceholderBehaviorEnum.ForceDotDotDotPlaceholder)  attributesDict.Add("placeholder", DotDotDot);
            else attributesDict.Remove("placeholder");

            attributesDict.Add("type", Type);
            attributesDict.Add("class", "form-control");
                
            if (Pattern != "") attributesDict.Add("pattern", Pattern);
            if (Step != "") attributesDict.Add("step", Step);
                
            var result = Render.TextBoxForModel(Value, attributesDict);
            result.AddOrUpdateAttr(attributes);
            return result;
        }
        #endregion

        #region ISupermodelDisplayTemplate implementation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return new Txt(DotDotDot);
        }
        #endregion

        #region IHiddenTemplate implemtation
        public override IGenerateHtml HiddenTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            throw new InvalidOperationException("HiddenTemplate() is invalid for PasswordTextBoxMvcModel");
        }
        #endregion

        #region Properties
        public PlaceholderBehaviorEnum PlaceholderBehavior { get; set; } = PlaceholderBehaviorEnum.Default;
        protected const string DotDotDot = "•••••••";
        #endregion
    }
}