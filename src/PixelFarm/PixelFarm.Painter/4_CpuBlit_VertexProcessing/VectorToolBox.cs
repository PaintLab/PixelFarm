//BSD, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.CpuBlit.VertexProcessing;
namespace PixelFarm.Drawing
{

    public static class VxsContext
    {
        public static VxsContext1 Temp1()
        {
            return new VxsContext1(true);
        }
        public static VxsContext1 Temp(out VertexStore vxs)
        {
            var tmp = new VxsContext1(true);
            vxs = tmp.vxs;
            return tmp;
        }

        //--------------------------------------------------------
        public static VxsContext2 Temp2()
        {
            return new VxsContext2(true);
        }
        public static VxsContext2 Temp(out VertexStore vxs1, out VertexStore vxs2)
        {
            var tmp = new VxsContext2(true);
            vxs1 = tmp.vxs1;
            vxs2 = tmp.vxs2;
            return tmp;
        }

        //--------------------------------------------------------
        public static VxsContext3 Temp3()
        {
            return new VxsContext3(true);
        }
        public static VxsContext3 Temp(out VertexStore vxs1,
            out VertexStore vxs2, out VertexStore vxs3)
        {
            var tmp = new VxsContext3(true);
            vxs1 = tmp.vxs1;
            vxs2 = tmp.vxs2;
            vxs3 = tmp.vxs3;
            return tmp;
        }

    }

    public struct VxsContext1 : IDisposable
    {
        public VertexStore vxs;
        internal VxsContext1(bool t)
        {
            VectorToolBox.GetFreeVxs(out vxs);
        }
        public void Dispose()
        {
            VectorToolBox.ReleaseVxs(ref vxs);
        }
    }
    public struct VxsContext2 : IDisposable
    {
        public VertexStore vxs1;
        public VertexStore vxs2;
        internal VxsContext2(bool t)
        {
            VectorToolBox.GetFreeVxs(out vxs1);
            VectorToolBox.GetFreeVxs(out vxs2);
        }
        public void Dispose()
        {
            //release
            VectorToolBox.ReleaseVxs(ref vxs1);
            VectorToolBox.ReleaseVxs(ref vxs2);
        }
    }

    public struct VxsContext3 : IDisposable
    {
        public VertexStore vxs1;
        public VertexStore vxs2;
        public VertexStore vxs3;
        internal VxsContext3(bool t)
        {
            VectorToolBox.GetFreeVxs(out vxs1);
            VectorToolBox.GetFreeVxs(out vxs2);
            VectorToolBox.GetFreeVxs(out vxs3);
        }
        public void Dispose()
        {
            //release
            VectorToolBox.ReleaseVxs(ref vxs1);
            VectorToolBox.ReleaseVxs(ref vxs2);
        }
    }

    public static class VectorToolBox
    {
        //for net20 -- check this
        //TODO: https://stackoverflow.com/questions/18333885/threadstatic-v-s-threadlocalt-is-generic-better-than-attribute

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
        public static void GetFreeVxs(out VertexStore vxs1, out VertexStore vxs2, out VertexStore vxs3)
        {
            vxs1 = GetFreeVxs();
            vxs2 = GetFreeVxs();
            vxs3 = GetFreeVxs();
        }
        public static void ReleaseVxs(ref VertexStore vxs1, ref VertexStore vxs2)
        {
            ReleaseVxs(ref vxs1);
            ReleaseVxs(ref vxs2);
        }

        public static void ReleaseVxs(ref VertexStore vxs1, ref VertexStore vxs2, ref VertexStore vxs3)
        {
            ReleaseVxs(ref vxs1);
            ReleaseVxs(ref vxs2);
            ReleaseVxs(ref vxs3);
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

        //-----------------------------------
        [System.ThreadStatic]
        static Stack<Stroke> s_strokePool = new Stack<Stroke>();
        public static void GetFreeStroke(out Stroke stroke, int w)
        {
            if (s_strokePool.Count > 0)
            {
                stroke = s_strokePool.Pop();
                stroke.Width = w;
            }
            else
            {
                stroke = new Stroke(w);
            }
        }
        public static void ReleaseStroke(ref Stroke stroke)
        {
            s_strokePool.Push(stroke);
            stroke = null;
        }
        //-----------------------------------


        [System.ThreadStatic]
        static Stack<PixelFarm.CpuBlit.PathWriter> s_pathWriters = new Stack<PixelFarm.CpuBlit.PathWriter>();
        public static void GetFreePathWriter(out PixelFarm.CpuBlit.PathWriter p)
        {
            if (s_pathWriters.Count > 0)
            {
                p = s_pathWriters.Pop();
            }
            else
            {
                p = new CpuBlit.PathWriter();
            }
        }
        public static void ReleasePathWriter(ref PixelFarm.CpuBlit.PathWriter p)
        {
            p.Clear();
            s_pathWriters.Push(p);
            p = null;
        }
        //-----------------------------------
        [System.ThreadStatic]
        static Stack<SimpleRect> s_simpleRects = new Stack<SimpleRect>();
        public static void GetFreeRectTool(out SimpleRect rectTool)
        {
            if (s_simpleRects.Count > 0)
            {
                rectTool = s_simpleRects.Pop();
            }
            else
            {
                rectTool = new SimpleRect();
            }
        }
        public static void ReleaseRectTool(ref SimpleRect rectTool)
        {
            s_simpleRects.Push(rectTool);
            rectTool = null;
        }

        //-----------------------------------
        [System.ThreadStatic]
        static Stack<Ellipse> s_ellipses = new Stack<Ellipse>();
        public static void GetFreeEllipseTool(out Ellipse ellipseTool)
        {
            if (s_ellipses.Count > 0)
            {
                ellipseTool = s_ellipses.Pop();
            }
            else
            {
                ellipseTool = new Ellipse();
            }
        }
        public static void ReleaseEllipseTool(ref Ellipse ellipseTool)
        {
            s_ellipses.Push(ellipseTool);
            ellipseTool = null;
        }

        //-------------
      
        [System.ThreadStatic]
        static Stack<RoundedRect> s_roundRects = new Stack<RoundedRect>();
        public static void GetFreeRoundRectTool(out RoundedRect roundRect)
        {
            if (s_roundRects.Count > 0)
            {
                roundRect = s_roundRects.Pop();
            }
            else
            {
                roundRect = new RoundedRect();
            }
        }
        public static void ReleaseRoundRect(ref RoundedRect roundRect)
        {
            s_roundRects.Push(roundRect);
            roundRect = null;
        }
    }
}