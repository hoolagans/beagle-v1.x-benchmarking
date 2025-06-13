using System.Threading.Tasks;
using System;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using Xamarin.Forms;    
using Supermodel.ReflectionMapper;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class DateReadOnlyXFModel : SingleCellReadOnlyUIComponentForTextXFModel
{
    #region Constructors
    public DateReadOnlyXFModel()
    {
        TextLabel = new Label { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation, FontSize = XFormsSettings.LabelFontSize, TextColor = XFormsSettings.ValueTextColor };
        StackLayoutView.Children.Add(TextLabel);
    }
    #endregion

    #region ICstomMapper implementations
    public override Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
            
        if (other == null) Value = null;
        else Value = (DateTime)(object)other; 

        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        if (typeof(T) == typeof(DateTime) && Value == null) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1} because {0} is null but {1} is not nullable", GetType().Name, typeof(T).Name));
        other = (T)(object)Value; //This assignment does not do anything but we still do it for consistency
        return Task.FromResult(other);
    }
    #endregion

    #region Properties
    public DateTime? Value
    {
        get
        {
            if (DateTime.TryParse(Text, out var result)) return result;
            else return null;
        }
        set => Text = value == null ? "" : value.Value.ToString("d");
    }
    public override string Text
    {
        get => TextLabel.Text;
        set => TextLabel.Text = value;
    }
    public Label TextLabel { get; }
    public override TextAlignment TextAlignmentIfApplies
    {
        get => TextLabel.HorizontalTextAlignment;
        set => TextLabel.HorizontalTextAlignment = value;
    }
    #endregion
}