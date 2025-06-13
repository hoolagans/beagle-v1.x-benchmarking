using System.IO;
using Xamarin.Forms;
using System.Collections.Generic;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class ImagesReadOnlyXFModel : BinaryFilesReadOnlyXFModel
{
    #region ISupermodelMobileDetailTemplate implementation
    public override List<Cell> RenderDetail(Page parentPage, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        ParentPage = parentPage;
        var cells = new List<Cell>();
        var imageIndex = 0;
        foreach (var imageFile in ModelsWithBinaryFileXFModels)
        {
            var file = imageFile;
            var index = imageIndex;

            var cell = new ImageCell
            {
                ImageSource = ImageSource.FromStream(() => new MemoryStream(file.BinaryFile.BinaryContent)),
                Text = file.Title
            };
            cell.Tapped += (_, _) => { ImageTappedHandler(index);};
            cells.Add(cell);
            imageIndex ++;
        }
        return cells;
    }
    public async void ImageTappedHandler(int imageIndex)
    {
        var imagesCarouselPage = new CarouselPage();
        foreach (var imageFile in ModelsWithBinaryFileXFModels)
        {
            // ReSharper disable once AccessToForEachVariableInClosure
            imagesCarouselPage.Children.Add(new ContentPage { Content = new Image { Source = ImageSource.FromStream(() => new MemoryStream(imageFile.BinaryFile.BinaryContent)), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand } });
        }
        imagesCarouselPage.CurrentPage = imagesCarouselPage.Children[imageIndex];
        await ParentPage.Navigation.PushAsync(imagesCarouselPage);
    }
    #endregion

    #region Properties
    public override bool ShowDisplayNameIfApplies { get; set; }
    public override string DisplayNameIfApplies { get; set; }
    public override TextAlignment TextAlignmentIfApplies { get; set; }
        
    protected Page ParentPage { get; set; }
    #endregion
}