//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.TextBreak;

namespace LayoutFarm.Composers
{
    public class MyManagedTextBreaker : ITextBreaker
    {
        CustomBreaker _textBreaker;
        public MyManagedTextBreaker()
        {
            //TODO: review config folder here            
            _textBreaker = CustomBreakerBuilder.NewCustomBreaker();
        }
        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList)
        {
            _textBreaker.BreakWords(inputBuffer, startIndex, len);
            _textBreaker.CopyBreakResults(breakAtList);

        }
        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<WordBreakInfo> breakAtList)
        {

        }
    }
}