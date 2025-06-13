using System;

namespace CsvMaker.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CsvMakerColumnOrderAttribute : Attribute
{
    public CsvMakerColumnOrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; set; }
}