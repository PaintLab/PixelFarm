﻿//MIT, 2014-present, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.CpuBlit;
using PixelFarm.Drawing;
namespace PixelFarm.DrawingGL
{
    class LinearGradientBrush
    {
        internal float[] _v2f;
        internal float[] _colors;
        public LinearGradientBrush(float[] v2f, float[] colors)
        {
            _v2f = v2f;
            _colors = colors;
        }

        public static LinearGradientBrush Resolve(Drawing.LinearGradientBrush linearGradientBrush)
        {
            LinearGradientBrush glGradient = linearGradientBrush.InnerBrush as LinearGradientBrush;
            if (glGradient == null)
            {
                //create a new one
                Build(linearGradientBrush, out float[] v2f, out float[] colors);
                glGradient = new LinearGradientBrush(v2f, colors);
                linearGradientBrush.InnerBrush = glGradient;
            }
            return glGradient;
        }

        /// <summary>
        /// we do not store input linearGradient
        /// </summary>
        /// <param name="linearGradient"></param>
        static void Build(Drawing.LinearGradientBrush linearGradient,
              out float[] v2f,
              out float[] colors)
        {
            ColorStop[] colorStops = linearGradient.ColorStops;


            s_vertices.Clear();
            s_v2fList.Clear();
            s_colorList.Clear();

            float x_1 = linearGradient.StartPoint.X;
            float y_1 = linearGradient.StartPoint.Y;

            double angleRad = linearGradient.Angle;
            double totalLen = linearGradient.Length;

            int pairCount = colorStops.Length - 1;

            ColorStop c0 = ColorStop.Empty;
            ColorStop c1 = ColorStop.Empty;

            //create a simple horizontal linear gradient bar 
            //and we will rotate and translate it to target pos
            for (int i = 0; i < pairCount; ++i)
            {
                c0 = colorStops[i];
                c1 = colorStops[i + 1];

                CalculateLinearGradientVxs(s_vertices,
                    i == 0,
                    i == pairCount - 1,
                   (float)(x_1 + (c0.Offset * totalLen)),
                   (float)((c1.Offset - c0.Offset) * totalLen),
                    c0,
                    c1);

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

        static void CalculateLinearGradientVxs(
          ArrayList<VertexC4V3f> vrx,
          bool isFirstPane,
          bool isLastPane, float x1, float distance,
          ColorStop stop1, ColorStop stop2)
        {
            //TODO: review here again

            Color c1 = stop1.Color;
            Color c2 = stop2.Color;

            //1. gradient distance  

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


    class CircularGradientBrush : IDisposable
    {

        internal GLBitmap _lookupBmp;
        internal float[] _v2f;
        internal float _cx;
        internal float _cy;
        internal float _r;
        public CircularGradientBrush(float[] v2f)
        {
            _v2f = v2f;
        }
        public void Dispose()
        {
            if (_lookupBmp != null)
            {
                _lookupBmp.Dispose();
                _lookupBmp = null;
            }
        }
        public static CircularGradientBrush Resolve(Drawing.CircularGradientBrush cirGradientBrush)
        {
            CircularGradientBrush glGradient = cirGradientBrush.InnerBrush as CircularGradientBrush;
            if (glGradient == null)
            {
                //create a new one
                Build(cirGradientBrush, out float[] v2f);
                glGradient = new CircularGradientBrush(v2f);
                //create a single horizontal line linear gradient bmp 
                //for texture look up
                //create MemBitmap for this lookup table
                GradientSpanGenExtensions.GenerateSampleGradientLine(cirGradientBrush, out Color[] sampleColors);
                MemBitmap lookupBmp = new MemBitmap(sampleColors.Length, 1);//1 pixel height?

                //
                unsafe
                {
                    int* ptr = (int*)MemBitmap.GetBufferPtr(lookupBmp).Ptr;
                    for (int i = 0; i < sampleColors.Length; ++i)
                    {
                        Color c = sampleColors[i];
                        *ptr = (int)c.ToABGR();
                        ptr++;
                    }
                }
                glGradient._lookupBmp = new GLBitmap(lookupBmp, true);
                glGradient._cx = cirGradientBrush.StartPoint.X;
                glGradient._cy = cirGradientBrush.StartPoint.Y;
                glGradient._r = (float)cirGradientBrush.Length;

                cirGradientBrush.InnerBrush = glGradient;
            }
            return glGradient;
        }

        /// <summary>
        /// we do not store input linearGradient
        /// </summary>
        /// <param name="linearGradient"></param>
        static void Build(Drawing.CircularGradientBrush linearGradient, out float[] v2f)
        {
            ColorStop[] colorStops = linearGradient.ColorStops;

            s_vertices.Clear();
            s_v2fList.Clear();

            float x_1 = linearGradient.StartPoint.X;
            float y_1 = linearGradient.StartPoint.Y;


            double totalLen = linearGradient.Length;


            ColorStop c0 = ColorStop.Empty;
            ColorStop c1 = ColorStop.Empty;

            //create a simple horizontal linear gradient bar 
            //and we will rotate and translate it to target pos

            CalculateLinearGradientVxs(s_vertices, 0f, 800f);
            //--------------------------------
            int j = s_vertices.Count;

            for (int m = 0; m < j; ++m)
            {
                VertexC4V3f v = s_vertices[m];
                double v_x = v.x;
                double v_y = v.y;
                //txMatrix.Transform(ref v_x, ref v_y);
                //vrx[i] = new VertexC4V3f(v.color, (float)v_x, (float)v_y);
                s_v2fList.Add((float)v_x);
                s_v2fList.Add((float)v_y); 
            }

            v2f = s_v2fList.ToArray();
        }

        static List<float> s_v2fList = new List<float>(); 
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

        static void CalculateLinearGradientVxs(
          ArrayList<VertexC4V3f> vrx, float x1, float distance)
        {
            //TODO: review here again 
            //color gradient pane 
            AddRect(vrx,
                Color.Black.ToABGR(), Color.Black.ToABGR(), //not use color
                x1, -800,
                distance, 1800);
        }
    }

}
