using System.Collections.Generic;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Models;

namespace Supermodel.Mobile.Runtime.Common.DataContext.Core;

public interface IQueryableReadableDataContext : IReadableDataContext
{
    #region Queries
    Task<List<TModel>> GetWhereAsync<TModel>(object searchBy, string sortBy = null, int? skip = null, int? take = null) where TModel : class, IModel, new();
    Task<long> GetCountWhereAsync<TModel>(object searchBy, int? skip = null, int? take = null) where TModel : class, IModel, new();
    #endregion

    #region Delayed Queries
    void DelayedGetWhere<TModel>(out DelayedModels<TModel> models, object searchBy, string sortBy = null, int? skip = null, int? take = null) where TModel : class, IModel, new();
    void DelayedGetCountWhere<TModel>(out DelayedCount count, object searchBy) where TModel : class, IModel, new();
    #endregion
}