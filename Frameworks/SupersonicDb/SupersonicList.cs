using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Supersonic.BNF;
using Supersonic.GC;
using Supersonic.IndexApi;
using Supersonic.Linq;
using Supersonic.Tran;

namespace Supersonic;

public class SupersonicList<TItem> : IQueryable<TItem> where TItem : class
{
    #region EmbeddedTypes
    public class ClustedIndexComparer<T> : IComparer<T> where T : TItem
    {
        #region Constructors
        public ClustedIndexComparer()
        {
            IsIIndexedListItem = typeof(ISupersonicListItem).IsAssignableFrom(typeof(TItem));
        }
        #endregion

        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(T x, T y)
        {
            return Comparer.Default.Compare(GetGuid(x),  GetGuid(y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid GetGuid(TItem item)
        {
            return IsIIndexedListItem ? ((ISupersonicListItem)item).Guid : item.GetRefGuid();
        }
        #endregion

        #region Properties
        protected bool IsIIndexedListItem { get; }
        #endregion
    }
    #endregion

    #region Constructors
    public SupersonicList()
    {
        SetUpIsIIndexedListItem();

        ClusteredIndex = new List<TItem>();

        Provider = new IndexedListQueryProvider<TItem>(this);
        Expression = Expression.Constant(this);
        SetUpAttributeBasedIndexes();
    }
    public SupersonicList(IEnumerable<TItem> items)
    {
        SetUpIsIIndexedListItem();

        ClusteredIndex = items.ToList();

        Provider = new IndexedListQueryProvider<TItem>(this);
        Expression = Expression.Constant(this);
        SetUpAttributeBasedIndexes();
    }
    public SupersonicList(IQueryProvider provider, Expression expression)
    {
        SetUpIsIIndexedListItem();

        ClusteredIndex = new List<TItem>();

        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        if (!typeof(IQueryable<TItem>).IsAssignableFrom(expression.Type)) throw new ArgumentOutOfRangeException(nameof(expression));
        SetUpAttributeBasedIndexes();
    }
    private void SetUpAttributeBasedIndexes()
    {
        //Read index definitions from the attributes
        var indexDefs = new List<IndexDefinition>();

        foreach (var propertyInfo in typeof(TItem).GetProperties())
        {
            var attributes = propertyInfo.GetCustomAttributes(true);
            foreach (var attr in attributes)
            {
                if (attr is ListIndexAttribute listIndexAttr)
                {
                    IndexDefinition indexDefinition;

                    if (listIndexAttr.Name == null || indexDefs.All(x => x.Name == null || x.Name != listIndexAttr.Name))
                    {
                        indexDefinition = new IndexDefinition { Name = listIndexAttr.Name, IsUnique = listIndexAttr.IsUnique };
                        indexDefs.Add(indexDefinition);
                    }
                    else
                    {
                        indexDefinition = indexDefs.Single(x => x.Name == listIndexAttr.Name);
                    }
                    if (indexDefinition.IsUnique != listIndexAttr.IsUnique) throw new Exception($"Index {listIndexAttr.Name} IsUnique property is inconsistent across the index properties.");
                    indexDefinition.Props.Add(new IndexDefinition.PropDefinition { Name = propertyInfo.Name, Order = listIndexAttr.Order } );
                }
            }
        }

        //Create Indexes 
        Parallel.ForEach(indexDefs, indexDef =>
        {
            if (indexDef.Name == null) indexDef.Name = indexDef.GenerateIndexNameBasedOnIndexDefinition();
            var index = Index<TItem>.CreateFromDefinition(this, indexDef);
            lock (Indexes) { Indexes.Add(indexDef.Name, index); }
        });

        //Rebuild all new indexes
        RebuildAllIndexes();
    }
    private void SetUpIsIIndexedListItem()
    {
        IsIIndexedListItem = typeof(ISupersonicListItem).IsAssignableFrom(typeof(TItem));
    }
    #endregion

    #region Query Methods
    public IEnumerable<TItem> GetItems()
    {
        return ClusteredIndex.AsEnumerable();
    }
    public IEnumerable<TItem> WhereWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constants
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.WhereWithIndex(condition);
    }
    public IEnumerable<TItem> WhereWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].WhereWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CountWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constants
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.CountWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CountWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].CountWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AnyWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constatnts
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.AnyWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AnyWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].AnyWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constatnts
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.AllWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].AllWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem SingleWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constatnts
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.SingleWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem SingleWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].SingleWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem SingleOrDefaultWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constatnts
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.SingleOrDefaultWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem SingleOrDefaultWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].SingleOrDefaultWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem FirstWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constatnts
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.FirstWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem FirstWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].FirstWithIndex(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem FirstOrDefaultWithIndex(Expression<Func<TItem, bool>> predicate)
    {
        //Evaluate variables into constatnts
        predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

        //Find matching index
        var success = TryFindIndexAndCondition(predicate.Body, out var index, out var condition);
        if (!success) throw new InvalidProgramException("Unable to find index matching the query");

        return index.FirstOrDefaultWithIndex(condition);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TItem FirstOrDefaultWithIndex(string indexName, Expression<Func<TItem, bool>> predicate)
    {
        return Indexes[indexName].FirstOrDefaultWithIndex(predicate);
    }

    public int ClusteredIndexOf(Guid guid, out bool itemFound, int? startIdx = null, int? endIdx = null)
    {
        if (Count == 0)
        {
            itemFound = false;
            return 0;
        }

        if (startIdx == null) startIdx = 0;
        if (endIdx == null) endIdx = Count - 1;

        var startResult = Comparer.Default.Compare(guid, GetGuid(ClusteredIndex[startIdx.Value]));
        if (startResult == 0)
        {
            itemFound = true;
            return startIdx.Value;
        }
        if (startResult < 0)
        {
            itemFound = false;
            return startIdx.Value;
        }

        var endResult = Comparer.Default.Compare(guid, GetGuid(ClusteredIndex[endIdx.Value]));
        if (endResult == 0)
        {
            itemFound = true;
            return endIdx.Value;
        }
        if (endResult > 0)
        {
            itemFound = false;
            return endIdx.Value + 1;
        }

        var midIdx = (int)(startIdx + endIdx) / 2;
        var midIdxGuid = GetGuid(ClusteredIndex[midIdx]);

        var midResult = Comparer.Default.Compare(guid, midIdxGuid);
        if (midResult == 0)
        {
            itemFound = true;
            return midIdx;
        }
            
        if (midIdx == startIdx)
        {
            itemFound = false;
            return midIdx + 1;
        }

        if (midResult > 0) return ClusteredIndexOf(guid, out itemFound, midIdx + 1, endIdx - 1);
        else return ClusteredIndexOf(guid, out itemFound, startIdx + 1, midIdx - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(TItem item)
    {
        return IndexOf(GetGuid(item));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(Guid guid)
    {
        var result = ClusteredIndexOf(guid, out var matchFound);
        if (!matchFound) return -1;
        return result;
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
        return $"Supersonic List. Count = {Count}";
    }
    #endregion

    #region Methods
    //TODO: remove this method. It is only used for testing
    public void PrintIndexesContents()
    {
        Console.WriteLine("Clustered Index");
        Console.WriteLine();
        foreach (var item in ClusteredIndex)
        {
            Console.WriteLine(item);
        }
        Console.WriteLine();

        foreach (var keyValuePair in Indexes)
        {
            Console.WriteLine($"Index: {keyValuePair.Key}");
            Console.WriteLine();
            keyValuePair.Value.PrintOrderedList();
            Console.WriteLine();
        }
    }

    public void Clear()
    {
        ClusteredIndex.Clear();
        foreach (var index in Indexes) index.Value.Clear();
    }

    public TItem this[int index]
    {
        get => ClusteredIndex[index];
        set => ClusteredIndex[index] = value;
    }

    public TItem GetByGuid(Guid guid)
    {
        var idx = ClusteredIndexOf(guid, out var itemFound);
        if (!itemFound) throw new ArgumentException($"Item with ID provided {guid} does not exist in the list", nameof(guid));
        return ClusteredIndex[idx];
    }
    public TItem GetByGuidOrDefault(Guid guid)
    {
        var idx = ClusteredIndexOf(guid, out var itemFound);
        if (!itemFound) return default(TItem);
        return ClusteredIndex[idx];
    }

    public bool Contains(TItem item)
    {
        return Contains(GetGuid(item));
    }
    public bool Contains(Guid guid)
    {
        ClusteredIndexOf(guid, out var itemFound);
        return itemFound;
    }

    public void Add(TItem item)
    {
        var idx = ClusteredIndexOf(GetGuid(item), out var itemFound);
        if (itemFound) throw new Exception($"Item with ID {GetGuid(item)} cannot be added becasue it already exists in the {nameof(SupersonicList<TItem>)}");
        ClusteredIndex.Insert(idx, item);

        if (AllIndexesDisabled)
        {
            foreach (var index in Indexes) index.Value.Add(item);
        }
        else
        {
            Parallel.ForEach(Indexes, index => index.Value.Add(item));
        }
    }

    public void Remove(TItem item)
    {
        Remove(GetGuid(item));
    }
    public void Remove(Guid guid)
    {
        var idx = ClusteredIndexOf(guid, out var itemFound);
        if (!itemFound) throw new Exception($"Item with GuiD {guid} cannot be removed becasue itdoes not exist in the {nameof(SupersonicList<TItem>)}");
        var item = ClusteredIndex[idx];
        ClusteredIndex.RemoveAt(idx);
        foreach (var index in Indexes) index.Value.Remove(item);
    }

    public int Count => ClusteredIndex.Count;

    public void AddIndex(string name, params Expression<Func<TItem, object>>[] indexProperties)
    {
        using (new SustainedLowLatencyGC())
        {
            if (name.Contains('#')) throw new ArgumentException("Index name may not contain '#'", nameof(name));
            if (Indexes.ContainsKey(name)) throw new ArgumentException($"Index with name {name} already exists", nameof(name));
            var index = Index<TItem>.Create(this, name, indexProperties);
            Indexes.Add(name, index);
        }
    }
    public void AddUniqueIndex(string name, params Expression<Func<TItem, object>>[] indexProperties)
    {
        using (new SustainedLowLatencyGC())
        {
            if (name.Contains('#')) throw new ArgumentException("Index name may not contain '#'", nameof(name));
            if (Indexes.ContainsKey(name)) throw new ArgumentException($"Index with name {name} already exists", nameof(name));
            var index = Index<TItem>.CreateUnique(this, name, indexProperties);
            Indexes.Add(name, index);
        }
    }
    public void DropIndex(string name)
    {
        if (name.Contains('#')) throw new ArgumentException("Index name may not contain '#'", nameof(name));
        if (!Indexes.ContainsKey(name)) throw new Exception($"Index with name {name} cannot be dropped because it does not exist");
        Indexes.Remove(name);
    }
    public void RebuildAllIndexes(bool includeClusteredIndex = true)
    {
        using (new SustainedLowLatencyGC())
        {
            if (SupersonicListSettings.RebuildIndexeslExecutionOrder == SupersonicListSettings.ExecutionOrder.Parallel)
            {
                Parallel.For(-1, Indexes.Count, i =>
                {
                    if (i == -1)
                    {
                        if (includeClusteredIndex) RebuildClusteredIndex();
                    }
                    else
                    {
                        Indexes.ElementAt(i).Value.Rebuild();
                    }
                });
            }
            else
            {
                if (includeClusteredIndex) RebuildClusteredIndex();
                foreach (var index in Indexes) index.Value.Rebuild();
            }
        }
    }
    public void RebuildIndex(string name)
    {
        using (new SustainedLowLatencyGC())
        {
            if (name.Contains('#')) throw new ArgumentException("Index name may not contain '#'", nameof(name));
            if (!Indexes.ContainsKey(name)) throw new ArgumentException($"Index with name {name} does not exist", nameof(name));
            Indexes[name].Rebuild();
        }
    }
    public void RebuildClusteredIndex()
    {
        using (new SustainedLowLatencyGC())
        {
            if (SupersonicListSettings.SortingMethod == SupersonicListSettings.SortMethod.Sort)
            {
                ClusteredIndex.Sort(new ClustedIndexComparer<TItem>());
            }
            else if (SupersonicListSettings.SortingMethod == SupersonicListSettings.SortMethod.ParallelOrderBy)
            {
                ClusteredIndex = ClusteredIndex.AsParallel().OrderBy(GetGuid).ToList();
            }
            else
            {
                ClusteredIndex = ClusteredIndex.OrderBy(GetGuid).ToList();
            }

            if (!IsClusteredIndexUnique()) throw new Exception("List countains objects with duplicate GUIDs");
        }
    }
    public void DisableAllIndexes()
    {
        foreach (var index in Indexes)
        {
            if (!index.Value.IsDisabled) index.Value.DisableIndex();
        }
    }
    public void EnableAllIndexes()
    {
        using (new SustainedLowLatencyGC())
        {
            if (SupersonicListSettings.RebuildIndexeslExecutionOrder == SupersonicListSettings.ExecutionOrder.Parallel)
            {
                Parallel.ForEach(Indexes, index =>
                {
                    if (index.Value.IsDisabled) index.Value.EnableIndex();
                });
            }
            else
            {
                foreach (var index in Indexes)
                {
                    if (index.Value.IsDisabled) index.Value.EnableIndex();
                }                    
            }

        }
    }

    public SupersonicListUpdateTransaction<TItem> CreateUpdateTran(params TItem[] updateItems)
    {
        return new SupersonicListUpdateTransaction<TItem>(this, updateItems);
    }

    internal bool TryFindIndexAndCondition(Expression expression, out Index<TItem> index, out Condition condition)
    {
        foreach (var indexedListIndex in Indexes)
        {
            index = indexedListIndex.Value;
            condition = index.ParseRootCondition(expression, out var errorMessage);
            if (errorMessage == null) return true;
        }

        index = null;
        condition = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Guid GetGuid(TItem item)
    {
        return IsIIndexedListItem ? ((ISupersonicListItem)item).Guid : item.GetRefGuid();
    }
    #endregion

    #region Helper Methods
    protected bool IsClusteredIndexUnique()
    {
        using (new SustainedLowLatencyGC())
        {
            if (ClusteredIndex.Count == 0) return true;
            var previousGuid = GetGuid(ClusteredIndex[0]);
            for (var i = 1; i < ClusteredIndex.Count; i++)
            {
                if (Comparer.Default.Compare(GetGuid(ClusteredIndex[i]), previousGuid) == 0) return false;
            }
            return true;
        }
    }
    #endregion

    #region IOrderedQueryable<T> implementation
    public IEnumerator<TItem> GetEnumerator()
    {
        // ReSharper disable once NotDisposedResourceIsReturned
        return Provider.Execute<IEnumerable<TItem>>(Expression).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        // ReSharper disable once NotDisposedResourceIsReturned
        return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
    }
    public Expression Expression { get; }
    public Type ElementType => typeof(TItem);
    public IQueryProvider Provider { get; }
    #endregion

    #region Properties
    protected List<TItem> ClusteredIndex { get; set; }
    internal Dictionary<string, Index<TItem>> Indexes { get; set; } = new();

    public bool AllIndexesDisabled => Indexes.Values.All(x => x.IsDisabled);

    protected bool IsIIndexedListItem { get; private set; }
    #endregion
}