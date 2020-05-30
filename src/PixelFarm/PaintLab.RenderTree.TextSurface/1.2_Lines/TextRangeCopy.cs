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
        public CopyRun(string rawContent)
        {
            RawContent = rawContent.ToCharArray();
        }
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
        public void CopyContentToStringBuilder(StringBuilder stbuilder)
        {
            throw new NotSupportedException();
            //if (IsLineBreak)
            //{
            //    stBuilder.Append("\r\n");
            //}
            //else
            //{
            //    stBuilder.Append(_mybuffer);
            //}
        }
    }

    public class TextRangeCopy
    {

        //TODO: review this again
        //use PlainTextDocument
        StringBuilder _stbuilder = new StringBuilder();
        public void AppendNewLine()
        {
            //push content of current line 
            //into plain doc
            _stbuilder.AppendLine();
        }
        public IEnumerable<string> GetLineIter()
        {
            //TODO: review this again
            using (System.IO.StringReader reader = new System.IO.StringReader(_stbuilder.ToString()))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }
        public void CopyContentToStringBuilder(StringBuilder stbuilder)
        {
            stbuilder.Append(_stbuilder.ToString());
        }

        public bool HasSomeRuns => _stbuilder.Length > 0;

        /// <summary>
        /// this will copy content of this run
        /// </summary>
        /// <param name="run"></param>
        public void AppendRun(Run run)
        {
            run.WriteTo(_stbuilder);             
        }
        public void AppendRun(CopyRun run)
        {
            _stbuilder.Append(run.RawContent);
        }

    }
}