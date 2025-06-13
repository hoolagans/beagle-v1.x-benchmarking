using Supermodel.Mobile.Runtime.Common.Models;

namespace Supermodel.Mobile.Runtime.Common.Repository;

public interface IRepoFactory
{
    IDataRepo<TModel> CreateRepo<TModel>() where TModel : class, IModel, new();
}