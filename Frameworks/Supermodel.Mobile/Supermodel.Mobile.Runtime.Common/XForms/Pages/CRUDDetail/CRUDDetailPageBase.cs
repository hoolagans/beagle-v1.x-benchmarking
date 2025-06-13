using System.Collections.Generic;
using System.Collections.ObjectModel;
using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using Supermodel.ReflectionMapper;
using System;
using System.Net;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Supermodel.Mobile.Runtime.Common.XForms.Views;
using Supermodel.Mobile.Runtime.Common.Utils;
using Supermodel.Mobile.Runtime.Common.Models;
using System.Reflection;
using Supermodel.Mobile.Runtime.Common.XForms.App;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;
using System.Collections;
using System.IO;
using System.Net.Http;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Validations;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

public abstract class CRUDDetailPageBase<TModel, TXFModel, TDataContext> : CRUDDetailPageCore<TModel, TXFModel>, IHaveActivityIndicator
    where TModel : class, ISupermodelNotifyPropertyChanged, IModel, new()
    where TXFModel : XFModel, new()
    where TDataContext : class, IDataContext, new()
{
    #region Initializers
    protected virtual Task<CRUDDetailPageBase<TModel, TXFModel, TDataContext>> InitAsync(ObservableCollection<TModel> models, string title, TModel model, TXFModel xfModel, TXFModel originalXFModel)
    {
        Title = title;

        if (CancelButton) AddCancelButton();

        Models = models;
        Model = model;
        XFModel = xfModel;
        OriginalXFModel = originalXFModel;

        return Task.FromResult(this);
    }
    #endregion
        
    #region IHaveActivityIndicator implementation
    public async Task WaitForPageToBecomeActiveAsync()
    {
        while(!PageActive) await Task.Delay(25);
    }
    public bool ActivityIndicatorOn
    {
        get => DetailView.ActivityIndicatorOn;
        set => DetailView.ActivityIndicatorOn = value;
    }
    public string Message
    {
        get => DetailView.Message;
        set => DetailView.Message = value;
    }
    #endregion

    #region Overrides
    protected abstract Task<TXFModel> GetBlankXFModelAsync();
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        InitContent();
        PageActive = true;
        if (XFModel.ContainsValidationErrros()) await DisplayAlert("Validation Errors", "Please correct problems with fields marked with '!'", "Ok");
    }
    protected override async void OnDisappearing()
    {
        PageActive = false;
        await OnDisappearingInternalAsync();
    }
    protected virtual async Task OnDisappearingInternalAsync()
    {
        var navigationStack = Navigation.NavigationStack;
        //Store current state of NavStack
        var navStackCount = navigationStack.Count;

        var goingBack = navigationStack.Last() == this;
        var parentPage = navigationStack[^(goingBack ? 2 : 3)];
        var childPage = goingBack ? null : navigationStack.Last();
        var pageShowing = goingBack ? parentPage : childPage;

        //Finish disappearing :)
        base.OnDisappearing();

        //Let the page finish disappearing from NavStack
        while (navigationStack.Count == navStackCount) await Task.Delay(100);

        var blankXFModel = await GetBlankXFModelAsync();
        if (DisappearingBecauseOfCancellation || (goingBack && XFModel.AreWritableFieldsEqual(blankXFModel))) return;

        //Try to validate locally. We validate even if the hash did not change since we only calculate hash to persistent fields
        var localVr = new ValidationResultList();
        if (!await AsyncValidator.TryValidateObjectAsync(XFModel, new ValidationContext(XFModel, new Dictionary<object,object> { { "CanBeCancellation", goingBack } }), localVr))
        {
            //if we had local validation errors
            XFModel.ShowValidationErrors(localVr);
            if (goingBack)
            {
                var page = (CRUDDetailPageBase<TModel, TXFModel, TDataContext>)ReflectionHelper.CreateType(GetType());
                await parentPage.Navigation.PushAsync(await page.InitAsync(Models, Title, Model, XFModel, OriginalXFModel));
            }
            else
            {
                await parentPage.Navigation.PopAsync();     
            }
            return;
        }

        //Map to Model
        var originalModelHash = ComputeModelHash(Model);
        ValidationResultList mappingVr = null;
        try
        {
            Model = await XFModel.MapToAsync(Model);
            if (pageShowing is IBasicCRUDDetailPage basicCRUDPageShowing) basicCRUDPageShowing.InitContent(); //we do this because on Android page appears before page disappears
        }
        catch (ValidationResultException ex)
        {
            mappingVr = ex.ValidationResultList;
        }
        if (mappingVr != null && mappingVr.Any())
        {
            //if we had mapping validation errors
            Model = await OriginalXFModel.MapToAsync(Model);
            if (pageShowing is IBasicCRUDDetailPage basicCRUDPageShowing) basicCRUDPageShowing.InitContent(); //we do this because on Android page appears before page disappears
            XFModel.ShowValidationErrors(mappingVr);
            if (goingBack)
            {
                var page = (CRUDDetailPageBase<TModel, TXFModel, TDataContext>)ReflectionHelper.CreateType(GetType());
                await parentPage.Navigation.PushAsync(await page.InitAsync(Models, Title, Model, XFModel, OriginalXFModel));
            }
            else
            {
                await parentPage.Navigation.PopAsync();     
            }
            return;
        }

        //if Model did not change, we don't need to save it
        if (ComputeModelHash(Model) == originalModelHash) 
        {
            //This is in case we had validation errors in the XFModel to begin with
            XFModel.ClearValidationErrors();
            return;
        }

        //Try to save to DataContext
        bool connectionLost;
        ValidationResultList serverVr = null;
        do
        {
            connectionLost = false;
            try
            {
                if (pageShowing is IHaveActivityIndicator pageShowingActivityIndicator)
                {
                    using (await ActivityIndicatorFor.CreateAsync(pageShowingActivityIndicator))
                    {
                        //var t1 = Task.Delay(250); //short delay so that saving indicator could be shown
                        //var t2 = SaveItemInternalAsync(Model);
                        //await Task.WhenAll(t1, t2);
                        await SaveItemInternalAsync(Model);
                    }
                }
                else
                {
                    await SaveItemInternalAsync(Model);
                }
                if (!Model.IsNew && Models.All(x => x.Id != Model.Id)) Models.Add(Model);
            }
            catch (SupermodelWebApiException ex1)
            {
                if (ex1.StatusCode == HttpStatusCode.Unauthorized)
                {
                    UnauthorizedHandler();
                }
                else if (ex1.StatusCode == HttpStatusCode.NotFound)
                {
                    Models.RemoveAll(x => x == Model);
                    await pageShowing.DisplayAlert("Not Found", "Item you are trying to update no longer exists.", "Ok");
                }
                else if (ex1.StatusCode == HttpStatusCode.Conflict)
                {
                    await parentPage.DisplayAlert("Unable to Delete", ex1.ContentJsonMessage, "Ok");
                }
                else if (ex1.StatusCode == HttpStatusCode.InternalServerError)
                {
                    connectionLost = true;
                    await parentPage.DisplayAlert("Internal Server Error", ex1.ContentJsonMessage, "Ok");
                }
                else
                {
                    connectionLost = true;
                    await pageShowing.DisplayAlert("Connection Lost", "Connection to the cloud cannot be established.", "Try again");
                }
            }
            catch (SupermodelDataContextValidationException ex2)
            {
                var vrl = ex2.ValidationErrors;
                if (vrl.Count != 1) throw new SupermodelException("vrl.Count != 1. This should never happen!");
                if (!vrl[0].Any()) throw new SupermodelException("!vrl[0].Any(): Server returned validation error with no validation results");
                serverVr = vrl[0];
            }
            catch (Exception netEx) when (netEx is HttpRequestException || netEx is IOException || netEx is WebException)
            {
                connectionLost = true;
                await pageShowing.DisplayAlert("Connection Lost", "Connection to the cloud cannot be established.", "Try again");
            }
            catch (Exception ex3)
            {
                await DisplayAlert("Unexpected Error", ex3.Message, "Try again");
                connectionLost = true;
            }
        } 
        while (connectionLost);  

        if (serverVr != null && serverVr.Any())
        {
            //if we had any validation errors while trying to save to DataContext
            Model = await OriginalXFModel.MapToAsync(Model); 
            XFModel.ShowValidationErrors(serverVr);

            if (goingBack)
            {
                var page = (CRUDDetailPageBase<TModel, TXFModel, TDataContext>)ReflectionHelper.CreateType(GetType());
                await parentPage.Navigation.PushAsync(await page.InitAsync(Models, Title, Model, XFModel, OriginalXFModel));
            }
            else
            {
                await parentPage.Navigation.PopAsync();   
            }
            return;
        }
        //If no validation issues, mark all properties as changed. This way the list will always update
        //foreach (var property in Model.GetType().GetTypeInfo().DeclaredProperties) Model.OnPropertyChanged(property.Name);
        MarkAllPropertiesChanged(Model);

        //This is in case we had validation errors in the XFModel to begin with
        XFModel.ClearValidationErrors();
    }
    protected void MarkAllPropertiesChanged(ISupermodelNotifyPropertyChanged model)
    {
        foreach (var property in model.GetType().GetTypeInfo().DeclaredProperties)
        {
            Model.OnPropertyChanged(property.Name);
            var propertyValue = model.PropertyGet(property.Name);
            if (propertyValue is ISupermodelNotifyPropertyChanged propertyValueChangedObj) MarkAllPropertiesChanged(propertyValueChangedObj);
            if (property.PropertyType != typeof(string) && propertyValue is IEnumerable propertyValueIEnumerableChanged)
            {
                foreach (var propertyValueInIEnumerable in propertyValueIEnumerableChanged)
                {
                    if (propertyValueInIEnumerable is ISupermodelNotifyPropertyChanged propertyValueInIEnumerableChangedObj) MarkAllPropertiesChanged(propertyValueInIEnumerableChangedObj);
                }
            }
        }
    }
    protected virtual async Task SaveItemInternalAsync(TModel model)
    {
        await using (FormsApplication.GetRunningApp().NewUnitOfWork<TDataContext>())
        {
            if (model.IsNew) model.Add();
            else model.Update();
            //await UnitOfWorkContext.FinalSaveChangesAsync();
        }
    }
    #endregion

    #region Properties
    protected bool PageActive { get; set; }
    #endregion
}