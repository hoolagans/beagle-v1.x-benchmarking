using System;

namespace Supermodel.Presentation.Cmd.ConsoleOutput;

public readonly struct ColorChange
{
    #region Constructors
    public ColorChange(int index, FBColors colors)
    {
        if (index < 0) throw new ArgumentException("index < 0");    
            
        Index = index;
        Colors = colors;
    }
    public ColorChange(int index, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
        :this(index, new FBColors(foregroundColor, backgroundColor)) { }
    #endregion

    #region Methods
    public ColorChange CloneWithOffset(int offset)
    {
        var index = Index + offset;
        return new ColorChange(index, Colors);
    }
    #endregion

    #region Properties
    public int Index { get; }
    public FBColors Colors { get; }
    #endregion
}