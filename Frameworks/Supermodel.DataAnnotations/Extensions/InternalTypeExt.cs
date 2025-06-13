using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Supermodel.DataAnnotations.Extensions;

//this is made internal not to interfere with ReflectionMapper version
internal static class InternalTypeExt
{
    #region Methods
    public static bool IsAnonymousType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        // HACK: The only way to detect anonymous types right now.
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
               && type.IsGenericType && type.Name.Contains("AnonymousType")
               && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
               && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }
    #endregion
}