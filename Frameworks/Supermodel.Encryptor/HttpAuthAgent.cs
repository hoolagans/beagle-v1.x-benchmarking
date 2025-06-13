using System;
using System.Text;

namespace Supermodel.Encryptor;

public static class HttpAuthAgent
{
    #region Header Creation Methods
    //example Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
    public static AuthHeader CreateBasicAuthHeader(string username, string password)
    {
        return new AuthHeader("Basic " + CreateBasicAuthTokenCredentials(username, password));
    }
    public static AuthHeader CreateSMCustomEncryptedAuthHeader(byte[] key, params string[] args)
    {
        return new AuthHeader("SMCustomEncrypted " + CreateSMCustomEncryptedAuthTokenCredentials(key, args));
    }

    public static string CreateBasicAuthToken(string username, string password)
    {
        return "Basic " + CreateBasicAuthTokenCredentials(username, password);
    }
    public static string CreateSMCustomEncryptedAuthToken(byte[] key, params string[] args)
    {
        return "SMCustomEncrypted " + CreateSMCustomEncryptedAuthTokenCredentials(key, args);
    }
    #endregion

    #region Header Read Methods
    public static void ReadBasicAuthToken(string authorizeHeader, out string username, out string password)
    {
        if (authorizeHeader == null) throw new ArgumentNullException(nameof(authorizeHeader));
        if (!authorizeHeader.StartsWith("Basic ")) throw new ArgumentException("'Basic' authorization scheme is expected");
		    
        var credentials = authorizeHeader.Replace("Basic ", "");
        ReadBasicAuthTokenCredentials(credentials, out username, out password);
    }
    public static void ReadSMCustomEncryptedAuthToken(byte[] key, string authorizeHeader, out string[] args)
    {
        if (authorizeHeader == null) throw new ArgumentNullException(nameof(authorizeHeader));
        if (!authorizeHeader.StartsWith("SMCustomEncrypted ")) throw new ArgumentException("'SMCustomEncrypted' authorization scheme is expected");

        var encryptedCredentials = authorizeHeader.Replace("SMCustomEncrypted ", "");
        ReadSMCustomEncryptedAuthTokenCredentials(key, encryptedCredentials, out args);
    }
    #endregion

    #region Lower-level credential methods
    public static string CreateBasicAuthTokenCredentials(string username, string password)
    {
        var payloadStr = string.Join(":", username, password);
        if (username.Contains(":") || password.Contains(":")) throw new ArgumentException("Username and Password cannot contain ':' character for Basic non-encrypted auth");
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadStr));
    }
    public static string CreateSMCustomEncryptedAuthTokenCredentials(byte[] key, params string[] args)
    {
        var payloadStrBuilder = new StringBuilder();
        var first = true;
        foreach (var param in args)
        {
            if (first)
            {
                first = false;
                payloadStrBuilder.Append(Converter.ByteArrToBase64String(Converter.StringToByteArr(param)));
            }
            else
            {
                payloadStrBuilder.Append(":" + Converter.ByteArrToBase64String(Converter.StringToByteArr(param)));
            }
        }
        var payloadCode = EncryptorAgent.Lock(key, payloadStrBuilder.ToString(), out var payloadIV);
            
        return Convert.ToBase64String(payloadCode) + "|" + Convert.ToBase64String(payloadIV);
    }
        
    public static void ReadBasicAuthTokenCredentials(string credentials, out string username, out string password)
    {
        var parts = Encoding.UTF8.GetString(Convert.FromBase64String(credentials)).Split(':');   
        if (parts.Length != 2) throw new ArgumentException("Authorization header is badly formatted");
        username = parts[0].Trim();
        password = parts[1].Trim();
    }
    public static void ReadSMCustomEncryptedAuthTokenCredentials(byte[] key, string encryptedCredentials, out string[] args)
    {
        if (key == null) throw new ArgumentException("key is required for parsing an encrypted auth header");
        if (string.IsNullOrWhiteSpace(encryptedCredentials)) throw new ArgumentException("encryptedCredentials is required for parsing an encrypted auth header");

        var encryptionParts = encryptedCredentials.Split('|');
        if (encryptionParts.Length != 2) throw new ArgumentException("Authorization header is badly formatted");

        var code = encryptionParts[0];
        var iv = encryptionParts[1];

        var payloadStr = EncryptorAgent.Unlock(key, Convert.FromBase64String(code), Convert.FromBase64String(iv));

        var argsStr = payloadStr.Split(':');
        args = new string[argsStr.Length];
        for (var i=0; i < argsStr.Length; i++) args[i] = Converter.ByteArrToString(Converter.Base64StringToByteArr(argsStr[i]));
    }
    #endregion
}