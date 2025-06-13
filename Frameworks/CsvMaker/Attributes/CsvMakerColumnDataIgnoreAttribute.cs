using System;

namespace CsvMaker.Attributes;

//This one ignores data in column in CSV file when we are reading it, setting the column to default value
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CsvMakerColumnDataIgnoreAttribute : Attribute { }