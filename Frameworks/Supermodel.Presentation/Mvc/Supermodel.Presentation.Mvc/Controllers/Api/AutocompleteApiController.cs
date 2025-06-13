using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;

namespace Supermodel.Presentation.Mvc.Controllers.Api;

[ApiController, Route("[controller]")]
public abstract class AutocompleteApiController<TEntity, TDataContext>  : ControllerBase 
    where TEntity : class, IEntity, new()
    where TDataContext : class, IDataContext, new()
{
    #region ActionMethods
    public virtual async Task<IActionResult> Get(string term)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            var items = GetItems();
            var entities = await AutocompleteAsync(items, term).ConfigureAwait(false);
            // ReSharper disable once ConvertClosureToMethodGroup
            var output = entities.Select(x => GetStringFromEntity(x));
            return StatusCode((int)HttpStatusCode.OK, output);
        }
    }
    #endregion

    #region Protected Helpers
    protected abstract Task<List<TEntity>> AutocompleteAsync(IQueryable<TEntity> items, string term);
    [NonAction] public abstract string GetStringFromEntity(TEntity entity);
    [NonAction] public abstract Task<TEntity?> GetEntityFromNameAsync(string uniqueName);

    protected virtual IQueryable<TEntity> GetItems()
    {
        return ControllerCommon.GetItems<TEntity>();
    }
    #endregion
}