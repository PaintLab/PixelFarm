//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.Text;

namespace LayoutFarm.TextFlow
{

    public enum DocumentActionName
    {
        CharTyping,
        SplitToNewLine,
        JointWithNextLine,
        DeleteChar,
        DeleteText,
        InsertText,
        FormatDocument
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
            EndLineNumber = _startLineNumber;//default
        }
        public abstract ChangeRegion ChangeRegion { get; }
        public int StartLineNumber => _startLineNumber;
        public int StartCharIndex => _startCharIndex;
        public int EndLineNumber { get; protected set; }
        public abstract void InvokeUndo(ITextFlowEditSession editSess);
        public abstract void InvokeRedo(ITextFlowEditSession editSess);
    }

    public class DocActionCharTyping : DocumentAction
    {
        readonly int _c;
        public DocActionCharTyping(int c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            _c = c;
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.Line;
        public int Char => _c;
        public override DocumentActionName Name => DocumentActionName.CharTyping;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex + 1);
            editSess.DoBackspace();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
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
    public class DocActionJoinWithNextLine : DocumentAction
    {
        public DocActionJoinWithNextLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.JointWithNextLine;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.SplitIntoNewLine();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.DoDelete();
        }
    }



    public class DocActionDeleteChar : DocumentAction
    {
        readonly int _c;
        public DocActionDeleteChar(int c, int lineNumber, int editSess)
            : base(lineNumber, editSess)
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
    public class DocActionDeleteText : DocumentAction
    {
        readonly TextCopyBuffer _deletedText;
        readonly int _endCharIndex;
        public DocActionDeleteText(TextCopyBuffer deletedTextRuns, int startLineNum, int startColumnNum,
            int endLineNum, int endColumnNum)
            : base(startLineNum, startColumnNum)
        {
            _deletedText = deletedTextRuns;
            _endCharIndex = endColumnNum;
            EndLineNumber = endLineNum;
        }
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public int EndCharIndex => _endCharIndex;
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
            editSess.TryMoveCaretTo(_endCharIndex);
            editSess.EndSelect();
            editSess.DoDelete();
        }
    }

    public class DocActionInsertText : DocumentAction
    {
        readonly TextCopyBuffer _newText;
        readonly int _endCharIndex;
        public DocActionInsertText(TextCopyBuffer insertingTextRuns,
            int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            _newText = insertingTextRuns;
            _endCharIndex = endCharIndex;
            EndLineNumber = endLineNumber;
        }

        public void CopyContent(System.Text.StringBuilder output)
        {
            _newText.CopyTo(output);
        }

        public int EndCharIndex => _endCharIndex;
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.InsertText;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.StartSelect();
            editSess.CurrentLineNumber = EndLineNumber;
            editSess.TryMoveCaretTo(_endCharIndex);
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
            }
        }
        public void ReverseLastUndoAction()
        {
            if (_reverseUndoAction.Count == 0) { return; } //early exit


            DocumentAction docAction = _reverseUndoAction.Pop();
            docAction.InvokeRedo(_editSession);
            _undoList.AddLast(docAction);
        }

        public DocumentAction PeekCommand => _undoList.Last.Value;

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

        //-----------------
        public override void AddChar(int c)
        {
            //create history
            if (!EnableUndoHistoryRecording) { return; }

            _undoList.AddLast(new DocActionCharTyping(c,
                 CurrentLineNo,
                 CurrentLineNewCharIndex));
        }
        public override void DoBackspace()
        {
            if (!EnableUndoHistoryRecording) { return; }

            PlainTextEditSession tme = _pte;
            if (tme.HasSelection)
            {
                var deletedText = new TextCopyBufferUtf32();
                tme.CopySelection(deletedText);
                tme.GetSelection(
                    out int startLineNo, out int startLineCharIndex,
                    out int endLineNo, out int endLineCharIndex);

                _undoList.AddLast(new DocActionDeleteText(deletedText,
                    startLineNo, startLineCharIndex,
                    endLineNo, endLineCharIndex));
            }
            else
            {
                int tempChar = _pte.TempCopyBuffer.GetChar(0);
                _undoList.AddLast(new DocActionDeleteChar(tempChar, _pte.CurrentLineNumber, _pte.NewCharIndex - 1));
            }
        }
        public override void DoDelete()
        {
            if (!EnableUndoHistoryRecording) { return; }

            PlainTextEditSession tme = _pte;
            var deletedText = new TextCopyBufferUtf32();
            tme.CopySelection(deletedText);
            tme.GetSelection(
                out int startLineNo, out int startLineCharIndex,
                out int endLineNo, out int endLineCharIndex);

            _undoList.AddLast(new DocActionDeleteText(deletedText,
                startLineNo, startLineCharIndex,
                endLineNo, endLineCharIndex));

        }
        public override void AddText(TextCopyBuffer buffer)
        {
            if (!EnableUndoHistoryRecording) { return; }

            PlainTextEditSession tme = _pte;
            tme.GetSelection(
                out int startLineNo, out int startLineCharIndex,
                out int endLineNo, out int endLineCharIndex);

            _undoList.AddLast(new DocActionInsertText(buffer,
                startLineNo, startLineCharIndex,
                endLineNo, endLineCharIndex));
        }
        public override void SplitIntoNewLine()
        {
            if (!EnableUndoHistoryRecording) { return; }

            _undoList.AddLast(new DocActionSplitToNewLine(CurrentLineNo, CurrentLineNewCharIndex));
        }
    }
}