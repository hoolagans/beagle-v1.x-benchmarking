using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvMaker.Attributes;
using CsvMaker.Interfaces;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.ReflectionMapper;

namespace CsvMaker.Extensions;

public static class CsvMaker
{
    public static StringBuilder ToCsvRow<T>(this T me, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        if (me is ICsvMakerCustom custom) sb = custom.ToCsvRowCustom(sb);
        else sb = me.ToCsvRowBase(sb);

        return sb;
    }
    public static StringBuilder ToCsvHeader<T>(this T me, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        if (me is ICsvMakerCustom custom) sb = custom.ToCsvHeaderCustom(sb);
        else sb = me.ToCsvHeaderBase(sb);

        return sb;
    }

    public static StringBuilder ToCsvHeaderBase<T>(this T me, StringBuilder? sb = null)
    {
        if (me == null) throw new ArgumentNullException(nameof(me));
            
        sb ??= new StringBuilder();

        var firstColumn = true;
        foreach (var property in me.GetType().GetPropertiesInOrderForMaker())
        {
            if (firstColumn) firstColumn = false;
            else sb.Append(", ");
                
            if (typeof (ICsvMakerCustom).IsAssignableFrom(property.PropertyType))
            {
                //We use existing object if it exists, otherwise we just create a blank object for our purposes
                var propertyObj = me.PropertyGet(property.Name);
                if (propertyObj == null)
                {
                    if (property.PropertyType.GetConstructor(Type.EmptyTypes) == null) throw new Exception($"Property '{property.Name}' is null and no default constructor exists for the type '{property.PropertyType.Name}'");
                    propertyObj = ReflectionHelper.CreateType(property.PropertyType);
                }
                    
                ((ICsvMakerCustom) propertyObj).ToCsvHeaderCustom(sb);
            }
            else
            {
                var header = property.GetCustomAttribute<CsvMakerColumnNameAttribute>() != null ?
                    property.GetCustomAttribute<CsvMakerColumnNameAttribute>().Name :
                    property.Name.InsertSpacesBetweenWords();
                sb.Append(header);
            }
        }
        return sb;
    }
    public static StringBuilder ToCsvRowBase<T>(this T me, StringBuilder? sb = null)
    {
        if (me == null) throw new ArgumentNullException(nameof(me));

        sb ??= new StringBuilder();

        var firstColumn = true;
        foreach (var property in me.GetType().GetPropertiesInOrderForMaker())
        {
            if (firstColumn) firstColumn = false;
            else sb.Append(",");

            var propertyObj = me.PropertyGet(property.Name);
            if (typeof(ICsvMakerCustom).IsAssignableFrom(property.PropertyType))
            {
                if (propertyObj != null)
                {
                    ((ICsvMakerCustom) propertyObj).ToCsvRowCustom(sb);
                }
                else
                {
                    //Need to add commas for the properties that exist in the type. We start with one because we already added one comma if this is not a first one
                    for (var i = 1; i < property.PropertyType.GetPropertiesInOrderForMaker().Count(); i++) sb.Append(",");
                }
            }
            else
            {
                if (propertyObj != null)
                {
                    if (propertyObj is bool boolObj) sb.Append(boolObj ? "Yes" : "No");
                    else sb.Append(propertyObj.ToString().PrepareCvsColumn());
                }
            }
        }
        return sb;
    }

    public static StringBuilder ToCsv<T>(this List<T> me, bool includeHeader = true, StringBuilder? sb = null) where T: class, new()
    {
        if (includeHeader)
        {
            var headerCsvModel = me.FirstOrDefault() ?? new T();
            sb = headerCsvModel.ToCsvHeader(sb);
            sb.AppendLine();
        }

        foreach (var item in me)
        {
            sb = item.ToCsvRow(sb);
            sb.AppendLine();
        }
        return sb!;
    }
}