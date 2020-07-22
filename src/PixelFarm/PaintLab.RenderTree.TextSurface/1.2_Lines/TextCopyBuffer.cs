//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.TextEditing
{
    public enum RunKind : byte
    {
        Text,
        Image,
        Solid
    }

    public class CopyRun
    {
        public RunKind RunKind { get; set; }
        public char[] RawContent { get; set; }

        public CopyRun(char[] rawContent)
        {
            RawContent = rawContent;
        }

        public int CharacterCount
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Image:
                    case RunKind.Solid: return 1;
                    case RunKind.Text:
                        return RawContent.Length;
                    default: throw new NotSupportedException();
                }
            }
        }
    }

    public class TextCopyBuffer
    {

        readonly StringBuilder _sb;
        public TextCopyBuffer()
        {
            _sb = new StringBuilder();
        }
        public void AppendNewLine()
        {
            //push content of current line 
            //into plain doc
            _sb.AppendLine();
        }
        public IEnumerable<string> GetLineIter()
        {
            //TODO: review this again

            using (System.IO.StringReader reader = new System.IO.StringReader(_sb.ToString()))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }


        public bool HasSomeRuns => _sb.Length > 0;

        public void AppendData(char[] buffer, int start, int len) => _sb.Append(buffer, start, len);

        public void Clear() => _sb.Length = 0;

        internal int Length => _sb.Length;

        internal void CopyTo(char[] charBuffer) => _sb.CopyTo(0, charBuffer, 0, _sb.Length);
        public void CopyTo(StringBuilder stbuilder)
        {
            //TODO: review here 

            char[] buff = new char[_sb.Length];
            _sb.CopyTo(0, buff, 0, buff.Length);

            stbuilder.Append(buff);
        }
        public override string ToString() => _sb.ToString();
    }
}