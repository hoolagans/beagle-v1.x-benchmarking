using System;
using System.ComponentModel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.Persistence.Entities;

[Obsolete("Use EF.Core mechanism to set up many-to-many relationships instead")]
public abstract class M2M<TEntity1, TEntity2> : Entity, IM2M
    where TEntity1: IEntity 
    where TEntity2: IEntity
{
    #region Constructors
    protected M2M()
    {
        if (typeof(TEntity1) == typeof(TEntity2)) throw new SupermodelException("M2M is not supported when TEntity1 and TEntity2 are the same");
    }
    #endregion
        
    #region Methods
    public virtual IEntity GetConnectionToOther(Type otherType)
    {
        if (typeof(TEntity1).IsAssignableFrom(otherType)) return Connection1;
        else if (typeof(TEntity2).IsAssignableFrom(otherType)) return Connection2;
        else throw new ArgumentException(nameof(otherType));
    }
    public virtual void SetConnectionToOther(IEntity other)
    {
        if (other is TEntity1 entity1) Connection1 = entity1;
        else if (other is TEntity2 entity2) Connection2 = entity2;
        else throw new ArgumentException(nameof(other));
    }
    #endregion

    #region Properties
    [Required] public virtual TEntity1 Connection1 { get; set; } = default!;
    [Required] public virtual TEntity2 Connection2 { get; set; } = default!;
    #endregion
}