using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public abstract class ValueObjectMvcModel : MvcModel, IRMapperCustom, IValidatableObject
    {
        #region IRMapperCustom implementation
        public virtual Task MapFromCustomAsync<T>(T other)
        {
            return this.MapFromCustomBaseAsync(other);
        }
        public virtual Task<T> MapToCustomAsync<T>(T other)
        {
            return this.MapToCustomBaseAsync(other);
        }
        #endregion

        #region IValidatableObject implementation
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new ValidationResultList();
        }
        #endregion
    }
}