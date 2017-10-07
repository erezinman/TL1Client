namespace TL1Client
{
    public class TL1DataLine
    {
        public int LineIndex { get; }
        public string RawLine { get; }

        public TL1DataLine(int lineIndex, string rawLine)
        {
            LineIndex = lineIndex;
            RawLine = rawLine;
        }
    }
}