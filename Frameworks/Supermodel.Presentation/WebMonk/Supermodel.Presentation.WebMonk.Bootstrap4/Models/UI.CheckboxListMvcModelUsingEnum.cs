using System;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class CheckboxListMvcModelUsingEnum<TEnum> : MultiSelectMvcModelUsingEnum<TEnum> where TEnum : struct, IConvertible
    {
        #region IEditorTemplate implementation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return CheckboxListEditorTemplate(Orientation, screenOrderFrom, screenOrderTo, attributes);
        }
        #endregion

        #region Properties
        public Orientation Orientation { get; set; } = Orientation.Vertical;
        #endregion
    }
}