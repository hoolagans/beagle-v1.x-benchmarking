using System;

namespace BrowserEmulator;

public class BrowserEmulatorException : ApplicationException 
{
    public BrowserEmulatorException(string message) : base(message) { }
}
//--------------------------------------------------------------------
public class EOFException : BrowserEmulatorException 
{
    public EOFException(string message) : base(message) { }
}
//--------------------------------------------------------------------
public class FieldDoesNotExistException : BrowserEmulatorException 
{
    public FieldDoesNotExistException(string message) : base(message) { }
}