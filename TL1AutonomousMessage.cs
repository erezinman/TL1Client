using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TL1Client
{
    public class TL1AutonomousMessage : TL1ReceivedHeaderedMessage
    {
        // Response second line format - <almcde>^<atag>^<verb>[^<modifier>[^<modifier>]]
        const string SCND_LINE_REGEX = @"^(?<almcode>\*C|\*\*|\* |A ) (?<atag>\d+)  (?<verb>[a-zA-Z_]+)(?<mods> ([a-zA-Z_]+){0,2})$";
        private static readonly Regex SecondLineRegex = new Regex(SCND_LINE_REGEX, RegexOptions.Compiled);

        public TL1AutonomousMessageAlarmCodes AlarmCode { get; private set; }
        public uint AutomaticMessageTag { get; private set; }
        public string Verb { get; private set; }
        public string Modifier1 { get; private set; }
        public string Modifier2 { get; private set; }
        public IReadOnlyList<TL1DataLine> AdditionalData { get; private set; }

        public static bool MatchesFormat(string secondLine)
        {
            return SecondLineRegex.IsMatch(secondLine);
        }

        public static TAutonomous Parse<TAutonomous>(string secondLine, TextReader reader)
            where TAutonomous : TL1AutonomousMessage, new()
        {
            var autoMessage = new TAutonomous();
            var match = SecondLineRegex.Match(secondLine);
            if (!match.Success)
                throw new FormatException($"Second line is not in the correct format. Second line was \"{secondLine}\".");

            switch (match.Groups["almcode"].Value)
            {
                case "*C":
                    autoMessage.AlarmCode = TL1AutonomousMessageAlarmCodes.Critical;
                    break;
                case "**":
                    autoMessage.AlarmCode = TL1AutonomousMessageAlarmCodes.Major;
                    break;
                case "* ":
                    autoMessage.AlarmCode = TL1AutonomousMessageAlarmCodes.Minor;
                    break;
                case "A ":
                    autoMessage.AlarmCode = TL1AutonomousMessageAlarmCodes.Autonmous;
                    break;
            }

            autoMessage.AutomaticMessageTag = uint.Parse(match.Groups["atag"].Value);
            autoMessage.Verb = match.Groups["verb"].Value;
            if (match.Groups["mods"].Captures.Count > 0)
            {
                autoMessage.Modifier1 = match.Groups["mods"].Captures[0].Value;
                if (match.Groups["mods"].Captures.Count > 1)
                {
                    autoMessage.Modifier2 = match.Groups["mods"].Captures[1].Value;
                }
            }

            var terminatorStrings = new[] {";", ">"};

            List<string> additionalData = new List<string>();
            while (!terminatorStrings.Contains(secondLine = reader.ReadLine()) )
                additionalData.Add(secondLine);

            autoMessage.AdditionalData = autoMessage.ParseAdditionalData(additionalData);
            return autoMessage;
        }

        protected virtual IReadOnlyList<TL1DataLine> ParseAdditionalData(IEnumerable<string> rawLines)
        {
            return rawLines.Select((s, i) => new TL1DataLine(i, s)).ToArray();
        }
    }

    public enum TL1AutonomousMessageAlarmCodes
    {
        Critical,
        Major,
        Minor,
        Autonmous
    }
}