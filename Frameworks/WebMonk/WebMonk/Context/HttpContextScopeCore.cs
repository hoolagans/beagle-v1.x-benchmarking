using System;
using System.Collections.Immutable;
using System.Linq;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.LogicalContext;

namespace WebMonk.Context;

public static class HttpContextScopeCore
{
    #region Methods and Properties
    public static HttpContext PopHttpContext()
    {
        try
        {
            _contextStackImmutable = _contextStackImmutable.Pop(out var context);
            return context;

        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("Stack is empty");
        }
    }
    public static void PushHttpContext(HttpContext context)
    {
        _contextStackImmutable = _contextStackImmutable.Push(context);
    }
    public static int StackCount => _contextStackImmutable.Count();
    public static HttpContext CurrentHttpContext
    {
        get
        {
            try
            {
                return _contextStackImmutable.Peek();
            }
            catch (InvalidOperationException)
            {
                throw new SupermodelException("Current HttpContext does not exist. All database access operations must be wrapped in 'using(new UnitOfWork())'");
            }
        }
    }
    #endregion

    #region Private variables
    // ReSharper disable once InconsistentNaming
    private static ImmutableStack<HttpContext> _contextStackImmutable
    {
        get
        {
            var contextStack = HttpCallContext.LogicalGetData(SupermodelDataContextStack) as ImmutableStack<HttpContext>;
            return contextStack ?? ImmutableStack.Create<HttpContext>();
        }
        set => HttpCallContext.LogicalSetData(SupermodelDataContextStack, value);
    }
    public const string SupermodelDataContextStack = "SupermodelDataContextStack";
    #endregion
}