using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.Persistence.Entities;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Templates;

namespace Supermodel.Presentation.WebMonk.Extensions;

public static class TypeExtensions
{
    #region Methods
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
        
        //By default, we do not scaffold enumerations (except for strings of course and unless they implement ISupermodelEditorTemplate)
        return result.Where(x => !(x.PropertyType != typeof(string) && 
                                   !typeof(IEditorTemplate).IsAssignableFrom(x.PropertyType) &&
                                   !typeof(IGenerateHtml).IsAssignableFrom(x.PropertyType) &&
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