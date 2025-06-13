using System;

namespace Supermodel.Presentation.WebMonk;

public class ModelStateInvalidException(object model) : Exception
{
    public object Model { get; protected set; } = model;
}