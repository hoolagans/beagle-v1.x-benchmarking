using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;
using System;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class CheckboxListMvcModelUsingEnum<TEnum> : MultiSelectMvcModelUsingEnum<TEnum> where TEnum : struct, IConvertible
    {
        #region IEditorTemplate implementation
        public override IHtmlContent EditorTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return CheckboxListEditorTemplate(Orientation, html, screenOrderFrom, screenOrderTo, markerAttribute);
        }
        #endregion

        #region Properties
        public Orientation Orientation { get; set; } = Orientation.Vertical;
        #endregion
    }
}