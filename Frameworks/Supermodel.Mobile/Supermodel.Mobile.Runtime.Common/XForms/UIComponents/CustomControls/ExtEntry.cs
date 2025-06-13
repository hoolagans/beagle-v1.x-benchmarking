using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;

public class ExtEntry : Entry
{
    #region Contructors
    public ExtEntry()
    {
        Border = true;
    }
    #endregion
        
    #region Properties
    public TextAlignment TextAlignment
    {
        get => _textAlign;
        set
        {
            if ( value == _textAlign) return;
            _textAlign = value;
            OnPropertyChanged();
        }
    }
    private TextAlignment _textAlign;
        
    public bool Border
    {
        get => _border;
        set
        {
            if ( value == _border) return;
            _border = value;
            OnPropertyChanged();
        }
    }
    private bool _border;
    #endregion
}