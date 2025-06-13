using System;

namespace Supermodel.ReflectionMapper;

public class ReflectionMapperException : Exception
{
    public ReflectionMapperException() { }
    public ReflectionMapperException(string msg) : base(msg) { }
}

public class ReflectionPropertyCantBeInvoked : ReflectionMapperException
{
    public ReflectionPropertyCantBeInvoked(Type type, string propertyName)
        : base($"Property '{propertyName}' does not exist in Type '{type.Name}'") { }
}

public class ReflectionMethodCantBeInvoked : ReflectionMapperException
{
    public ReflectionMethodCantBeInvoked(Type type, string methodName)
        : base($"Method '{methodName}' does not exist in Type '{type.Name}'") { }
}    
    
public class PropertyCantBeAutomappedException : ReflectionMapperException
{
    public PropertyCantBeAutomappedException(string msg) : base(msg) { }
}