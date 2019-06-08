using UnityEngine;
using System.Collections;

public static class StringUtils 
{
    public static string s_NULL = "NULL";
    public static string s_EMPTY = "";

    public static string s_INVALID = "a32bfd9e"; // CRC32 for 'BUG'

    public static bool IsNullOrEmpty(string i_String)
    {
        return (i_String == s_NULL || i_String == s_EMPTY);
    }

    public static int GetHashCode(string input)
    {
        return input.GetHashCode();  
    }

    public static string TrimStart(this string target, string trimString)
    {
        string result = target;
        while (result.StartsWith(trimString))
        {
            result = result.Substring(trimString.Length);
        }

        return result;
    }

    public static string TrimEnd(this string target, string trimString)
    {
        string result = target;
        while (result.EndsWith(trimString))
        {
            result = result.Substring(0, result.Length - trimString.Length);
        }

        return result;
    }
}

public static class SubstringExtensions
{
    /// <summary>
    /// Get string value between [first] a and [last] b.
    /// </summary>
    public static string Between(this string value, string a, string b)
    {
        int posA = value.IndexOf(a);
        int posB = value.LastIndexOf(b);
        if (posA == -1)
        {
            return "";
        }
        if (posB == -1)
        {
            return "";
        }

        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= posB)
        {
            return "";
        }

        return value.Substring(adjustedPosA, posB - adjustedPosA);
    }

    /// <summary>
    /// Get string value after [first] a.
    /// </summary>
    public static string Before(this string value, string a)
    {
        int posA = value.IndexOf(a);
        if (posA == -1)
        {
            return "";
        }

        return value.Substring(0, posA);
    }

    /// <summary>
    /// Get string value after [last] a.
    /// </summary>
    public static string After(this string value, string a)
    {
        int posA = value.LastIndexOf(a);
        if (posA == -1)
        {
            return "";
        }

        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= value.Length)
        {
            return "";
        }

        return value.Substring(adjustedPosA);
    }
}
