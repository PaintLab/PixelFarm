//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.Text;

namespace LayoutFarm.TextEditing.Commands
{
    public class DocActionFormatting : DocumentAction
    {

        readonly int _endCharIndex;
        readonly TextSpanStyle _textStyle;
        public DocActionFormatting(TextSpanStyle textStyle, int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            _textStyle = textStyle;
            _endCharIndex = endCharIndex;
            EndLineNumber = endLineNumber;
        }
        public int EndCharIndex => _endCharIndex;
        public override ChangeRegion ChangeRegion => ChangeRegion.LineRange;
        public override DocumentActionName Name => DocumentActionName.FormatDocument;
        public override void InvokeUndo(ITextFlowEditSession editSess)
        {
            editSess.CurrentLineNumber = _startLineNumber;
            editSess.TryMoveCaretTo(_startCharIndex);
            editSess.StartSelect();
            editSess.CurrentLineNumber = EndLineNumber;
            editSess.TryMoveCaretTo(_endCharIndex);
            editSess.EndSelect();
        }
        public override void InvokeRedo(ITextFlowEditSession editSess)
        {
        }
    }

    class DocumentCommandCollection
    {
        readonly LinkedList<DocumentAction> _undoList = new LinkedList<DocumentAction>();
        readonly Stack<DocumentAction> _reverseUndoAction = new Stack<DocumentAction>();
        readonly TextFlowEditSession _editSession;
        int _maxCommandsCount = 20; //TODO: configurable

        public DocumentCommandCollection(TextFlowEditSession textEditSession)
        {
            _editSession = textEditSession;
        }
        public void Clear()
        {
            _undoList.Clear();
            _reverseUndoAction.Clear();
        }

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

        public void AddDocAction(DocumentAction docAct)
        {
            if (_editSession.EnableUndoHistoryRecording)
            {
                _undoList.AddLast(docAct);
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
                _editSession.EnableUndoHistoryRecording = false;
                _editSession.UndoMode = true;

                docAction.InvokeUndo(_editSession);
                //sync content ...   
                _editSession.EnableUndoHistoryRecording = true;
                _editSession.UndoMode = false;
                _reverseUndoAction.Push(docAction);
            }
        }
        public void ReverseLastUndoAction()
        {
            if (_reverseUndoAction.Count > 0)
            {

                _editSession.EnableUndoHistoryRecording = false;
                _editSession.UndoMode = true;

                DocumentAction docAction = _reverseUndoAction.Pop();

                _editSession.UndoMode = false;
                _editSession.EnableUndoHistoryRecording = true;

                docAction.InvokeRedo(_editSession);
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