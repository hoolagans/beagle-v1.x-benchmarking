using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.WebMonk.Models.Api;
using Supermodel.ReflectionMapper;
using WebMonk.Results;

namespace Supermodel.Presentation.WebMonk.Controllers.Api;

public abstract class EnhancedCRUDApiController<TEntity, TApiModel, TSearchApiModel, TDataContext> : EnhancedCRUDApiController<TEntity, TApiModel, TApiModel, TSearchApiModel, TDataContext>
    where TDataContext : class, IDataContext, new()
    where TEntity : class, IEntity, new()
    where TApiModel : ApiModelForEntity<TEntity>, new()
    where TSearchApiModel : SearchApiModel, new();

public abstract class EnhancedCRUDApiController<TEntity, TDetailApiModel, TListApiModel, TSearchApiModel, TDataContext> : CRUDApiController<TEntity, TDetailApiModel, TListApiModel, TDataContext>
    where TDataContext : class, IDataContext, new()
    where TEntity : class, IEntity, new()
    where TDetailApiModel : ApiModelForEntity<TEntity>, new()
    where TListApiModel : ApiModelForEntity<TEntity>, new()
    where TSearchApiModel : SearchApiModel, new()
{
    #region Action Methods
    public virtual async Task<ActionResult> GetWhereAsync(TSearchApiModel smSearchBy, int? smSkip = null, int? smTake = null, string? smSortBy = null)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            var entities = await GetPagedSortedAndSearchedItems(smSkip, smTake, smSortBy, smSearchBy).ToListAsync().ConfigureAwait(false);
            var apiModels = new List<TListApiModel>();
            apiModels = await apiModels.MapFromAsync(entities).ConfigureAwait(false);
            return new JsonApiResult(apiModels);
        }
    }
    public virtual async Task<ActionResult> GetCountWhereAsync(TSearchApiModel smSearchBy, int? smSkip = null, int? smTake = null, string? smSortBy = null)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            var count = await GetPagedSortedAndSearchedItems(smSkip, smTake, smSortBy, smSearchBy).CountAsync().ConfigureAwait(false);
            return new JsonApiResult(count);
        }
    }
    #endregion

    #region Protected Helpers
    protected virtual IQueryable<TEntity> GetPagedSortedAndSearchedItems(int? skip, int? take, string? sortBy, TSearchApiModel searchBy)
    {
        var items = GetItems();
        items = ApplySearchBy(items, searchBy);
        items = ApplySortBy(items, sortBy);
        items = ApplySkipAndTake((IOrderedQueryable<TEntity>)items, skip, take);

        return items;
    }
    protected virtual IQueryable<TEntity> ApplySearchBy(IQueryable<TEntity> items, TSearchApiModel searchBy)
    {
        return items;
    }
    #endregion
}