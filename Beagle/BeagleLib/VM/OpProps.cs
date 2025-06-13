namespace BeagleLib.VM;

public readonly struct OpProps
{
    #region Constructors
    public OpProps(CommandTypeEnum commandType, sbyte stackEffect, byte minStackRequired)
    {
        CommandType = commandType;
        StackEffect = stackEffect;
        MinStackRequired = minStackRequired;
    }
    #endregion

    #region Properties
    public CommandTypeEnum CommandType { get; }
    public sbyte StackEffect { get; }
    public byte MinStackRequired { get; }
    #endregion
}