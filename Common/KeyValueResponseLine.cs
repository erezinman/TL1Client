using System.Collections.Generic;
using TL1Client;

namespace TL1Client.Common
{
    public class KeyValueResponseLine : TL1DataLine
    {
        public IReadOnlyDictionary<string, string> Headers { get; }

        public KeyValueResponseLine(int lineIndex, IReadOnlyDictionary<string, string> headers, string rawLine) : base(lineIndex, rawLine)
        {
            Headers = headers;
        }
    }
}