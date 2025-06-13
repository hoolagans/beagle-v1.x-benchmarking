using System.Text;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.Models.Base;

public abstract class BrightChartsD3MvcModelBase : D3MvcModelBase
{
    #region Custom Types
    public enum GridEnum { Vertical, Horizontal, Full }
    #endregion

    #region Methods
    protected string SetColorWidthHeightIsAnimated()
    {
        var sb = new StringBuilder();
        sb.Append(".colorSchema(britecharts.colors.colorSchemas.britecharts)");
        if (Width != null) sb.Append($".width({Width})");
        if (Height != null) sb.Append($".height({Height})");
        sb.Append($".isAnimated({IsAnimated.ToString().ToLower()})");
        return sb.ToString();
    }
    #endregion

    #region Properties
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool IsAnimated { get; set; } = true;
    #endregion
}