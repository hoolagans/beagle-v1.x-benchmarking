using System;

namespace CsvMaker.Attributes;

//This one ignores property when creating CSV
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CsvMakerPropertyIgnoreAttribute : Attribute { }