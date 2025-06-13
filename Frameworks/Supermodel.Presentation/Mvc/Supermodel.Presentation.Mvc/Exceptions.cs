using System;

namespace Supermodel.Presentation.Mvc;

public class ModelStateInvalidException : Exception
{
    public ModelStateInvalidException(object model)
    {
        Model = model;
    }

    public object Model { get; protected set; }
}