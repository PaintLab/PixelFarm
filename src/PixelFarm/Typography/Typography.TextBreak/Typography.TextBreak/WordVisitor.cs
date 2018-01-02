//MIT, 2016-2018, WinterDev
// some code from icu-project
// © 2016 and later: Unicode, Inc. and others.
// License & terms of use: http://www.unicode.org/copyright.html#License

using System;
using System.Collections.Generic;

namespace Typography.TextBreak
{
    public enum VisitorState
    {
        Init,
        Parsing,
        OutOfRangeChar,
        End,

    }
    public class WordVisitor
    {
        CustomBreaker ownerBreak;
        List<int> breakAtList = new List<int>();
        char[] buffer;
        int bufferLen;
        int startIndex;
        int currentIndex;
        char currentChar;
        int latestBreakAt;

        Stack<int> tempCandidateBreaks = new Stack<int>();


        public WordVisitor(CustomBreaker ownerBreak)
        {
            this.ownerBreak = ownerBreak;
        }
        public void LoadText(char[] buffer, int index)
        {
            this.buffer = buffer;
            this.bufferLen = buffer.Length;
            this.startIndex = currentIndex = index;
            this.currentChar = buffer[currentIndex];
            breakAtList.Clear();
            tempCandidateBreaks.Clear();
            latestBreakAt = 0;
        }
        static char[] s_empty = new char[0];
        public void ResetText()
        {
            
            buffer = s_empty;
            this.bufferLen = 0;        
            startIndex = 0;
            this.currentChar = '\0';
            breakAtList.Clear();
            tempCandidateBreaks.Clear();
            latestBreakAt = 0;

        }
        public VisitorState State
        {
            get;
            set;
        }
        public int CurrentIndex
        {
            get { return this.currentIndex; }
        }
        public char Char
        {
            get { return currentChar; }
        }


        public bool IsEnd
        {
            get { return currentIndex >= bufferLen - 1; }
        }


        public void AddWordBreakAt(int index)
        {

#if DEBUG
            if (index == latestBreakAt)
            {
                throw new NotSupportedException();
            }
#endif
            this.latestBreakAt = index;
            breakAtList.Add(index);
        }
        public int LatestBreakAt
        {
            get { return this.latestBreakAt; }
        }
        public void SetCurrentIndex(int index)
        {
            this.currentIndex = index;
            if (index < buffer.Length)
            {
                currentChar = buffer[index];
            }
            else
            {
                //can't read next
                //the set state= end
                this.State = VisitorState.End;
            }
        }

        public List<int> GetBreakList()
        {
            return breakAtList;
        }

        internal Stack<int> GetTempCandidateBreaks()
        {
            return this.tempCandidateBreaks;
        }


    }

}