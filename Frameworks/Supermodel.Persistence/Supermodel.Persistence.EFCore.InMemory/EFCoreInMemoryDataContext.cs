using Microsoft.EntityFrameworkCore;
using Supermodel.Persistence.Repository;

namespace Supermodel.Persistence.EFCore.InMemory;

public abstract class EFCoreInMemoryDataContext : EFCoreDataContext
{
    #region Constructors
    protected EFCoreInMemoryDataContext(string databaseName, IRepoFactory? customRepoFactory = null) : base(databaseName, customRepoFactory){}
    #endregion

    #region Overrides
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase(ConnectionString)
            .UseLazyLoadingProxies()
            .EnableSensitiveDataLogging();
    }
    #endregion
}