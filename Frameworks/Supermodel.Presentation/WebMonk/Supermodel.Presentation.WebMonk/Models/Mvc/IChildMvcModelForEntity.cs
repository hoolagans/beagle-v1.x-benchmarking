using Supermodel.Persistence.Entities;

namespace Supermodel.Presentation.WebMonk.Models.Mvc;

public interface IChildMvcModelForEntity : IMvcModelForEntity
{
    long? ParentId { get; set; }
}
    
public interface IChildMvcModelForEntity<in TEntity, TParentEntity> : IChildMvcModelForEntity
    where TEntity : class, IEntity, new()
    where TParentEntity : class, IEntity, new()
{
    TParentEntity? GetParentEntity(TEntity entity);
    void SetParentEntity(TEntity entity, TParentEntity? parent);
}