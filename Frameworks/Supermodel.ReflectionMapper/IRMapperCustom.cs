using System.Threading.Tasks;

namespace Supermodel.ReflectionMapper;

public interface IRMapperCustom
{
#nullable  disable
    Task MapFromCustomAsync<T>(T other);
    Task<T> MapToCustomAsync<T>(T other);
}