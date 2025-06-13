//TODO: finish this class later if needed
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading;

//namespace Supersonic.Concurrent
//{
//    public class ConcurrentSupersonicList<ItemT> : /*IQueryable<ItemT>,*/ IDisposable where ItemT : class
//    {
//        #region Constructors
//        public ConcurrentSupersonicList()
//        {
//            List = new SupersonicList<ItemT>();
//        }
//        public ConcurrentSupersonicList(IEnumerable<ItemT> items)
//        {
//            List = new SupersonicList<ItemT>(items);
//        }
//        #endregion

//        #region Overrides
//        public void Clear()
//        {
//            using (new WriteLock(RWLock)) List.Clear();
//        }
//        public ItemT this[int index]
//        {
//            get { using (new ReadLock(RWLock)) return List[index]; }
//            set { using (new WriteLock(RWLock)) List[index] = value; }
//        }
//        public ItemT GetByGuid(Guid guid)
//        {
//            using (new ReadLock(RWLock)) return List.GetByGuid(guid);
//        }
//        public ItemT GetByGuidOrDefault(Guid guid)
//        {
//            using (new ReadLock(RWLock)) return List.GetByGuidOrDefault(guid);
//        }
//        public bool Contains(ItemT item)
//        {
//            using (new ReadLock(RWLock)) return List.Contains(item);
//        }
//        public bool Contains(Guid guid)
//        {
//            using (new ReadLock(RWLock)) return List.Contains(guid);            
//        }
//        public void Add(ItemT item)
//        {
//            using (new WriteLock(RWLock)) List.Add(item);
//        }
//        public void Remove(ItemT item)
//        {
//            using (new WriteLock(RWLock)) List.Remove(item);
//        }
//        public void Remove(Guid guid)
//        {
//            using (new WriteLock(RWLock)) List.Remove(guid);
//        }
//        public int Count { get { using (new ReadLock(RWLock)) return List.Count; } }

//        public void AddIndex(string name, params Expression<Func<ItemT, object>>[] indexProperties)
//        {
//            using (new WriteLock(RWLock)) List.AddIndex(name, indexProperties);
//        }
//        public void AddUniqueIndex(string name, params Expression<Func<ItemT, object>>[] indexProperties)
//        {
//            using (new WriteLock(RWLock)) List.AddUniqueIndex(name, indexProperties);
//        }
//        public void DropIndex(string name)
//        {
//            using (new WriteLock(RWLock)) List.DropIndex(name);
//        }
//        public void RebuildAllIndexes()
//        {
//            using (new WriteLock(RWLock)) List.RebuildAllIndexes();
//        }
//        public void RebuildIndex(string name)
//        {
//            using (new WriteLock(RWLock)) List.RebuildIndex(name);
//        }
//        public void RebuildClusteredIndex()
//        {
//            using (new WriteLock(RWLock)) List.RebuildClusteredIndex();
//        }
//        public void DisableAllIndexes()
//        {
//            using (new WriteLock(RWLock)) List.DisableAllIndexes();
//        }
//        public void EnableAllIndexes()
//        {
//            using (new WriteLock(RWLock)) List.EnableAllIndexes();
//        }

//        #endregion

//        #region IDisposable implemetation
//        public void Dispose()
//        {
//            RWLock?.Dispose();
//        }
//        #endregion

//        #region Properties
//        private ReaderWriterLockSlim RWLock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
//        private SupersonicList<ItemT> List { get; }
//        #endregion
//    }
//}
