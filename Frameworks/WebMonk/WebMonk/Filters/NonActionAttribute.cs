using System.Threading.Tasks;
using WebMonk.Filters.Base;

namespace WebMonk.Filters;

public class NonActionAttribute : ActionFilterAttribute
{
    #region Overrides
    public override Task<ActionFilterResult> BeforeActionAsync(ActionFilterContext filterContext)
    {
        return Task.FromResult(ActionFilterResult.Skip);
    }
    #endregion
}