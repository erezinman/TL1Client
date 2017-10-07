using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Text.RegularExpressions;

namespace TL1Client
{
    /// <summary>
    /// Represents a response to a TL1Request.
    /// </summary>
    public class TL1Response : TL1ReceivedHeaderedMessage
    {
        // Response second line format - M^^<ctag>^<result> cr lf
        const string SCND_LINE_REGEX = @"^M  (?<ctag>[a-zA-Z0-9_]{1,6}) (?<result>CMPLD|PRTL|DENY|RTRY|DELAY)$";
        private static readonly Regex SecondLineRegex = new Regex(SCND_LINE_REGEX, RegexOptions.Compiled);

        /// <summary>
        /// The completion result of the request.
        /// </summary>
        public ResponseResult Result { get; private set; }

        /// <summary>
        /// A correlation tag (CTAG) is used to correlate a response or an acknowledgement to an earlier input message.
        /// The CTag is comprised of up to 6 alpha-numeric characters.
        /// </summary>
        /// <remarks>
        /// When a response message is sent, it uses the same CTAG to indicate the command to which it is responding.  It is,
        /// therefore, the user’s responsibility to ensure that CTAGs are unique for each message.  A replicated CTAG will not
        /// cause an error directly, but it will create uncertainty when responses are received with duplicate CTAGs.
        /// </remarks>
        public string CorrelationTag { get; private set; }

        /// <summary>
        /// A collection which contains lines of data that represent the data returned by the response.
        /// </summary>
        public IReadOnlyList<TL1DataLine> AdditionalData { get; private set; }

        /// <summary>
        /// Checks if a messages is a TL1Response according to the second line of the message.
        /// </summary>
        /// <param name="secondLine"></param>
        /// <returns></returns>
        public static bool MatchesFormat(string secondLine)
        {
            return SecondLineRegex.IsMatch(secondLine);
        }


        /// <summary>
        /// Parses the response of type <see cref="TResponse"/>. This method assumes that MatchesFormat(secondLine) = true.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="secondLine"></param>
        /// <param name="reader"></param>
        /// <exception cref="FormatException">Could be thrown in cases of unexpected message format.</exception>
        /// <returns></returns>
        public static TResponse Parse<TResponse>(string secondLine, TextReader reader)
            where TResponse : TL1Response, new()
        {
            var response = new TResponse();
            var match = SecondLineRegex.Match(secondLine);
            if (!match.Success)
                throw new FormatException($"Second line is not in the correct format. Second line was \"{secondLine}\".");

            response.CorrelationTag = match.Groups["ctag"].Value;
            switch (match.Groups["result"].Value)
            {
                case "COMPLD":
                    response.Result = ResponseResult.Completed;
                    break;
                case "PRTL":
                    response.Result = ResponseResult.Partial;
                    break;
                case "DENY":
                    response.Result = ResponseResult.Deny;
                    break;
                case "RTRY":
                    response.Result = ResponseResult.Retry;
                    break;
                case "DELAY":
                    response.Result = ResponseResult.Delay;
                    break;
            }

            List<string> responseData = new List<string>();
            while ((secondLine = reader.ReadLine()) != ";")
                responseData.Add(secondLine);

            response.AdditionalData = response.ParseAdditionalData(responseData);
            return response;
        }

        /// <summary>
        /// When overriden, handles the unhandled lines of data, and returns the parsed data segments.
        /// </summary>
        /// <param name="rawLines">A collection of the raw string data that accompanies the response.</param>
        /// <returns></returns>
        protected virtual IReadOnlyList<TL1DataLine> ParseAdditionalData(IEnumerable<string> rawLines)
        {
            return rawLines.Select((s, i) => new TL1DataLine(i, s)).ToArray();
        }
    }

    public enum ResponseResult
    {
        /// <summary>
        /// Total successful execution of the input command.
        /// </summary>
        Completed,

        /// <summary>
        /// Partial successful execution of the input command. 
        /// </summary>
        Partial,

        /// <summary>
        /// Total denial of the input command.
        /// </summary>
        Deny,

        /// <summary>
        /// Successful queuing of the input command submitted for delayed activation.
        /// </summary>
        Delay,

        /// <summary>
        /// Output response of a input retrieve command that retrieves extensive amount of information from the network element and uses more time for processing.
        /// </summary>
        Retry
    }
}