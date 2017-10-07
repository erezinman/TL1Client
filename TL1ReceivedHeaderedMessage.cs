using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace TL1Client
{
    public class TL1ReceivedHeaderedMessage : TL1ReceivedMessage
    {
        // Message start format: ^^^<sid>^<YY-MM-DD>^<HH:MM:SS> cr lf
        const string FRST_LINE_REGEX = @"^   (?<sid>[a-zA-Z0-9_]{1,20}) (?<dtime>\d\d\-\d\d\-\d\d \d\d\:\d\d\:\d\d)$";

        private static readonly Regex FirstLineRegex = new Regex(FRST_LINE_REGEX, RegexOptions.Compiled);

        public string SystemID { get; private set; }
        public DateTime Time { get; private set; }

        public static bool MatchesFormat(string s)
        {
            return FirstLineRegex.IsMatch(s);
        }

        public static TL1ReceivedHeaderedMessage Parse<TResponse, TAutonomous>(string firstLine, TextReader reader)
            where TResponse : TL1Response, new()
            where TAutonomous : TL1AutonomousMessage, new()
        {
            var match = FirstLineRegex.Match(firstLine);
            string systemID = match.Groups["sid"].Value;
            var time = DateTime.ParseExact(match.Groups["dtime"].Value, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            firstLine = reader.ReadLine();

            TL1ReceivedHeaderedMessage ret;
            if (TL1Response.MatchesFormat(firstLine))
            {
                ret = TL1Response.Parse<TResponse>(firstLine, reader);
            }
            else if (TL1AutonomousMessage.MatchesFormat(firstLine))
            {
                ret = TL1AutonomousMessage.Parse<TAutonomous>(firstLine, reader);
            }
            else
                throw new FormatException($"Unknwon incomming message - Second line was \"{firstLine}\"");

            ret.SystemID = systemID;
            ret.Time = time;

            return ret;
        }
    }
}