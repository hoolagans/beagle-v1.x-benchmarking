using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;

namespace Supermodel.Persistence.Entities;

public interface IEntity : IAsyncValidatableObject
{
    long Id { get; set; }
    void Add();
    void Delete();
    bool IsNewModel();
    Task BeforeSaveAsync(OperationEnum operation);
}