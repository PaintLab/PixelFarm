//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using System.Text;

using LayoutFarm.RenderBoxes;
using LayoutFarm.UI;

using PixelFarm.Drawing;

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
}