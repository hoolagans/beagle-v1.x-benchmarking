using System.Diagnostics;

namespace BeagleLib.Util;

public static class Rnd
{
    #region Static Properties
    public static bool RandomBool() => Random.Next(2) == 0;
    public static bool RandomBoolWithChance(double percent) => Random.NextDouble() <= percent;

    public static float RandomDoubleOrHalf(float value)
    {
        switch (Random.Next(2))
        {
            case 0: return value * (1.0f + _random!.NextSingle());
            case 1: return value / (1.0f + _random!.NextSingle());
            default:
                Debug.Fail("RandomSign: Invalid divide/multiply decision");
                throw new Exception("RandomSign: Invalid divide/multiply decision");
        }
    }
    public static float RandomMul10OrDiv10(float value)
    {
        switch (Random.Next(2))
        {
            case 0: return value * (1.0f + _random!.NextSingle() * 9.0f);
            case 1: return value / (1.0f + _random!.NextSingle() * 9.0f);
            default:
                Debug.Fail("RandomSign: Invalid divide/multiply decision");
                throw new Exception("RandomSign: Invalid divide/multiply decision");
        }
    }

    public static sbyte RandomSign
    {
        get
        {
            switch (Random.Next(2))
            {
                case 0: return 1;
                case 1: return -1;
                default:
                    Debug.Fail("RandomSign: Invalid sign");
                    throw new Exception("RandomSign: Invalid sign");
            }
        }
    }

    public static Random Random
    {
        get
        {
            if (_random == null) _random = new Random();
            return _random;
        }
    }
    
    [ThreadStatic] private static Random? _random;
    #endregion
}