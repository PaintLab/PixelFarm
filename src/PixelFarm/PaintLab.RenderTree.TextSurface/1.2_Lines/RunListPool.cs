//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.TextFlow
{
    static class Temp<Owner, T>
    {
        public readonly struct TempContext : IDisposable
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
}