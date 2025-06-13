namespace BeagleLib.MathStackLib;

public static class ListExt
{
    public static IEnumerable<T> PopLast<T>(this List<T> list, int count)
    {
        var lastItems = list.TakeLast(count).ToList();
        list.RemoveRange(list.Count - count, count);
        return lastItems;
    }
}