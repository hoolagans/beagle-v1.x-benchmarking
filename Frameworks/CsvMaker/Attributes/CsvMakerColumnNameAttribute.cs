using System;

namespace CsvMaker.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CsvMakerColumnNameAttribute : Attribute
{
    public CsvMakerColumnNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}