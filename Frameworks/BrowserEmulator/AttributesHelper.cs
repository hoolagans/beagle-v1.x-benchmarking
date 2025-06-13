using System.Collections.Generic;
using System.ComponentModel;

namespace BrowserEmulator;

public static class AttributesHelper
{
    public static Dictionary<string, string> AnonymousObjectToHtmlAttributes(object htmlAttributes)
    {
        var result = new Dictionary<string, string>();

        if (htmlAttributes != null)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes))
            {
                var objValue = property.GetValue(htmlAttributes);
                var strValue = objValue == null ? "" : objValue.ToString();
                result.Add(property.Name.Replace('_', '-'), strValue);
            }
        }
        return result;
    }
}