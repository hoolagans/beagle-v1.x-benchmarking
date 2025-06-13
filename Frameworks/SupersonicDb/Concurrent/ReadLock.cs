using System;
using System.Threading;

namespace Supersonic.Concurrent;

internal class ReadLock : IDisposable
{
    #region Constructors
    public ReadLock(ReaderWriterLockSlim rwLock)
    {
        RWLock = rwLock;
        RWLock.EnterReadLock();
    }
    #endregion

    #region IDisposable implementation
    public void Dispose()
    {
        RWLock.ExitReadLock();
    }
    #endregion

    #region Properties
    public ReaderWriterLockSlim RWLock { get; }
    #endregion
}