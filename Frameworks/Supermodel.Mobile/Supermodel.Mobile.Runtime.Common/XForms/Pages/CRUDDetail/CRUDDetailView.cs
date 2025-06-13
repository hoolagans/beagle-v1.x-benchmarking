using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.Views;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

public class CRUDDetailView : ViewWithActivityIndicator<TableView>
{
    public CRUDDetailView() : base(new TableView { Intent = TableIntent.Form, HasUnevenRows = true, Root = new TableRoot()}){}
}