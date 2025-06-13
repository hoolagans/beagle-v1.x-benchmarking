using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.Presentation.Cmd.ConsoleOutput;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Cmd.Models;

public abstract class CmdModelForEntityCore : CmdModel, IRMapperCustom
{
    #region Methods
    public bool IsNewModel() => Id == 0;
    #endregion

    #region IRMapperCustom implementation
    public virtual Task MapFromCustomAsync<T>(T other)
    {
        return this.MapFromCustomBaseAsync(other);
    }
    public virtual Task<T> MapToCustomAsync<T>(T other)
    {
        return this.MapToCustomBaseAsync(other);
    }
    #endregion

    #region Standard Properties for Mvc Models
    [ScaffoldColumn(false)] public virtual long Id { get; set; }
 
    [ScaffoldColumn(false), NotRMapped] public virtual StringWithColor Label => new(LabelInternal);
    [ScaffoldColumn(false), NotRMapped] protected abstract string LabelInternal { get; }
        
    [ScaffoldColumn(false), NotRMapped] public virtual bool IsDisabled => false;
    #endregion
}