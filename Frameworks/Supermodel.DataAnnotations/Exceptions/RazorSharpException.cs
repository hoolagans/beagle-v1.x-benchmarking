using System;

namespace Supermodel.DataAnnotations.Exceptions;

public class SupermodelException : Exception
{
    #region Constructors
    public SupermodelException(){ }
    public SupermodelException(string message):base(message){ }
    #endregion
}