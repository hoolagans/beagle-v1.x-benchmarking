#nullable disable

namespace WebMonk.Context.WMHttpListenerObjects;

public interface IHttpListenerContext
{
    IHttpListenerRequest Request { get; }
    IHttpListenerResponse Response { get; }
}