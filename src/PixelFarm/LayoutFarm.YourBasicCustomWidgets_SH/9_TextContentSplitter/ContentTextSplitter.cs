//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.Composers
{
    //temp here, 
    //these will be moved later
    public struct WordBreakInfo
    {
        public int breakAt;
        public byte wordKind;
    }
    public interface ITextBreaker
    {
        void DoBreak(char[] inputBuffer, int startIndex, int len, List<WordBreakInfo> breakAtList);
        void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList);
    }

    //TODO: review here
    public struct TextSplitBounds
    {
        public readonly int startIndex;
        public readonly int length;
        public TextSplitBounds(int startIndex, int length)
        {
            this.startIndex = startIndex;
            this.length = length;
        }

        public int RightIndex => startIndex + length;

        public static readonly TextSplitBounds Empty = new TextSplitBounds();

#if DEBUG
        public override string ToString()
        {
            return startIndex + ":+" + length;
        }
#endif

    }
    //TODO: review here
    public static class Default
    {
        public static ITextBreaker TextBreaker { get; set; }
    }

}

namespace LayoutFarm.CustomWidgets
{

    using LayoutFarm.Composers;
    public class ContentTextSplitter
    {

        ITextBreaker _textBreaker;
        List<int> _breakAtList = new List<int>();
        public ContentTextSplitter()
        {
            _textBreaker = Default.TextBreaker;
        }
        public ITextBreaker TextBreaker
        {
            get => _textBreaker;
            set => _textBreaker = value;
        }
        public IEnumerable<TextSplitBounds> ParseWordContent(char[] textBuffer, int startIndex, int appendLength)
        {
            if (appendLength > 0)
            {
                int s_index = startIndex;

                _textBreaker.DoBreak(textBuffer, startIndex, appendLength, _breakAtList);

                int j = _breakAtList.Count;
                int pos = 0;
                for (int i = 0; i < j; ++i)
                {
                    int sepAt = _breakAtList[i];
                    int len = sepAt - pos;
                    yield return new TextSplitBounds(s_index, len);
                    s_index = startIndex + sepAt;
                    pos = sepAt;
                }

                if (s_index < textBuffer.Length)
                {
                    yield return new TextSplitBounds(s_index, textBuffer.Length - s_index);
                }
                _breakAtList.Clear();
            }

        }
    }
}