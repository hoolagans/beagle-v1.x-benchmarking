using System;

namespace Supermodel.Presentation.Cmd;

public class ModelStateInvalidException(object model) : Exception
{
    public object Model { get; protected set; } = model;
}