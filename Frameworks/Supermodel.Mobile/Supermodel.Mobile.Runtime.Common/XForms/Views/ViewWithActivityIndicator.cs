using Xamarin.Forms;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Services;

namespace Supermodel.Mobile.Runtime.Common.XForms.Views;

public class ViewWithActivityIndicator<TContentView> : AbsoluteLayout, IHaveActivityIndicator where TContentView : View
{
    #region Constructors
    public ViewWithActivityIndicator(TContentView contentView)
    {
        ContentView = contentView;
        ActivityIndicator = new ActivityIndicator();
        //MessageLabel = new Label { TextColor = Pick.ForPlatform(Color.Black, Color.White, Color.Black), Text = Message };
        // ReSharper disable once VirtualMemberCallInConstructor
        MessageLabel = new Label { TextColor = Pick.ForPlatform(Color.Black, Color.White), Text = Message };
        ActivityIndicatorAndMessageStackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };
        ActivityIndicatorAndMessageStackLayout.Children.Add(ActivityIndicator);
        ActivityIndicatorAndMessageStackLayout.Children.Add(MessageLabel);

        GrayOutOverlay = new BoxView { Color = new Color (0, 0, 0, 0.4) };

        HorizontalOptions = LayoutOptions.FillAndExpand;
        VerticalOptions = LayoutOptions.FillAndExpand;

        SetLayoutFlags(ContentView, AbsoluteLayoutFlags.All);
        SetLayoutBounds(ContentView, new Rectangle(0, 0, 1f, 1f));

        SetLayoutFlags(GrayOutOverlay, AbsoluteLayoutFlags.All);
        SetLayoutBounds(GrayOutOverlay, new Rectangle(0, 0, 1f, 1f));

        SetLayoutFlags(ActivityIndicatorAndMessageStackLayout, AbsoluteLayoutFlags.PositionProportional);
        SetLayoutBounds(ActivityIndicatorAndMessageStackLayout, new Rectangle(0.5, 0.5, AutoSize, AutoSize));
            
        Children.Add(ContentView);
        Children.Add(GrayOutOverlay);
        Children.Add(ActivityIndicatorAndMessageStackLayout);
        // ReSharper disable once VirtualMemberCallInConstructor
        ActivityIndicatorOn = false;
    }
    #endregion

    #region IHaveActivityIndicator implementation
    public async Task WaitForPageToBecomeActiveAsync()
    {
        while(ContentView == null) await Task.Delay(25);
    }

    public virtual bool ActivityIndicatorOn
    {
        get => _activityIndicatorOn;
        set
        {
            _activityIndicatorOn = value;
            ActivityIndicator.IsEnabled = ActivityIndicator.IsRunning = ActivityIndicator.IsVisible = GrayOutOverlay.IsEnabled = GrayOutOverlay.IsVisible = value;
            MessageLabel.IsVisible = value;
        }
    }
    private bool _activityIndicatorOn;

    public virtual string Message 
    {
        get => _message;
        set => _message = MessageLabel.Text = value;
    }
    private string _message;

    #endregion

    #region Properties
    public BoxView GrayOutOverlay { get; set; }
    public StackLayout ActivityIndicatorAndMessageStackLayout { get; set; }
    public ActivityIndicator ActivityIndicator { get; set; }
    public Label MessageLabel { get; set; }
        
    public TContentView ContentView { get; set; }
    #endregion
}