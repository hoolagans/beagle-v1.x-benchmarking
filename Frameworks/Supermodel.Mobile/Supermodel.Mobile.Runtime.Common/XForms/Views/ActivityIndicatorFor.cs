using System;
using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.XForms.Views;

public class ActivityIndicatorFor : IDisposable
{
    public ActivityIndicatorFor(IHaveActivityIndicator element, string message = null, bool showActivityIndicator = true)
    {
        Element = element;
        Element.ActivityIndicatorOn = showActivityIndicator;
        Element.Message = message;
    }
    public static async Task<ActivityIndicatorFor> CreateAsync(IHaveActivityIndicator element, string message = null, bool showActivityIndicator = true)
    {
        await element.WaitForPageToBecomeActiveAsync();
        return new ActivityIndicatorFor(element, message, showActivityIndicator);
    }
        
    public void Dispose()
    {
        Element.ActivityIndicatorOn = false;
    }

    // ReSharper disable once InconsistentNaming
    public IHaveActivityIndicator Element { get; set; }
}