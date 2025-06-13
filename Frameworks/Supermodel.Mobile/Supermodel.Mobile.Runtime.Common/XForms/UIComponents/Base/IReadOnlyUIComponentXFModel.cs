using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using Xamarin.Forms;  

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public interface  IReadOnlyUIComponentXFModel : ISupermodelMobileDetailTemplate
{
    bool ShowDisplayNameIfApplies { get; set; }
    string DisplayNameIfApplies { get; set; }
    TextAlignment TextAlignmentIfApplies { get; set; }
}