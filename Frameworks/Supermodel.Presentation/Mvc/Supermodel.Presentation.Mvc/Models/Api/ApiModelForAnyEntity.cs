using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Models.Api;

public abstract class ApiModelForAnyEntity : IViewModelForAnyEntity
{
    #region Methods
    public bool IsNewModel() => Id == 0;
    #endregion
        
    #region ICustom mapper implementation
    public virtual Task MapFromCustomAsync<T>(T other)
    {
        return this.MapFromCustomBaseAsync(other);
    }
    public virtual async Task<T> MapToCustomAsync<T>(T other)
    {
        if (other == null || (long)other.PropertyGet("Id")! != Id)
        {
            var type = typeof(T);

            IEntity otherObj;
            if (Id == 0) otherObj = (IEntity)ReflectionHelper.CreateType(type);
            else otherObj = await RepoFactory.CreateForRuntimeType(type).GetIEntityByIdAsync(Id);

            return (T)await this.MapToAsync(otherObj, type);
        }
        return await this.MapToCustomBaseAsync(other);
    }
    #endregion

    #region Validation
    public virtual Task<ValidationResultList> ValidateAsync(ValidationContext validationContext)
    {
        return Task.FromResult(new ValidationResultList());
    }
    #endregion

    #region Properties
    public virtual long Id { get; set; }
    #endregion
}