using System;
using System.Collections.Generic;
using System.Linq;

namespace Supersonic.Tran;

public class SupersonicListUpdateTransaction<TItem> : IDisposable where TItem : class
{
    #region Constructors
    public SupersonicListUpdateTransaction(SupersonicList<TItem> supersonicList, params TItem[] updateItems)
    {
        SupersonicList = supersonicList;
        UpdateItems = updateItems;
        ItemIndexOfAtIndexes = new Dictionary<string, int[]>();
        foreach (var index in SupersonicList.Indexes)
        {
            ItemIndexOfAtIndexes.Add(index.Key, new int[UpdateItems.Length]);
            for (var i = 0; i < UpdateItems.Length; i++)
            {
                ItemIndexOfAtIndexes[index.Key][i] = index.Value.IndexOf(UpdateItems[i], out var itemFound);
                if (!itemFound) throw new Exception($"Item with Guid {SupersonicList.GetGuid(UpdateItems[i])} not found in index {index.Key}");
            }
        }

        //We want to remove items from the list from bigger to smaller, so that index of bigger does not change when removing smaller
        foreach (var index in SupersonicList.Indexes)
        {
            ItemIndexOfAtIndexes[index.Key] = ItemIndexOfAtIndexes[index.Key].OrderByDescending(x => x).ToArray();
        }
    }
    #endregion

    #region IDisposable impementation
    public void Dispose()
    {
        //Remove from all indexes
        foreach (var index in SupersonicList.Indexes)
        {
            foreach (var idx in ItemIndexOfAtIndexes[index.Key])
            {
                index.Value.RemoveAt(idx);
            }
        }

        //Add to all indexes
        foreach (var index in SupersonicList.Indexes)
        {
            foreach (var updateItem in UpdateItems)
            {
                index.Value.Add(updateItem);
            }
        }
    }
    #endregion

    #region Properties
    protected SupersonicList<TItem> SupersonicList { get; set; }
    protected TItem[] UpdateItems { get; set; }
    protected Dictionary<string, int[]> ItemIndexOfAtIndexes { get; set; }
    #endregion
}