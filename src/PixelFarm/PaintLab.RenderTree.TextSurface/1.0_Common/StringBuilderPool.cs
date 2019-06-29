//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.TextEditing
{
    static class StringBuilderPool
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
    }

    struct TempTextLineCopyContext : System.IDisposable
    {
        StringBuilder _tempStBuilder;
        public TempTextLineCopyContext(TextLine textline, out PixelFarm.Drawing.TextBufferSpan buffSpan)
        {
            _tempStBuilder = StringBuilderPool.GetFreeStringBuilder();
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
                StringBuilderPool.ReleaseStringBuilder(_tempStBuilder);
                _tempStBuilder = null;
            }
        }
    }


}