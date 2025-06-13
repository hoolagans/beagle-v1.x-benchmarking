using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;

public abstract class MultiSelectMvcModelUsing<TMvcModel> : MultiSelectMvcModel, IRMapperCustom, IAsyncInit where TMvcModel : MvcModelForEntityCore
{
    #region Constructors
    protected MultiSelectMvcModelUsing()
    {
        var parentMvcModelType = ReflectionHelper.IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(typeof(TMvcModel), typeof(Bs4.MvcModelForEntity<>));
        if (parentMvcModelType == null) throw new SupermodelException("MultiSelectMvcModelUsing has invalid type argument");

        var entityType = parentMvcModelType.GenericTypeArguments[0];
        if (!typeof(IEntity).IsAssignableFrom(entityType)) throw new SupermodelException("MultiSelectMvcModelUsing has invalid type argument");
            
        EntityType = entityType;
    }
    #endregion
        
    #region IAsyncInit implementation
    public virtual bool AsyncInitialized { get; protected set; }
    public virtual async Task InitAsync()
    {
        if (AsyncInitialized) return;
            
        var repo = RepoFactory.CreateForRuntimeType(EntityType);
        var entities = await repo.GetIEntityAllAsync().ConfigureAwait(false);
        Options = await GetMultiSelectOptionsAsync(entities).ConfigureAwait(false);

        AsyncInitialized = true;
    }
    protected virtual async Task<List<Option>> GetMultiSelectOptionsAsync<TEntity>(IEnumerable<TEntity> entities)
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
            
        //Create deep copy
        var listOfOptions = ((List<Option>)UnitOfWorkContext.CustomValues[myTypeName]!).Select(x => new Option(x.Value, x.Label, x.IsDisabled, x.Selected)).ToList();
        return listOfOptions;
    }
    #endregion
        
    #region IRMapperCustom implementation
    public Task MapFromCustomAsync<T>(T other)
    {
        #pragma warning disable CS0618 // Type or member is obsolete
        if (other == null) throw new ArgumentNullException(nameof(other));

        if (EntityType == null ) throw new SupermodelException("TMvcModel must be a valid MvcModelForEntity<> in order to MapFromAsync");

        // ReSharper disable once InconsistentNaming
        if (other is IEnumerable<IM2M> m2mOther)
        {
            // ReSharper disable once InconsistentNaming
            foreach (var m2m in m2mOther)
            {
                var match = Options.Find(x => x.Value == m2m.GetConnectionToOther(EntityType).Id.ToString(CultureInfo.InvariantCulture));
                if (match != null) match.Selected = true;
            }
            return Task.CompletedTask;
        }

        if (other is IEnumerable<IEntity> entityOther) //note that IM2M can also be IEntity, that's why order is important
        {
            // ReSharper disable once InconsistentNaming
            foreach (var entity in entityOther)
            {
                var match = Options.Find(x => x.Value == entity.Id.ToString(CultureInfo.InvariantCulture));
                if (match != null) match.Selected = true;
            }
            return Task.CompletedTask;
        }

        throw new Exception("MultiSelectMvcModelUsing.MapFromCustomAsync: other is neither IEnumerable<IM2M> nor IEnumerable<IEntity>");
        #pragma warning restore CS0618 // Type or member is obsolete
    }
    public async Task<T> MapToCustomAsync<T>(T other)
    {
        #pragma warning disable CS0618 // Type or member is obsolete
        // ReSharper disable once InconsistentNaming
        var innerType = typeof(T).GetICollectionGenericArg();
        if (innerType == null) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");

        if (typeof(IM2M).IsAssignableFrom(innerType))
        {
            ICollection collection;
            if (typeof(T).IsGenericType) collection = (ICollection)(other ?? ReflectionHelper.CreateGenericType(typeof(List<>), typeof(T).GetGenericArguments()[0]));
            else collection = (ICollection)(other ?? ReflectionHelper.CreateType(typeof(T)));

            var entityRepo = RepoFactory.CreateForRuntimeType(EntityType);

            //Add or Leave alone
            foreach (var option in Options)
            {
                if (!option.Selected) continue;
                var id = long.Parse(option.Value);

                if (((IEnumerable<IM2M>)collection).All(x => x.GetConnectionToOther(EntityType).Id != id))
                {
                    var newM2M = (IM2M)ReflectionHelper.CreateType(innerType);
                    var newEntity = await entityRepo.GetIEntityByIdAsync(id);
                    newM2M.SetConnectionToOther(newEntity);
                    collection.AddToCollection(newM2M);
                }
            }

            //Delete
            // ReSharper disable once InconsistentNaming
            foreach (IM2M m2m in (IEnumerable<IM2M>)collection)
            {
                if (!Options.Any(x => x.Selected && x.Value == m2m.GetConnectionToOther(EntityType).Id.ToString()))
                {
                    var iEntity = (IEntity)m2m;
                    if (!iEntity.IsNewModel()) iEntity.Delete();
                    //collection.RemoveFromCollection(iEntity);
                }
            }

            return (T)collection;
        }

        if (typeof(IEntity).IsAssignableFrom(innerType))
        {
            ICollection collection;
            if (typeof(T).IsGenericType) collection = (ICollection)(other ?? ReflectionHelper.CreateGenericType(typeof(List<>), typeof(T).GetGenericArguments()[0]));
            else collection = (ICollection)(other ?? ReflectionHelper.CreateType(typeof(T)));

            var entityRepo = RepoFactory.CreateForRuntimeType(EntityType);

            //Add or Leave alone
            foreach (var option in Options)
            {
                if (!option.Selected) continue;
                var id = long.Parse(option.Value);

                if (((IEnumerable<IEntity>)collection).All(x => x.Id != id))
                {
                    var newEntity = await entityRepo.GetIEntityByIdAsync(id);
                    collection.AddToCollection(newEntity);
                }
            }

            //Delete
            var entitiesToDelete = new List<IEntity>();
            foreach (var entity in (IEnumerable<IEntity>)collection)
            {
                if (!Options.Any(x => x.Selected && x.Value == entity.Id.ToString())) entitiesToDelete.Add(entity);
            }
            foreach (var entity in entitiesToDelete)
            {
                collection.RemoveFromCollection(entity);
            }

            return (T)collection;
        }

        throw new SupermodelException("MultiSelectMvcModelUsing.MapToCustomAsync(): other is neither IEnumerable<IM2M> nor IEnumerable<IEntity>");
        #pragma warning restore CS0618 // Type or member is obsolete
    }
    #endregion

    #region Properties
    protected Type EntityType { get; }
    #endregion
}