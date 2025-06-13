using System.Reflection;
using WebMonk.HttpRequestHandlers.Controllers;

namespace WebMonk.Filters.Base;

public class ActionFilterContext 
{
    #region Constructors
    public ActionFilterContext(ControllerBase controller, MethodInfo actionMethodInfo)
    {
        Controller = controller;
        ActionMethodInfo = actionMethodInfo;
    }
    #endregion

    #region Properties
    public ControllerBase Controller { get; }
    public MethodInfo ActionMethodInfo { get; }
    #endregion
}