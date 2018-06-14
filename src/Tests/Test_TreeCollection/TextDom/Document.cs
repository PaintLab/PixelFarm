//MIT, 2018-present, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
namespace PaintLab.DocumentPro
{
    //immutable theme


    public class Document
    {

    }

    public class TextLine
    {
        char[] _lineChars;
        public TextLine(char[] lineChars, int startAtCharIndex, int len, LineEndKind endLineMark)
        {
            this._lineChars = lineChars;
            this.EndLineMark = endLineMark;
            this.StartAtCharIndex = startAtCharIndex;
            this.Length = len;
        }
        public LineEndKind EndLineMark
        {
            get;
            private set;
        }
        public int StartAtCharIndex { get; private set; }
        public int Length { get; private set; }
        public char GetChatAt(int columnIndex)
        {
            //by convention 
            //first char start at 1
            if (columnIndex < 1)
            {
                return '\0';
            }
            //
            return _lineChars[columnIndex - 1];
        }

        public bool ContainsCharIndex(int charIndex)
        {
            return charIndex >= StartAtCharIndex && charIndex < (StartAtCharIndex + Length);
        }

#if DEBUG
        public override string ToString()
        {
            return new string(_lineChars);
        }
#endif
    }
     
    public class TextSource
    {
        //immutatble text
        char[] _text;
        List<TextLine> _lines = new List<TextLine>();
        public TextSource(char[] text)
        {
            this._text = text;
            //for input text
            //1. separate into lines
            TextLineSplitter.ParseTextToLines(text, _lines);
        }
        /// <summary>
        /// line start at 1, column start at 1
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public char GetCharAt(int line, int column)
        {


            //by convention
            //first line start at 1 and column start at 1
            //1st line = 1-base index
            //1st column = 1-base index


            int internalLineNo = line - 1;
            int internalColNo = column - 1;
            TextLine textline = _lines[internalLineNo];
            return textline.GetChatAt(column);

        }

        /// <summary>
        /// get char from specific index offset 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char GetCharAt(int index)
        {
            //get char at specific index
            return _text[index];
        }

        /// <summary>
        /// insert a single char to the text
        /// </summary>
        /// <param name="index"></param>
        /// <param name="c"></param>
        public void Insert(int index, char c)
        {
            //_text buffer array are changed!
            //1. find a text line at the index
            //2. get the line, just add the to the line
            //-----------------

        }
        TextLine FindTextLine(int charIndex)
        {
            int j = _lines.Count;
            for (int i = 0; i < j; ++i)
            {
                //find a line
                TextLine t = _lines[i];
                if (t.ContainsCharIndex(charIndex))
                {
                    return t;
                }
            }
            return null;
        }

    }





    public enum LineEndKind : byte
    {
        End,
        N, // \n
        R, // \r
        RN //\r\n
    }

    static class TextLineSplitter
    {
        public static void ParseTextToLines(char[] inputTextBuffer, List<TextLine> outputLinePosList)
        {
            int inputBufferLen = inputTextBuffer.Length;
            int lim = inputBufferLen - 1; //**
            int startAt = 0;
            for (int i = 0; i < lim; ++i)
            {
                char c = inputTextBuffer[i];
                if (c == '\n')
                {
                    int len = i - startAt;
                    char[] lineBuff = new char[len];
                    Array.Copy(inputTextBuffer, startAt, lineBuff, 0, len);

                    outputLinePosList.Add(new TextLine(
                        lineBuff, startAt, len,
                        LineEndKind.N));
                    startAt = i + 1;
                }
                else if (c == '\r')
                {
                    if (inputTextBuffer[i + 1] == '\n')
                    {
                        int len = i - startAt;
                        char[] lineBuff = new char[len];
                        Array.Copy(inputTextBuffer, startAt, lineBuff, 0, len);

                        //\r\n
                        outputLinePosList.Add(new TextLine(
                            lineBuff, startAt, len,
                            LineEndKind.RN));
                        i++;
                        startAt = i + 1;
                    }
                    else
                    {
                        int len = i - startAt;
                        char[] lineBuff = new char[len];
                        Array.Copy(inputTextBuffer, startAt, lineBuff, 0, len);

                        //\r
                        outputLinePosList.Add(new TextLine(
                            lineBuff, startAt, len,
                            LineEndKind.R));

                        startAt = i + 1;
                    }
                }
            }
            //for the remaining part

            if (startAt < lim - 1)
            {

                int len = inputBufferLen - startAt;
                char[] lineBuff = new char[len];
                Array.Copy(inputTextBuffer, startAt, lineBuff, 0, len);
                outputLinePosList.Add(new TextLine(
                    lineBuff, startAt, len,
                    LineEndKind.End));
                startAt += len;
            }
            else
            {
                //1 blank line
                char[] lineBuff = new char[0];
                outputLinePosList.Add(new TextLine(
                  lineBuff, startAt, 0,
                  LineEndKind.End));
            }

        }
    }



}