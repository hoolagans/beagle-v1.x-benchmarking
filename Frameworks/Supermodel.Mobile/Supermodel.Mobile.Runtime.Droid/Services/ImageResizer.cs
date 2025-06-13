using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Supermodel.Mobile.Runtime.Common.Services;

namespace Supermodel.Mobile.Runtime.Droid.Services;

public class ImageResizer : IImageResizer
{
    public async Task<byte[]> ResizeImageAsync(byte[] imageData, float maxWidth, float maxHeight)
    {
        var result = await ResizeImageAndroidAsync(imageData, maxWidth, maxHeight);
        return result;
    }
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private Task<byte[]> ResizeImageAndroidAsync(byte[] imageData, float width, float height)
    {
        // Load the bitmap 
        Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
        //
        float ZielHoehe;
        float ZielBreite;
        //
        // ReSharper disable once PossibleNullReferenceException
        var Hoehe = originalImage.Height;
        var Breite = originalImage.Width;
        //
        if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
        {
            ZielHoehe = height;
            float teiler = Hoehe / height;
            ZielBreite = Breite / teiler;
        }
        else // Breite (61 für Avatar) ist Master
        {
            ZielBreite = width;
            float teiler = Breite / width;
            ZielHoehe = Hoehe / teiler;
        }
        //
        Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, false);
        // 
        using (MemoryStream ms = new MemoryStream())
        {
            // ReSharper disable once PossibleNullReferenceException
            resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
            return Task.FromResult(ms.ToArray());
        }
    }
}