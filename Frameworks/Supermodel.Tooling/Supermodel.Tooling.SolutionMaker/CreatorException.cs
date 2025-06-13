using System;

namespace Supermodel.Tooling.SolutionMaker;

public class CreatorException : Exception
{
    public CreatorException(string msg) : base(msg) { }
}