//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextEditing
{
    static class Temp<Owner, T>
    {
        public struct TempContext : IDisposable
        {
            internal readonly T _tool;
            internal TempContext(out T tool)
            {
                Temp<Owner, T>.GetFreeItem(out _tool);
                tool = _tool;
            }
            public void Dispose()
            {
                Temp<Owner, T>.Release(_tool);
            }
        }

        public delegate T CreateNewItemDelegate();
        public delegate void ReleaseItemDelegate(T item);


        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static CreateNewItemDelegate s_newHandler;
        [System.ThreadStatic]
        static ReleaseItemDelegate s_releaseCleanUp;

        public static TempContext Borrow(out T freeItem)
        {
            return new TempContext(out freeItem);
        }

        public static void SetNewHandler(CreateNewItemDelegate newHandler, ReleaseItemDelegate releaseCleanUp = null)
        {
            //set new instance here, must set this first***
            if (s_pool == null)
            {
                s_pool = new Stack<T>();
            }
            s_newHandler = newHandler;
            s_releaseCleanUp = releaseCleanUp;
        }
        internal static void GetFreeItem(out T freeItem)
        {
            if (s_pool.Count > 0)
            {
                freeItem = s_pool.Pop();
            }
            else
            {
                freeItem = s_newHandler();
            }
        }
        internal static void Release(T item)
        {
            s_releaseCleanUp?.Invoke(item);
            s_pool.Push(item);
            //... 
        }
        public static bool IsInit()
        {
            return s_pool != null;
        }
    }


    static class RunListPool
    {
        public static Temp<TextLineBox, List<Run>>.TempContext Borrow(out List<Run> runList)
        {
            if (!Temp<TextLineBox, List<Run>>.IsInit())
            {
                Temp<TextLineBox, List<Run>>.SetNewHandler(() => new List<Run>(),
                s => s.Clear()
                );
            }
            return Temp<TextLineBox, List<Run>>.Borrow(out runList);
        }
        public static Temp<TextLineBox, LinkedList<Run>>.TempContext Borrow(out LinkedList<Run> runList)
        {
            if (!Temp<TextLineBox, LinkedList<Run>>.IsInit())
            {
                Temp<TextLineBox, LinkedList<Run>>.SetNewHandler(() => new LinkedList<Run>(),
                s => s.Clear()
                );
            }
            return Temp<TextLineBox, LinkedList<Run>>.Borrow(out runList);
        }
    }

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
        public void ReplaceAll(IEnumerable<Run> textRuns)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            if (textRuns != null)
            {
                foreach (Run r in textRuns)
                {
                    this.AddLast(r);
                }
            }

            this.LocalResumeLineReArrange();
        }
    }
}