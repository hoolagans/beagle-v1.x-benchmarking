using System;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class RadioSelectMvcModelUsingEnum<TEnum> : SingleSelectMvcModelUsingEnum<TEnum> where TEnum : struct, IConvertible
    {
        #region Constructors
        public RadioSelectMvcModelUsingEnum(){}
        public RadioSelectMvcModelUsingEnum(TEnum selectedEnum) : this()
        {
            SelectedEnum = selectedEnum;
        }
        #endregion

        #region ISupermodelEditorTemplate implementation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return CommonRadioEditorTemplate(this, DivHtmlAttributesAsDict, InputHtmlAttributesAsDict, LabelHtmlAttributesAsDict);
        }
        #endregion

        #region Properties
        public object DivHtmlAttributesAsObj { set => DivHtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? DivHtmlAttributesAsDict { get; set; }

        public object InputHtmlAttributesAsObj { set => InputHtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? InputHtmlAttributesAsDict { get; set; }

        public object LabelHtmlAttributesAsObj { set => LabelHtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? LabelHtmlAttributesAsDict { get; set; }
        #endregion    
    }
}