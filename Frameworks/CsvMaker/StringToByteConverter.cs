using System.Text;

namespace CsvMaker;

public static class StringToByteConverter
{
    public static byte[] GetBytes(this string str)
    {
        return Encoding.Default.GetBytes(str);
    }

    public static string GetString(this byte[] bytes)
    {
        return Encoding.Default.GetString(bytes);
    }
}