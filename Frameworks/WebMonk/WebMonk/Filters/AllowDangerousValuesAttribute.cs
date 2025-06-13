using System.Threading.Tasks;
using WebMonk.Context;
using WebMonk.Filters.Base;

namespace WebMonk.Filters;

public class AllowDangerousValuesAttribute : ActionFilterAttribute
{
    #region Overrides
    public override Task<ActionFilterResult> BeforeActionAsync(ActionFilterContext filterContext)
    {
        CurrentBlockDangerousValueProviderValues = HttpContext.Current.BlockDangerousValueProviderValues;
        HttpContext.Current.BlockDangerousValueProviderValues = false;
        return base.BeforeActionAsync(filterContext);
    }

    public override Task<ActionFilterResult> AfterActionAsync(ActionFilterContext filterContext)
    {
        HttpContext.Current.BlockDangerousValueProviderValues = CurrentBlockDangerousValueProviderValues;
        return base.AfterActionAsync(filterContext);
    }
    #endregion

    #region Properties
    protected bool CurrentBlockDangerousValueProviderValues { get; set; } = true; //this is set to true just in case
    #endregion
}