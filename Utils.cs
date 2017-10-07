using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TL1Client
{
    public static class Utils
    {
        public static string EscapeString(string s)
        {
            int i;
            if (s.Contains(';'))
                throw new FormatException("TL1 commands can not contain semi-colons.");
            while ((i = s.IndexOfAny(new[] { '\"', ',', '\'', '\\', '=' })) != -1)
            {
                s = s.Insert(i, "\\");
            }
            return s;
        }
        public static string UnescapeString(string s)
        {
            var escapedChars = new[] { '\"', ',', '\'', '\\', '=' };

            int i = 0;
            while ((i = s.IndexOf('\\', i + 1)) != -1)
            {
                if (escapedChars.Contains(s[i + 1]))
                    s = s.Remove(i);
            }
            return s;
        }

        public static Dictionary<string, string> GetHeadersDictionaryFromMatch(Match match, string keyGroupName = "key", string valueGroupName = "value")
        {
            int numOfArgs = match.Groups[keyGroupName].Captures.Count;
            Dictionary<string, string> arguments = new Dictionary<string, string>(numOfArgs);
            for (int j = 0; j < numOfArgs - 1; j++)
            {
                arguments[match.Groups[keyGroupName].Captures[j].Value] = Utils.UnescapeString(match.Groups[valueGroupName].Captures[j].Value);
            }
            return arguments;
        }
    }
}
