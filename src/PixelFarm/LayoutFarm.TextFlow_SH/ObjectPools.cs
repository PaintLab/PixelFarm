//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using Typography.Text;

namespace LayoutFarm.TextFlow
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
                 sb => sb.Length = 0 //reset
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

    public struct TextUtf16RangeCopyPoolContext<T> : System.IDisposable
    {
        TextCopyBufferUtf16 _copyBuffer;

        static TextUtf16RangeCopyPoolContext()
        {
            //once
            ObjectPool<TextCopyBufferUtf16, T>.SetDelegates(
                () => new TextCopyBufferUtf16(),
                 rngCpy => rngCpy.Clear()
                );
        }
        public TextUtf16RangeCopyPoolContext(out TextCopyBufferUtf16 output)
        {
            _copyBuffer = output = ObjectPool<TextCopyBufferUtf16, T>.GetFreeInstance();
        }
        public void Dispose()
        {
            if (_copyBuffer != null)
            {
                ObjectPool<TextCopyBufferUtf16, T>.ReleaseInstance(_copyBuffer);
                _copyBuffer = null;
            }
        }
    }

    public struct TextUtf32RangeCopyPoolContext<T> : System.IDisposable
    {
        TextCopyBufferUtf32 _copyBuffer;

        static TextUtf32RangeCopyPoolContext()
        {
            //once
            ObjectPool<TextCopyBufferUtf32, T>.SetDelegates(
                () => new TextCopyBufferUtf32(),
                 rngCpy => rngCpy.Clear()
                );
        }
        public TextUtf32RangeCopyPoolContext(out TextCopyBufferUtf32 output)
        {
            _copyBuffer = output = ObjectPool<TextCopyBufferUtf32, T>.GetFreeInstance();
        }
        public void Dispose()
        {
            if (_copyBuffer != null)
            {
                ObjectPool<TextCopyBufferUtf32, T>.ReleaseInstance(_copyBuffer);
                _copyBuffer = null;
            }
        }
    }

}