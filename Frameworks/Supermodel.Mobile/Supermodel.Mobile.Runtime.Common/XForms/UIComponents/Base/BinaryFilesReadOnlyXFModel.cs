using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.ReflectionMapper;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public abstract class BinaryFilesReadOnlyXFModel : IReadOnlyUIComponentXFModel, IRMapperCustom
{
    #region Constructors
    protected BinaryFilesReadOnlyXFModel()
    {
        ModelsWithBinaryFileXFModels = new ObservableCollection<ModelWithBinaryFileXFModel>();
    }
    #endregion

    #region Custom Mapper implementation
    public virtual async Task MapFromCustomAsync<T>(T other)
    {
        if (!(other is IEnumerable<IModelWithBinaryFile>)) throw new SupermodelException(GetType().Name + " can only map to Lists of type that implements IModelWithBinaryFile");
        var modelsWithImage = (IEnumerable<IModelWithBinaryFile>)other;
            
        ModelsWithBinaryFileXFModels ??= new ObservableCollection<ModelWithBinaryFileXFModel>();
        ModelsWithBinaryFileXFModels.Clear();
        foreach (var modelWithImage in modelsWithImage) ModelsWithBinaryFileXFModels.Add(await new ModelWithBinaryFileXFModel().MapFromAsync(modelWithImage));
    }

    public virtual Task<T> MapToCustomAsync<T>(T other)
    {
        //its read-only, so we do nothing
        return Task.FromResult(other); 
    }
    #endregion

    #region Methods
    public abstract List<Cell> RenderDetail(Page parentPage, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue);
    #endregion

    #region Properties
    public ObservableCollection<ModelWithBinaryFileXFModel> ModelsWithBinaryFileXFModels { get; private set; }

    public abstract bool ShowDisplayNameIfApplies { get; set; }
    public abstract string DisplayNameIfApplies { get; set; }
    public abstract TextAlignment TextAlignmentIfApplies { get; set; }
    #endregion
}