using Supermodel.ReflectionMapper;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Presentation.Mvc.Models.Mvc;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public abstract class ChildMvcModelForEntity<TEntity, TParentEntity> : MvcModelForEntity<TEntity>, IChildMvcModelForEntity<TEntity, TParentEntity>
        where TEntity : class, IEntity, new()
        where TParentEntity : class, IEntity, new()
    {
        #region IChildViewModelForEntity implementation
        [ScaffoldColumn(false), NotRMapped] public virtual long? ParentId { get; set; }
            
        public abstract TParentEntity? GetParentEntity(TEntity entity);
        public abstract void SetParentEntity(TEntity entity, TParentEntity? parent);
        #endregion

        #region IRMapperCustom implementation
        public override async Task MapFromCustomAsync<T>(T other)
        {
            await base.MapFromCustomAsync(other);
                
            if (other == null) throw new SupermodelException("GetParentEntity() cannot be called with other parameter = null");
            var parentEntity = GetParentEntity((TEntity)(object)other);

            if (parentEntity == null) ParentId = null;
            else ParentId = parentEntity.Id;
        }
        public override async Task<T> MapToCustomAsync<T>(T other)
        {
            other = await base.MapToCustomAsync(other);

            var parentEntity = GetParentEntity((TEntity)(object)other!);
            if (parentEntity == null && ParentId != null ||
                parentEntity != null && parentEntity.Id != ParentId)
            {
                // ReSharper disable RedundantSuppressNullableWarningExpression
                if (ParentId == null) SetParentEntity((TEntity)(object)other!, null);
                else SetParentEntity((TEntity)(object)other!, await RepoFactory.Create<TParentEntity>().GetByIdAsync((long)ParentId));
                // ReSharper restore RedundantSuppressNullableWarningExpression
            }

            return other;
        }
        #endregion
    }
}