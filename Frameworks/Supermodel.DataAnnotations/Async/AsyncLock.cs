using System;
using System.Threading;
using System.Threading.Tasks;

namespace Supermodel.DataAnnotations.Async;

public class AsyncLock
{
    #region Embedded Types
    public struct Releaser : IDisposable
    {
        #region Contructors
        internal Releaser(AsyncLock toRelease) { _toRelease = toRelease; }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            if (_toRelease != null) _toRelease._semaphore.Release();
        }
        #endregion

        #region Attributes
        private readonly AsyncLock _toRelease;
        #endregion
    }
    #endregion

    #region Constructors
    public AsyncLock()
    {
        _semaphore = new AsyncSemaphore(1);
        _releaser = Task.FromResult(new Releaser(this));
    }
    #endregion

    #region Methods
    public Task<Releaser> LockAsync()
    {
        var wait = _semaphore.WaitAsync();
        return wait.IsCompleted ?
            _releaser :
            wait.ContinueWith((_, state) => new Releaser((AsyncLock)state), this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
    #endregion

    #region Attributes
    private readonly AsyncSemaphore _semaphore;
    private readonly Task<Releaser> _releaser;
    #endregion
}