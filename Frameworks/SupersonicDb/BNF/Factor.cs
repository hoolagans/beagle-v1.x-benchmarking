namespace Supersonic.BNF;

internal class Factor
{
    #region EmbeddedTypes
    public enum OperationEnum
    {
        Equal, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual
    }
    #endregion

    #region Methods
    public bool IsEquality => Operation == OperationEnum.Equal;
    #endregion

    #region Properties
    public int PropertyIndex { get; set; }
    public OperationEnum Operation { get; set; }
    public object Constant { get; set; }
    #endregion
}