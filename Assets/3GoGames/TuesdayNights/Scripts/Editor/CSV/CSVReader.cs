using UnityEngine;

using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List< Dictionary<string, string> > Read(string file)
    {
        List<Dictionary<string, string> > output = new List<Dictionary<string, string> >();

        TextAsset data = Resources.Load(file) as TextAsset;
        if (data != null)
        {
            string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1)
            {
                return output;
            }

            string[] header = Regex.Split(lines[0], SPLIT_RE);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = Regex.Split(lines[i], SPLIT_RE);

                if (values.Length == 0 || values[0] == "")
                    continue;

                Dictionary<string, string> entry = new Dictionary<string, string>();

                for (int j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                    entry[header[j]] = value;
                }

                output.Add(entry);
            }
        }

        return output;
    }
}