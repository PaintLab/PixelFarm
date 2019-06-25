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
            _internalTextLayerController.Clear();

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
        public int LineCount => _internalTextLayerController.LineCount;
        //
        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            _internalTextLayerController.ReplaceLocalContent(nBackspace, t);
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<EditableRun> textRuns)
        {
            _internalTextLayerController.ReplaceCurrentLineTextRun(textRuns);
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            _internalTextLayerController.CopyCurrentLine(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            _internalTextLayerController.CopyLine(lineNum, output);
        }
        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            _internalTextLayerController.CopyAllToPlainText(stBuilder);
        }

        public void SplitCurrentLineToNewLine()
        {
            _internalTextLayerController.SplitCurrentLineIntoNewLine();
        }
        public void AddTextRun(EditableRun textspan)
        {
            _internalTextLayerController.AddTextRunToCurrentLine(textspan);
        }
        //
        public EditableRun CurrentTextRun => _internalTextLayerController.CurrentTextRun;
        //
        public void GetSelectedText(StringBuilder output)
        {
            _internalTextLayerController.CopySelectedTextToPlainText(output);
        }
         
    }
}