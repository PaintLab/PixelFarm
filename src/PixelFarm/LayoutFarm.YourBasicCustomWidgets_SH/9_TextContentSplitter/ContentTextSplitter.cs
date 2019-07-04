//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Composers;
namespace LayoutFarm.CustomWidgets
{

    public class ContentTextSplitter
    {

        ITextBreaker _textBreaker;
        List<int> _breakAtList = new List<int>();
        public ContentTextSplitter()
        {
            _textBreaker = LayoutFarm.Composers.Default.TextBreaker;
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