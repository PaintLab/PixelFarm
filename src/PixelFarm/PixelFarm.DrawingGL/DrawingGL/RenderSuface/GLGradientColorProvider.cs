//MIT, 2014-present, WinterDev
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

            ColorStop c0;
            ColorStop c1;

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

            //var txMatrix = PixelFarm.CpuBlit.VertexProcessing.Affine.New(
            // PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-x_1, -y_1),
            // PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Rotate(angleRad),
            // PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(x_1, y_1)
            // );

            var txMatrix = PixelFarm.CpuBlit.VertexProcessing.AffineMat.Iden;
            txMatrix.Translate(-x_1, -y_1);
            txMatrix.Rotate(angleRad);
            txMatrix.Translate(x_1, y_1);
            
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


    class RadialGradientBrush : IDisposable
    {

        internal GLBitmap _lookupBmp;
        internal float[] _v2f;
        internal float _cx;
        internal float _cy;
        internal float _r;
        internal PixelFarm.CpuBlit.VertexProcessing.Affine _invertedAff;
        internal bool _hasSignificateAlphaCompo;

        public RadialGradientBrush(float[] v2f)
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
        public static RadialGradientBrush Resolve(Drawing.RadialGradientBrush radGradientBrush)
        {
            RadialGradientBrush glGradient = radGradientBrush.InnerBrush as RadialGradientBrush;
            if (glGradient == null)
            {
                //temp fix 
                //check if some color stop has alpha 


                //create a new one
                Build(radGradientBrush, out float[] v2f);
                glGradient = new RadialGradientBrush(v2f);


                ColorStop[] colorStops = radGradientBrush.ColorStops;
                for (int i = 0; i < colorStops.Length; ++i)
                {
                    ColorStop stop = radGradientBrush.ColorStops[i];
                    if (stop.Color.A < 255 * 0.8) //temp fix 0.8
                    {
                        glGradient._hasSignificateAlphaCompo = true;
                        break;
                    }
                }

                //create a single horizontal line linear gradient bmp 
                //for texture look up
                //create MemBitmap for this lookup table
                GradientSpanGenExtensions.GenerateSampleGradientLine(radGradientBrush, out Color[] sampleColors);
                MemBitmap lookupBmp = new MemBitmap(sampleColors.Length, 1);//1 pixel height 

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
                glGradient._cx = radGradientBrush.StartPoint.X;
                glGradient._cy = radGradientBrush.StartPoint.Y;
                glGradient._r = (float)radGradientBrush.Length;
                if (radGradientBrush.CoordTransformer != null)
                {
                    glGradient._invertedAff = (PixelFarm.CpuBlit.VertexProcessing.Affine)radGradientBrush.CoordTransformer.CreateInvert();
                }

                radGradientBrush.InnerBrush = glGradient;
            }
            return glGradient;
        }

        /// <summary>
        /// we do not store input linearGradient
        /// </summary>
        /// <param name="linearGradient"></param>
        static void Build(Drawing.RadialGradientBrush linearGradient, out float[] v2f)
        {
            ColorStop[] colorStops = linearGradient.ColorStops;

            //create a simple horizontal linear gradient bar 
            //and we will rotate and translate it to target pos 
            v2f = new float[12];
            AddRect(v2f, 0, 0, 2000, 800);
        }

        static void AddRect(float[] vrx,
          float x, float y,
          float w, float h)
        {
            //horizontal gradient
            //vrx.Append(new VertexC4V3f(c1, x, y));
            //vrx.Append(new VertexC4V3f(c2, x + w, y));
            //vrx.Append(new VertexC4V3f(c2, x + w, y + h));
            //vrx.Append(new VertexC4V3f(c2, x + w, y + h));
            //vrx.Append(new VertexC4V3f(c1, x, y + h));
            //vrx.Append(new VertexC4V3f(c1, x, y));
            vrx[0] = x; vrx[1] = y;
            vrx[2] = (x + w); vrx[3] = (y);
            vrx[4] = (x + w); vrx[5] = (y + h);
            vrx[6] = (x + w); vrx[7] = (y + h);
            vrx[8] = (x); vrx[9] = (y + h);
            vrx[10] = (x); vrx[11] = (y);
        }
    }



    class PolygonGradientBrush
    {
        internal float[] _v2f;
        internal float[] _colors;
        public PolygonGradientBrush(float[] v2f, float[] colors)
        {
            _v2f = v2f;
            _colors = colors;
        }

        public static PolygonGradientBrush Resolve(Drawing.PolygonGradientBrush polygonGr, PixelFarm.CpuBlit.VertexProcessing.TessTool tess)
        {
            PolygonGradientBrush glGradient = polygonGr.InnerBrush as PolygonGradientBrush;
            if (glGradient == null)
            {
                //create a new one
                Build(polygonGr, tess, out float[] v2f, out float[] colors);
                glGradient = new PolygonGradientBrush(v2f, colors);
                polygonGr.InnerBrush = glGradient;
            }
            return glGradient;
        }

        /// <summary>
        /// we do not store input linearGradient
        /// </summary>
        /// <param name="linearGradient"></param>
        static void Build(Drawing.PolygonGradientBrush linearGradient,
              PixelFarm.CpuBlit.VertexProcessing.TessTool tess,
              out float[] v2f,
              out float[] colors)
        {
            List<Drawing.PolygonGradientBrush.ColorVertex2d> vertices = linearGradient.Vertices;
            s_v2fList.Clear();
            s_colorList.Clear();

            int j = vertices.Count;
            for (int m = 0; m < j; ++m)
            {
                Drawing.PolygonGradientBrush.ColorVertex2d v = vertices[m];
                s_v2fList.Add(v.X);
                s_v2fList.Add(v.Y);
            }


            ushort[] indexList = PixelFarm.CpuBlit.VertexProcessing.TessToolExtensions.TessAsTriIndexArray(
                 tess, s_v2fList.ToArray(), null,
                 out float[] tessCoords,
                 out int vertexCount
                  );

            int n1 = 0;
            int n2 = 0;
            float[] colors2 = new float[indexList.Length * 4];
            float[] tessCoord2 = new float[indexList.Length * 2];

            for (int i = 0; i < indexList.Length; ++i)
            {
                ushort index = indexList[i];
                Drawing.PolygonGradientBrush.ColorVertex2d v = vertices[index];
                Color color = v.C;

                //a,b,g,r 
                colors2[n1] = (color.R / 255f);//r
                colors2[n1 + 1] = (color.G / 255f);//g 
                colors2[n1 + 2] = (color.B / 255f); //b
                colors2[n1 + 3] = (color.A / 255f); //a 

                tessCoord2[n2] = v.X;
                tessCoord2[n2 + 1] = v.Y;

                n1 += 4;
                n2 += 2;
            }
            v2f = tessCoord2;
            colors = colors2;
        }

        static List<float> s_v2fList = new List<float>();
        static List<float> s_colorList = new List<float>();


    }

}
