using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;

namespace Supermodel.Persistence.Entities;

public abstract class SingletonEntity : Entity
{
    #region Overrides
    protected override void DeleteInternal()
    {
        throw new UnableToDeleteException("Singleton Entity cannot be deleted");
    }
    #endregion

    #region Validation
    public override async Task<ValidationResultList> ValidateAsync(ValidationContext validationContext)
    {
        var vr = await base.ValidateAsync(validationContext);
        if (Id != 1 && Id != 0) vr.Add(new ValidationResult("There could only be one record for a SingleTon Entity"));
        return vr;
    }
    #endregion
}