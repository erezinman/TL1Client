using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TL1Client.Common
{
    class TL1CommonResponse : TL1Response
    {
        const string ERROR_CODE_REGEX = @"^   (?<errcode>\w+)$";
        const string ERROR_VARIALES_REGEX = @"^   ""(?<errvars>[^;])""$";

        static readonly Regex ErrorCodeRegex = new Regex(ERROR_CODE_REGEX, RegexOptions.Compiled);
        static readonly Regex ErrorVariablesRegex = new Regex(ERROR_VARIALES_REGEX, RegexOptions.Compiled);

        public string ErrorCode { get; private set; }

        public IReadOnlyList<string> ErrorParameters { get; private set; }

        #region Overrides of TL1Response


        protected override IReadOnlyList<TL1DataLine> ParseAdditionalData(IEnumerable<string> rawLines)
        {
            switch (Result)
            {
                case ResponseResult.Deny:
                    return parseDenyResult(rawLines);
                case ResponseResult.Partial:
                    return parsePartialResult(rawLines);
                case ResponseResult.Completed:
                    return parseCompletedResult(rawLines);
                
                default: // This "common" response supports only the above results.
                    throw new FormatException($"Unsupported result '{Result}' (#{(int) Result}).");
            }
        }

        #endregion

        private List<TL1DataLine> parseDenyResult(IEnumerable<string> rawLines)
        {
            List<TL1DataLine> lines = new List<TL1DataLine>(1);
            using (var en = rawLines.GetEnumerator())
            {
                int i = 0;
                while (en.MoveNext())
                {
                    Match match;
                    switch (i)
                    {
                        case 0:
                            match = ErrorCodeRegex.Match(en.Current);
                            if (!match.Success)
                                throw new FormatException("Couldn't parse error code properly.");
                            ErrorCode = match.Groups["errcode"].Value;
                            break;
                        case 1:
                            match = ErrorVariablesRegex.Match(en.Current);
                            if (match.Success)
                            {
                                ErrorParameters = match.Groups["errvars"].Value.Split(':');
                            }
                            break;
                        default:
                            var line = CommentResponseLine.ParseIfMatches(i - 2, en); // Expecting only comments by now.
                            if (line != null)
                                lines.Add(line);
                            else
                                throw new FormatException($"Got invalid line when parsing the 'DENY' response. Line is \"{en.Current}\"");
                            break;
                    }
                    i++;
                }
                if (i < 2)
                    throw new FormatException("Expected more lines when parsing the 'DENY' response.");
            }
            return lines;
        }

        private List<TL1DataLine> parsePartialResult(IEnumerable<string> rawLines)
        {
            List<TL1DataLine> lines = new List<TL1DataLine>();

            using (var en = rawLines.GetEnumerator())
            {
                int i = 0;
                while (en.MoveNext())
                {
                    var line = (TL1DataLine) PartialResultResponseLine.ParseIfMatches(en.Current, i)
                               ?? CommentResponseLine.ParseIfMatches(i, en);
                    if (line == null)
                    {
                        throw new FormatException($"Unexpected line format when parsing the 'PRTL' response. Line is \"{en.Current}\".");
                    }

                    lines.Add(line);
                    i++;
                }
                if (i == 0)
                    throw new FormatException("No description lines found when parsing the 'PRTL' response.");

            }

            return lines;
        }

        private List<TL1DataLine> parseCompletedResult(IEnumerable<string> rawLines)
        {
            var lines = new List<TL1DataLine>();
            using (var en = rawLines.GetEnumerator())
                while (en.MoveNext())
                {
                    lines.Add(CommentResponseLine.ParseIfMatches(lines.Count, en) ?? new TL1DataLine(lines.Count, en.Current));
                }

            return lines;
        }
    }
}