using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TL1Client
{
    public static class Utils
    {
        /// <summary>
        /// Escapes a string containing characters that should be escaped according to the TL1 protocol.
        /// </summary>
        /// <param name="s">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        /// <exception cref="FormatException">Thrown if the string contains a semi-colon (';').</exception>
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

        /// <summary>
        /// Unescapes a string containing escaped characters according to the TL1 protocol.
        /// </summary>
        /// <param name="s">The string to unescape.</param>
        /// <returns>The unescaped string.</returns>
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

        /// <summary>
        /// Gets a key-value mapping from a regular-expression-match that has the them grouped by name.
        /// </summary>
        /// <param name="match">The <see cref="Match"/> to extract the data from.</param>
        /// <param name="keyGroupName">The group name of the "key" group.</param>
        /// <param name="valueGroupName">The group name of the "value" group.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHeadersDictionaryFromMatch(Match match, string keyGroupName = "key", string valueGroupName = "value")
        {
            int numOfArgs = match.Groups[keyGroupName].Captures.Count;
            Dictionary<string, string> arguments = new Dictionary<string, string>(numOfArgs);
            for (int j = 0; j < numOfArgs; j++)
            {
                arguments[match.Groups[keyGroupName].Captures[j].Value] = UnescapeString(match.Groups[valueGroupName].Captures[j].Value);
            }
            return arguments;
        }

        /// <summary>
        /// A returns regular expression that (supposedly) efficiently allows for any valid escaped character, 
        /// doesn't allow the invalid characters or unescaped characters, and either allows or disallows empty words.
        /// </summary>
        /// <remarks>
        /// Based on the regular expression from here https://stackoverflow.com/questions/5695240/php-regex-to-ignore-escaped-quotes-within-quotes.
        /// </remarks>
        public static string GetEscapableWordRegex(string allowedOnlyEscaped = @"""\-=,", string disallowedEvenIfEscaped = ";", bool allowEmptyWord = true)
        {
            allowedOnlyEscaped = Regex.Escape(allowedOnlyEscaped ?? "");
            disallowedEvenIfEscaped = Regex.Escape(disallowedEvenIfEscaped ?? "");
            if (disallowedEvenIfEscaped == "")
                if (allowedOnlyEscaped == "")
                    return $".{(allowEmptyWord ? "*" : "+")}";
                else
                    return $@"[^{allowedOnlyEscaped}]{(allowEmptyWord ? "*" : "+")}(?:\\.[^{allowedOnlyEscaped}]*)*";
            if (allowedOnlyEscaped == "")
                return $@"[^{disallowedEvenIfEscaped}]{(allowEmptyWord ? "*" : "+")}";

            return $@"[^{allowedOnlyEscaped}{disallowedEvenIfEscaped}]{(allowEmptyWord ? "*" : "+")}" 
                + $@"(?:\\[^{disallowedEvenIfEscaped}][^{allowedOnlyEscaped}{disallowedEvenIfEscaped}]*)*";
        }
    }
}
