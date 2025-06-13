using System;
using System.Text.RegularExpressions;

namespace BrowserEmulator;

public class Common
{
    public static bool ValidateRx(string input, string pattern) {
        Match m = Regex.Match(input, pattern);
        if (m.Success == false) return false;
        if (m.Length != input.Length) return false;
        return true;
    }

    public static string PrintMMYYYY(DateTime val) {
        return val.Month + "/" + val.Year;
    }

    public static string Print(DateTime val) {
        return val.ToShortDateString();
    }

    public static string Print(int val) {
        string ret;
        if (val < 0) ret = "";
        else ret = val.ToString();
        return ret;
    }

    public static string PrintF0(float val) {
        string ret;
        if (val < 0) ret = "";
        else ret = Math.Round(val, 0).ToString("F0");
        return ret;
    }

    public static string PrintF1(float val) {
        string ret;
        if (val < 0) ret = "";
        else ret = Math.Round(val, 1).ToString("F1");
        return ret;
    }

    public static string PrintF2Negative(float val) {
        return Math.Round(val, 2).ToString("F2");
    }

    public static string PrintF2(float val) {
        string ret;
        if (val < 0) ret = "";
        else ret = Math.Round(val, 2).ToString("F2");
        return ret;
    }

    public static string PrintD2(int val) {
        return val.ToString("D2");
    }

    public static string PrintD4(int val) {
        return val.ToString("D4");
    }
	
    public static string PrintD5(int val) {
        return val.ToString("D5");
    }

    public static string PrintDollarAmountF2(float val) {
        string strVal = PrintF2(val);

        string afterDot = strVal.Substring(strVal.IndexOf(".", StringComparison.Ordinal), 3);
        strVal = strVal.Substring(0, strVal.IndexOf(".", StringComparison.Ordinal));

        if (strVal.Length > 3) {
            int maxCommas = (strVal.Length / 3) - 1;

            if ((strVal.Length % 3) == 0) maxCommas = maxCommas - 1;

            for (int i = 0; i <= maxCommas; i++)
            {
                var commaPos = ((i + 1) * 3) + i;
                strVal = strVal.Substring(0, strVal.Length - commaPos) + "," + strVal.Substring(strVal.Length - commaPos, commaPos);
            }
        }
        return (strVal + afterDot);
    }

    public static string PrintF3(float val) {
        string ret;
        if (val < 0) ret = "";
        else ret = Math.Round(val, 3).ToString("F3");
        return ret;
    }
}