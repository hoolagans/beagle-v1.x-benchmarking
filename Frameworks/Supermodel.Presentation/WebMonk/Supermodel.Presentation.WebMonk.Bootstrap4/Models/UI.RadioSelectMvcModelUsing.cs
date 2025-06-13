using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class RadioSelectMvcModelUsing<TMvcModel> : SingleSelectMvcModelUsing<TMvcModel> where TMvcModel : MvcModelForEntityCore
    {
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