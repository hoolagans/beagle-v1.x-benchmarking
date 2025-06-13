using System;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
    #region Constructors
    public TooltipAttribute(string tooltip)
    {
        Tooltip = tooltip;
    }
    #endregion

    #region Properties
    public string Tooltip { get; }
    #endregion
}