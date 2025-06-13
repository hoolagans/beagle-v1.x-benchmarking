using System;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.XForms.Views;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDList;

public class CRUDListView<TModel> : StackLayout where TModel : class, ISupermodelListTemplate, new()
{
    #region Constructors
    public CRUDListView(Func<TModel, Task> selectItem, EventHandler deleteHandler, bool searchBar)
    {
        if (searchBar)
        {
            Spacing = 1;
            SearchBar = new SearchBar { Placeholder = "Type your search term here" };
            Children.Add(SearchBar);
        }

        ListPanel = new ViewWithActivityIndicator<ListView>(new ListView
        {
            ItemTemplate = new TModel().GetListCellDataTemplate((sender, _) => 
                { 
                    var model = (TModel)((MenuItem)sender).CommandParameter;
                    selectItem(model);
                }, 
                deleteHandler),

            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            IsPullToRefreshEnabled = true
        });
        Children.Add(ListPanel);
    }
    #endregion

    #region Properties
    public SearchBar SearchBar { get; }
    public ViewWithActivityIndicator<ListView> ListPanel { get; }
    #endregion
}