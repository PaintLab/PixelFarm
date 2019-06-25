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
                //add line break on the last end

                this.EndWithLineBreak = true;
                EditableTextLine newline = EditableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                if (EditableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else if (afterTextRun.NextTextRun == null)
            {
                this.EndWithLineBreak = true;
                EditableTextLine newline = EditableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                if (EditableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else
            {
                List<EditableRun> tempTextRuns = new List<EditableRun>(this.RunCount);
                if (afterTextRun != null)
                {
                    foreach (EditableRun t in GetVisualElementForward(afterTextRun.NextTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }

                this.EndWithLineBreak = true;
                this.LocalSuspendLineReArrange();
                EditableTextLine newTextline = EditableFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                int j = tempTextRuns.Count;
                newTextline.LocalSuspendLineReArrange(); int cx = 0;
                for (int i = 0; i < j; ++i)
                {
                    EditableRun t = tempTextRuns[i];
                    this.Remove(t);
                    newTextline.AddLast(t);
                    RenderElement.DirectSetLocation(t, cx, 0);
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
                EditableFlowLayer.InsertNewLine(_currentLineNumber + 1);
            }
            else
            {
                List<EditableRun> tempTextRuns = new List<EditableRun>();
                if (beforeTextRun != null)
                {
                    foreach (EditableRun t in GetVisualElementForward(beforeTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }
                this.EndWithLineBreak = true;
                EditableTextLine newTextline = EditableFlowLayer.InsertNewLine(_currentLineNumber + 1);
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
                //if (t.IsLineBreak)
                //{
                //    throw new NotSupportedException();
                //}

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
            //if (t.IsLineBreak)
            //{
            //    throw new NotSupportedException();
            //}

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
            //if (t.IsLineBreak)
            //{
            //    throw new NotSupportedException();
            //}

            LinkedList<EditableRun> colllectRun = new LinkedList<EditableRun>();
            foreach (EditableRun r in GetVisualElementForward(this.FirstRun, t))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        LinkedList<EditableRun> CollectRightRuns(EditableRun t)
        {
            //if (t.IsLineBreak)
            //{
            //    throw new NotSupportedException();
            //}
            LinkedList<EditableRun> colllectRun = new LinkedList<EditableRun>();
            foreach (EditableRun r in EditableFlowLayer.TextRunForward(t, this.LastRun))
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