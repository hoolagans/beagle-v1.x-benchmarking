using System;

namespace WebMonk.ModeBinding;

[AttributeUsage(AttributeTargets.Property)]
public class DoNotBindAttribute : Attribute { }