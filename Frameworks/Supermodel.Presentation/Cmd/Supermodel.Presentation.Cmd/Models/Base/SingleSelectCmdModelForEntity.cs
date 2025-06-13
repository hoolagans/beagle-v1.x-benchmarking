using System.Globalization;
using System.Threading.Tasks;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Presentation.Cmd.Extensions;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Cmd.Models.Base;

public abstract class SingleSelectCmdModelForEntity : SingleSelectCmdModel
{
    #region ICustomMapper implemtation
#nullable disable
    public override Task MapFromCustomAsync<T>(T other)
    {
        var otherType = typeof(T);
        if (!otherType.IsEntityType()) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {otherType.Name}");

        var entity = (IEntity)other;
        SelectedValue = entity?.Id.ToString(CultureInfo.InvariantCulture) ?? "";

        return Task.CompletedTask;
    }
    public override async Task<T> MapToCustomAsync<T>(T other)
    {
        var otherType = typeof(T);
        if (!otherType.IsEntityType()) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {otherType.Name}");

        if (string.IsNullOrEmpty(SelectedValue)) return (T)(object)null;

        var id = long.Parse(SelectedValue);
        var entity = (IEntity)other;
        if (entity != null && entity.Id == id) return (T)entity;

        var repo = RepoFactory.CreateForRuntimeType(otherType);
        var newEntity = await repo.GetIEntityByIdAsync(id).ConfigureAwait(false);

        return (T)newEntity;        
    }

    #endregion
}