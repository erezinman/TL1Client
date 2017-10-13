using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TL1Client;

namespace TL1Client.Common
{
    public class CommentResponseLine : TL1DataLine
    {
        // Comment Format: ^^^/* <free-text-including-new-lines> */ cr lf
        const string COMMENT_START_REGEX = @"^   /\*";
        const string COMMENT_END_REGEX = @".*\*/$";
        static readonly Regex CommenStartRegex = new Regex(COMMENT_START_REGEX, RegexOptions.Compiled);
        static readonly Regex CommentEndRegex = new Regex(COMMENT_END_REGEX, RegexOptions.Compiled);

        public string Comment { get; }

        public CommentResponseLine(int lineIndex,string comment, string rawLine) : base(lineIndex, rawLine)
        {
            Comment = comment;
        }

        public static CommentResponseLine ParseIfMatches(IEnumerator<string> en, int i)
        {
            var match = CommenStartRegex.Match(en.Current);
            if (match.Success)
            {
                var canEnumerate = true;
                var sb = new StringBuilder();
                while (canEnumerate && !CommentEndRegex.IsMatch(en.Current))
                {
                    sb.AppendLine(en.Current);
                    canEnumerate = en.MoveNext();
                }

                if (!canEnumerate)
                    throw new FormatException("Unclosed comment.");

                sb.Append(en.Current);

                return new CommentResponseLine(i, sb.ToString(5, sb.Length - 7), sb.ToString());
            }
            return null;
        }
    }
}