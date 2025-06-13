using System.Globalization;
using System.Threading.Tasks;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class DateTimeMvcModel : DateTimeMvcModelCore
    {
        #region Constructors
        public DateTimeMvcModel()
        {
            Type = "datetime-local";
        }
        #endregion

        #region Overrides to set the correct format
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-ddTHH:mm");
            return base.EditorTemplate(screenOrderFrom, screenOrderTo, attributes);
        }
        public override IGenerateHtml HiddenTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-ddTHH:mm");
            return base.HiddenTemplate(screenOrderFrom, screenOrderTo, attributes);
        }
        #endregion

        #region IDisplayModelTemplate implementation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return new Txt(DateTimeValue == null ? "" : DateTimeValue.Value.ToString(CultureInfo.CurrentCulture));
        }
        #endregion

        #region IRMapperCustom implemtation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other).ConfigureAwait(false);
                
            //Set correct format
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-ddTHH:mm");
        }
        #endregion
    }
}