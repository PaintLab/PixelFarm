//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using Typography.Text;

namespace LayoutFarm.TextEditing
{
    static class ObjectPool<I, T>
    {
        [System.ThreadStatic]
        static Stack<I> s_pool;

        static Func<I> s_newInstance;
        static Action<I> s_reset;

        public static void SetDelegates(Func<I> newInstance, Action<I> reset)
        {
            s_newInstance = newInstance;
            s_reset = reset;
        }

        public static I GetFreeInstance()
        {
            if (s_pool == null)
            {
                s_pool = new Stack<I>();
            }

            if (s_pool.Count > 0)
            {
                return s_pool.Pop();
            }
            else
            {
                return s_newInstance();
            }
        }
        public static void ReleaseInstance(I output)
        {
            s_reset(output);
            s_pool.Push(output);
        }
    }

    public struct StringBuilderPoolContext<T> : System.IDisposable
    {
        StringBuilder _cacheStBuilder;
        static StringBuilderPoolContext()
        {
            //once
            ObjectPool<StringBuilder, T>.SetDelegates(
                () => new StringBuilder(),
                 sb => sb.Length = 0
                );
        }
        public StringBuilderPoolContext(out StringBuilder stbuilder)
        {
            _cacheStBuilder = stbuilder = ObjectPool<StringBuilder, T>.GetFreeInstance();
        }
        public void Dispose()
        {
            if (_cacheStBuilder != null)
            {
                ObjectPool<StringBuilder, T>.ReleaseInstance(_cacheStBuilder);
                _cacheStBuilder = null;
            }
        }
    }

    public struct TextRangeCopyPoolContext<T> : System.IDisposable
    {
        TextCopyBuffer _copyBuffer;

        static TextRangeCopyPoolContext()
        {
            //once
            ObjectPool<TextCopyBuffer, T>.SetDelegates(
                () => new TextCopyBuffer(),
                 rngCpy => rngCpy.Clear()
                );
        }
        public TextRangeCopyPoolContext(out TextCopyBuffer output)
        {
            _copyBuffer = output = ObjectPool<TextCopyBuffer, T>.GetFreeInstance();
        }
        public void Dispose()
        {
            if (_copyBuffer != null)
            {
                ObjectPool<TextCopyBuffer, T>.ReleaseInstance(_copyBuffer);
                _copyBuffer = null;
            }
        }
    }

}