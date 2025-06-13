using System.Threading.Tasks;

namespace Supermodel.DataAnnotations;

public interface IAsyncInit
{
    bool AsyncInitialized { get; }
    Task InitAsync();
}