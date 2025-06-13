using System.Text.RegularExpressions;

namespace CsvMaker.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Allows left-to-right invocation of string formatting: <code>"Some format {0}".Format("insert this.");</code>
    /// "Format" conflicts with static method, and single letter method name makes calls more concise.
    /// </summary>
    public static string? EscapeSingleQuotes(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return str.Replace("'", "~");
    }
    public static string? EscapeCommas(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return str.Replace(",", "~");
    }
    public static string? EscapeCommasSingleQuotes(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return str.EscapeCommas()!.EscapeSingleQuotes();
    }
    public static string? EscapeCommasSingleQuotesNewLine(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        return Regex.Replace(str.EscapeCommasSingleQuotes()!, @"\r\n?|\n", " ");
    }
    public static string PrepareCvsColumn(this string str)
    {
        //handle double-quotes
        str = str.Replace("\"", "\"\"");

        //handle comma
        if (str.Contains(",") || str.Contains("\"")) str = $"\"{str}\"";

        return str;
    }
}