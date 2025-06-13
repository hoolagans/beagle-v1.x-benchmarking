using System;
using System.Linq;
using System.Text;
using System.Web;
using Supermodel.DataAnnotations.Misc;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class HtmlAttrAttribute : Attribute
{
    #region Constructors
    public HtmlAttrAttribute(string name, string value)
    {
        Attributes = new AttributesDict { { name, value } };
    }
    public HtmlAttrAttribute(string[] names, string[] values)
    {
        Attributes = new AttributesDict();
        if (names.Length != values.Length) throw new ArgumentException("names.Length != values.Length");
        for(var i = 0; i < names.Length; i++) Attributes.Add(names[i], values[i]);
    }
    #endregion

    #region Methods and Method-like properties
    public string Attr => GenerateMyAttributesString();
    #endregion

    #region Private Methods
    //if updating these methods, update the same methods in Tag class
    //private static AttributesDict AnonymousObjectToAttributesDict(object? attributes)
    //{
    //    if (attributes == null) return new AttributesDict();
    //    if (attributes is AttributesDict readyDictionary) return readyDictionary;
    //    if (!IsAnonymousType(attributes.GetType())) throw new ArgumentException("Must be an anonymous type or IDictionary<string, object?>", nameof(attributes));

    //    var dictionary = new AttributesDict();
    //    foreach (var propertyInfo in attributes.GetType().GetProperties())
    //    {
    //        var name = propertyInfo.Name.ToLower();
    //        var value = propertyInfo.GetValue(attributes)?.ToString();
    //        dictionary.Add(name, value);
    //    }
    //    return dictionary;
    //}       
    private string GenerateMyAttributesString()
    {
        if (!Attributes.Keys.Any()) return "";
        var sb = new StringBuilder(" ");
        foreach (var pair in Attributes)
        {
            if (pair.Value != null) sb.Append($"{HttpUtility.HtmlEncode(pair.Key.Replace("_", "-"))}=\"{HttpUtility.HtmlEncode(pair.Value)}\" ");
        }
        return $" {sb.ToString().Trim()}";
    }
    //private static bool IsAnonymousType(Type type)
    //{
    //    if (type == null) throw new ArgumentNullException(nameof(type));

    //    // HACK: The only way to detect anonymous types right now.
    //    return IsDefined(type, typeof(CompilerGeneratedAttribute), false)
    //           && type.IsGenericType && type.Name.Contains("AnonymousType")
    //           && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
    //           && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    //}
    #endregion
        
    #region Properties
    public AttributesDict Attributes { get; }
    #endregion
}