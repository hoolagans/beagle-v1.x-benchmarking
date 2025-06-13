using System;

namespace Supermodel.Mobile.CodeGen;

public class MobileModelAttributeAttribute : Attribute
{
    public MobileModelAttributeAttribute(string attrContent)
    {
        AttrContent = attrContent;
    }
        
    public string AttrContent { get; }
}