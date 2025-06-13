using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class DropdownMvcModelUsing<TMvcModel> : SingleSelectMvcModelUsing<TMvcModel> where TMvcModel : MvcModelForEntityCore
    {
        #region IEditorTemplate implementation
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return CommonDropdownEditorTemplate(this, HtmlAttributesAsDict);
        }
        #endregion

        #region Properties
        public object HtmlAttributesAsObj { set => HtmlAttributesAsDict = AttributesDict.FromAnonymousObject(value); }
        public AttributesDict HtmlAttributesAsDict { get; set; } = new();
        #endregion
    }
}