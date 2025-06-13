using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;
using Supermodel.Presentation.Mvc.Extensions;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

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

        #region Overrides
        public override IHtmlContent DisplayTemplate<TModel>(IHtmlHelper<TModel> html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string? markerAttribute = null)
        {
            return (DateTimeValue == null ? "" : DateTimeValue.Value.ToShortDateString()).ToHtmlEncodedHtmlString();
        }
        #endregion

        #region IRMapperCustom implemtation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other);
                
            //Remove time part
            if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("yyyy-MM-dd");
                
        }
        // ReSharper disable once RedundantAssignment
        public override async Task<T> MapToCustomAsync<T>(T other)
        {
            var result = await base.MapToCustomAsync(other);
                
            //Remove time part
            if (result != null) result = (T)(object)((DateTime)(object)result).Date;
            return result;
        }
        #endregion
    }
}