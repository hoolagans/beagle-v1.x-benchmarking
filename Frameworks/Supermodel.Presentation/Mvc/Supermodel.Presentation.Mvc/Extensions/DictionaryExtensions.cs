using System.Collections.Generic;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class DictionaryExtensions
{
    //public static IDictionary<string, object> AddOrAppendCssClass(this IDictionary<string, object> me, string cssClass)
    //{
    //    if (me.ContainsKey("class"))
    //    {
    //        var classCss = me["class"].ToString()!;
    //        if (!classCss.Contains(cssClass)) 
    //        {
    //            me["class"] = classCss
    //                .Replace("class=\"", $"class=\"{cssClass} ")
    //                .Replace("class='", $"class='{cssClass} ");
    //        }
    //    }
    //    else
    //    {
    //        me.Add("class", cssClass);
    //    }
    //    return me;
    //}
    public static IDictionary<string, string?> AddOrAppendCssClass(this IDictionary<string, string?> me, string newCssClass)
    {
        if (me.ContainsKey("class"))
        {
            var existingCssClass = me["class"];
            if (!existingCssClass!.Contains(newCssClass)) me["class"] = $"{existingCssClass} {newCssClass}";
        }
        else
        {
            me.Add("class", newCssClass);
        }
        return me;
    }
}