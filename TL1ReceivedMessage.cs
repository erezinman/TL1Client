using System;
using System.IO;
using TL1Client;

namespace TL1Client
{
    public class TL1ReceivedMessage
    {
        public static TL1ReceivedMessage Parse<TResponse, TAutonomous, TAckCodesEnum>(Stream s)
            where TResponse : TL1Response, new()
            where TAutonomous : TL1AutonomousMessage, new()
            where TAckCodesEnum : struct
        {
            using (var reader = new StreamReader(s))
                return Parse<TResponse, TAutonomous, TAckCodesEnum>(reader);
        }

        public static TL1ReceivedMessage Parse<TResponse, TAutonomous, TAckCodesEnum>(string s)
            where TResponse : TL1Response, new()
            where TAutonomous : TL1AutonomousMessage, new()
            where TAckCodesEnum : struct
        {
            using (var reader = new StringReader(s))
                return Parse<TResponse, TAutonomous, TAckCodesEnum>(reader);
        }

        public static TL1ReceivedMessage Parse<TResponse, TAutonomous, TAckCodesEnum>(TextReader reader)
            where TResponse : TL1Response, new()
            where TAutonomous : TL1AutonomousMessage, new()
            where TAckCodesEnum : struct
        {
            try
            {
                var firstLine = reader.ReadLine();

                if (TL1ReceivedHeaderedMessage.MatchesFormat(firstLine))
                {
                    return TL1ReceivedHeaderedMessage.Parse<TResponse, TAutonomous>(firstLine, reader);
                }
                else if (TL1Acknowledgement<TAckCodesEnum>.MatchesFormat(firstLine))
                {
                    return TL1Acknowledgement<TAckCodesEnum>.Parse(firstLine, reader);
                }
                else
                    throw new FormatException($"Unknwon incomming message - Second line was \"{firstLine}\"");

            }
            catch (Exception ex)
            {
                throw new FormatException("Given response is in an incorrect format.", ex);
            }
        }

        public static TL1ReceivedMessage Parse(TextReader reader)
        {
            return Parse<TL1Response, TL1AutonomousMessage, TL1AcknowledgementCodes>(reader);
        }
        public static TL1ReceivedMessage Parse(Stream s)
        {
            return Parse<TL1Response, TL1AutonomousMessage, TL1AcknowledgementCodes>(s);
        }
        public static TL1ReceivedMessage Parse(string s)
        {
            return Parse<TL1Response, TL1AutonomousMessage, TL1AcknowledgementCodes>(s);
        }
    }
}