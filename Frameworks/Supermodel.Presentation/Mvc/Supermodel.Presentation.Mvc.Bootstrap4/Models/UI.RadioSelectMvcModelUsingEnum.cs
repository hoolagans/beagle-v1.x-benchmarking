using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

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
        public override IHtmlContent EditorTemplate<T>(IHtmlHelper<T> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return CommonRadioEditorTemplate(html, DivHtmlAttributesAsDict, InputHtmlAttributesAsDict, LabelHtmlAttributesAsDict);
        }
        #endregion

        #region Properties
        public object? DivHtmlAttributesAsObj { set => DivHtmlAttributesAsDict = value == null ? null : AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? DivHtmlAttributesAsDict { get; set; }

        public object? InputHtmlAttributesAsObj { set => InputHtmlAttributesAsDict = value == null ? null : AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? InputHtmlAttributesAsDict { get; set; }

        public object? LabelHtmlAttributesAsObj { set => LabelHtmlAttributesAsDict = value == null ? null : AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? LabelHtmlAttributesAsDict { get; set; }
        #endregion    
    }
}