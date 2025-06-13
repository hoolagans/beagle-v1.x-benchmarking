using Microsoft.EntityFrameworkCore;
using Supermodel.Persistence.Repository;

namespace Supermodel.Persistence.EFCore.SQLServer;

public abstract class EFCoreSQLServerDataContext : EFCoreDataContext
{
    #region Constructors
    protected EFCoreSQLServerDataContext(string connectionString, IRepoFactory? customRepoFactory = null) 
        : base(connectionString, customRepoFactory){ }
    #endregion

    #region Overrides
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(ConnectionString)
            .UseLazyLoadingProxies()                
            .EnableSensitiveDataLogging();
    }
    #endregion
}