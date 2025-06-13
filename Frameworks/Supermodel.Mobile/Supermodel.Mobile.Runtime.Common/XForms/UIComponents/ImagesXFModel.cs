using System.Linq;
using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using System.IO;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using Supermodel.ReflectionMapper;
using Supermodel.Mobile.Runtime.Common.XForms.Views;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomCells;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Supermodel.Mobile.Runtime.Common.Services;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class ImagesXFModel : BinaryFilesWritableXFModel
{
    #region EmbeddedTypes
    public enum ImageViewModeEnum { ImageZoomEnabled, SwipingThroughImagesEnabled }
    #endregion

    #region Constructors
    public ImagesXFModel()
    {
        AddNewCell = new AddNewCell(XFormsSettings.AddNewImageFileName) { Text = "Add New Image" };
    }
    #endregion

    #region ISupermodelMobileDetailTemplate implementation
    public override List<Cell> RenderDetail(Page parentPage, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        ParentPage = parentPage;
        Cells.Clear();
        var imageIndex = 0;
        foreach (var imageFile in ModelsWithBinaryFileXFModels)
        {
            var file = imageFile;
            var index = imageIndex;

            var cell = new ImageCellWithEditableText
            {
                ImageSource = ImageSource.FromStream(() => new MemoryStream(file.BinaryFile.BinaryContent)),
                Text = file.Title,
                Placeholder = "Image Title",
            };
            cell.TextEntry.TextChanged += (_, _) => { file.Title = cell.Text; };

            var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true };
            deleteAction.Clicked += (_, _) => { ImageDeletedHandler(file, cell); };
            cell.ContextActions.Add(deleteAction);

            //This we do for Android -- otherwise the tap is not recognized
            //This is instead of cell.Tapped += (sender, args) => { ImageTappedHandler(index); };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (_, _) => { ImageTappedHandler(index); };
            cell.Image.GestureRecognizers.Add(tapGestureRecognizer);

            Cells.Add(cell);
            imageIndex++;
        }
        AddNewCell.ParentPage = parentPage;
        AddNewCell.Tapped += (_, _) => { AddNewTapped(); };
        Cells.Add(AddNewCell);
        return Cells;
    }
    public virtual async void AddNewTapped()
    {
        if (_actionSheetOpen) return;
        _actionSheetOpen = true;

        await CrossMedia.Current.Initialize();

        var options = new List<string>();
        if (CrossMedia.Current.IsPickPhotoSupported) options.Add("Photo Library");
        if (CrossMedia.Current.IsTakePhotoSupported) options.Add("Take Photo");
        var action = await ParentPage.DisplayActionSheet(null, "Cancel", null, options.ToArray());
        _actionSheetOpen = false;
        try
        {
            MediaFile mediaFile;
            switch (action)
            {
                case "Photo Library":
                {
                    mediaFile = await CrossMedia.Current.PickPhotoAsync();
                    break;
                }
                case "Take Photo":
                {
                    var storeCameraMediaOptions = new StoreCameraMediaOptions
                    {
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false,
                        Directory = "Media",
                        Name = "pic.jpg"
                    };
                    mediaFile = await CrossMedia.Current.TakePhotoAsync(storeCameraMediaOptions);
                    break;
                }
                case "Cancel":
                {
                    mediaFile = null;
                    break;
                }
                default: throw new Exception("Invalid photo option. This should never happen");
            }
            if (mediaFile != null)
            {
                var binaryContent = ReadFully(mediaFile.GetStream());
                binaryContent = await SharedService.Instantiate<IImageResizer>().ResizeImageAsync(binaryContent, 1024, 1024);

                //Insert image into ModelsWithBinaryFileXFModels list
                var file = new ModelWithBinaryFileXFModel { Id = 0, Title = "", BinaryFile = new BinaryFileXFModel { BinaryContent = binaryContent, FileName = "image.png" } };
                ModelsWithBinaryFileXFModels.Add(file);

                //Insert image cell
                var cell = new ImageCellWithEditableText
                {
                    ImageSource = ImageSource.FromStream(() => new MemoryStream(file.BinaryFile.BinaryContent)),
                    Text = file.Title,
                    Placeholder = "Image Title",
                };
                cell.TextEntry.TextChanged += (_, _) => { file.Title = cell.Text; };

                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true };
                deleteAction.Clicked += (_, _) => { ImageDeletedHandler(file, cell); };
                cell.ContextActions.Add(deleteAction);

                var tableView = (ViewWithActivityIndicator<TableView>)ParentPage.PropertyGet("DetailView");
                if (tableView == null) throw new SystemException("tableView == null");
                var section = tableView.ContentView.Root.Single(x => x.Contains(AddNewCell));
                var index = section.IndexOf(AddNewCell);

                //This we do for Android -- otherwise the tap is not recognized
                //This is instead of cell.Tapped += (sender, args) => { ImageTappedHandler(index); };
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (_, _) => { ImageTappedHandler(index); };
                cell.Image.GestureRecognizers.Add(tapGestureRecognizer);

                section.Insert(index, cell);
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }
    public virtual void ImageDeletedHandler(ModelWithBinaryFileXFModel file, Cell cell)
    {
        ModelsWithBinaryFileXFModels.Remove(file);

        var tableView = (ViewWithActivityIndicator<TableView>)ParentPage.PropertyGet("DetailView");
        if (tableView == null) throw new SystemException("tableView == null");
        foreach (var section in tableView.ContentView.Root) section.Remove(cell);
    }
    public virtual async void ImageTappedHandler(int imageIndex)
    {
        if (ImageViewMode == ImageViewModeEnum.SwipingThroughImagesEnabled)
        {
            var imagesCarouselPage = new CarouselPage();
            foreach (var imageFile in ModelsWithBinaryFileXFModels)
            {
                var imagePage = CreateSingleImagePage(imageFile, false);
                imagesCarouselPage.Children.Add(imagePage);
            }
            imagesCarouselPage.CurrentPage = imagesCarouselPage.Children[imageIndex];
            await ParentPage.Navigation.PushModalAsync(imagesCarouselPage);
        }
        else
        {
            var imagePage = CreateSingleImagePage(ModelsWithBinaryFileXFModels[imageIndex], true);
            await ParentPage.Navigation.PushModalAsync(imagePage);
        }
    }

    public static byte[] ReadFully(Stream input)
    {
        var buffer = new byte[16 * 1024];
        using (var ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
            return ms.ToArray();
        }
    }

    protected virtual ContentPage CreateSingleImagePage(ModelWithBinaryFileXFModel imageFile, bool withZoom)
    {
        Image image;
        if (withZoom)
        {
            image = new ZoomImage
            {
                Source = ImageSource.FromStream(() => new MemoryStream(imageFile.BinaryFile.BinaryContent)),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
        }
        else
        {
            image = new Image
            {
                Source = ImageSource.FromStream(() => new MemoryStream(imageFile.BinaryFile.BinaryContent)),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
        }

        AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0f, 0f, 1f, 1f));

        var button = new Button
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.End
        };

        //if (!string.IsNullOrEmpty(CloseIconFileName)) button/*.ImageSource*/.Image = new FileImageSource { File = CloseIconFileName };
        if (!string.IsNullOrEmpty(CloseIconFileName)) button.ImageSource = new FileImageSource { File = CloseIconFileName };
        else button.Text = "✖";

        button.Clicked += async (_, _) =>
        {
            await ParentPage.Navigation.PopModalAsync(true);
        };
        AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0.975f, 0.025f, 0.1f, 0.1f));

        var imagePage = new ContentPage
        {
            Content = new AbsoluteLayout
            {
                Children = { image, button }
            }
        };

        return imagePage;
    }
    #endregion

    #region Properties
    public string CloseIconFileName { get; set; }

    public override bool ShowDisplayNameIfApplies { get; set; }
    public override string DisplayNameIfApplies { get; set; }
    public override TextAlignment TextAlignmentIfApplies { get; set; }

    protected Page ParentPage { get; set; }
    protected AddNewCell AddNewCell { get; }

    public override string ErrorMessage
    {
        get => AddNewCell.ErrorMessage;
        set => AddNewCell.ErrorMessage = value;
    }
    public override bool Required
    {
        get => AddNewCell.Required;
        set => AddNewCell.Required = value;
    }

    public List<Cell> Cells { get; } = new List<Cell>();

    public ImageViewModeEnum ImageViewMode { get; set; } = ImageViewModeEnum.ImageZoomEnabled;
    protected bool _actionSheetOpen;
    #endregion
}