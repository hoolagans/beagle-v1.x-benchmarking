using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.WebMonk.Models.Api;

public abstract class ApiModelForEntity<TEntity> : ApiModelForAnyEntity where TEntity : class, IEntity, new()
{
    #region Validation
    //The default implementation just grabs domain model validation but this can be overriden
    public override async Task<ValidationResultList> ValidateAsync(ValidationContext validationContext)
    {
        var tempEntityForValidation = await CreateTempValidationEntityAsync().ConfigureAwait(false);
        var vr = new ValidationResultList();
        await AsyncValidator.TryValidateObjectAsync(tempEntityForValidation, new ValidationContext(tempEntityForValidation), vr).ConfigureAwait(false); 
        return vr;
    }
    #endregion

    #region Private Helper Methods
    protected TEntity CastToEntity(object? obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        return (TEntity)obj;
    }
    protected virtual async Task<TEntity> CreateTempValidationEntityAsync()
    {
        TEntity? entity;
        var key = $"Item_{Id}";
        if (UnitOfWorkContext.CustomValues.ContainsKey(key))
        {
            entity = (TEntity?)UnitOfWorkContext.CustomValues[key];
            if (entity == null) throw new NoNullAllowedException("UnitOfWorkContext.CustomValues[key] == null");
        }
        else
        {
            entity = IsNewModel() ? 
                (TEntity)CreateEntity() : 
                await RepoFactory.Create<TEntity>().GetByIdAsync(Id);
        }

        var entityCopyForValidation = UnitOfWorkContext.CloneDetached(entity);
        entityCopyForValidation = await this.MapToAsync(entityCopyForValidation);
        return entityCopyForValidation;
    }
    public virtual IEntity CreateEntity()
    {
        return new TEntity { Id = Id };
    }
    #endregion
}