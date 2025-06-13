using System.Runtime.CompilerServices;

namespace BeagleLib.Util;

public class BigArray<T>
{
    #region Constructors
    public BigArray(long length)
    {
        checked
        {
            if (length > (long)BlockSize * int.MaxValue) throw new ArgumentOutOfRangeException(nameof(length));

            Length = length;

            long numBlocks = length / BlockSize;
            int lastBlockSize;
            if (numBlocks * BlockSize < length)
            {
                numBlocks += 1;
                lastBlockSize = (int)(length % BlockSize);
            }
            else
            {
                lastBlockSize = BlockSize;
            }

            _elements = new T[numBlocks][];
            for (int i = 0; i < numBlocks - 1; i++)
            {
                _elements[i] = GC.AllocateUninitializedArray<T>(BlockSize);
            }
            _elements[numBlocks - 1] = GC.AllocateUninitializedArray<T>(lastBlockSize);
        }
    }
    #endregion

    #region Properties and Indexer
    public long Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    public T this[long elementIdx]
    {
        // getter and setter must be very simple in order to ensure that they get inlined into their caller
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            checked
            {
                int blockNum = (int)(elementIdx >> BlockSizeLog2);
                int elementNumberInBlock = (int)(elementIdx & BlockSizeMinus1);
                return _elements[blockNum][elementNumberInBlock];
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            checked
            {
                int blockNum = (int)(elementIdx >> BlockSizeLog2);
                int elementNumberInBlock = (int)(elementIdx & BlockSizeMinus1);
                _elements[blockNum][elementNumberInBlock] = value;
            }
        }
    }
    #endregion

    #region Data and Consts
    private readonly T[][] _elements;

    private const int BlockSize = 1_073_741_824;
    private const int BlockSizeMinus1 = BlockSize - 1;
    private const int BlockSizeLog2 = 30;
    #endregion
}