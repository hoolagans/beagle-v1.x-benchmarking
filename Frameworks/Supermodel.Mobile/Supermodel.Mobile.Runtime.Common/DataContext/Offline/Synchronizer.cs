using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Mobile.Runtime.Common.DataContext.Sqlite;
using Supermodel.Mobile.Runtime.Common.DataContext.WebApi;
using Supermodel.Mobile.Runtime.Common.Exceptions;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.ReflectionMapper;
using Supermodel.Mobile.Runtime.Common.Repository;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;
using Supermodel.Mobile.Runtime.Common.PersistentDict;
using Supermodel.Mobile.Runtime.Common.UnitOfWork;

namespace Supermodel.Mobile.Runtime.Common.DataContext.Offline;

public abstract class Synchronizer<TModel, TWebApiDataContext, TSqliteDataContext> 
    where TModel : class, IModel, new()
    where TSqliteDataContext : SqliteDataContext, new()
    where TWebApiDataContext : WebApiDataContext, new()
{
    #region Constructiors
    protected Synchronizer()
    {
        RefreshFromMasterAfterSynch = true;
    }
    #endregion

    #region Methods
    public async Task SynchronizeAsync()
    {
        await using(var webApiUOW = new UnitOfWork<TWebApiDataContext>())
        {
            SetUpWebApiContext(UnitOfWorkContext<TWebApiDataContext>.CurrentDataContext);

            await using(var sqliteUOW = new UnitOfWork<TSqliteDataContext>())
            {
                SetUpSqliteContext(UnitOfWorkContext<TSqliteDataContext>.CurrentDataContext);
                    
                //----------------------------------------------------------------------------------------
                //Load all the models to synchronize
                //----------------------------------------------------------------------------------------
                var masterModels = await LoadAllSynchFromMasterAsync();
                var localModels = await LoadAllSynchFromLocalAsync();
                    
                //----------------------------------------------------------------------------------------
                //Run the synching algorithm
                //----------------------------------------------------------------------------------------
                await SynchListsAsync(masterModels, localModels);

                //----------------------------------------------------------------------------------------
                //Let's try to validate all the models that are to be saved locally
                //----------------------------------------------------------------------------------------
                SupermodelDataContextValidationException localValidationException = null;
                try
                {
                    await UnitOfWorkContext<TSqliteDataContext>.CurrentDataContext.ValidatePendingActionsAsync();
                }
                catch (SupermodelDataContextValidationException ex)
                {
                    localValidationException = ex;
                }
                catch (Exception)
                {
                    webApiUOW.Context.CommitOnDispose = sqliteUOW.Context.CommitOnDispose = false;
                    throw;
                }
                if (localValidationException != null)
                {
                    await ResolveLocalValidationError(localValidationException, webApiUOW, sqliteUOW);
                }

                //----------------------------------------------------------------------------------------
                //Register for refresh if needed
                //----------------------------------------------------------------------------------------

                DelayedModels<TModel> delayedMasterModels = null;
                if (RefreshFromMasterAfterSynch)
                {
                    var sqliteDataContext = UnitOfWorkContext<TSqliteDataContext>.PopDbContext();
                    UnitOfWorkContext.DetectUpdates();
                    UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteDataContext);
                    RegisterDelayedLoadAllSynchFromMaster(out delayedMasterModels);
                }

                //----------------------------------------------------------------------------------------
                //Then try to save changes to the web api
                //----------------------------------------------------------------------------------------
                UnitOfWorkContext<TSqliteDataContext>.PopDbContext();

                SupermodelDataContextValidationException serverValidationException = null;
                try
                {
                    await UnitOfWorkContext.FinalSaveChangesAsync();
                }
                catch (SupermodelDataContextValidationException ex)
                {
                    serverValidationException = ex;
                }
                catch(Exception)
                {
                    webApiUOW.Context.CommitOnDispose = sqliteUOW.Context.CommitOnDispose = false;
                    throw;
                }
                    
                if (serverValidationException != null)
                {
                    await ResolveServerValidationError(serverValidationException, webApiUOW, sqliteUOW);
                }
                UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteUOW.Context);

                //----------------------------------------------------------------------------------------
                //Refresh local models if we have data to refresh with (that is if we registered for it earlier)
                //----------------------------------------------------------------------------------------
                if (delayedMasterModels != null)
                {
                    foreach (var localModel in localModels)
                    {
                        var matchingDelayedMasterModel = delayedMasterModels.Values.SingleOrDefault(x => x.Id == localModel.Id);
                        if (matchingDelayedMasterModel != null) await CopyModel1IntoModel2Async(matchingDelayedMasterModel, localModel);
                    }
                }

                //----------------------------------------------------------------------------------------
                //If that succeeds, then we save changes to the local db, should not have any more validation errors, since we already checked
                //----------------------------------------------------------------------------------------
                LastSynchDateTimeUtc = DateTime.UtcNow;
                //await UnitOfWorkContext.FinalSaveChangesAsync();
            }
        }
    }
    public bool IsUploadPending(TModel model)
    {
        return LastSynchDateTimeUtc == null || GetModifiedDateTimeUtc(model) > LastSynchDateTimeUtc;
    }
    #endregion

    #region Main Algorithm
    protected virtual async Task SynchListsAsync(List<TModel> masterModels, List<TModel> localModels)
    {
        //find updated on the server, update locally
        //find updated on the client, update on the server
        //find created on the server, create locally
        //find deleted locally, delete on the server
        foreach (var masterModel in masterModels)
        {
            //Try to find a matching local model
            var matchingLocalModel = localModels.SingleOrDefault(x => x.Id == masterModel.Id);
            if (matchingLocalModel != null) //if we found one
            {
                if (GetModifiedDateTimeUtc(masterModel) > LastSynchDateTimeUtc && GetModifiedDateTimeUtc(matchingLocalModel) > LastSynchDateTimeUtc)
                {
                    //if model was updated on both server and client, call the hook to let the user resolve conflict
                    await HandleModelUpdatedOnServerAndDeviceAsync(masterModel, matchingLocalModel);
                }
                else
                {
                    //otherwise, we just figure out the newer one and copy it into the older one
                    await CopyNewerModelIntoOlderAsync(masterModel, matchingLocalModel);
                }
            }
            else //if not, there could be two scenarios:
            {
                //If the model on the server was created after our last synch or if we never synched before
                if (GetCreatedDateTimeUtc(masterModel) > LastSynchDateTimeUtc || LastSynchDateTimeUtc == null)
                {
                    //we need to add the master model to our local storage
                    //var localModel = new TModel();
                    //CopyModel1IntoModel2(masterModel, localModel);
                    //localModel.Add();
                    masterModel.Add(); //Add master model to local storage
                }
                else
                {
                    //otherwise it means that we deleted the model on the client and now we need to delete it from the server
                    if (GetModifiedDateTimeUtc(masterModel) > LastSynchDateTimeUtc)
                    {
                        //if since our last synch the model was modified on the server and deleted on the client, call the hook to let the user resolve conflict
                        HandleModelUpdatedOnServerDeletedOnDevice(masterModel);
                    }
                    else
                    {
                        var sqliteDataContext = UnitOfWorkContext<TSqliteDataContext>.PopDbContext();
                        masterModel.Delete();
                        UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteDataContext);
                    }
                }
            }
        }

        //find created locally, create on the server
        //find deleted on the server, delete locally
        foreach (var localModel in localModels)
        {
            if (localModel.Id < 0)
            {
                //if this model was never created on the server, go ahead and create it
                var sqliteDataContext = UnitOfWorkContext<TSqliteDataContext>.PopDbContext();
                localModel.Id = 0; //make sure when we add it on the server, Id == 0. When it is saved on the server, the local model's id should get updated
                localModel.Add(); //add local model to server context
                UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteDataContext);
            }
            else
            {
                //Try to find a matching server model
                var matchingServerModel = masterModels.SingleOrDefault(x => x.Id == localModel.Id);
                if (matchingServerModel == null) //if one exists, we already updated it and need not worry
                {
                    //otherwise, it means that the model was deleted on the server
                    if (GetModifiedDateTimeUtc(localModel) > LastSynchDateTimeUtc)
                    {
                        //if since our last synch the model was modified on the device and deleted on the client, call the hook to let the user resolve conflict
                        HandleModelUpdatedOnDeviceDeletedOnServer(localModel);
                    }
                    else
                    {
                        localModel.Delete();
                    }
                }
            }
        }
    }
    #endregion

    #region Conflict Resolution Methods
    protected virtual Task ResolveLocalValidationError(SupermodelDataContextValidationException validationException, UnitOfWork<TWebApiDataContext> webApiUOW, UnitOfWork<TSqliteDataContext> sqliteUOW)
    {
        //default implementation just rolls back transactions and rethrows the exception
        webApiUOW.Context.CommitOnDispose = sqliteUOW.Context.CommitOnDispose = false;
        throw validationException;
    }
    protected virtual Task ResolveServerValidationError(SupermodelDataContextValidationException validationException, UnitOfWork<TWebApiDataContext> webApiUOW, UnitOfWork<TSqliteDataContext> sqliteUOW)
    {
        //default implementation just rolls back transactions and rethrows the exception
        webApiUOW.Context.CommitOnDispose = sqliteUOW.Context.CommitOnDispose = false;
        throw validationException;
    }
    protected virtual void HandleModelUpdatedOnDeviceDeletedOnServer(TModel localModel)
    {
        //default implements "last one wins" approach
        localModel.Delete();
    }
    protected virtual Task HandleModelUpdatedOnServerAndDeviceAsync(TModel masterModel, TModel localModel)
    {
        //default implements "last one wins" approach
        return CopyNewerModelIntoOlderAsync(masterModel, localModel);
    }
    protected virtual void HandleModelUpdatedOnServerDeletedOnDevice(TModel masterModel)
    {
        //default implements "last one wins" approach
        var sqliteDataContext = UnitOfWorkContext<TSqliteDataContext>.PopDbContext();
        masterModel.Delete();
        UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteDataContext);
    }
    #endregion

    #region Modified and Created Utc DateTime Resolution
    public abstract DateTime GetModifiedDateTimeUtc(TModel model);
    public abstract DateTime GetCreatedDateTimeUtc(TModel model);
    #endregion

    #region Helpers that are Meant to be Overriden for Customization
    protected virtual void RegisterDelayedLoadAllSynchFromMaster(out DelayedModels<TModel> delayedMasterModels)
    {
        var sqliteDataContext = UnitOfWorkContext<TSqliteDataContext>.PopDbContext();
        RepoFactory.Create<TModel>().DelayedGetAll(out delayedMasterModels);
        UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteDataContext);
    }
    protected virtual Task<List<TModel>> LoadAllSynchFromMasterAsync()
    {
        var sqliteDataContext = UnitOfWorkContext<TSqliteDataContext>.PopDbContext();
        var result = RepoFactory.Create<TModel>().GetAllAsync();
        UnitOfWorkContext<TSqliteDataContext>.PushDbContext(sqliteDataContext);
        return result;
    }
    protected virtual Task<List<TModel>> LoadAllSynchFromLocalAsync()
    {
        return RepoFactory.Create<TModel>().GetAllAsync();
    }
    public virtual async Task CopyModel1IntoModel2Async(TModel model1, TModel model2)
    {
        if (model1.BroughtFromMasterDbOnUtc == null || model2.BroughtFromMasterDbOnUtc == null) throw new SupermodelException("(model1.BroughtFromMasterDbOnUtc == null || model2.BroughtFromMasterDbOnUtc == null): this should not happen");
        model1.BroughtFromMasterDbOnUtc = model2.BroughtFromMasterDbOnUtc = (model1.BroughtFromMasterDbOnUtc > model2.BroughtFromMasterDbOnUtc ? model1.BroughtFromMasterDbOnUtc : model2.BroughtFromMasterDbOnUtc);
        await model2.MapFromAsync(model1, true); //note that we force shallow copy here for performance reasons
    }
    public virtual async Task CopyNewerModelIntoOlderAsync(TModel model1, TModel model2)
    {
        if (GetModifiedDateTimeUtc(model1) > GetModifiedDateTimeUtc(model2)) await CopyModel1IntoModel2Async(model1, model2);
        else await CopyModel1IntoModel2Async(model2, model1);
    }
    protected abstract void SetUpWebApiContext(TWebApiDataContext context);
    protected abstract void SetUpSqliteContext(TSqliteDataContext context);
    #endregion

    #region LastSynch DateTime Handling
    public virtual DateTime? LastSynchDateTimeUtc
    {
        get => _lastSynchDateTimeUtc ??= LastSynchDateTimeUtcInternal;
        set => _lastSynchDateTimeUtc = LastSynchDateTimeUtcInternal = value;
    }
    private DateTime? _lastSynchDateTimeUtc;
        
    protected virtual DateTime? LastSynchDateTimeUtcInternal
    {
        get
        {
            if (!Properties.Dict.ContainsKey("smLastSynchDateTimeUtc")) return null;
            return Properties.Dict["smLastSynchDateTimeUtc"] as DateTime?;
        }
        set
        {
            Properties.Dict["smLastSynchDateTimeUtc"] = value;
#pragma warning disable 4014
            Properties.Dict.SaveToDiskAsync();
#pragma warning restore 4014
        }
    }
    #endregion

    #region Properties
    public bool RefreshFromMasterAfterSynch { get; set; }
    #endregion
}