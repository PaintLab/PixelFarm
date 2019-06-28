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
        public class TextLine
        {
            LinkedList<CopyRun> _runs = new LinkedList<CopyRun>();
            public TextLine()
            {

            }
            public IEnumerable<CopyRun> GetRunIter()
            {
                var node = _runs.First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;//**
                }

            }
            public int RunCount => _runs.Count;
            public void Append(CopyRun run) => _runs.AddLast(run);
            public void CopyContentToStringBuilder(StringBuilder stbuilder)
            {
                foreach (CopyRun run in _runs)
                {
                    stbuilder.Append(run.RawContent);
                }
            }
        }

        TextLine _currentLine;
        LinkedList<TextLine> _lines;
        public TextRangeCopy()
        {
            _currentLine = new TextLine();//create default blank lines
        }
        public bool HasSomeRuns
        {
            get
            {
                if (_lines == null)
                {
                    return _currentLine.RunCount > 0;
                }
                else
                {
                    //has more than 1 line (at least we have a line break)
                    return true;
                }
            }
        }
        public IEnumerable<TextLine> GetTextLineIter()
        {
            if (_lines == null)
            {
                yield return _currentLine;
            }
            else
            {
                var node = _lines.First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;//***
                }
            }
        }
        public void AppendNewLine()
        {
            if (_lines == null)
            {
                _lines = new LinkedList<TextLine>();
                //add current line ot the collection
                _lines.AddLast(_currentLine);
            }
            //new line
            _currentLine = new TextLine();
            _lines.AddLast(_currentLine);
        }
        public void AddRun(CopyRun copyRun)
        {
            _currentLine.Append(copyRun);
        }
        public void Clear()
        {
            if (_lines != null)
            {
                _lines.Clear();
                _lines = null;
            }
            _currentLine = new TextLine();
        }
        public void CopyContentToStringBuilder(StringBuilder stbuilder)
        {
            if (!HasSomeRuns) return;
            //

            if (_lines == null)
            {
                _currentLine.CopyContentToStringBuilder(stbuilder);
            }
            else
            {
                bool passFirstLine = false;
                foreach (TextLine line in _lines)
                {
                    if (passFirstLine)
                    {
                        stbuilder.AppendLine();
                    }
                    line.CopyContentToStringBuilder(stbuilder);
                    passFirstLine = true;
                }
            }
        }
    }


}