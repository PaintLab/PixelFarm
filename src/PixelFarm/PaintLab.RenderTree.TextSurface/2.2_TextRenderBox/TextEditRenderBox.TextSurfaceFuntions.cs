//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using System.Text;
namespace LayoutFarm.Text
{
    partial class TextEditRenderBox
    {
        TextSurfaceEventListener textSurfaceEventListener;
        public TextSurfaceEventListener TextSurfaceListener
        {
            get
            {
                return textSurfaceEventListener;
            }
            set
            {
                textSurfaceEventListener = value;
                if (value != null)
                {
                    textSurfaceEventListener.SetMonitoringTextSurface(this);
                }
            }
        }
        bool IsMultiLine
        {
            get
            {
                return _isMultiLine;
            }
        }

        public override void ClearAllChildren()
        {
            _internalTextLayerController.Clear();
            this._textLayer.Clear();
            base.ClearAllChildren();
        }



        static Stack<StringBuilder> stringBuilderPool = new Stack<StringBuilder>();
        static StringBuilder GetFreeStringBuilder()
        {
            if (stringBuilderPool.Count > 0)
            {
                return stringBuilderPool.Pop();
            }
            else
            {
                return new StringBuilder();
            }
        }
        static void ReleaseStringBuilder(StringBuilder stBuilder)
        {
            stBuilder.Length = 0;
            stringBuilderPool.Push(stBuilder);
        }


        public int LineCount
        {
            get
            {
                return _internalTextLayerController.LineCount;
            }
        }
        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            _internalTextLayerController.ReplaceLocalContent(nBackspace, t);
        }
        public void LoadTextRun(IEnumerable<EditableRun> textRuns)
        {
            _internalTextLayerController.LoadTextRun(textRuns);
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<EditableRun> textRuns)
        {
            _internalTextLayerController.ReplaceCurrentLineTextRun(textRuns);
        }
        /// <summary>
        /// replace specific line number with textruns
        /// </summary>
        /// <param name="lineNum"></param>
        /// <param name="textRuns"></param>
        public void ReplaceLine(int lineNum, IEnumerable<EditableRun> textRuns)
        {
            _internalTextLayerController.ReplaceLine(lineNum, textRuns);
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
            this._internalTextLayerController.SplitCurrentLineIntoNewLine();
        }
        public void AddTextRun(EditableRun textspan)
        {
            _internalTextLayerController.AddTextRunToCurrentLine(textspan);
        }

        public EditableRun CreateEditableTextRun(string str)
        {
            var span = new EditableTextRun(this.Root, str, this.currentSpanStyle);
            span.UpdateRunWidth();
            return span;
        }
        public EditableRun CreateEditableTextRun(char[] charBuffer)
        {
            var span = new EditableTextRun(this.Root, charBuffer, this.currentSpanStyle);
            span.UpdateRunWidth();
            return span;
        }
        public EditableRun CreateFreezeTextRun(char[] charBuffer)
        {
            var span = new SolidTextRun(this.Root, charBuffer, this.currentSpanStyle);
            span.UpdateRunWidth();
            return span;
        }

        public EditableRun CurrentTextRun
        {
            get
            {
                return _internalTextLayerController.CurrentTextRun;
            }
        }
        public void GetSelectedText(StringBuilder output)
        {
            _internalTextLayerController.CopySelectedTextToPlainText(output);
        }
    }
}