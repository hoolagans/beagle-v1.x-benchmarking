using System;

namespace WebMonk.RazorSharp.Exceptions;

public class RazorSharpException : Exception
{
    #region Constructors
    public RazorSharpException(){ }
    public RazorSharpException(string message):base(message){ }
    #endregion
}