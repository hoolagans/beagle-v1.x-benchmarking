using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Supersonic.Linq;

internal class IndexedListQueryProvider<TItem> : IQueryProvider where TItem : class
{
    #region Constructors
    public IndexedListQueryProvider(SupersonicList<TItem> supersonicList)
    {
        SupersonicList = supersonicList;
    }
    #endregion

    #region Methods
    public IQueryable CreateQuery(Expression expression)
    {
        try
        {
            //return new IndexedList<ItemT>(this, expression);
            return (IQueryable)Activator.CreateInstance(typeof(SupersonicList<>).MakeGenericType(expression.Type), this, expression);
        }
        catch (TargetInvocationException ex)
        {
            // ReSharper disable once PossibleNullReferenceException
            throw ex.InnerException;
        }
    }
    public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
    {
        try
        {
            //return new IndexedList<ResultT>(this, expression);
            return (IQueryable<TResult>)Activator.CreateInstance(typeof(SupersonicList<>).MakeGenericType(typeof(TResult)), this, expression);
        }
        catch (TargetInvocationException ex)
        {
            // ReSharper disable once PossibleNullReferenceException
            throw ex.InnerException;
        }
    }
    public object Execute(Expression expression)
    {
        return IndexedListQueryContext<TItem>.Execute(SupersonicList, expression, false);
    }
    public TResult Execute<TResult>(Expression expression)
    {
        var isEnumerable = typeof(TResult).Name == "IEnumerable`1";
        return (TResult)IndexedListQueryContext<TItem>.Execute(SupersonicList, expression, isEnumerable);
    }
    #endregion

    #region Properties
    public SupersonicList<TItem> SupersonicList { get; }
    #endregion
}