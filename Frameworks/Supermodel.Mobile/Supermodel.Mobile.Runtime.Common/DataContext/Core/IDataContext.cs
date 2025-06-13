namespace Supermodel.Mobile.Runtime.Common.DataContext.Core;

using System;
using Models;
using Repository;
using System.Collections.Generic;
    
public interface IDataContext : IAsyncDisposable
{
    #region Configuration
    bool CommitOnDispose { get; set; }
    bool IsReadOnly { get; }
    void MakeReadOnly();
    #endregion

    #region Context RepoFactory
    IDataRepo<TModel> CreateRepo<TModel>() where TModel : class, IModel, new();
    #endregion

    #region CustomValues
    Dictionary<string, object> CustomValues { get; } 
    #endregion
}