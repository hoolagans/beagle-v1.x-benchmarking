using Supermodel.DataAnnotations.Misc;

namespace Supermodel.Presentation.WebMonk.Extensions;

public static class DictionaryExtensions
{
    public static AttributesDict AddOrAppendCssClass(this AttributesDict me, string newCssClass)
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