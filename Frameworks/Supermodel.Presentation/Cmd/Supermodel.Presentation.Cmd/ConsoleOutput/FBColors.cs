using System;

namespace Supermodel.Presentation.Cmd.ConsoleOutput;

public readonly struct FBColors(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor = null)
{
    #region Constructors

    public static FBColors FromCurrent()
    {
        return new FBColors(Console.ForegroundColor, Console.BackgroundColor);
    }
    public static FBColors FromCurrentInverse()
    {
        return new FBColors(Console.BackgroundColor, Console.ForegroundColor);
    }
    #endregion

    #region Equality operations
    public static bool operator== (FBColors a, FBColors b)
    {
        return a.Equals(b);
    }
    public static bool operator!= (FBColors a, FBColors b)
    {
        return !a.Equals(b);
    }
    public bool Equals(FBColors other)
    {
        return ForegroundColor == other.ForegroundColor && BackgroundColor == other.BackgroundColor;
    }
    public override bool Equals(object? obj)
    {
        return obj is FBColors other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(ForegroundColor, BackgroundColor);
    }
    #endregion
        
    #region Methods
    public void SetColors()
    {
        if (ForegroundColor != null) Console.ForegroundColor = ForegroundColor.Value;
        if (BackgroundColor != null) Console.BackgroundColor = BackgroundColor.Value;
    }
    #endregion

    #region Properties
    public ConsoleColor? ForegroundColor { get; } = foregroundColor;
    public ConsoleColor? BackgroundColor { get; } = backgroundColor;

    #endregion
}