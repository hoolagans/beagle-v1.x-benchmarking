using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Supermodel.DataAnnotations.Validations;

public interface IAsyncValidatableObject
{
    Task<ValidationResultList> ValidateAsync(ValidationContext validationContext);
}