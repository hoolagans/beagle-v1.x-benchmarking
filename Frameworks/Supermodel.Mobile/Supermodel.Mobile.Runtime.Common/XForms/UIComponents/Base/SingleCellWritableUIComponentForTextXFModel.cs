using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public abstract class SingleCellWritableUIComponentForTextXFModel : SingleCellWritableUIComponentXFModel, IHaveTextProperty
{
    #region ICustomMapper implemtation
    public override Task MapFromCustomAsync<T>(T other)
    {
        return SingleCellUIComponentForTextXFModelCommonLib.MapFromCustomAsync(this, other);
    }

    public override Task<T> MapToCustomAsync<T>(T other)
    {
        return SingleCellUIComponentForTextXFModelCommonLib.MapToCustomAsync(this, other);
    }
    #endregion

    #region Properties
    public abstract string Text { get; set; }
    public override object WrappedValue => Text;
    #endregion
}