using System;
using System.Threading.Tasks;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class DateMvcModel : DateTimeMvcModelCore
    {
        #region Constructors
        public DateMvcModel()
        {
            Type = "date";
        }
        #endregion

        #region IRMapperCustom implemtation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other).ConfigureAwait(false);
                
            //Remove time part
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-dd");
                
        }
        // ReSharper disable once RedundantAssignment
        public override async Task<T> MapToCustomAsync<T>(T other)
        {
            var result = await base.MapToCustomAsync(other).ConfigureAwait(false);
                
            //Remove time part
            if (result != null) result = (T)(object)((DateTime)(object)result).Date;
            return result;
        }
        #endregion

        #region Overrides to set the correct format
        public override IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-dd");
            return base.EditorTemplate(screenOrderFrom, screenOrderTo, attributes);
        }
        public override IGenerateHtml HiddenTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-dd");
            return base.HiddenTemplate(screenOrderFrom, screenOrderTo, attributes);
        }
        #endregion

        #region IDisplayTemplate implemantation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return new Txt(DateTimeValue == null ? "" : DateTimeValue.Value.ToShortDateString());
        }
        #endregion
    }
}