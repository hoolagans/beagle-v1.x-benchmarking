using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.XForms.Views;

public interface IHaveActivityIndicator
{
    Task WaitForPageToBecomeActiveAsync();
    bool ActivityIndicatorOn { get; set; }
    string Message { get; set; }
}