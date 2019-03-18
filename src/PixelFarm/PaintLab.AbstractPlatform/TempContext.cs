//MIT, 2019-present, WinterDev 
using System;
using System.Collections.Generic;

namespace LayoutFarm
{
    public struct TempContext<T> : IDisposable
    {
        internal readonly T _tool;
        internal TempContext(out T tool)
        {
            Temp<T>.GetFreeItem(out _tool);
            tool = _tool;
        }
        public void Dispose()
        {
            Temp<T>.Release(_tool);
        }
    }

    public static class Temp<T>
    {
        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static Func<T> s_newHandler;
        [System.ThreadStatic]
        static Action<T> s_releaseCleanUp;

        public static TempContext<T> Borrow(out T freeItem)
        {
            return new TempContext<T>(out freeItem);
        }

        public static void SetNewHandler(Func<T> newHandler, Action<T> releaseCleanUp = null)
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


}