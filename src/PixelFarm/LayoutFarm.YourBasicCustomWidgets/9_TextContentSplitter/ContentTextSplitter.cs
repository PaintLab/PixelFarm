//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Composers;
namespace LayoutFarm.CustomWidgets
{

    public class ContentTextSplitter
    {

        ITextBreaker textBreaker;
        List<int> breakAtList = new List<int>();
        public ContentTextSplitter()
        {
            this.textBreaker = LayoutFarm.Composers.Default.TextBreaker;
        }
        public ITextBreaker TextBreaker
        {
            get { return this.textBreaker; }
            set { this.textBreaker = value; }
        }
        public IEnumerable<TextSplitBound> ParseWordContent(char[] textBuffer, int startIndex, int appendLength)
        {
            if (appendLength > 0)
            {
                int s_index = startIndex;

                textBreaker.DoBreak(textBuffer, startIndex, appendLength, breakAtList);

                int j = breakAtList.Count;
                int pos = 0;
                for (int i = 0; i < j; ++i)
                {
                    int sepAt = breakAtList[i];
                    int len = sepAt - pos;
                    yield return new TextSplitBound(s_index, len);
                    s_index = startIndex + sepAt;
                    pos = sepAt;
                }

                if (s_index < textBuffer.Length)
                {
                    yield return new TextSplitBound(s_index, textBuffer.Length - s_index);
                }
                breakAtList.Clear();
            }

        }
    }
}