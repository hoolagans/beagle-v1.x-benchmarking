using System;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class HideLabelAttribute : Attribute 
{ 
    public bool KeepLabelSpace { get; set; }
}