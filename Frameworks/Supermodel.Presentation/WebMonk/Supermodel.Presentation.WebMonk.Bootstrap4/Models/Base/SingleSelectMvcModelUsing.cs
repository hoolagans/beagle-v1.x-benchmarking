using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;

public abstract class SingleSelectMvcModelUsing<TMvcModel> : SingleSelectMvcModelForEntity, IAsyncInit where TMvcModel : MvcModelForEntityCore
{
    #region Constructors
    protected SingleSelectMvcModelUsing()
    {
        SelectedValue = "";
    }
    protected SingleSelectMvcModelUsing(long selectedId)
    {
        SelectedValue = selectedId.ToString(CultureInfo.InvariantCulture);
    }
    #endregion

    #region IAsyncInit implementation
    public bool AsyncInitialized { get; protected set; }
    public virtual async Task InitAsync()
    {
        var mvcModelForEntityBaseType = ReflectionHelper.IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(typeof(TMvcModel), typeof(Bs4.MvcModelForEntity<>));
        if (mvcModelForEntityBaseType == null) throw new SupermodelException("DropdownMvcModelUsing<MvcModelT> has invalid type parameter");

        var entityType = mvcModelForEntityBaseType.GetGenericArguments()[0];
        var repo = RepoFactory.CreateForRuntimeType(entityType);
        var entities = await repo.GetIEntityAllAsync().ConfigureAwait(false);

        Options = await GetDropdownOptionsAsync(entities).ConfigureAwait(false);
        AsyncInitialized = true;
    }
    protected virtual async Task<List<Option>> GetDropdownOptionsAsync<TEntity>(IEnumerable<TEntity> entities)
    {
        var myTypeName = GetType().FullName ?? throw new SupermodelException("SingleSelectMvcModelUsing<TMvcModel>.GetDropdownOptionsAsync: GetType().FullName is null");
        if (!UnitOfWorkContext.CustomValues.ContainsKey(myTypeName))
        {
            var mvcModels = new List<TMvcModel>();
            mvcModels = await mvcModels.MapFromAsync(entities.ToList()).ConfigureAwait(false);
            mvcModels = mvcModels.OrderBy(p => p.Label).ToList();
            UnitOfWorkContext.CustomValues[myTypeName] = !mvcModels.Any() ?
                new List<Option>() :
                mvcModels.Select(item => new Option(item.Id.ToString(CultureInfo.InvariantCulture), item.Label, item.IsDisabled)).ToList();
        }
        return (List<Option>)UnitOfWorkContext.CustomValues[myTypeName]!;
    }
    #endregion

    #region Properties
    public long? SelectedId
    {
        get
        {
            if (long.TryParse(SelectedValue, out var id)) return id;
            return null;
        }
    }
    #endregion
}