using System;
using System.Text;

namespace Supermodel.Encryptor;

public static class Converter
{
    public static byte[] StringToByteArr(string str)
    {
        return Encoding.Unicode.GetBytes(str);
    }
    public static string ByteArrToString(byte[] bytes)
    {
        return Encoding.Unicode.GetString(bytes);
    }
        
    public static string ByteArrToBase64String(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }
    public static byte[] Base64StringToByteArr(string str)
    {
        return Convert.FromBase64String(str);
    }
        
    public static string BinaryToHex(byte[] data)
    {
        //if (data == null) return null;
        var hex = new char[checked(data.Length * 2)];
        for (var i = 0; i < data.Length; i++)
        {
            var thisByte = data[i];
            hex[2 * i] = NibbleToHex((byte)(thisByte >> 4));        // high nibble
            hex[2 * i + 1] = NibbleToHex((byte)(thisByte & 0xf));   // low nibble
        }
        return new string(hex);
    }
    public static char NibbleToHex(byte nibble)
    {
        return (char)(nibble < 10 ? nibble + '0' : nibble - 10 + 'A');
    }
}