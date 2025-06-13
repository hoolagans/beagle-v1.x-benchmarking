using System.Reflection;

namespace Supermodel.ReflectionMapper;

public class RMPropertyMetadata
{
    #region Consructors
    public RMPropertyMetadata(object obj, PropertyInfo propertyInfo)
    {
        Obj = obj;
        PropertyInfo = propertyInfo;
    }
    #endregion

    #region Methods
    public object CreateDefaultInstance()
    {
        return ReflectionHelper.CreateType(PropertyInfo.PropertyType);
    }
        
    public object? Get()
    {
        return Obj.PropertyGet(PropertyInfo.Name);
    }
    public object GetOrDefault()
    {
        return Get() ?? CreateDefaultInstance();
    }
        
    public void Set(object? val, bool ignoreNoSetMethod, object[]? index = null)
    {
        Obj.PropertySet(PropertyInfo.Name, val, ignoreNoSetMethod, index);
    }

    public bool IsMarkedNotRMappedForMappingFrom()
    {
        return PropertyInfo.GetCustomAttribute(typeof(NotRMappedAttribute), true) != null || PropertyInfo.GetCustomAttribute(typeof(NotRMappedFromAttribute), true) != null;
    }
    public bool IsMarkedNotRMappedForMappingTo()
    {
        return PropertyInfo.GetCustomAttribute(typeof(NotRMappedAttribute), true) != null || PropertyInfo.GetCustomAttribute(typeof(NotRMappedToAttribute), true) != null;
    }
        
    public bool IsMarkedForShallowCopyFrom()
    {
        return PropertyInfo.GetCustomAttribute(typeof(RMCopyShallowAttribute), true) != null || PropertyInfo.GetCustomAttribute(typeof(RMCopyShallowFromAttribute), true) != null;
    }
    public bool IsMarkedForShallowCopyTo()
    {
        return PropertyInfo.GetCustomAttribute(typeof(RMCopyShallowAttribute), true) != null || PropertyInfo.GetCustomAttribute(typeof(RMCopyShallowToAttribute), true) != null;
    }
    #endregion

    #region Properties
    public PropertyInfo PropertyInfo { get; }
    public object Obj { get; }
    #endregion
}