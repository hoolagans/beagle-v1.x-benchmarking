using System;
using Supermodel.Persistence.Entities;

namespace Supermodel.Presentation.Cmd.Extensions;

public static class TypeExtensions
{
    #region Methods
    public static bool IsEntityType(this Type me)
    {
        return typeof (IEntity).IsAssignableFrom(me);
    }
    #endregion
}