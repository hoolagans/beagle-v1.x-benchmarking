using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class DropdownMvcModelUsingEnum<TEnum> : SingleSelectMvcModelUsingEnum<TEnum> where TEnum : struct, IConvertible
    {
        #region Constructors
        public DropdownMvcModelUsingEnum(){}
        public DropdownMvcModelUsingEnum(TEnum selectedEnum) : this()
        {
            SelectedEnum = selectedEnum;
        }
        #endregion

        #region ISupermodelEditorTemplate implementation
        public override IHtmlContent EditorTemplate<T>(IHtmlHelper<T> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return CommonDropdownEditorTemplate(html, HtmlAttributesAsDict);
        }
        #endregion

        #region Properties
        public object? HtmlAttributesAsObj { set => HtmlAttributesAsDict = value == null ? null : AttributesDict.FromAnonymousObject(value); }
        public AttributesDict? HtmlAttributesAsDict { get; set; }
        #endregion
    }
}