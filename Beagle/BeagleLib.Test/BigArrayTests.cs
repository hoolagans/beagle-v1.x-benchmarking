//using BeagleLib.Util;

//namespace BeagleLib.Test;

//public class BigArrayTests
//{
//    [Test] public void TestBigArray1()
//    {
//        const long size = (long)int.MaxValue * 2 + 2;
//        var arr = new BigArray<long>(size);

//        for (long i = 0; i < arr.Length; i++)
//        {
//            arr[i] = -i;
//        }

//        for (long i = 0; i < size; i++)
//        {
//            Assert.That(arr[i] == -(long)i, $"i = {i}, arr[i] = {arr[i]}, -(long)i = {-(long)i}");
//        }
//    }
//    [Test] public void TestBigArray2()
//    {
//        const long size = (long)int.MaxValue * 2;
//        var arr = new BigArray<long>(size);

//        for (long i = 0; i < arr.Length; i++)
//        {
//            arr[i] = -i;
//        }

//        for (long i = 0; i < size; i++)
//        {
//            Assert.That(arr[i] == -i, $"i = {i}, arr[i] = {arr[i]}, -(long)i = {-i}");
//        }
//    }
//    [Test] public void TestBigArray3()
//    {
//        const long size = (long)int.MaxValue * 2;
//        var arr = new BigArray<long>(size);

//        Assert.Throws<IndexOutOfRangeException>(() => arr[-1] = 5);
//        Assert.Throws<IndexOutOfRangeException>(() => _ =arr[-1]);
//    }
//}