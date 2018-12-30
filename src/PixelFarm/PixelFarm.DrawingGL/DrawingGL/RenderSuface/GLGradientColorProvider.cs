//MIT, 2014-present, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.CpuBlit;
using PixelFarm.Drawing;
namespace PixelFarm.DrawingGL
{
    class GLGradientBrush
    {
        internal float[] _v2f;
        internal float[] _colors;
        public GLGradientBrush(float[] v2f, float[] colors)
        {
            _v2f = v2f;
            _colors = colors;
        }

        public static GLGradientBrush Resolve(LinearGradientBrush linearGradientBrush)
        {
            GLGradientBrush glGradient = linearGradientBrush.InnerBrush as GLGradientBrush;
            if (glGradient == null)
            {
                //create a new one
                GLGradientColorProvider.Build(linearGradientBrush, out float[] v2f, out float[] colors);
                glGradient = new GLGradientBrush(v2f, colors);
                linearGradientBrush.InnerBrush = glGradient;
            }
            return glGradient;
        }
        public static GLGradientBrush Resolve(CircularGradientBrush circularGradientBrush)
        {
            GLGradientBrush glGradient = circularGradientBrush.InnerBrush as GLGradientBrush;
            if (glGradient == null)
            {
                //TODO: implement this...
            }
            return glGradient;
        }
    }
    static class GLGradientColorProvider
    {

        /// <summary>
        /// we do not store input linearGradient
        /// </summary>
        /// <param name="linearGradient"></param>
        internal static void Build(LinearGradientBrush linearGradient,
                out float[] v2f,
                out float[] colors)
        {
            int pairCount = linearGradient.PairCount;
            int i = 0;
            s_vertices.Clear();
            s_v2fList.Clear();
            s_colorList.Clear();

            float x_1 = 0;
            float y_1 = 0;
            double angleRad = 0;
            foreach (LinearGradientPair pair in linearGradient.GetColorPairIter())
            {
                if (i == 0)
                {
                    angleRad = pair.Angle;
                    pair.GetProperSwapVertices(
                        out float x1, out float y1, out Color c1,
                        out float x2, out float y2, out Color c2
                    );
                    x_1 = x1;
                    y_1 = y1;
                }

                CalculateLinearGradientVxs(s_vertices,
                    i == 0,
                    i == pairCount - 1,
                    pair);

                i++;
            }

            var txMatrix = PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix(
             PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-x_1, -y_1),
             PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Rotate(angleRad),
             PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(x_1, y_1)
             );

            //----------------------------------
            int j = s_vertices.Count;

            for (int m = 0; m < j; ++m)
            {
                VertexC4V3f v = s_vertices[m];
                double v_x = v.x;
                double v_y = v.y;
                txMatrix.Transform(ref v_x, ref v_y);
                //vrx[i] = new VertexC4V3f(v.color, (float)v_x, (float)v_y);
                s_v2fList.Add((float)v_x);
                s_v2fList.Add((float)v_y);

                uint color = v.color;
                //a,b,g,r 
                s_colorList.Add((color & 0xff) / 255f);//r
                s_colorList.Add(((color >> 8) & 0xff) / 255f);//g 
                s_colorList.Add(((color >> 16) & 0xff) / 255f); //b
                s_colorList.Add(((color >> 24) & 0xff) / 255f); //a
            }

            v2f = s_v2fList.ToArray();
            colors = s_colorList.ToArray();
        }

        static List<float> s_v2fList = new List<float>();
        static List<float> s_colorList = new List<float>();
        static ArrayList<VertexC4V3f> s_vertices = new ArrayList<VertexC4V3f>(); //reusable
        static void AddRect(ArrayList<VertexC4V3f> vrx,
          uint c1, uint c2,
          float x, float y,
          float w, float h)
        {
            //horizontal gradient
            vrx.Append(new VertexC4V3f(c1, x, y));
            vrx.Append(new VertexC4V3f(c2, x + w, y));
            vrx.Append(new VertexC4V3f(c2, x + w, y + h));
            vrx.Append(new VertexC4V3f(c2, x + w, y + h));
            vrx.Append(new VertexC4V3f(c1, x, y + h));
            vrx.Append(new VertexC4V3f(c1, x, y));
        }
        //----------------------------------------------------------------------------


        static void CalculateLinearGradientVxs(
          ArrayList<VertexC4V3f> vrx,
          bool isFirstPane,
          bool isLastPane, LinearGradientPair pair)
        {
            //1. gradient distance 
            float distance = (float)pair.Distance;

            pair.GetProperSwapVertices(
                out float x1, out float y1, out Color c1,
                out float x2, out float y2, out Color c2
                );

            if (isFirstPane)
            {
                //left solid rect pane 
                AddRect(vrx,
                    c1.ToABGR(), c1.ToABGR(),
                    -600, -800,
                    x1 + 600, 1800);
            }

            //color gradient pane 
            AddRect(vrx,
                c1.ToABGR(), c2.ToABGR(),
                x1, -800,
                distance, 1800);

            if (isLastPane)
            {
                //right solid pane
                if (1200 - (x1 + distance) > 0)
                {
                    AddRect(vrx,
                        c2.ToABGR(), c2.ToABGR(),
                        (x1 + distance), -800,
                        1200 - (x1 + distance), 1800);
                }
            }
        }
    }
}
