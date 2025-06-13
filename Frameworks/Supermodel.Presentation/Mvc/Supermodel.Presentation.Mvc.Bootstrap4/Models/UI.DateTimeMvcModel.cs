using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;
using Supermodel.Presentation.Mvc.Extensions;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

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

        #region IDisplayTemplate implementation
        public override IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return (DateTimeValue == null ? "" : DateTimeValue.Value.ToString(CultureInfo.CurrentCulture)).ToHtmlEncodedHtmlString();
        }
        #endregion

        #region IRMapperCustom implemtation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other);
                
            //Set correct format
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-ddTHH:mm");
        }
        #endregion
    }
}