using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;
using Supermodel.Presentation.Mvc.Extensions;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

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

        #region ISupermodelDisplayTemplate implementation
        public override IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return (DateTimeValue == null ? "" : DateTimeValue.Value.ToString("h:mm tt")).ToHtmlEncodedHtmlString();
        }
        #endregion

        #region IRMapperCustom implemtation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other);
                
            //Remove time part
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("HH:mm");
                
        }
        // ReSharper disable once RedundantAssignment
        public override async Task<T> MapToCustomAsync<T>(T other)
        {
            var result = await base.MapToCustomAsync(other);
                
            //Remove time part
            if (result != null) result = (T)(object)default(DateTime).Add(((DateTime)(object)result).TimeOfDay);
            return result;
        }
        #endregion
    }
}