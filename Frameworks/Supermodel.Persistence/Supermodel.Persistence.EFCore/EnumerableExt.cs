using System.Collections.Generic;
using System.Linq;
using Supermodel.Persistence.Entities;

namespace Supermodel.Persistence.EFCore;

public static class EnumerableExt
{
    public static TEntity[] ToArraySetIds<TEntity>(this IEnumerable<TEntity> me) where TEntity : class, IEntity, new()
    { 
        var array = me.ToArray();
        for(var i = 0; i < array.Length; i++) array[i].Id = i+1;
        return array;
    }
}