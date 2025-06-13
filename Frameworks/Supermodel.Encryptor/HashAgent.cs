using System;
using System.Security.Cryptography;
using System.Text;

namespace Supermodel.Encryptor;

public static class HashAgent
{
    public static string HashString(string str)
    {
        return HashByteArray(Converter.StringToByteArr(str));
    }

    public static string HashByteArray(byte[] data)
    {
        var hasher = new SHA1CryptoServiceProvider();
        return Convert.ToBase64String(hasher.ComputeHash(data));
    }

    public static string GenerateGuidSalt()
    {
        return Guid.NewGuid().ToString();
    }
    public static string Generate128BitSalt()
    {
        return Convert.ToBase64String(GenerateBinary128BitSalt()); 
    }

    public static byte[] GenerateBinaryGuidSalt()
    {
        return Converter.StringToByteArr(GenerateGuidSalt());
    }
    public static byte[] GenerateBinary128BitSalt ()
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public static string Generate5MinTimeStampSalt(DateTime dt, string format = "{0:|yyyy|M|d|H|m}")
    {
        //return $"{RoundUp5Min(dt):|yyyy|M|d|H|m}";
        return string.Format(format, RoundUp5Min(dt));
    }

    public static string HashPasswordSHA1(string password, string salt)
    {
        return HashPassword(password, salt, new SHA1CryptoServiceProvider());
    }
    public static string HashPasswordSHA1Unicode(string password, string salt)
    {
        return HashPasswordUnicode(password, salt, new SHA1CryptoServiceProvider());
    }
    public static byte[] HashPasswordSHA1(string password, byte[] salt)
    {
        return HashPassword(password, salt, new SHA1CryptoServiceProvider());
    }
    public static byte[] HashPasswordSHA1Unicode(string password, byte[] salt)
    {
        return HashPasswordUnicode(password, salt, new SHA1CryptoServiceProvider());
    }
        
    public static string HashPasswordMD5(string password, string salt)
    {
        return HashPassword(password, salt, new MD5CryptoServiceProvider());            
    }
    public static string HashPasswordMD5Unicode(string password, string salt)
    {
        return HashPasswordUnicode(password, salt, new MD5CryptoServiceProvider());
    }
    public static byte[] HashPasswordMD5(string password, byte[] salt)
    {
        return HashPassword(password, salt, new MD5CryptoServiceProvider());
    }
    public static byte[] HashPasswordMD5Unicode(string password, byte[] salt)
    {
        return HashPasswordUnicode(password, salt, new MD5CryptoServiceProvider());
    }
        
    public static string HashPasswordSHA256(string password, string salt)
    {
        return HashPassword(password, salt, new SHA256CryptoServiceProvider());            
    }
    public static string HashPasswordSHA256Unicode(string password, string salt)
    {
        return HashPasswordUnicode(password, salt, new SHA256CryptoServiceProvider());
    }
    public static byte[] HashPasswordSHA256(string password, byte[] salt)
    {
        return HashPassword(password, salt, new SHA256CryptoServiceProvider());            
    }
    public static byte[] HashPasswordSHA256Unicode(string password, byte[] salt)
    {
        return HashPasswordUnicode(password, salt, new SHA256CryptoServiceProvider());
    }

    public static string HashPasswordSHA512(string password, string salt)
    {
        return HashPassword(password, salt, new SHA512CryptoServiceProvider());            
    }
    public static string HashPasswordSHA512Unicode(string password, string salt)
    {
        return HashPasswordUnicode(password, salt, new SHA512CryptoServiceProvider());
    }
    public static byte[] HashPasswordSHA512(string password, byte[] salt)
    {
        return HashPassword(password, salt, new SHA512CryptoServiceProvider());            
    }
    public static byte[] HashPasswordSHA512Unicode(string password, byte[] salt)
    {
        return HashPasswordUnicode(password, salt, new SHA512CryptoServiceProvider());            
    }

    private static string HashPassword(string password, string salt, HashAlgorithm hasher)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (salt == null) throw new ArgumentNullException(nameof(salt));
            
        return Converter.BinaryToHex(hasher.ComputeHash(Encoding.UTF8.GetBytes(password + salt)));
    }
    private static string HashPasswordUnicode(string password, string salt, HashAlgorithm hasher)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (salt == null) throw new ArgumentNullException(nameof(salt));

        return Converter.BinaryToHex(hasher.ComputeHash(Converter.StringToByteArr(password + salt)));
    }
    private static byte[] HashPassword(string password, byte[] salt, HashAlgorithm hasher)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (salt == null) throw new ArgumentNullException(nameof(salt));

        return hasher.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
    }
    private static byte[] HashPasswordUnicode(string password, byte[] salt, HashAlgorithm hasher)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (salt == null) throw new ArgumentNullException(nameof(salt));

        return hasher.ComputeHash(Converter.StringToByteArr(password + salt));
    }
        
    private static DateTime RoundUp5Min(DateTime dt)
    {
        var d = TimeSpan.FromMinutes(5);
        return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
    }
}