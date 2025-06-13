using System;

namespace CsvMaker.CsvString;

public class UnexpectedHeaderException : Exception
{
    public UnexpectedHeaderException(string msg) : base(msg) { }
}