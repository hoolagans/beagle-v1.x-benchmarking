 using System.Threading.Tasks;
 using Supermodel.ReflectionMapper;

 namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base; 

 public abstract class SingleCellWritableUIComponentXFModel : SingleCellWritableUIComponentWithoutBackingXFModel, IRMapperCustom
 {
     #region ICustomMapper implementation
     public abstract Task MapFromCustomAsync<T>(T other);
     public abstract Task<T> MapToCustomAsync<T>(T other);
     #endregion
 }