//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    partial class EditableTextLine
    {

        public void AddLineBreakAfter(EditableRun afterTextRun)
        {
            if (afterTextRun == null)
            {
                this.EndWithLineBreak = true;
                EditableTextLine newline = _editableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                if (_editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else if (afterTextRun.NextRun == null)
            {
                this.EndWithLineBreak = true;
                EditableTextLine newline = _editableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                if (_editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else
            {
                
                //TODO: use pool
                List<EditableRun> tempTextRuns = new List<EditableRun>(this.RunCount);
                if (afterTextRun != null)
                {
                    foreach (EditableRun t in GetVisualElementForward(afterTextRun.NextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }

                this.EndWithLineBreak = true;
                this.LocalSuspendLineReArrange();

                EditableTextLine newTextline = _editableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                int j = tempTextRuns.Count;
                newTextline.LocalSuspendLineReArrange();
                int cx = 0;
                for (int i = 0; i < j; ++i)
                {
                    EditableRun t = tempTextRuns[i];
                    this.Remove(t);
                    newTextline.AddLast(t);
                    EditableRun.DirectSetLocation(t, cx, 0);
                    cx += t.Width;
                }

                newTextline.LocalResumeLineReArrange();
                this.LocalResumeLineReArrange();
            }
        }
        void AddLineBreakBefore(EditableRun beforeTextRun)
        {
            if (beforeTextRun == null)
            {
                this.EndWithLineBreak = true;
                _editableFlowLayer.InsertNewLine(_currentLineNumber + 1);
            }
            else
            {
                //TODO: use pool
                List<EditableRun> tempTextRuns = new List<EditableRun>();
                if (beforeTextRun != null)
                {
                    foreach (EditableRun t in GetVisualElementForward(beforeTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }
                this.EndWithLineBreak = true;
                EditableTextLine newTextline = _editableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                this.LocalSuspendLineReArrange();
                newTextline.LocalSuspendLineReArrange();
                int j = tempTextRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    EditableRun t = tempTextRuns[i];
                    this.Remove(t);
                    newTextline.AddLast(t);
                }
                this.LocalResumeLineReArrange();
                newTextline.LocalResumeLineReArrange();
            }
        }

        void RemoveLeft(EditableRun t)
        {
            if (t != null)
            {

                LinkedList<EditableRun> tobeRemoveTextRuns = CollectLeftRuns(t);
                LinkedListNode<EditableRun> curNode = tobeRemoveTextRuns.First;
                LocalSuspendLineReArrange();
                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
                LocalResumeLineReArrange();
            }
        }
        void RemoveRight(EditableRun t)
        {

            LinkedList<EditableRun> tobeRemoveTextRuns = CollectRightRuns(t);
            LinkedListNode<EditableRun> curNode = tobeRemoveTextRuns.First;
            LocalSuspendLineReArrange();
            while (curNode != null)
            {
                Remove(curNode.Value);
                curNode = curNode.Next;
            }
            LocalResumeLineReArrange();
        }

        LinkedList<EditableRun> CollectLeftRuns(EditableRun t)
        {

            LinkedList<EditableRun> colllectRun = new LinkedList<EditableRun>();
            foreach (EditableRun r in GetVisualElementForward(this.FirstRun, t))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        LinkedList<EditableRun> CollectRightRuns(EditableRun t)
        {

            LinkedList<EditableRun> colllectRun = new LinkedList<EditableRun>();
            foreach (EditableRun r in _editableFlowLayer.TextRunForward(t, this.LastRun))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        public void ReplaceAll(IEnumerable<EditableRun> textRuns)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            if (textRuns != null)
            {
                foreach (EditableRun r in textRuns)
                {
                    this.AddLast(r);
                }
            }

            this.LocalResumeLineReArrange();
        }
    }
}