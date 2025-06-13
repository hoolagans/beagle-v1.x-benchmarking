using System.Threading.Tasks;

namespace WebMonk.Filters.Base;

public interface IActionFilter
{
    Task<ActionFilterResult> BeforeActionAsync(ActionFilterContext filterContext);
    Task<ActionFilterResult> AfterActionAsync(ActionFilterContext filterContext);

    int Priority { get; }
}