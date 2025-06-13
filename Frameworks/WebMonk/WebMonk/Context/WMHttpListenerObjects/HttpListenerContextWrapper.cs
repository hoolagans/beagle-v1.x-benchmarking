#nullable disable

using System.Net;

namespace WebMonk.Context.WMHttpListenerObjects;

public class HttpListenerContextWrapper : IHttpListenerContext
{
    #region Constructors
    public HttpListenerContextWrapper(HttpListenerContext context)
    {
        Context = context;
    }
    #endregion

    #region IHttpListenerContext implementation
    public IHttpListenerRequest Request
    {
        get
        {
            {
                _request ??= new HttpListenerRequestWrapper(Context.Request);
                return _request;
            }
        }
    }
    private IHttpListenerRequest _request;

    public IHttpListenerResponse Response 
    { 
        get 
        { 
            _response ??= new HttpListenerResponseWrapper(Context.Response);
            return _response;
        } 
    }
    private IHttpListenerResponse _response;
    #endregion

    #region Properties
    protected HttpListenerContext Context { get; }
    #endregion
}