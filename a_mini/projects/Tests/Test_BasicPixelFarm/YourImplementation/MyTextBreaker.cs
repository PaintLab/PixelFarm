//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.TextBreak;
using LayoutFarm.TextBreak.ICU;

namespace LayoutFarm.Composers
{
    public class MyManagedTextBreaker : ITextBreaker
    {
        CustomBreaker myTextBreaker;
        public MyManagedTextBreaker()
        {
            //TODO: review config folder here            
            myTextBreaker = CustomBreakerBuilder.NewCustomBreaker();
        }
        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList)
        {
            myTextBreaker.BreakWords(inputBuffer, startIndex);
            myTextBreaker.LoadBreakAtList(breakAtList);

        }
    }
    public class MyNativeTextBreaker : ITextBreaker
    {
        NativeTextBreaker myTextBreaker;
        public MyNativeTextBreaker()
        {
            myTextBreaker = new NativeTextBreaker(LayoutFarm.TextBreak.ICU.TextBreakKind.Word, "en-US");
        }
        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList)
        {
            myTextBreaker.DoBreak(inputBuffer, startIndex, len, splitBound =>
            {
                breakAtList.Add(splitBound.startIndex + splitBound.length);
            });
        }
    }

}