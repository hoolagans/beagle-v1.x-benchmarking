using System;
using Microsoft.EntityFrameworkCore.Storage;
using Supermodel.Persistence.DataContext;

namespace Supermodel.Persistence.EFCore;

public class EFCoreTransaction : IDataContextTransaction
{
    #region Constructors
    public EFCoreTransaction(IDbContextTransaction transaction)
    {
        Transaction = transaction;
    }
    #endregion

    #region Methods
    public void Dispose()
    {
        Transaction.Dispose();
    }

    public void Commit()
    {
        Transaction.Commit();
    }

    public void Rollback()
    {
        Transaction.Rollback();
    }
    #endregion

    #region Properties
    public Guid TransactionGuid => Transaction.TransactionId;
    protected IDbContextTransaction Transaction { get; set; }
    #endregion
}