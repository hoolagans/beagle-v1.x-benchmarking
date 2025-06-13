using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.DataAnnotations.Expressions;

public static class ExpressionExt
{
    public static PropertyInfo GetPropertyByFullName(this Type me, string expression)
    {
        var propertyNameParts = expression.Split('.');
        var type = me;
        PropertyInfo? propertyInfo = null;
        foreach (var propertyNamePart in propertyNameParts)
        {
            //Find square brackets
            var regex = new Regex(@"\[(.*?)\]");
            var matches = regex.Matches(propertyNamePart);
            if (matches.Count == 0)
            {
                propertyInfo = type!.GetProperty(propertyNamePart);
                if (propertyInfo == null) throw new SupermodelException($"propertyInfo == null for {propertyNamePart}");

                type = propertyInfo.PropertyType;
            }
            else if (matches.Count == 1)
            {
                var match = matches[0];
                var indexerPropertyName = propertyNamePart.Replace(match.Value, "").Trim();
                    
                propertyInfo = type!.GetProperty(indexerPropertyName);
                if (propertyInfo == null) throw new SupermodelException($"propertyInfo == null for {indexerPropertyName}");
                    
                if (propertyInfo.PropertyType.IsArray) 
                {
                    type = propertyInfo.PropertyType.GetElementType();
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) 
                {
                    type = propertyInfo.PropertyType.GenericTypeArguments[0];
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) 
                {
                    type = propertyInfo.PropertyType.GenericTypeArguments[1];
                }
                else
                {
                    throw new ArgumentException($"{expression} is not a valid expression", nameof(expression));
                }
            }
            else
            {
                throw new ArgumentException($"{expression} is not a valid expression", nameof(expression));
            }
        }
        if (propertyInfo == null) throw new ArgumentException($"'{expression}' is an invalid expression", nameof(expression));
        return propertyInfo;
    }

    public static (PropertyInfo?, Type, object?) GetPropertyInfoPropertyTypeAndValueByFullName(this object obj, string expression)
    {
        if (string.IsNullOrEmpty(expression)) return (null, obj.GetType(), obj);
            
        var propertyNameParts = expression.Split('.');
        var type = obj.GetType();
        PropertyInfo? propertyInfo = null;
        foreach (var propertyNamePart in propertyNameParts)
        {
            //Find square brackets
            var regex = new Regex(@"\[(.*?)\]");
            var matches = regex.Matches(propertyNamePart);
            if (matches.Count == 0)
            {
                propertyInfo = type!.GetProperty(propertyNamePart);
                if (propertyInfo == null) throw new SupermodelException($"propertyInfo == null for {propertyNamePart}");

                type = propertyInfo.PropertyType;
                obj = propertyInfo.GetValue(obj);
            }
            else if (matches.Count == 1)
            {
                var match = matches[0];
                var indexerPropertyName = propertyNamePart.Replace(match.Value, "").Trim();
                var index = match.Groups[1].Value.Trim();
                    
                propertyInfo = type!.GetProperty(indexerPropertyName);
                if (propertyInfo == null) throw new SupermodelException($"propertyInfo == null for {indexerPropertyName}");

                if (propertyInfo.PropertyType.IsArray) 
                {
                    type = propertyInfo.PropertyType.GetElementType();
                    var arr = (Array)propertyInfo.GetValue(obj);
                    obj = arr.GetValue(int.Parse(index));
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) 
                {
                    type = propertyInfo.PropertyType.GenericTypeArguments[0];
                    var list = (IList)propertyInfo.GetValue(obj);
                    obj = list[int.Parse(index)];
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) 
                {
                    type = propertyInfo.PropertyType.GenericTypeArguments[1];
                    var dict = (IDictionary)propertyInfo.GetValue(obj);
                    obj = dict[index];
                }
                else
                {
                    throw new ArgumentException($"{expression} is not a valid expression", nameof(expression));
                }
            }
            else
            {
                throw new ArgumentException($"{expression} is not a valid expression", nameof(expression));
            }

        }
        if (propertyInfo == null) throw new ArgumentException($"'{expression}' is an invalid expression", nameof(expression));
        return (propertyInfo, type, obj);
    }
}