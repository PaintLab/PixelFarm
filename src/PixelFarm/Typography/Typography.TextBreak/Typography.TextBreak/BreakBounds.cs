//MIT, 2016-2017, WinterDev
//some code from ICU project with BSD license

namespace Typography.TextBreak
{

    delegate void OnBreak(BreakBounds breakBounds);

    class BreakBounds
    {
        public int startIndex;
        public int length;
        public bool stopNext;
        public WordKind kind;
    }

    enum WordKind : byte
    {
        Whitespace,
        NewLine,
        Text,
        Number,
        Punc
    } 

}