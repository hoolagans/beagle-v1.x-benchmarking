using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supermodel.DataAnnotations;

public class RegexHelper
{
    #region Methods
    public bool IsValidEmail(string strIn)
    {
        Invalid = false;
        if (string.IsNullOrEmpty(strIn)) return false;

        // Use IdnMapping class to convert Unicode domain names.
        strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
        if (Invalid) return false;

        // Return true if strIn is in valid e-mail format.
        return Regex.IsMatch(strIn, EmailRegex, RegexOptions.IgnoreCase);
    }

    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        var idn = new IdnMapping();

        var domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            Invalid = true;
        }
        return match.Groups[1].Value + domainName;
    }
    #endregion

    #region Properties
    public const string EmailRegex = @"^(?("")(""[^""]+?""@)|(([0-9A-Za-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9A-Za-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9A-Za-z][-\w]*[0-9A-Za-z]*\.)+[a-zA-Z0-9]{2,17}))$";
    protected bool Invalid { get; set; } 
    #endregion
}