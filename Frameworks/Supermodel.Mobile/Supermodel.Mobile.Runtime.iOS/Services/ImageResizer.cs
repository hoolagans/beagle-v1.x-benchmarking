using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Drawing;
using Supermodel.Mobile.Runtime.Common.Services;
using UIKit;

namespace Supermodel.Mobile.Runtime.iOS.Services;

public class ImageResizer : IImageResizer
{
    public async Task<byte[]> ResizeImageAsync(byte[] imageData, float maxWidth, float maxHeight)
    {
        var result = await ResizeImageIOSAsync(imageData, maxWidth, maxHeight);
        return result;
    }
        
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public Task<byte[]> ResizeImageIOSAsync(byte[] imageData, float width, float height)
    {
        // Load the bitmap
        UIImage originalImage = ImageFromByteArray(imageData);
        //
        var Hoehe = originalImage.Size.Height;
        var Breite = originalImage.Size.Width;
        //
        nfloat ZielHoehe;
        nfloat ZielBreite;
        //

        if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
        {
            ZielHoehe = height;
            nfloat teiler = Hoehe / height;
            ZielBreite = Breite / teiler;
        }
        else // Breite (61 for Avatar) ist Master
        {
            ZielBreite = width;
            nfloat teiler = Breite / width;
            ZielHoehe = Hoehe / teiler;
        }
        //
        width = (float)ZielBreite;
        height = (float)ZielHoehe;
        //
        UIGraphics.BeginImageContext(new SizeF(width, height));
        originalImage.Draw(new RectangleF(0, 0, width, height));
        var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
        UIGraphics.EndImageContext();
        //
        var bytesImagen = resizedImage.AsJPEG().ToArray();
        resizedImage.Dispose();
        return Task.FromResult(bytesImagen);
    }
    public static UIImage ImageFromByteArray(byte[] data)
    {
        if (data == null)
        {
            return null;
        }
        //
        UIImage image;
        try
        {
            image = new UIImage(Foundation.NSData.FromArray(data));
        }
        catch (Exception e)
        {
            Console.WriteLine("Image load failed: " + e.Message);
            return null;
        }
        return image;
    }
}