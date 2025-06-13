using System;

namespace WebMonk.Exceptions;

public class WebMonkException : Exception
{
    #region Constructors
    public WebMonkException(){ }
    public WebMonkException(string message):base(message){ }
    #endregion
}