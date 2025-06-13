using System;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class NoRequiredLabelAttribute : Attribute { }