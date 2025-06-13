namespace WebMonk.Exceptions;

public class HttpRequestValidationException : WebMonkException
{
    #region Constructors
    public HttpRequestValidationException(){ }
    public HttpRequestValidationException(string message):base(message){ }
    #endregion
}