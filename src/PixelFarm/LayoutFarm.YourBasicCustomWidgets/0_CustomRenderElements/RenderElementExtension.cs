//Apache2, 2014-2018, WinterDev

using LayoutFarm.UI;
using System.Collections.Generic;
namespace LayoutFarm
{
    public static class RenderElementExtension
    {
        public static void AddChild(this RenderElement renderBox, UIElement ui)
        {
            renderBox.AddChild(ui.GetPrimaryRenderElement(renderBox.Root));
        }
    }
}

namespace PixelFarm.Drawing
{
    public static class VectorToolBox
    {
        [System.ThreadStatic]
        static Stack<VertexStore> s_vxsPool = new Stack<VertexStore>();
        public static void GetFreeVxs(out VertexStore vxs1)
        {
            vxs1 = GetFreeVxs();
        }
        public static void GetFreeVxs(out VertexStore vxs1, out VertexStore vxs2)
        {
            vxs1 = GetFreeVxs();
            vxs2 = GetFreeVxs();
        }
        public static void ReleaseVxs(ref VertexStore vxs1, ref VertexStore vxs2)
        {
            ReleaseVxs(ref vxs1);
            ReleaseVxs(ref vxs2);
        }
        public static void ReleaseVxs(ref VertexStore vxs1)
        {
            vxs1.Clear();
            s_vxsPool.Push(vxs1);
            vxs1 = null;
        }
        static VertexStore GetFreeVxs()
        {
            if (s_vxsPool.Count > 0)
            {
                return s_vxsPool.Pop();
            }
            else
            {
                return new VertexStore();
            }
        }



        static Stack<Agg.Stroke> s_strokePool = new Stack<Agg.Stroke>();
        public static void GetFreeStroke(out Agg.Stroke stroke, int w)
        {
            if (s_strokePool.Count > 0)
            {
                stroke = s_strokePool.Pop();
                stroke.Width = w;
            }
            else
            {
                stroke = new Agg.Stroke(w);
            }
        }
        public static void ReleaseStroke(ref Agg.Stroke stroke)
        {
            s_strokePool.Push(stroke);
            stroke = null;
        }
    }
}