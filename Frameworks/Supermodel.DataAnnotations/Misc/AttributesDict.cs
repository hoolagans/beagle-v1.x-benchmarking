using System;
using System.Collections.Generic;
using System.Dynamic;
using Supermodel.DataAnnotations.Extensions;

namespace Supermodel.DataAnnotations.Misc;

public class AttributesDict : Dictionary<string, string?>
{
    #region Constructors
    public AttributesDict() : base(StringComparer.OrdinalIgnoreCase) { }
    public AttributesDict(AttributesDict dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase) { }
    #endregion

    #region Methods
    public IDictionary<string, object?> ToMvcDictionary()
    {
        var result = new Dictionary<string, object?>();
        foreach (var key in Keys) result[key] = this[key];
        return result;
    }
        
    public bool KeyExistsAndEqualsTo(string key, string value)
    {
        if (!ContainsKey(key)) return false;
        if (this[key] == null) return false;
        return this[key]!.Equals(value, StringComparison.InvariantCultureIgnoreCase);
    }
    public bool KeyExistsAndStartsWith(string key, string value)
    {
        if (!ContainsKey(key)) return false;
        if (this[key] == null) return false;
        return this[key]!.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
    }
    public bool KeyExistsAndContains(string key, string value)
    {
        if (!ContainsKey(key)) return false;
        if (this[key] == null) return false;
        return this[key]!.Contains(value, StringComparison.InvariantCultureIgnoreCase);
    }

    public static AttributesDict FromAnonymousObject(object? attributes)
    {
        if (attributes == null) return new AttributesDict();
        if (attributes is AttributesDict readyDictionary) return readyDictionary;
        if (!attributes.GetType().IsAnonymousType() && !(attributes is ExpandoObject)) throw new ArgumentException("Must be an anonymous type or AttributesDict", nameof(attributes));

        var dictionary = new AttributesDict();
        foreach (var propertyInfo in attributes.GetType().GetProperties())
        {
            var name = propertyInfo.Name.ToLower();
            var value = propertyInfo.GetValue(attributes)?.ToString();
            dictionary.Add(name, value);
        }
        return dictionary;
    }
    #endregion
}