//2014 BSD,WinterDev   

//MatterHackers
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using OpenTK.Graphics.OpenGL;
namespace PixelFarm.Agg
{
    /// <summary>
    /// to bitmap
    /// </summary>  
    public class GLScanlineRasToDestBitmapRenderer
    {

        ArrayList<ColorRGBA> tempSpanColors = new ArrayList<ColorRGBA>();

        ArrayList<int> xyPointBuffer = new ArrayList<int>();
        ArrayList<uint> pointColorBuffer = new ArrayList<uint>();

        public GLScanlineRasToDestBitmapRenderer()
        {

        }
        protected bool UseCustomRenderSingleScanLine
        {
            get;
            set;
        }

        public void RenderWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                LayoutFarm.Drawing.Color color)
        {
            if (!sclineRas.RewindScanlines()) { return; } //early exit
            //----------------------------------------------- 
            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            if (this.UseCustomRenderSingleScanLine)
            {

            }
            else
            {
                //generate point buffer anc color buffer ?

                while (sclineRas.SweepScanline(scline))
                {
                    this.pointColorBuffer.Clear();
                    this.xyPointBuffer.Clear();
                    //render solid single scanline
                    int y = scline.Y;
                    int num_spans = scline.SpanCount;
                    byte[] covers = scline.GetCovers();
                    for (int i = 1; i <= num_spans; ++i)
                    {
                        ScanlineSpan span = scline.GetSpan(i);
                        if (span.len > 0)
                        {
                            //outline
                            GLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                        }
                        else
                        {
                            //fill
                            int x = span.x;
                            int x2 = (x - span.len - 1);
                            GLBlendHL(x, y, x2, color, covers[span.cover_index]);
                        }
                    }
                    DrawPoints(xyPointBuffer, pointColorBuffer);
                }
                //------------------------
            }
        }

        static void DrawPoint(float x1, float y1)
        {
            unsafe
            {
                float* arr = stackalloc float[2];
                arr[0] = x1; arr[1] = y1;

                byte* indices = stackalloc byte[1];
                indices[0] = 0;

                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Points, 1, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }

        static void DrawPoints(ArrayList<int> pointsList, ArrayList<uint> colorsList)
        {
            unsafe
            {
                int n = pointsList.Count / 2;
                int[] points = pointsList.Array;
                uint[] cbuff = colorsList.Array;

                fixed (uint* chead = &cbuff[0])
                fixed (int* arr = &points[0])
                {

                    int* indices = stackalloc int[n];
                    for (int i = n - 1; i >= 0; --i)
                    {
                        indices[i] = i;
                    }
                    GL.EnableClientState(ArrayCap.ColorArray);
                    GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, (IntPtr)chead);


                    GL.EnableClientState(ArrayCap.VertexArray); //***
                    //vertex
                    GL.VertexPointer(2, VertexPointerType.Int, 0, (IntPtr)arr);
                    GL.DrawElements(BeginMode.Points, n, DrawElementsType.UnsignedInt, (IntPtr)indices);                    
                    GL.DisableClientState(ArrayCap.VertexArray);
                    
                    
                    GL.DisableClientState(ArrayCap.ColorArray);

                }
            }
        }
        static void DrawLine(float x1, float y1, float x2, float y2)
        {
            unsafe
            {
                float* arr = stackalloc float[4];
                arr[0] = x1; arr[1] = y1;
                arr[2] = x2; arr[3] = y2;

                byte* indices = stackalloc byte[2];
                indices[0] = 0; indices[1] = 1;

                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        const int BASE_MASK = 255;

        void GLBlendHL(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
        {
            if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(color.A) * (cover + 1)) >> 8);

            var colors = this.pointColorBuffer;
            var points = this.xyPointBuffer;
            if (alpha == BASE_MASK)
            {

                switch (len)
                {
                    case 0:
                        {
                        } break;
                    case 1:
                        {
                            //colors.AddVertex(LayoutFarm.Drawing.Color.FromArgb(alpha, color));

                            var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);

                            colors.AddVertex(c.ToARGB());
                            //colors.AddVertex(c.B);
                            //colors.AddVertex(c.G);
                            //colors.AddVertex(c.R);
                            //colors.AddVertex(c.A);


                            points.AddVertex(x1);
                            points.AddVertex(y);
                        } break;
                    default:
                        {
                            for (int i = 0; i < len; ++i)
                            {

                                //colors.AddVertex(LayoutFarm.Drawing.Color.FromArgb(alpha, color));

                                var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);
                                colors.AddVertex(c.ToARGB());

                                points.AddVertex(x1 + i);
                                points.AddVertex(y);

                            }
                            //DrawLine(x1, y, x2 + 1, y);
                        } break;
                }
            }
            else
            {
                int xpos = x1; 
                do
                {
                    //GL.Color4(LayoutFarm.Drawing.Color.FromArgb(alpha, color));
                    //DrawPoint(xpos, y);
                    var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);
                    //colors.AddVertex(c.A);
                    //colors.AddVertex(c.R);
                    //colors.AddVertex(c.G);
                    //colors.AddVertex(c.B);
                    colors.AddVertex(c.ToARGB());
                    points.AddVertex(xpos);
                    points.AddVertex(y);

                    xpos++;
                }
                while (--len != 0);
            }
        }
        //static void GLBlendHL2(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
        //{
        //    if (color.A == 0) { return; }

        //    int len = x2 - x1 + 1;
        //    int alpha = (((int)(color.A) * (cover + 1)) >> 8);
        //    if (alpha == BASE_MASK)
        //    {
        //        GL.Color4(color);
        //        switch (len)
        //        {
        //            case 0:
        //                {
        //                } break;
        //            case 1:
        //                {
        //                    DrawPoint(x1, y);
        //                } break;
        //            default:
        //                {
        //                    DrawLine(x1, y, x2 + 1, y);
        //                } break;
        //        }

        //    }
        //    else
        //    {
        //        int xpos = x1;
        //        do
        //        {
        //            GL.Color4(LayoutFarm.Drawing.Color.FromArgb(alpha, color));
        //            DrawPoint(xpos, y);
        //            xpos++;
        //        }
        //        while (--len != 0);
        //    }
        //}

        void GLBlendSolidHSpan(int x, int y, int len,
            LayoutFarm.Drawing.Color sourceColor,
            byte[] covers, int coversIndex)
        {
            int colorAlpha = sourceColor.A;
            if (colorAlpha == 0) { return; }

            unchecked
            {

                int xpos = x;
                var colors = this.pointColorBuffer;
                var points = this.xyPointBuffer;
                do
                {
                    //foreach single pixel
                    int alpha = ((colorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                    if (alpha == BASE_MASK)
                    {

                        //colors.AddVertex(sourceColor.A);
                        //colors.AddVertex(sourceColor.R);
                        //colors.AddVertex(sourceColor.G);
                        //colors.AddVertex(sourceColor.B);
                        //colors.AddVertex(sourceColor.B);
                        //colors.AddVertex(sourceColor.G);
                        //colors.AddVertex(sourceColor.R);
                        //colors.AddVertex(sourceColor.A);

                        colors.AddVertex(sourceColor.ToARGB());
                        //colors.AddVertex(sourceColor);
                        points.AddVertex(xpos);
                        points.AddVertex(y);

                        //GL.Color4(sourceColor);
                        //DrawPoint(xpos, y);
                        xpos++;
                    }
                    else
                    {
                        //GL.Color4(LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor));
                        //DrawPoint(xpos, y);

               
                        var c = LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor);
                         
                        
                        //colors.AddVertex(c.B);
                        //colors.AddVertex(c.G);
                        //colors.AddVertex(c.R);
                        //colors.AddVertex(c.A);
                        colors.AddVertex(c.ToARGB());


                        points.AddVertex(xpos);
                        points.AddVertex(y);

                        xpos++;
                    }
                    coversIndex++;
                }
                while (--len != 0);




            }

        }

        //static void GLBlendSolidHSpan2(int x, int y, int len, LayoutFarm.Drawing.Color sourceColor, byte[] covers, int coversIndex)
        //{
        //    int colorAlpha = sourceColor.A;
        //    if (colorAlpha == 0) { return; }

        //    unchecked
        //    {

        //        int xpos = x;
        //        do
        //        {
        //            //foreach single pixel
        //            int alpha = ((colorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
        //            if (alpha == BASE_MASK)
        //            {
        //                GL.Color4(sourceColor);
        //                DrawPoint(xpos, y);
        //                xpos++;
        //            }
        //            else
        //            {
        //                GL.Color4(LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor));
        //                DrawPoint(xpos, y);
        //                xpos++;
        //            }
        //            coversIndex++;
        //        }
        //        while (--len != 0);




        //    }

        //}


        public void RenderWithSpan(IImageReaderWriter dest,
                 GLScanlineRasterizer sclineRas,
                 GLScanline scline,
                ISpanGenerator spanGenerator)
        {
            if (!sclineRas.RewindScanlines()) { return; } //early exit
            //-----------------------------------------------

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);

            spanGenerator.Prepare();


            if (dest.Stride / 4 > (tempSpanColors.AllocatedSize))
            {
                //if not enough -> alloc more
                tempSpanColors.Clear(dest.Stride / 4);
            }

            ColorRGBA[] colorArray = tempSpanColors.Array;

            while (sclineRas.SweepScanline(scline))
            {

                //render single scanline 
                int y = scline.Y;
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();

                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    int x = span.x;
                    int span_len = span.len;
                    bool firstCoverForAll = false;

                    if (span_len < 0) { span_len = -span_len; firstCoverForAll = true; } //make absolute value

                    //1. generate colors -> store in colorArray
                    spanGenerator.GenerateColors(colorArray, 0, x, y, span_len);

                    //2. blend color in colorArray to destination image
                    dest.BlendColorHSpan(x, y, span_len,
                        colorArray, 0,
                        covers, span.cover_index,
                        firstCoverForAll);
                }

            }
        }
        protected virtual void CustomRenderSingleScanLine(
            IImageReaderWriter dest,
            GLScanline scline,
            ColorRGBA color)
        {
            //implement
        }
    }


    //----------------------------
    public class CustomScanlineRasToDestBitmapRenderer : ScanlineRasToDestBitmapRenderer
    {

    }
}
