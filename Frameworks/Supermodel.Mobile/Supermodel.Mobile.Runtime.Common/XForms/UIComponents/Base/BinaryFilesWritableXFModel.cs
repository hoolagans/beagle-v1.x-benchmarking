using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Supermodel.ReflectionMapper;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public abstract class BinaryFilesWritableXFModel : BinaryFilesReadOnlyXFModel, IWritableUIComponentXFModel
{
    #region Custom Mapper implementation
    public override async Task<T> MapToCustomAsync<T>(T other)
    {
        var modelsWithImage = (IEnumerable<IModelWithBinaryFile>)other;

        // modelsWithImages == null during validation
        if (modelsWithImage != null)
        {
            //remove all
            modelsWithImage.ExecuteMethod("Clear");

            //add all the images for which an exact match was not found
            foreach (var modelWithBinaryFileXFModel in ModelsWithBinaryFileXFModels)
            {
                var modelsWithImageUnderlyingType = other.GetType().GenericTypeArguments[0];
                var modelWithImage = (IModelWithBinaryFile)ReflectionHelper.CreateType(modelsWithImageUnderlyingType);
                modelsWithImage.ExecuteMethod("Add", modelWithImage);

                //update BinaryFile
                await modelWithBinaryFileXFModel.MapToAsync(modelWithImage);
            }
        }
        return other;
    }
    #endregion

    #region Properties
    public abstract string ErrorMessage { get; set; }
    public abstract bool Required { get; set; }
    public object WrappedValue => ModelsWithBinaryFileXFModels.FirstOrDefault();
    #endregion
}