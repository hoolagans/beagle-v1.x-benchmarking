using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Supermodel.Encryptor;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.XForms.App;
using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using Xamarin.Forms;
using System.Linq;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

public abstract class CRUDDetailPageCore<TModel, TXFModel> : ContentPage, IBasicCRUDDetailPage
    where TModel : class, ISupermodelNotifyPropertyChanged, IModel, new()
    where TXFModel : XFModel, new()
{
    #region Overrides
    protected virtual void AddCancelButton()
    {
        // ReSharper disable once AsyncVoidLambda
        var cancelToolbarItem = new ToolbarItem("Cancel", CancelBtnIconFilename, async () => {
            DisappearingBecauseOfCancellation = true;
            await Navigation.PopAsync(true);
        });
        ToolbarItems.Add(cancelToolbarItem);
    }
    protected virtual void UnauthorizedHandler()
    {
        FormsApplication.GetRunningApp().HandleUnauthorized();
    }
    // ReSharper disable once UnusedParameter.Global
    protected virtual string ComputeModelHash(TModel model)
    {
        //We hash Json to ignore changes that do not get persisted
        //return JsonConvert.SerializeObject(Model).GetMD5Hash();
        return JsonConvert.SerializeObject(model).GetMD5Hash();
    }
    #endregion

    #region Methods
    //override this method to affect the entire view
    public virtual void InitContent()
    {
        //If we use the commented out code below list of children does not get updated when a child is updated
            
        //if (DetailView == null)
        //{
        //    DetailView = new CRUDDetailView();
        //    Content = StackLayout = new StackLayout { Children = { DetailView } };

        //    OnLoad();
        //    InitDetailView();
        //}
        DetailView = new CRUDDetailView();
        Content = StackLayout = new StackLayout { Children = { DetailView } };

        OnLoad();
        InitDetailView();
    }
    //Override this method to create sections
    public virtual void InitDetailView()
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        var sectionResolver = XFModel as IHaveSectionNames;

        var lastCell = XFModel.RenderDetail(this)?.LastOrDefault();
        if (lastCell != null)
        {
            var sectionNum = 0;
            while (true)
            {
                var cells = XFModel.RenderDetail(this, sectionNum, sectionNum + 99);
                if (cells.Any())
                {
                    var sectionName = sectionResolver?.GetSectionName(sectionNum);
                    var section = string.IsNullOrEmpty(sectionName) ? new TableSection() : new TableSection(sectionName);
                    DetailView.ContentView.Root.Add(section);
                    foreach (var cell in cells) section.Add(cell);
                }
                if (cells.Contains(lastCell)) break;
                sectionNum += 100;
            }
        }
    }
    public virtual void OnLoad(){}
    #endregion

    #region Properties
    public StackLayout StackLayout { get; set; }
    public CRUDDetailView DetailView { get; set; }
    public ObservableCollection<TModel> Models { get; set; } 
    public TModel Model { get; set; }
    public TXFModel XFModel { get; set; }
    public XFModel GetXFModel() { return XFModel; }
    public T GetXFModel<T>() where T : XFModel { return (T)(XFModel)XFModel; } //This is property in spirit

    public TXFModel OriginalXFModel { get; set; }

    protected virtual bool CancelButton => false;
    protected virtual string CancelBtnIconFilename => null;
    protected bool DisappearingBecauseOfCancellation { get; set; } //default is false
    #endregion
}