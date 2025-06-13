using System;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.ReflectionMapper;
using Supermodel.Mobile.Runtime.Common.UnitOfWork;

namespace Supermodel.Mobile.Runtime.Common.Repository;

public static class RepoFactory
{
    public static IDataRepo<TModel> Create<TModel>() where TModel : class, IModel, new()
    {
        return UnitOfWorkContextCore.CurrentDataContext.CreateRepo<TModel>();
    }
    public static object CreateForRuntimeType(Type modelType)
    {
        return ReflectionHelper.ExecuteStaticGenericMethod(typeof(RepoFactory), "Create", new[] { modelType });
    }
}