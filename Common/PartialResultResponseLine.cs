using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TL1Client.Common
{
    public class PartialResultResponseLine : KeyValueResponseLine
    {
        // Format:
        // ^^^"<AID>:ERCDE=<value>[,<keyword>=<value>,....<keyword>=<value>]" cr lf +
        // "\v" are replaced by the regular expressions for values, and "\k" are replaced by the regular expressions for keys.
        const string PARTIAL_RESULT_DESCIPTION_LINE_REGEX = @"^   ""(?<aid>[a-zA-Z0-9_]+):(?:(?<key>\v)=(?<value>\v))(?:,(?:(?<key>\v)=(?<value>\v)))*""$";

        private static readonly Regex PartialResultDescriptionLineRegex = new Regex(PARTIAL_RESULT_DESCIPTION_LINE_REGEX
            .Replace(@"\v", Utils.GetEscapableWordRegex(@"\""=,"))
            .Replace(@"\k", Utils.GetEscapableWordRegex(@"\""=,", allowEmptyWord: false)), RegexOptions.Compiled);

        public string AccessID { get; }

        public PartialResultResponseLine(int lineIndex, string aid, IReadOnlyDictionary<string, string> headers, string rawLine) : base(lineIndex, headers, rawLine)
        {
            AccessID = aid;
        }

        public static PartialResultResponseLine ParseIfMatches(string line, int i)
        {
            var match = PartialResultDescriptionLineRegex.Match(line);
            if (match.Success)
            {
                return new PartialResultResponseLine(i, match.Groups["aid"].Value, Utils.GetHeadersDictionaryFromMatch(match), line);
            }
            return null;
        }

    }
}