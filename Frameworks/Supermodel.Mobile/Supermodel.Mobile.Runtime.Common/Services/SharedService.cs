using System;
using System.Collections.Concurrent;
using System.Reflection;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.ReflectionMapper;

namespace Supermodel.Mobile.Runtime.Common.Services;

public static class SharedService
{
    #region EmbeddedTypes
    [AttributeUsage(AttributeTargets.Interface)]
    public class SingletonAttribute : Attribute{}

    [AttributeUsage(AttributeTargets.Interface)]
    public class ImplementedByAttribute : Attribute
    {
        #region Constructors
        public ImplementedByAttribute(string iosType, string androidType, string netCoreType)
        {
            IOSType = iosType ?? throw new ArgumentNullException(nameof(iosType));
            DroidType = androidType ?? throw new ArgumentNullException(nameof(androidType));
            NetCoreType = netCoreType;
        }
        #endregion

        #region Properties
        public string IOSType { get; }
        public string DroidType { get; }
        public string NetCoreType { get; }
        #endregion
    }
    #endregion

    #region Methods
    public static TInterface Instantiate<TInterface>(params object[] paramObjects) 
    {
        var interfaceType = typeof(TInterface);
        if (!interfaceType.IsInterface) throw new ArgumentException("Generic type T must be an interface");
        var singleton = interfaceType.GetCustomAttribute<SingletonAttribute>() != null;
            
        var typeName = GetClassToInstantiateFullName<TInterface>();           

        if (!singleton)
        {
            return InstantiateByType<TInterface>(typeName, paramObjects);
        }
        else
        {
            if (!Singletons.ContainsKey(interfaceType)) Singletons[interfaceType] = InstantiateByType<TInterface>(typeName, paramObjects);
            return (TInterface)Singletons[interfaceType];
        }
    }
    private static TInterface InstantiateByType<TInterface>(string typeName, object[] paramObjects)
    {
        if (typeName == null) throw new ArgumentNullException(nameof(typeName));

        try
        {
            var typeType = Type.GetType(typeName) ?? throw new SystemException("Type.GetType(typeName) == null");
            return (TInterface)ReflectionHelper.CreateType(typeType, paramObjects);
        }
        catch (Exception ex)
        {
            throw new SupermodelException($"Unable to create type {typeName} or cast it to {typeof(TInterface).FullName}: {ex.Message}");
        }
    }

    public static string GetClassToInstantiateFullName<TInterface>()
    {
        return GetClassToInstantiateFullName(typeof(TInterface));
    }
    public static string GetClassToInstantiateFullName(Type interfaceType)
    {
        var implementedByAttr = interfaceType.GetCustomAttribute<ImplementedByAttribute>();

        string typeName;
        if (implementedByAttr != null)
        {
            typeName = Pick.ForPlatform(implementedByAttr.IOSType, implementedByAttr.DroidType, implementedByAttr.NetCoreType);
            if (typeName == null) throw new SupermodelException("Unsupported Platform");
        }
        else
        {
            var iTypeNamespace = interfaceType.Namespace ?? throw new SupermodelException("Can't determine namespace");
                
            string typeNamespace;
            if (iTypeNamespace.EndsWith(".Common")) 
            {
                typeNamespace = iTypeNamespace.Substring(0, iTypeNamespace.Length - ".Common".Length) + Pick.ForPlatform(".iOS.", ".Droid.", ".NetCore.");
            }
            else 
            {
                typeNamespace = iTypeNamespace.Replace(".Common.", Pick.ForPlatform(".iOS.", ".Droid.", ".NetCore."));
            }

            var iTypeName = interfaceType.Name;
            if (!iTypeName.StartsWith("I")) throw new ArgumentException("Generic type T must be an interface and start with 'I'");
                
            typeName = $"{typeNamespace}.{iTypeName.Substring(1, iTypeName.Length-1)}, Supermodel.Mobile.Runtime.{Pick.ForPlatform("iOS", "Droid", "NetCore")}";
        }
            
        return typeName;
    }
    #endregion

    #region Properties
    public static ConcurrentDictionary<Type, object> Singletons { get; } = new ConcurrentDictionary<Type, object>();
    #endregion
}