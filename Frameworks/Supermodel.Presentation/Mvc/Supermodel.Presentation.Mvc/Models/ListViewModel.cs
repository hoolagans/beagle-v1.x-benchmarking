using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations;
using Supermodel.Persistence.Entities;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Models;

public class ListViewModel<TViewModelForEntity, TEntity> : List<TViewModelForEntity>, IRMapperCustom, IAsyncInit
    where TViewModelForEntity : IViewModelForAnyEntity, new()
    where TEntity : class, IEntity, new()
{
    #region IRMapperCustom implemtation
    public async Task MapFromCustomAsync<T>(T other)
    {
        Clear();
        var entityList = (ICollection<TEntity>?)other!;
        if (entityList == null) throw new ArgumentNullException(nameof(other));
        foreach (var entity in entityList.ToList())
        {
            var mvcModel = new TViewModelForEntity();
            if (mvcModel is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
            mvcModel = await mvcModel.MapFromAsync(entity);
            Add(mvcModel);
        }
    }
    public async Task<T> MapToCustomAsync<T>(T other)
    {
        var entityList = (ICollection<TEntity>?)other!;
        if (entityList == null) throw new ArgumentNullException(nameof(other));

        //Add or Update
        foreach (var viewModel in this)
        {
            TEntity? entityMatch = null;
            if (!viewModel.IsNewModel()) entityMatch = entityList.SingleOrDefault(x => x.Id == viewModel.Id);
            if (entityMatch != null)
            {
                await viewModel.MapToAsync(entityMatch);
            }
            else
            {
                var newEntity = new TEntity();
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (newEntity is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
                newEntity = await viewModel.MapToAsync(newEntity);
                entityList.Add(newEntity);
            }
        }

        //Delete
        foreach (var entity in entityList.ToList())
        {
            if (this.All(x => x.Id != entity.Id)) 
            {
                entityList.Remove(entity);
                entity.Delete();
            }
        }

        return (T)entityList;        
    }
    #endregion

    #region IAsyncInit implementation
    public bool AsyncInitialized { get; set; }
    public virtual async Task InitAsync()
    {
        if (AsyncInitialized) return;
            
        foreach (var viewModelForEntity in this)
        {
            if (viewModelForEntity is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
        }

        AsyncInitialized = true;
    }
    #endregion
}