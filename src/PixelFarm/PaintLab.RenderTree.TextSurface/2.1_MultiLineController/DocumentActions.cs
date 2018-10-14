//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.Text
{
    abstract class DocumentAction
    {
        protected int startLineNumber;
        protected int startCharIndex;
        public DocumentAction(int lineNumber, int charIndex)
        {
            this.startLineNumber = lineNumber;
            this.startCharIndex = charIndex;
        }

        public abstract void InvokeUndo(InternalTextLayerController textLayer);
        public abstract void InvokeRedo(InternalTextLayerController textLayer);
    }

    class DocActionCharTyping : DocumentAction
    {
        char c;
        public DocActionCharTyping(char c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            this.c = c;
        }

        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.DoBackspace();
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.AddCharToCurrentLine(c);
        }
    }

    class DocActionSplitToNewLine : DocumentAction
    {
        public DocActionSplitToNewLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.DoEnd();
            textLayer.DoDelete();
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.SplitCurrentLineIntoNewLine();
        }
    }
    class DocActionJoinWithNextLine : DocumentAction
    {
        public DocActionJoinWithNextLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.SplitCurrentLineIntoNewLine();
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.DoDelete();
        }
    }


    class DocActionDeleteChar : DocumentAction
    {
        char c;
        public DocActionDeleteChar(char c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            this.c = c;
        }
        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.AddCharToCurrentLine(c);
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.DoDelete();
        }
    }
    class DocActionDeleteRange : DocumentAction
    {
        List<EditableRun> deletedTextRuns;
        int endLineNumber;
        int endCharIndex;
        public DocActionDeleteRange(List<EditableRun> deletedTextRuns, int startLineNum, int startColumnNum,
            int endLineNum, int endColumnNum)
            : base(startLineNum, startColumnNum)
        {
            this.deletedTextRuns = deletedTextRuns;
            this.endLineNumber = endLineNum;
            this.endCharIndex = endColumnNum;
        }

        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CancelSelect();
            textLayer.AddTextRunsToCurrentLine(deletedTextRuns);
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.StartSelect();
            textLayer.CurrentLineNumber = endLineNumber;
            textLayer.TryMoveCaretTo(endCharIndex);
            textLayer.EndSelect();
            textLayer.DoDelete();
        }
    }

    class DocActionInsertRuns : DocumentAction
    {
        EditableRun singleInsertTextRun;
        IEnumerable<EditableRun> insertingTextRuns;
        int endLineNumber;
        int endCharIndex;
        public DocActionInsertRuns(IEnumerable<EditableRun> insertingTextRuns,
            int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            this.insertingTextRuns = insertingTextRuns;
            this.endLineNumber = endLineNumber;
            this.endCharIndex = endCharIndex;
        }
        public DocActionInsertRuns(EditableRun insertingTextRun,
           int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            this.singleInsertTextRun = insertingTextRun;
            this.endLineNumber = endLineNumber;
            this.endCharIndex = endCharIndex;
        }
        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.StartSelect();
            textLayer.CurrentLineNumber = endLineNumber;
            textLayer.TryMoveCaretTo(endCharIndex);
            textLayer.EndSelect();
            textLayer.DoDelete();
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            if (singleInsertTextRun != null)
            {
                textLayer.AddTextRunToCurrentLine(singleInsertTextRun);
            }
            else
            {
                textLayer.AddTextRunsToCurrentLine(insertingTextRuns);
            }
        }
    }
    class DocActionFormatting : DocumentAction
    {
        int endLineNumber;
        int endCharIndex;
        TextSpanStyle textStyle;
        public DocActionFormatting(TextSpanStyle textStyle, int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            this.textStyle = textStyle;
            this.endLineNumber = endLineNumber;
            this.endCharIndex = endCharIndex;
        }


        public override void InvokeUndo(InternalTextLayerController textLayer)
        {
            textLayer.CurrentLineNumber = startLineNumber;
            textLayer.TryMoveCaretTo(startCharIndex);
            textLayer.StartSelect();
            textLayer.CurrentLineNumber = endLineNumber;
            textLayer.TryMoveCaretTo(endCharIndex);
            textLayer.EndSelect();
        }
        public override void InvokeRedo(InternalTextLayerController textLayer)
        {
        }
    }

    class DocumentCommandCollection
    {
        LinkedList<DocumentAction> undoList = new LinkedList<DocumentAction>();
        Stack<DocumentAction> reverseUndoAction = new Stack<DocumentAction>();
        int maxCommandsCount = 20;
        InternalTextLayerController textdom;
        public DocumentCommandCollection(InternalTextLayerController textdomManager)
        {
            this.textdom = textdomManager;
        }

        public void Clear()
        {
            undoList.Clear();
            reverseUndoAction.Clear();
        }

        public int MaxCommandCount
        {
            get
            {
                return maxCommandsCount;
            }
            set
            {
                maxCommandsCount = value;
                if (undoList.Count > maxCommandsCount)
                {
                    int diff = undoList.Count - maxCommandsCount;
                    for (int i = 0; i < diff; i++)
                    {
                        undoList.RemoveFirst();
                    }
                }
            }
        }

        public void AddDocAction(DocumentAction docAct)
        {
            if (textdom.EnableUndoHistoryRecording)
            {
                undoList.AddLast(docAct);
                EnsureCapacity();
            }
        }

        void EnsureCapacity()
        {
            if (undoList.Count > maxCommandsCount)
            {
                undoList.RemoveFirst();
            }
        }
        public void UndoLastAction()
        {
            DocumentAction docAction = PopUndoCommand();
            if (docAction != null)
            {
                textdom.EnableUndoHistoryRecording = false;
                docAction.InvokeUndo(textdom);
                textdom.EnableUndoHistoryRecording = true;
                reverseUndoAction.Push(docAction);
            }
        }
        public void ReverseLastUndoAction()
        {
            if (reverseUndoAction.Count > 0)
            {
                textdom.EnableUndoHistoryRecording = false;
                DocumentAction docAction = reverseUndoAction.Pop();
                textdom.EnableUndoHistoryRecording = true;
                docAction.InvokeRedo(textdom);
                undoList.AddLast(docAction);
            }
        }
        public DocumentAction PeekCommand
        {
            get
            {
                return undoList.Last.Value;
            }
        }
        public int Count
        {
            get
            {
                return undoList.Count;
            }
        }
        public DocumentAction PopUndoCommand()
        {
            if (undoList.Count > 0)
            {
                DocumentAction lastCmd = undoList.Last.Value;
                undoList.RemoveLast();
                return lastCmd;
            }
            else
            {
                return null;
            }
        }
    }
}