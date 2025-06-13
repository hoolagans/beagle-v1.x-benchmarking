using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.Services;

[SharedService.Singleton]
public interface IImageResizer
{
    Task<byte[]> ResizeImageAsync(byte[] imageData, float maxWidth, float maxHeight);
}