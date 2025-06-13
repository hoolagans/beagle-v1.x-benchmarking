using System;
using WebMonk.Exceptions;

namespace WebMonk.Extensions;

//this is made internal not to interfere with ReflectionMapper version
internal static class InternalTypeExt
{
    #region Methods
    public static bool IsComplexType(this Type me)
    {
        if (me == typeof(string)) return false;
        if (me == typeof(byte[])) return false;
        if (me.IsGenericType && me.GetGenericTypeDefinition() == typeof(Nullable<>)) return false;

        return (me.IsClass || me.IsStruct());
    }
    public static bool IsStruct(this Type me)
    {
        return me.IsValueType && !me.IsPrimitive && !me.IsEnum && me != typeof(decimal);
    }
    #endregion
}

public static class PublicTypeExt
{
    #region Methods
    public static string GetMvcControllerName(this Type me)
    {
        var myTypeName = me.Name;
        if (!myTypeName.EndsWith("MvcController")) throw new WebMonkException("Mvc Controllers must end with MvcController");
        return myTypeName[..^"MvcController".Length];
    }
    public static string GetApiControllerName(this Type me)
    {
        var myTypeName = me.Name;
        if (!myTypeName.EndsWith("ApiController")) throw new WebMonkException("Api Controllers must end with ApiController");
        return myTypeName[..^"ApiController".Length];
    }
    #endregion
}