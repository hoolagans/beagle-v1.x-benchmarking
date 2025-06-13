using System;

namespace Supermodel.Persistence;

public class UnableToDeleteException : Exception
{
    public UnableToDeleteException(string errorMessageToDisplay) : base(errorMessageToDisplay) {}
}