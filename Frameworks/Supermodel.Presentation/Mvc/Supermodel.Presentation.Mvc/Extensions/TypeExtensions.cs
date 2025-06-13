using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.Entities;
using Supermodel.Presentation.Mvc.Models.Mvc.Rendering;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class TypeExtensions
{
    #region Properties
    public static string GetControllerName(this Type me)
    {
        if (!typeof(ControllerBase).IsAssignableFrom(me)) throw new SupermodelException("Type passed is not a valid MVC controller");

        var controllerName = me.Name;
        if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)) controllerName = controllerName.RemoveControllerSuffix();
        return controllerName;
    }
    public static bool IsEntityType(this Type me)
    {
        return typeof (IEntity).IsAssignableFrom(me);
    }
    public static IEnumerable<PropertyInfo> GetDetailPropertyInfosInOrder(this Type me, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        var result = me.GetProperties()
            .Where(x => x.GetCustomAttribute<ScaffoldColumnAttribute>() == null || x.GetCustomAttribute<ScaffoldColumnAttribute>()!.Scaffold)
            .Where(x => (x.GetCustomAttribute<ScreenOrderAttribute>() != null ? x.GetCustomAttribute<ScreenOrderAttribute>()!.Order : 100) >= screenOrderFrom)
            .Where(x => (x.GetCustomAttribute<ScreenOrderAttribute>() != null ? x.GetCustomAttribute<ScreenOrderAttribute>()!.Order : 100) <= screenOrderTo)
            .OrderBy(x => x.GetCustomAttribute<ScreenOrderAttribute>() != null ? x.GetCustomAttribute<ScreenOrderAttribute>()!.Order : 100);
        
        //By default we do not scaffold enumerations (except for strings of course and unless they implement ISupermodelEditorTemplate)
        return result.Where(x => !(x.PropertyType != typeof(string) && 
                                   !typeof(ISupermodelEditorTemplate).IsAssignableFrom(x.PropertyType) && 
                                   typeof(IEnumerable).IsAssignableFrom(x.PropertyType)));
    }
    public static IEnumerable<PropertyInfo> GetListPropertyInfosInOrder(this Type me)
    {
        return me.GetProperties()
            .Where(x => x.GetCustomAttribute<ListColumnAttribute>() != null)
            .OrderBy(x => x.GetCustomAttribute<ListColumnAttribute>()!.HeaderOrder);
    }
    #endregion
}