namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public interface IWritableUIComponentXFModel : IReadOnlyUIComponentXFModel
{
    string ErrorMessage { get; set; }
    bool Required { get; set; }
    object WrappedValue { get; }
}