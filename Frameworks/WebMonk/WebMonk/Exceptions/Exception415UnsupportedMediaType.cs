namespace WebMonk.Exceptions;

public class Exception415UnsupportedMediaType : WebMonkException
{
    public Exception415UnsupportedMediaType(string? contentType) : base($"Unknown Content-Type: '{contentType}'") { }
}