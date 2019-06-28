//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.TextEditing
{
    partial class TextEditRenderBox
    {
        TextSurfaceEventListener _textSurfaceEventListener;
        public TextSurfaceEventListener TextSurfaceListener
        {
            get => _textSurfaceEventListener;
            set
            {
                _textSurfaceEventListener = value;
                if (value != null)
                {
                    _textSurfaceEventListener.SetMonitoringTextSurface(this);
                }
            }
        }
        //
        bool IsMultiLine => _isMultiLine;
        //
        public override void ClearAllChildren()
        {
            _editSession.Clear();
            base.ClearAllChildren();
        }

        [System.ThreadStatic]
        static Stack<StringBuilder> s_stringBuilderPool;

        static StringBuilder GetFreeStringBuilder()
        {

            if (s_stringBuilderPool == null)
            {
                s_stringBuilderPool = new Stack<StringBuilder>();
            }

            if (s_stringBuilderPool.Count > 0)
            {
                return s_stringBuilderPool.Pop();
            }
            else
            {
                return new StringBuilder();
            }
        }
        static void ReleaseStringBuilder(StringBuilder stBuilder)
        {
            stBuilder.Length = 0;
            s_stringBuilderPool.Push(stBuilder);
        }

        //
        public int LineCount => _editSession.LineCount;
        //
        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            _editSession.ReplaceLocalContent(nBackspace, t);
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<Run> textRuns)
        {
            _editSession.ReplaceCurrentLineTextRun(textRuns);
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            _editSession.CopyCurrentLine(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            _editSession.CopyLine(lineNum, output);
        }
        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            _editSession.CopyAllToPlainText(stBuilder);
        }
        public void SplitCurrentLineToNewLine()
        {
            _editSession.SplitCurrentLineIntoNewLine();
        }
        public void AddTextRun(Run textspan)
        {
            _editSession.AddTextRunToCurrentLine(textspan);
        }
        public void AddTextRun(char[] buffer)
        {
            _editSession.AddTextRunToCurrentLine(buffer);
        }
        public void AddTextLine(PlainTextLine textLine)
        {
            _editSession.AddTextLine(textLine);
        }
        //
        public Run CurrentTextRun => _editSession.CurrentTextRun;
        //
        public void GetSelectedText(StringBuilder output)
        {
            _editSession.CopySelectedTextToPlainText(output);
        }

    }
}