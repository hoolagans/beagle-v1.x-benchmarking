using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net;
using Supermodel.Mobile.Runtime.Common.Exceptions;
using Supermodel.Mobile.Runtime.Common.XForms.Views;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;
using Supermodel.Mobile.Runtime.Common.UnitOfWork;
using Supermodel.Mobile.Runtime.Common.Repository;
using Supermodel.Mobile.Runtime.Common.XForms.App;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDList;

public abstract class EnhancedCRUDListPage<TModel, TDataContext> : CRUDListPageCore<TModel> 
    where TModel : class, ISupermodelListTemplate, IModel, new()
    where TDataContext : class, IDataContext, new()
{
    #region Event Handlers
    public virtual async void RunSearchHandler(object sender, TextChangedEventArgs args)
    {
        var searchTerm = ListView.SearchBar.Text;
        await Task.Delay(750);
        if (searchTerm != ListView.SearchBar.Text) return; //we must still be typing, let's wait for it to finish

        await LoadListContentAsync(0, Take, searchTerm);
    }
    public override async void ItemAppearingHandler(object sender, ItemVisibilityEventArgs args)
    {
        if (LoadedAll || LoadingInProgress) return;
        var searchTerm = ListView.SearchBar.Text;
        if (Models.Last() == args.Item) await LoadListContentAsync(Models.Count, Take, searchTerm);
    }
    public virtual async void RefreshingHandler(object sender, EventArgs args)
    {
        Models = null;
        await LoadListContentAsync(searchTerm: ListView.SearchBar.Text, showActivityIndicator: false);
        ListView.ListPanel.ContentView.IsRefreshing = false;
    }
    #endregion

    #region Overrides
    protected override void InitContent(bool readOnly)
    {
        Content = StackLayout = new StackLayout();
        if (readOnly) ListView = new CRUDListView<TModel>(SelectItem, null, true);
        else Content = ListView = new CRUDListView<TModel>(SelectItem, DeleteItemHandler, true);
        ListView.SearchBar.TextChanged += RunSearchHandler;
        ListView.ListPanel.ContentView.Refreshing += RefreshingHandler;
        StackLayout.Children.Add(ListView);
    }
    protected override async Task<bool> DeleteItemInternalAsync(TModel model)
    {
        await using (FormsApplication.GetRunningApp().NewUnitOfWork<TDataContext>())
        {
            model.Delete();
            //await UnitOfWorkContext.FinalSaveChangesAsync();
            return true;
        }
    }
    protected virtual async Task<List<TModel>> GetItemsInternalAsync(int skip, int? take, string searchTerm)
    {
        await using (FormsApplication.GetRunningApp().NewUnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var repo = RepoFactory.Create<TModel>();
            return await repo.GetWhereAsync(new { SearchTerm = searchTerm }, null, skip, take); //We assume SimpleSearchApiModel is used here
        }
    }
    #endregion

    #region Methods
    public virtual async Task LoadListContentAsync(int skip = 0, int? take = -1, string searchTerm = null, bool showActivityIndicator = true)
    {
        if (take < 0) take = Take;
        if (searchTerm == null) searchTerm = "";
            
        bool connectionLost;
        do
        {
            connectionLost = false;
            try
            {
                LoadingInProgress = true;
                LoadedAll = false;
                    
                if (Models == null) Models = new ObservableCollection<TModel>();
                if (skip == 0) Models.Clear();
                    
                if (showActivityIndicator)
                {
                    using(new ActivityIndicatorFor(ListView.ListPanel))
                    {
                        var models = await GetItemsInternalAsync(skip, take, searchTerm);
                        if (take == null || models.Count < take) LoadedAll = true;

                        foreach (var model in models)
                        {
                            if (Models.All(x => x.Id != model.Id)) Models.Add(model);
                        }
                        ListView.ListPanel.ContentView.ItemsSource = Models;
                    }
                }
                else
                {
                    var models = await GetItemsInternalAsync(skip, take, searchTerm);
                    if (take == null || models.Count < take) LoadedAll = true;

                    foreach (var model in models)
                    {
                        if (Models.All(x => x.Id != model.Id)) Models.Add(model);
                    }
                    ListView.ListPanel.ContentView.ItemsSource = Models;
                }
            }
            catch (SupermodelWebApiException ex1)
            {
                if (ex1.StatusCode == HttpStatusCode.Unauthorized)
                {
                    UnauthorizedHandler();
                }
                else if (ex1.StatusCode == HttpStatusCode.InternalServerError)
                {
                    connectionLost = true;
                    await DisplayAlert("Internal Server Error", ex1.ContentJsonMessage, "Ok");
                }
                else
                {
                    connectionLost = true;
                    await DisplayAlert("Connection Lost", "Connection to the cloud cannot be established.", "Try again");
                }
            }
            catch (Exception netEx) when (netEx is HttpRequestException || netEx is IOException || netEx is WebException)
            {
                connectionLost = true;
                await DisplayAlert("Connection Lost", "Connection to the cloud cannot be established.", "Try again");
            }
            catch (Exception ex2)
            {
                connectionLost = true;
                await DisplayAlert("Unexpected Error", ex2.Message, "Try again");
            }
            finally
            {
                LoadingInProgress = false;
            }
        } 
        while (connectionLost);
    }
    #endregion
}