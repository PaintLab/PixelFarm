//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm.TextEditing.Commands
{
    public enum DocumentActionName
    {
        CharTyping,
        SplitToNewLine,
        JointWithNextLine,
        DeleteChar,
        DeleteRange,
        InsertRuns,
        FormatDocument
    }
    public interface ITextLayerController
    {
        int CurrentLineNumber { get; set; }
        void TryMoveCaretTo(int charIndex, bool backward = false);
        bool DoBackspace();
        void AddCharToCurrentLine(char c);
        VisualSelectionRangeSnapShot DoDelete();
        void DoEnd();
        void DoHome();
        void SplitCurrentLineIntoNewLine();
        void AddTextRunsToCurrentLine(TextRangeCopy copy);
        void AddTextRunToCurrentLine(CopyRun copy);
        //
        void StartSelect();
        void EndSelect();
        void CancelSelect();
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
        public int StartLineNumber => _startLineNumber;
        public int StartCharIndex => _startCharIndex;
        public abstract void InvokeUndo(ITextLayerController textLayer);
        public abstract void InvokeRedo(ITextLayerController textLayer);
    }

    public class DocActionCharTyping : DocumentAction
    {
        readonly char _c;
        public DocActionCharTyping(char c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            _c = c;
        }
        public char Char => _c;
        public override DocumentActionName Name => DocumentActionName.CharTyping;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.DoBackspace();
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.AddCharToCurrentLine(_c);
        }
    }


    public class DocActionSplitToNewLine : DocumentAction
    {
        public DocActionSplitToNewLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override DocumentActionName Name => DocumentActionName.SplitToNewLine;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.DoEnd();
            textLayer.DoDelete();
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.SplitCurrentLineIntoNewLine();
        }
    }
    public class DocActionJoinWithNextLine : DocumentAction
    {
        public DocActionJoinWithNextLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override DocumentActionName Name => DocumentActionName.JointWithNextLine;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.SplitCurrentLineIntoNewLine();
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.DoDelete();
        }
    }


    public class DocActionDeleteChar : DocumentAction
    {
        readonly char _c;
        public DocActionDeleteChar(char c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            _c = c;
        }
        public char Char => _c;
        public override DocumentActionName Name => DocumentActionName.DeleteChar;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.AddCharToCurrentLine(_c);
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.DoDelete();
        }
    }
    public class DocActionDeleteRange : DocumentAction
    {
        TextRangeCopy _deletedTextRuns;
        readonly int _endLineNumber;
        readonly int _endCharIndex;
        public DocActionDeleteRange(TextRangeCopy deletedTextRuns, int startLineNum, int startColumnNum,
            int endLineNum, int endColumnNum)
            : base(startLineNum, startColumnNum)
        {
            _deletedTextRuns = deletedTextRuns;
            _endLineNumber = endLineNum;
            _endCharIndex = endColumnNum;
        }
        public int EndLineNumber => _endLineNumber;
        public int EndCharIndex => _endCharIndex;
        public override DocumentActionName Name => DocumentActionName.DeleteRange;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CancelSelect();
            //add text to lines...
            //TODO: check if we need to preserve format or not?
            textLayer.AddTextRunsToCurrentLine(_deletedTextRuns);
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.StartSelect();
            textLayer.CurrentLineNumber = _endLineNumber;
            textLayer.TryMoveCaretTo(_endCharIndex);
            textLayer.EndSelect();
            textLayer.DoDelete();
        }
    }

    public class DocActionInsertRuns : DocumentAction
    {
        CopyRun _singleInsertTextRun;
        TextRangeCopy _insertingTextRuns;
        int _endLineNumber;
        int _endCharIndex;
        public DocActionInsertRuns(TextRangeCopy insertingTextRuns,
            int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            _insertingTextRuns = insertingTextRuns;
            _endLineNumber = endLineNumber;
            _endCharIndex = endCharIndex;
        }

        public DocActionInsertRuns(CopyRun insertingTextRun,
           int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            _singleInsertTextRun = insertingTextRun;
            _endLineNumber = endLineNumber;
            _endCharIndex = endCharIndex;
        }
        public void CopyContent(System.Text.StringBuilder output)
        {
            if (_singleInsertTextRun != null)
            {
                output.Append(_singleInsertTextRun.RawContent);
            }
            else
            {
                _insertingTextRuns.CopyContentToStringBuilder(output);
            }
        }

        public int EndLineNumber => _endLineNumber;
        public int EndCharIndex => _endCharIndex;
        public override DocumentActionName Name => DocumentActionName.InsertRuns;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.StartSelect();
            textLayer.CurrentLineNumber = _endLineNumber;
            textLayer.TryMoveCaretTo(_endCharIndex);
            textLayer.EndSelect();
            textLayer.DoDelete();
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            if (_singleInsertTextRun != null)
            {
                textLayer.AddTextRunToCurrentLine(_singleInsertTextRun);
            }
            else
            {
                textLayer.AddTextRunsToCurrentLine(_insertingTextRuns);
            }
        }
    }
    public class DocActionFormatting : DocumentAction
    {
        int _endLineNumber;
        int _endCharIndex;
        TextSpanStyle _textStyle;
        public DocActionFormatting(TextSpanStyle textStyle, int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            _textStyle = textStyle;
            _endLineNumber = endLineNumber;
            _endCharIndex = endCharIndex;
        }
        public int EndLineNumber => _endLineNumber;
        public int EndCharIndex => _endCharIndex;
        public override DocumentActionName Name => DocumentActionName.FormatDocument;
        public override void InvokeUndo(ITextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = _startLineNumber;
            textLayer.TryMoveCaretTo(_startCharIndex);
            textLayer.StartSelect();
            textLayer.CurrentLineNumber = _endLineNumber;
            textLayer.TryMoveCaretTo(_endCharIndex);
            textLayer.EndSelect();
        }
        public override void InvokeRedo(ITextLayerController textLayer)
        {
        }
    }

    public class DocumentCommandListener
    {
        public virtual void AddDocAction(DocumentAction docAct)
        {
        }
        public virtual void RefreshLineContent(int lineNum, System.Text.StringBuilder line)
        {
        }

    }

    class DocumentCommandCollection
    {
        LinkedList<DocumentAction> _undoList = new LinkedList<DocumentAction>();
        Stack<DocumentAction> _reverseUndoAction = new Stack<DocumentAction>();
        int _maxCommandsCount = 20;
        InternalTextLayerController _textLayerController;
        DocumentCommandListener _docCmdListner;
        public DocumentCommandCollection(InternalTextLayerController textdomManager)
        {
            _textLayerController = textdomManager;
        }
        public DocumentCommandListener Listener
        {
            get => _docCmdListner;
            set
            {
                _docCmdListner = value;
            }
        }
        public void Clear()
        {
            _undoList.Clear();
            _reverseUndoAction.Clear();
        }

        public int MaxCommandCount
        {
            get
            {
                return _maxCommandsCount;
            }
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

        public void AddDocAction(DocumentAction docAct)
        {
            if (_textLayerController.EnableUndoHistoryRecording)
            {
                _undoList.AddLast(docAct);
                _docCmdListner?.AddDocAction(docAct);

                EnsureCapacity();
            }
        }

        void EnsureCapacity()
        {
            if (_undoList.Count > _maxCommandsCount)
            {
                _undoList.RemoveFirst();
            }
        }
        public void UndoLastAction()
        {
            DocumentAction docAction = PopUndoCommand();
            if (docAction != null)
            {
                _textLayerController.EnableUndoHistoryRecording = false;
                _textLayerController.UndoMode = true;

                docAction.InvokeUndo(_textLayerController);
                //sync content ... 



                System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
                _textLayerController.CopyCurrentLine(stbuilder);
                _docCmdListner?.RefreshLineContent(_textLayerController.CurrentLineNumber, stbuilder);


                _textLayerController.EnableUndoHistoryRecording = true;
                _textLayerController.UndoMode = false;
                _reverseUndoAction.Push(docAction);
            }
        }
        public void ReverseLastUndoAction()
        {
            if (_reverseUndoAction.Count > 0)
            {

                _textLayerController.EnableUndoHistoryRecording = false;
                _textLayerController.UndoMode = true;

                DocumentAction docAction = _reverseUndoAction.Pop();

                _textLayerController.UndoMode = false;
                _textLayerController.EnableUndoHistoryRecording = true;

                docAction.InvokeRedo(_textLayerController);
                _undoList.AddLast(docAction);
            }
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
    }
}