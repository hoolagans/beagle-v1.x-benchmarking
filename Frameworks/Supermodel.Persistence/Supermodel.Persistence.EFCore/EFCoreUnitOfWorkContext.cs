using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Supermodel.Persistence.UnitOfWork;

namespace Supermodel.Persistence.EFCore;

public class EFCoreUnitOfWorkContext : UnitOfWorkContext
{
    #region Constructors
    protected EFCoreUnitOfWorkContext(){ }
    #endregion

    #region Properties
    public static DatabaseFacade Database => ((DbContext)UnitOfWorkContextCore.CurrentDataContext).Database;
    public static bool LoadReadOnlyEntitiesAsNoTracking
    {
        get => ((EFCoreDataContext)UnitOfWorkContextCore.CurrentDataContext).LoadReadOnlyEntitiesAsNoTracking;
        set => ((EFCoreDataContext)UnitOfWorkContextCore.CurrentDataContext).LoadReadOnlyEntitiesAsNoTracking = value;
    }
    public static bool ValidateOnSaveEnabled
    {
        get => ((EFCoreDataContext)UnitOfWorkContextCore.CurrentDataContext).ValidateOnSaveEnabled;
        set => ((EFCoreDataContext)UnitOfWorkContextCore.CurrentDataContext).ValidateOnSaveEnabled = value;
    }
    #endregion
}