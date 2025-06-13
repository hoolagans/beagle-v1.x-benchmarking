using System;
using System.Threading.Tasks;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class TimeMvcModel : DateTimeMvcModelCore
    {
        #region Constructors
        public TimeMvcModel()
        {
            Type = "time";
        }
        #endregion

        #region IRMapperCustom implemtation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other).ConfigureAwait(false);
                
            //Remove time part
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("HH:mm");
                
        }
        // ReSharper disable once RedundantAssignment
        public override async Task<T> MapToCustomAsync<T>(T other)
        {
            var result = await base.MapToCustomAsync(other).ConfigureAwait(false);
                
            //Remove time part
            if (result != null) result = (T)(object)default(DateTime).Add(((DateTime)(object)result).TimeOfDay);
            return result;
        }
        #endregion

        #region IDisplayTemplate implementation
        public override IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null)
        {
            return new Txt(DateTimeValue == null ? "" : DateTimeValue.Value.ToString("h:mm tt"));
        }
        #endregion
    }
}