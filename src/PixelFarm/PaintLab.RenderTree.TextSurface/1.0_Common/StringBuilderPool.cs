//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.TextEditing
{
    public static class StringBuilderPool<T>
    {
        [System.ThreadStatic]
        static Stack<StringBuilder> s_stringBuilderPool;

        public static StringBuilder GetFreeStringBuilder()
        {
            if (s_stringBuilderPool == null)
            {
                s_stringBuilderPool = new Stack<StringBuilder>();
            }

            if (s_stringBuilderPool.Count > 0)
            {
                return s_stringBuilderPool.Pop();
            }
            else
            {
                return new StringBuilder();
            }
        }
        public static void ReleaseStringBuilder(StringBuilder stBuilder)
        {
            stBuilder.Length = 0;
            s_stringBuilderPool.Push(stBuilder);
        }


        public static StringBuilderPoolContext<T> GetFreeStringBuilder(out StringBuilder stbuilder)
        {
            return new StringBuilderPoolContext<T>(out stbuilder);
        }
    }

    public struct StringBuilderPoolContext<T> : System.IDisposable
    {
        StringBuilder _cacheStBuilder;
        public StringBuilderPoolContext(out StringBuilder stbuilder)
        {
            _cacheStBuilder = stbuilder = StringBuilderPool<T>.GetFreeStringBuilder();
        }
        public void Dispose()
        {
            if (_cacheStBuilder != null)
            {
                StringBuilderPool<T>.ReleaseStringBuilder(_cacheStBuilder);
                _cacheStBuilder = null;
            }
        }
    }

    struct TempTextLineCopyContext : System.IDisposable
    {
        class StringBuilderPoolOwner { } //temp class for our private string builder

        StringBuilder _tempStBuilder;
        public TempTextLineCopyContext(TextLine textline, out PixelFarm.Drawing.TextBufferSpan buffSpan)
        {
            _tempStBuilder = StringBuilderPool<StringBuilderPoolOwner>.GetFreeStringBuilder();
            textline.CopyLineContent(_tempStBuilder);

            int len = _tempStBuilder.Length;
            char[] charBuffer = new char[len];
            _tempStBuilder.CopyTo(0, charBuffer, 0, len);
            buffSpan = new PixelFarm.Drawing.TextBufferSpan(charBuffer);
        }
        public void Dispose()
        {
            if (_tempStBuilder != null)
            {
                StringBuilderPool<StringBuilderPoolOwner>.ReleaseStringBuilder(_tempStBuilder);
                _tempStBuilder = null;
            }
        }
    }


}