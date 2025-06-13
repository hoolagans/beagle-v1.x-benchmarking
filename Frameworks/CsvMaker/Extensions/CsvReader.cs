using System;
using System.Collections.Generic;
using System.Reflection;
using CsvMaker.Attributes;
using CsvMaker.CsvString;
using CsvMaker.Interfaces;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.ReflectionMapper;

namespace CsvMaker.Extensions;

public static class CsvReader
{
    #region Methods
    public static T ValidateCsvHeaderRow<T>(this T me, CsvStringReader sr)
    {
        if (me is ICsvReaderCustom custom) return custom.ValidateCsvHeaderCustom<T>(sr);
        else return me.ValidateCsvHeaderRowBase(sr);
    }
    public static T ReadCsvRow<T>(this T me, CsvStringReader sr)
    {
        if (me is ICsvReaderCustom custom) return custom.ReadCsvRowCustom<T>(sr);
        else return me.ReadCsvRowBase(sr);
    }

    public static T ValidateCsvHeaderRowBase<T>(this T me, CsvStringReader sr)
    {
        if (me == null) throw new ArgumentNullException(nameof(me));
            
        foreach (var property in me.GetType().GetPropertiesInOrderForReader())
        {
            if (typeof(ICsvReaderCustom).IsAssignableFrom(property.PropertyType))
            {
                //We use existing object if it exists, otherwise we just create a blank object for our purposes
                var propertyObj = me.PropertyGet(property.Name);
                if (propertyObj == null)
                {
                    if (property.PropertyType.GetConstructor(Type.EmptyTypes) == null) throw new Exception($"Property '{property.Name}' is null and no default constructor exists for the type '{property.PropertyType.Name}'");
                    propertyObj = ReflectionHelper.CreateType(property.PropertyType);
                }
                    
                ((ICsvReaderCustom)propertyObj).ValidateCsvHeaderCustom<T>(sr);
            }
            else
            {
                var expectedHeader = property.GetCustomAttribute<CsvMakerColumnNameAttribute>() != null ?
                    property.GetCustomAttribute<CsvMakerColumnNameAttribute>().Name :
                    property.Name.InsertSpacesBetweenWords();
                var csvHeader = sr.ReadNextColumn();

                if (!csvHeader.Equals(expectedHeader, StringComparison.InvariantCultureIgnoreCase)) throw new UnexpectedHeaderException($"'{csvHeader}' while '{expectedHeader}' is expected");
            }
        }
        sr.ReadEOLorEOF();
        return me;
    }
    public static T ReadCsvRowBase<T>(this T me, CsvStringReader sr)
    {
        if (me == null) throw new ArgumentNullException(nameof(me));
            
        foreach (var property in me.GetType().GetPropertiesInOrderForReader())
        {
            //We use existing object if it exists, otherwise we just create a blank object for our purposes
            var propertyObj = me.PropertyGet(property.Name);

            if (typeof(ICsvReaderCustom).IsAssignableFrom(property.PropertyType))
            {
                if (propertyObj == null)
                {
                    if (property.PropertyType.GetConstructor(Type.EmptyTypes) == null) throw new Exception($"Property '{property.Name}' is null and no default constructor exists for the type '{property.PropertyType.Name}'");
                    propertyObj = ReflectionHelper.CreateType(property.PropertyType);
                }

                ((ICsvReaderCustom)propertyObj).ValidateCsvHeaderCustom<T>(sr);
            }
            else
            {
                string csvColumnStr;
                try
                {
                    csvColumnStr = sr.ReadNextColumn();
                    if (property.GetCustomAttribute<CsvMakerColumnDataIgnoreAttribute>() != null) csvColumnStr = "";
                }
                catch (EOFException)
                {
                    return me;
                }
                    
                try
                {
                    if (string.IsNullOrWhiteSpace(csvColumnStr)) me.PropertySet(property.Name, property.PropertyType.DefaultValue());
                    else if (property.PropertyType == typeof(string)) me.PropertySet(property.Name, csvColumnStr);

                    else if (property.PropertyType == typeof(byte) || property.PropertyType == typeof(byte?)) me.PropertySet(property.Name, byte.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));
                    else if (property.PropertyType == typeof(sbyte) || property.PropertyType == typeof(sbyte?)) me.PropertySet(property.Name, sbyte.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));

                    else if (property.PropertyType == typeof(ushort) || property.PropertyType == typeof(ushort?)) me.PropertySet(property.Name, ushort.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));
                    else if (property.PropertyType == typeof(short) || property.PropertyType == typeof(short?)) me.PropertySet(property.Name, short.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));

                    else if (property.PropertyType == typeof(uint) || property.PropertyType == typeof(uint?)) me.PropertySet(property.Name, uint.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));
                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?)) me.PropertySet(property.Name, int.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));

                    else if (property.PropertyType == typeof(ulong) || property.PropertyType == typeof(ulong?)) me.PropertySet(property.Name, ulong.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));
                    else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?)) me.PropertySet(property.Name, long.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));

                    else if (property.PropertyType == typeof(char) || property.PropertyType == typeof(char?)) me.PropertySet(property.Name, char.Parse(csvColumnStr));

                    else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?)) me.PropertySet(property.Name, float.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));
                        
                    else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?)) me.PropertySet(property.Name, double.Parse(csvColumnStr.Replace(",", "").Replace("$", "")));
                        
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)) me.PropertySet(property.Name, DateTime.Parse(csvColumnStr));
                        
                    else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?)) me.PropertySet(property.Name, ParseBool(csvColumnStr));

                    else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?)) me.PropertySet(property.Name, Guid.Parse(csvColumnStr));

                    else if (property.PropertyType.IsEnumOrNullableEnum()) me.PropertySet(property.Name, ParseEnum(property.PropertyType, csvColumnStr));
                        
                    else throw new Exception($"'{property.PropertyType.Name}' type is not supported");
                }
                catch (FormatException)
                {
                    var expectedHeader = property.GetCustomAttribute<CsvMakerColumnNameAttribute>() != null ?
                        property.GetCustomAttribute<CsvMakerColumnNameAttribute>().Name :
                        property.Name.InsertSpacesBetweenWords();
                    throw new FormatException($"Unable to parse value '{csvColumnStr}' for column '{expectedHeader}'");
                }
            }
        }
        sr.ReadEOLorEOF();
        return me;
    }

    public static List<T> ReadCsv<T>(this List<T> me, CsvStringReader sr, bool validateHeader = true) where T: class, new()
    {
        if (validateHeader) new T().ValidateCsvHeaderRow(sr); //Check file format matches our expectations
            
        var resultList = new List<T>();

        while(!sr.IsEOF())
        {
            var item = new T();
            item.ReadCsvRow(sr);
            resultList.Add(item);
        }

        me.Clear();
        me.AddRange(resultList);
        return me;
    }
    #endregion

    #region Private Helpers
    private static bool ParseBool(string str)
    {
        var trimmedLowerStr = str.ToLower().Trim();
        if (trimmedLowerStr == "1") return true;
        if (trimmedLowerStr == "x") return true;
        if (trimmedLowerStr == "y") return true;
        if (trimmedLowerStr == "yes") return true;
        if (trimmedLowerStr == "t") return true;
        if (trimmedLowerStr == "true") return true;
        return false;
    }
    private static Enum? ParseEnum(Type enumType, string str)
    {
        try
        {
            if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>) && enumType.GenericTypeArguments[0].IsEnum)
            {
                if (string.IsNullOrEmpty(str)) return null;
                enumType = enumType.GenericTypeArguments[0];
                return (Enum)Enum.Parse(enumType, str);
            }
            else if (enumType.IsEnum) 
            {
                return (Enum)Enum.Parse(enumType, str);
            }
            else 
            {
                throw new ArgumentException(nameof(enumType));
            }
        }
        catch (Exception)
        {
            var trimmedLowerStr = str.ToLower().Trim();
            var enumValues = Enum.GetValues(enumType);
            foreach (var enumValue in enumValues)
            {
                if (enumValue.GetDescription().ToLower() == trimmedLowerStr) return (Enum)enumValue;
            }
            throw;
        }
    }
    #endregion
}