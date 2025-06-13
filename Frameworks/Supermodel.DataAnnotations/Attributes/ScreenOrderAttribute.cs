using System;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ScreenOrderAttribute : Attribute 
{
    #region Constructors
    public ScreenOrderAttribute(int order)
    {
        Order = order;
    }
    #endregion

    #region Properties
    public int Order { get; }
    #endregion
}