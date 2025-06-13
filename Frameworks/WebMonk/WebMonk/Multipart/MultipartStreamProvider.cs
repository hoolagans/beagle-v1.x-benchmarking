using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebMonk.Multipart;

public abstract class MultipartStreamProvider
{
    private struct AsyncVoid{}
        
    private readonly Collection<HttpContent> _contents = new();

    public Collection<HttpContent> Contents
    {
        get { return _contents; }
    }

    public abstract Stream GetStream(HttpContent parent, HttpContentHeaders headers);

    public virtual Task ExecutePostProcessingAsync()
    {
        return Task.FromResult(default(AsyncVoid));
    }

    public virtual Task ExecutePostProcessingAsync(CancellationToken cancellationToken)
    {
        // Call the other overload to maintain backward compatibility.
        // ReSharper disable once MethodSupportsCancellation
        return ExecutePostProcessingAsync();
    }
}