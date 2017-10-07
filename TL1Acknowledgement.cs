using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TL1Client
{
    /// <summary>
    /// An acknowledgement message is a special reply sent by the NE (Network Element) in connection with a delayed command. This special
    /// response is issued after the receipt of the command and indicates the status of the request.
    /// </summary>
    /// <typeparam name="TAckCodesEnum"> An enumeration type for the different acknowledgement code (case-sensitive parsing).</typeparam>
    public class TL1Acknowledgement<TAckCodesEnum> : TL1ReceivedMessage
        where TAckCodesEnum : struct
    {
        // Ack format: <acknowledgment code> ^ <ctag> cr lf <
        const string FRST_LINE_REGEX = @"^(?<ack_code>\w+$) (?<ctag>[a-zA-Z0-9_]{1,6})(?:\<)$";
        private static readonly Regex FirstLineRegex = new Regex(FRST_LINE_REGEX, RegexOptions.Compiled);

        /// <summary>
        /// Serve as a means of correlating an input command with its associated output response. The OS assigns an arbitrary non-zero CTAG value and it is the responsibility of the NE to copy this value into the output response associated with that input command. The value of CTAG must either be a TL1 identifier or a non-zero decimal number consisting of not more than six characters.
        /// </summary>
        /// <remarks>
        /// I use string here to allow alpha-numeric characters.
        /// </remarks>
        public string CorrelationTag { get; private set; }
        public TAckCodesEnum AcknowledgementCode { get; private set; }

        private TL1Acknowledgement()
        {

        }

        public static bool MatchesFormat(string s)
        {
            return FirstLineRegex.IsMatch(s);
        }

        public static TL1Acknowledgement<TAckCodesEnum> Parse(string firstLine, TextReader reader)
        {
            var match = FirstLineRegex.Match(firstLine);
            var ack = new TL1Acknowledgement<TAckCodesEnum>
            {
                AcknowledgementCode = (TAckCodesEnum) Enum.Parse(typeof(TAckCodesEnum), match.Groups["ack_code"].Value),
                CorrelationTag = match.Groups["ctag"].Value
            };
            if (firstLine.EndsWith("<")) return ack;
            if ((firstLine = reader.ReadLine()) != "<")
                throw new FormatException($"Unexpected acknowledgement message format. Second line is \"{firstLine}\".");
            return ack;
        }
    }

    /// <summary>
    /// An enum representing the basic response codes in acknowledgement messages.
    /// </summary>
    public enum TL1AcknowledgementCodes
    {
        /// <summary>
        /// In Progress - Sent if the NE cannot execute a request within 2 seconds.
        /// </summary>
        IP,
        /// <summary>
        /// Printout Follows - Command execution is in progress and a response will be sent upon completion.
        /// </summary>
        PF,
        /// <summary>
        /// Current stauts is OK - Command has been executed successfully.
        /// </summary>
        OK,
        /// <summary>
        /// No Acknowledgement - Execution status is unknown (an error).
        /// </summary>
        NA,
        /// <summary>
        /// No Good - Valid command, but cannot be executed due to a parameters issue (an error).
        /// </summary>
        NG,
        /// <summary>
        /// Repeat Later - System resources are not currently available to process your command.
        /// </summary>
        RL
    }
}