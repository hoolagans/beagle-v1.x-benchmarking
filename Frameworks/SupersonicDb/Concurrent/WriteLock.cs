using System;
using System.Threading;

namespace Supersonic.Concurrent;

internal class WriteLock : IDisposable
{
    #region Constructors
    public WriteLock(ReaderWriterLockSlim rwLock)
    {
        RWLock = rwLock;
        RWLock.EnterWriteLock();
    }
    #endregion

    #region IDisposable implementation
    public void Dispose()
    {
        RWLock.ExitWriteLock();
    }
    #endregion

    #region Properties
    public ReaderWriterLockSlim RWLock { get; }
    #endregion
}