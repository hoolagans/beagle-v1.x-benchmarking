using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Supermodel.DataAnnotations.Expressions;

namespace Supermodel.DataAnnotations.Attributes;

public static class DisplayNameHelper
{
    #region Methods
    public static string InsertSpacesBetweenWords(this string str)
    {
        var result = Regex.Replace(str, @"(\B[A-Z][^A-Z]+)|\B(?<=[^A-Z]+)([A-Z]+)(?![^A-Z])", " $1$2");
        return result
            .Replace(" Or ", " or ")
            .Replace(" And ", " and ")
            .Replace(" Of ", " of ")
            .Replace(" On ", " on ")
            .Replace(" The ", " the ")
            .Replace(" For ", " for ")
            .Replace(" Per ", " per ")
            .Replace(" At ", " at ")
            .Replace(" A ", " a ")
            .Replace(" In ", " in ")
            .Replace(" By ", " by ")
            .Replace(" About ", " about ")
            .Replace(" To ", " to ")
            .Replace(" From ", " from ")
            .Replace(" With ", " with ")
            .Replace(" Over ", " over ")
            .Replace(" Into ", " into ")
            .Replace(" Without ", " without ");
    }

    public static string GetDisplayNameForProperty(this Type type, string propertyName)
    {
        var propertyInfo = type.GetPropertyByFullName(propertyName);
            
        // ReSharper disable once AssignNullToNotNullAttribute
        var attr = propertyInfo.GetCustomAttribute(typeof (DisplayNameAttribute), true);
        return attr != null ? ((DisplayNameAttribute)attr).DisplayName : propertyInfo.Name.InsertSpacesBetweenWords();
    }
    #endregion
}