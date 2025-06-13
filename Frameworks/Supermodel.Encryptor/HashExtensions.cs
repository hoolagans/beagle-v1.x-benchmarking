using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Supermodel.Encryptor;

public static class HashExtensions
{
    public static string GetHash(this object instance, HashAlgorithm hasher)
    {
        return ComputeHash(instance, hasher);
    }
    public static string GetMD5Hash(this object instance)
    {
        return instance.GetHash(new MD5CryptoServiceProvider());
    }
    public static string GetSHA1Hash(this object instance)
    {
        return instance.GetHash(new SHA1CryptoServiceProvider());
    }        
    public static string GetSHA256Hash(this object instance)
    {
        return instance.GetHash(new SHA256CryptoServiceProvider());
    }        
    public static string GetSHA512Hash(this object instance)
    {
        return instance.GetHash(new SHA512CryptoServiceProvider());
    }        
    private static string ComputeHash(object instance, HashAlgorithm hasher)
    {
        var serializer = new DataContractSerializer(instance.GetType());
        using (var memoryStream = new MemoryStream())
        {
            serializer.WriteObject(memoryStream, instance);
            return Convert.ToBase64String(hasher.ComputeHash(memoryStream.ToArray()));
        }
    }
}