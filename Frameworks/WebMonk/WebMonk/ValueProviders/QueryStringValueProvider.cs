using System.Threading.Tasks;
using WebMonk.Context.WMHttpListenerObjects;

namespace WebMonk.ValueProviders;

public class QueryStringValueProvider : ValueProvider
{
    #region Methods
    public virtual Task<IValueProvider> InitAsync(IHttpListenerRequest request)
    {
        return base.InitAsync(request.QueryString);
    }
    #endregion
}