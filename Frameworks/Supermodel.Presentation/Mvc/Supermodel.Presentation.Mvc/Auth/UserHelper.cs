using System.Threading.Tasks;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.Mvc.Context;

namespace Supermodel.Presentation.Mvc.Auth;

public static class UserHelper
{
    #region Methods
    public static long? GetCurrentUserId()
    {
        return RequestHttpContext.CurrentUserId;
    }
    public static string? GetCurrentUserLabel()
    {
        return RequestHttpContext.CurrentUserLabel;
    }

    public static async Task<TUserEntity?> GetCurrentUserAsync<TUserEntity, TDataContext>()
        where TDataContext : class, IDataContext, new()
        where TUserEntity : UserEntity<TUserEntity, TDataContext>, new()
    {
        await using(new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            var repo = RepoFactory.Create<TUserEntity>();
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return null;
            return await repo.GetByIdAsync((long)currentUserId);
        }
    }
    #endregion
}