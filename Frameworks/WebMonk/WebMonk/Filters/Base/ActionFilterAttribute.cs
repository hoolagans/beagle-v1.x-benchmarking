using System;
using System.Threading.Tasks;

namespace WebMonk.Filters.Base;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class ActionFilterAttribute : Attribute, IActionFilter
{
    public virtual Task<ActionFilterResult> BeforeActionAsync(ActionFilterContext filterContext)
    {
        return Task.FromResult(ActionFilterResult.Proceed); //Do nothing
    }

    public virtual Task<ActionFilterResult> AfterActionAsync(ActionFilterContext filterContext)
    {
        return Task.FromResult(ActionFilterResult.Proceed); //Do nothing
    }

    public virtual int Priority => 100;
}