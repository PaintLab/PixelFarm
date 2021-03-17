//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.Text;

namespace LayoutFarm.TextFlow
{

    public enum DocumentActionName
    {
        AddChar,//add single char
        AddText, //add more than1 char

        SplitToNewLine, //
        DeleteNewLine,
        JoinWithUpperLine,

        DeleteWithBackspace,//delete char with backspace
        DeleteChar,//delete char with delete button

        DeleteText,//delete range of text 

    }
    public interface ITextFlowSelectSession
    {
        int CurrentLineNumber { get; set; }
        void StartSelect();
        void EndSelect();
        void CancelSelect();
        void TryMoveCaretTo(int charIndex, bool backward = false);
        void DoEnd();
        void DoHome();
    }
    public interface ITextFlowEditSession : ITextFlowSelectSession
    {
        //
        void AddChar(int c);
        void AddText(TextCopyBuffer copy);
        //
        void DoDelete();
        bool DoBackspace();
        //
        void SplitIntoNewLine();

    }

    public enum ChangeRegion
    {
        Line,
        LineRange,
        AllLines,
    }
    public abstract class DocumentAction
    {
        public abstract DocumentActionName Name { get; }
        protected int _startLineNumber;
        protected int _startCharIndex;
        public DocumentAction(int lineNumber, int charIndex)
        {
            _startLineNumber = lineNumber;
            _startCharIndex = charIndex;
        }
        public abstract ChangeRegion ChangeRegion { get; }
        public int StartLineNumber => _startLineNumber;
        public int StartCharIndex => _startCharIndex;

        public abstract void InvokeUndo(ITextFlowEditSession editSess);
        public abstract void InvokeRedo(ITextFlowEditSession editSess);
        public int HxStepNumber { get; set; }
    }

    /// <summary>
    /// sincle chracter typeing
    /// </summary>
    public class DocActionAddChar : DocumentAction
    {
        readonly int _c;
        public DocActionAddChar(int c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            _c = c;
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.Line;
        public int Char => _c;
        public override DocumentActionName Name => DocumentActionName.AddChar;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex + 1);
            editSess.DoBackspace();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            //move to specific
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.AddChar(_c);
        }
#if DEBUG
        public override string ToString()
        {
            return "+" + ((char)_c).ToString();
        }
#endif
    }

    public class DocActionSplitToNewLine : DocumentAction
    {
        public DocActionSplitToNewLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.SplitToNewLine;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.DoEnd();
            editSess.DoDelete();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.SplitIntoNewLine();
        }
    }


    /// <summary>
    /// delete single char
    /// </summary>
    public class DocActionDeleteChar : DocumentAction
    {
        readonly int _c;
        public DocActionDeleteChar(int c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            _c = c;
        }
        public int Char => _c;
        public override ChangeRegion ChangeRegion => ChangeRegion.Line;
        public override DocumentActionName Name => DocumentActionName.DeleteChar;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.AddChar(_c);
            editSess.TryMoveCaretTo(_startCharIndex); //move back
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.DoDelete();
        }
        public override string ToString()
        {
            return "-" + ((char)_c).ToString();
        }
    }

    public class DocActionBackspace : DocumentAction
    {
        readonly int _c;
        public DocActionBackspace(int c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            _c = c;
        }
        public int Char => _c;
        public override ChangeRegion ChangeRegion => ChangeRegion.Line;
        public override DocumentActionName Name => DocumentActionName.DeleteWithBackspace;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex + 1);
            editSess.AddChar(_c);
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.DoBackspace();
        }
        public override string ToString()
        {
            return "-" + ((char)_c).ToString();
        }
    }


    public class DocActionDeleteNewLine : DocumentAction
    {
        public DocActionDeleteNewLine(int lineNumber)
          : base(lineNumber, 0)
        {
        }
        /// <summary>
        /// snapshot char count
        /// </summary>
        public int CurrentLineCharCount { get; set; }
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.DeleteNewLine;

        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(CurrentLineCharCount);
            editSess.SplitIntoNewLine();
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.DoEnd();//
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.DoEnd();
            editSess.DoDelete();
        }
        public override string ToString()
        {
            return "-newline";
        }
    }
    public class DocActionJoinWithUpperLine : DocumentAction
    {
        public DocActionJoinWithUpperLine(int lineNumber)
          : base(lineNumber, 0)
        {
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.JoinWithUpperLine;
        public int PrevLineCharIndex { get; set; }
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber - 1;
            editSess.TryMoveCaretTo(PrevLineCharIndex);

            editSess.SplitIntoNewLine();
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.DoHome();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.DoHome();
            editSess.DoBackspace();
        }
        public override string ToString()
        {
            return "-newline";
        }
    }

    /// <summary>
    /// delete a range of text
    /// </summary>
    public class DocActionDeleteText : DocumentAction
    {
        readonly TextCopyBuffer _deletedText;
        public DocActionDeleteText(TextCopyBuffer deletedTextRuns, int startLineNum, int startCharIndex,
            int endLineNum, int endColumnNum)
            : base(startLineNum, startCharIndex)
        {
            _deletedText = deletedTextRuns;
            EndLineNumber = endLineNum;
            EndCharIndex = endColumnNum;
        }

        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public int EndCharIndex { get; }
        public int EndLineNumber { get; }
        public override DocumentActionName Name => DocumentActionName.DeleteText;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CancelSelect();
            //add text to lines...
            //TODO: check if we need to preserve format or not?
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.AddText(_deletedText);
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.StartSelect();
            editSess.CurrentLineNumber = EndLineNumber;
            editSess.TryMoveCaretTo(EndCharIndex);
            editSess.EndSelect();
            editSess.DoDelete();
        }
    }

    /// <summary>
    /// insert text
    /// </summary>
    public class DocActionInsertText : DocumentAction
    {
        readonly TextCopyBuffer _newText;
        bool _eval_textRange;
        int _endLineNo;
        int _endLineCharIndex;

        public DocActionInsertText(TextCopyBuffer insertingTextRuns, int startLineNumber, int startCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            //if we have selection
            _newText = insertingTextRuns;
        }
        public void CopyContent(System.Text.StringBuilder output)
        {
            _newText.CopyTo(output);
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.AddText;

        void EvalTextRange()
        {
            if (_eval_textRange) { return; } //once

            _newText.GetReader(out Typography.TextBreak.InputReader r);
            _endLineNo = _startLineNumber;
            _endLineCharIndex = _startLineNumber + _newText.Length;
            while (r.ReadLine(out int begin, out int len, out Typography.TextBreak.InputReader.LineEnd endWith))
            {
                //text may be multi line                
                if (endWith != Typography.TextBreak.InputReader.LineEnd.None)
                {
                    _endLineCharIndex = len;
                    _endLineNo++;
                }
            }
            _eval_textRange = true;
        }

        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            //have selection or not
            //delete test
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.StartSelect();
            //how many line involve this
            EvalTextRange();
            //
            editSess.CurrentLineNumber = _endLineNo;
            editSess.TryMoveCaretTo(_endLineCharIndex);
            editSess.EndSelect();
            editSess.DoDelete();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.AddText(_newText);
        }
    }

    //--------------------------------

    public class HistoryCollector : TextFlowEditSessionListener
    {
        readonly LinkedList<DocumentAction> _undoList = new LinkedList<DocumentAction>();
        readonly Stack<DocumentAction> _reverseUndoAction = new Stack<DocumentAction>();

        int _maxCommandsCount = 20; //TODO: configurable
        ITextFlowEditSession _editSession;
        PlainTextEditSession _pte;
        public HistoryCollector(ITextFlowEditSession editSession, PlainTextEditSession pte)
        {
            _editSession = editSession;
            _pte = pte;
            EnableUndoHistoryRecording = true;
        }
        public void Clear()
        {
            _undoList.Clear();
            _reverseUndoAction.Clear();
        }
        public bool EnableUndoHistoryRecording { get; set; }
        public int MaxCommandCount
        {
            get => _maxCommandsCount;
            set
            {
                _maxCommandsCount = value;
                if (_undoList.Count > _maxCommandsCount)
                {
                    int diff = _undoList.Count - _maxCommandsCount;
                    for (int i = 0; i < diff; i++)
                    {
                        _undoList.RemoveFirst();
                    }
                }
            }
        }


        void EnsureCapacity()
        {
            if (_undoList.Count > _maxCommandsCount)
            {
                _undoList.RemoveFirst();
            }
        }
        public void UndoLastAction(ITextFlowEditSession textEditSession)
        {
            DocumentAction docAction = PopUndoCommand();
            if (docAction != null)
            {
                docAction.InvokeUndo(textEditSession);
                //sync content ...   
                _reverseUndoAction.Push(docAction);
                //check if next command has the same step number?
                while (Count > 0)
                {
                    DocumentAction cmd = PeekCommand();
                    if (cmd.HxStepNumber == docAction.HxStepNumber)
                    {
                        cmd.InvokeUndo(textEditSession);
                        //sync content ...   
                        _reverseUndoAction.Push(cmd);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        public void ReverseLastUndoAction()
        {
            if (_reverseUndoAction.Count == 0) { return; } //early exit

            DocumentAction docAction = _reverseUndoAction.Pop();
            docAction.InvokeRedo(_editSession);
            _undoList.AddLast(docAction);
        }

        public DocumentAction PeekCommand() => _undoList.Last.Value;

        public int Count => _undoList.Count;

        public DocumentAction PopUndoCommand()
        {
            if (_undoList.Count > 0)
            {
                DocumentAction lastCmd = _undoList.Last.Value;
                _undoList.RemoveLast();
                return lastCmd;
            }
            else
            {
                return null;
            }
        }

        public override void AddChar(int c)
        {

            if (!EnableUndoHistoryRecording) { return; }

            AppendToUndoList(new DocActionAddChar(c,
                 CurrentLineNo,
                 CurrentLineNewCharIndex));
        }

        void AppendToUndoList(DocumentAction action)
        {
            action.HxStepNumber = HxStepNumber;
            _undoList.AddLast(action);
        }
        public override void DoBackspace()
        {
            if (!EnableUndoHistoryRecording) { return; }

            if (_pte.HasSelection)
            {
                var deletedText = new TextCopyBufferUtf32();
                _pte.CopySelection(deletedText);
                _pte.GetSelection(
                    out int startLineNo, out int startLineCharIndex,
                    out int endLineNo, out int endLineCharIndex);

                AppendToUndoList(new DocActionDeleteText(deletedText,
                    startLineNo, startLineCharIndex,
                    endLineNo, endLineCharIndex));
            }
            else
            {
                if (_pte.NewCharIndex == 0)
                {
                    //goto upper
                    if (_pte.CurrentLineNumber > 0)
                    {
                        //get previous line
                        int prevLineCharCount = _pte.GetLineCharCount(_pte.CurrentLineNumber - 1);
                        AppendToUndoList(new DocActionJoinWithUpperLine(_pte.CurrentLineNumber) { PrevLineCharIndex = prevLineCharCount });
                    }
                }
                else
                {
                    int tempChar = _pte.TempCopyBuffer.GetChar(0);
                    AppendToUndoList(new DocActionBackspace(tempChar, _pte.CurrentLineNumber, _pte.NewCharIndex));
                }

            }
        }
        public override void DoDelete()
        {
            if (!EnableUndoHistoryRecording) { return; }

            if (_pte.HasSelection)
            {
                var deletedText = new TextCopyBufferUtf32();
                _pte.CopySelection(deletedText);
                _pte.GetSelection(
                    out int startLineNo, out int startLineCharIndex,
                    out int endLineNo, out int endLineCharIndex);

                AppendToUndoList(new DocActionDeleteText(deletedText,
                    startLineNo, startLineCharIndex,
                    endLineNo, endLineCharIndex));
            }
            else
            {
                if (_pte.IsOnTheEnd())
                {
                    //end of the line
                    if (CurrentLineNo < _pte.LineCount - 1)
                    {
                        //
                        AppendToUndoList(new DocActionDeleteNewLine(_pte.CurrentLineNumber) { CurrentLineCharCount = _pte.GetLineCharCount(_pte.CurrentLineNumber) });
                    }

                }
                else
                {
                    int tempChar = _pte.TempCopyBuffer.GetChar(0);
                    AppendToUndoList(new DocActionDeleteChar(tempChar, _pte.CurrentLineNumber, _pte.NewCharIndex));
                }
            }
        }

        public override void AddText(TextCopyBuffer buffer)
        {
            if (!EnableUndoHistoryRecording) { return; }
#if DEBUG
            if (_pte.HasSelection) { throw new NotSupportedException(); }
#endif

            AppendToUndoList(new DocActionInsertText(buffer, CurrentLineNo, CurrentLineNewCharIndex));

        }
        public override void SplitIntoNewLine()
        {
            if (!EnableUndoHistoryRecording) { return; }

            AppendToUndoList(new DocActionSplitToNewLine(CurrentLineNo, CurrentLineNewCharIndex));
        }

        //-----------------
        internal int HxStepNumber { get; private set; }
        internal void ResetHxStepNumber()
        {
            HxStepNumber = 0;
        }
        internal void IncrementHxStepNumber()
        {
            if (!EnableUndoHistoryRecording) { return; }
            HxStepNumber++;
        }
    }
}