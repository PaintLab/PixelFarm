//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{  

    partial class TextLineBox
    {
        public void AddLineBreakAfter(Run afterTextRun)
        {
            if (afterTextRun == null)
            {
                this.EndWithLineBreak = true;
                TextLineBox newline = _textFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                if (_textFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else if (afterTextRun.NextRun == null)
            {
                this.EndWithLineBreak = true;
                TextLineBox newline = _textFlowLayer.InsertNewLine(_currentLineNumber + 1);
                //
                if (_textFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else
            {

                using (RunListPool.Borrow(out List<Run> tempTextRuns))
                {
                    if (afterTextRun != null)
                    {
                        foreach (Run t in GetRunIterForward(afterTextRun.NextRun))
                        {
                            tempTextRuns.Add(t);
                        }
                    }

                    bool thisEndWithLineBreak = this.EndWithLineBreak;

                    this.EndWithLineBreak = true;
                    this.LocalSuspendLineReArrange();

                    TextLineBox newTextline = _textFlowLayer.InsertNewLine(_currentLineNumber + 1);
                    newTextline.EndWithLineBreak = thisEndWithLineBreak;

                    //
                    int j = tempTextRuns.Count;
                    newTextline.LocalSuspendLineReArrange();
                    int cx = 0;
                    for (int i = 0; i < j; ++i)
                    {
                        Run t = tempTextRuns[i];
                        this.Remove(t);
                        newTextline.AddLast(t);
                        Run.DirectSetLocation(t, cx, 0);
                        cx += t.Width;
                    }

                    newTextline.LocalResumeLineReArrange();
                    this.LocalResumeLineReArrange();
                }

            }
        }
        void AddLineBreakBefore(Run beforeTextRun)
        {
            if (beforeTextRun == null)
            {
                this.EndWithLineBreak = true;
                _textFlowLayer.InsertNewLine(_currentLineNumber + 1);
            }
            else
            {
                //TODO: use pool
                using (RunListPool.Borrow(out List<Run> tempTextRuns))
                {
                    if (beforeTextRun != null)
                    {
                        foreach (Run t in GetRunIterForward(beforeTextRun))
                        {
                            tempTextRuns.Add(t);
                        }
                    }
                    this.EndWithLineBreak = true;
                    TextLineBox newTextline = _textFlowLayer.InsertNewLine(_currentLineNumber + 1);
                    //
                    this.LocalSuspendLineReArrange();
                    newTextline.LocalSuspendLineReArrange();
                    int j = tempTextRuns.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        Run t = tempTextRuns[i];
                        this.Remove(t);
                        newTextline.AddLast(t);
                    }
                    this.LocalResumeLineReArrange();
                    newTextline.LocalResumeLineReArrange();
                }
            }
        }

        void RemoveLeft(Run t)
        {
            if (t == null) return;

            LocalSuspendLineReArrange();
            using (RunListPool.Borrow(out LinkedList<Run> tobeRemoveTextRuns))
            {
                CollectLeftRuns(t, tobeRemoveTextRuns);
                LinkedListNode<Run> curNode = tobeRemoveTextRuns.First;

                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
            }
            LocalResumeLineReArrange();
        }
        void RemoveRight(Run t)
        {
            LocalSuspendLineReArrange();

            using (RunListPool.Borrow(out LinkedList<Run> tobeRemoveTextRuns))
            {
                CollectRightRuns(t, tobeRemoveTextRuns);

                LinkedListNode<Run> curNode = tobeRemoveTextRuns.First;

                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
            }

            LocalResumeLineReArrange();
        }

        void CollectLeftRuns(Run t, LinkedList<Run> output)
        {
            foreach (Run r in GetRunIterForward(this.FirstRun, t))
            {
                output.AddLast(r);
            }
        }
        void CollectRightRuns(Run t, LinkedList<Run> output)
        {
            foreach (Run r in _textFlowLayer.TextRunForward(t, this.LastRun))
            {
                output.AddLast(r);
            }
        }
        public void ReplaceAll(IEnumerable<Run> runs)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            if (runs != null)
            {
                foreach (Run r in runs)
                {
                    this.AddLast(r);
                }
            }

            this.LocalResumeLineReArrange();
        }
    }
}