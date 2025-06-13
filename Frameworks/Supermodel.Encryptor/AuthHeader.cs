namespace Supermodel.Encryptor;

public class AuthHeader
{
    #region Constructors
    public AuthHeader(string headerName, string authToken)
    {
        HeaderName = headerName;
        AuthToken = authToken;
    }
    public AuthHeader(string authToken) : this("Authorization", authToken){}
    #endregion

    #region Properties
    public string HeaderName { get; set; }
    public string AuthToken { get; set; }
    #endregion
}