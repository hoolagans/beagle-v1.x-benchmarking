using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebMonk.HttpRequestHandlers;

public interface IHttpRequestHandler
{
    #region Embedded Types
    //https://exceptionnotfound.net/asp-net-mvc-demystified-actionresults/
    //https://blog.eduonix.com/web-programming-tutorials/learn-different-types-of-action-results-in-mvc/

    public class HttpRequestHandlerResult
    {
        #region Constructors
        public HttpRequestHandlerResult(bool success, Func<Task>? executeResultFuncAsync) : this(success)
        {
            if (success && executeResultFuncAsync == null) throw new ArgumentException("success && executeResultFuncAsync == null");
            if (!success && executeResultFuncAsync != null) throw new ArgumentException("!success && executeResultFuncAsync != null");
            ExecuteResultFuncAsync = executeResultFuncAsync;
        }
        protected HttpRequestHandlerResult(bool success)
        {
            Success = success;
        }
        #endregion

        #region Overrides
        public virtual Task ExecuteResultAsync()
        {
            if (Success && ExecuteResultFuncAsync != null) return ExecuteResultFuncAsync.Invoke();
            else return Task.CompletedTask;
        }
        #endregion

        #region Properties
        public bool Success { get; }
        protected Func<Task>? ExecuteResultFuncAsync { get; }
        public static HttpRequestHandlerResult False { get; } = new(false, null);
        #endregion
    }
    #endregion

    #region Methods
    Task<HttpRequestHandlerResult> TryExecuteHttpRequestAsync(CancellationToken cancellationToken);
    #endregion

    #region Properties
    int Priority { get; }
    bool SaveSessionState { get; }
    #endregion
}