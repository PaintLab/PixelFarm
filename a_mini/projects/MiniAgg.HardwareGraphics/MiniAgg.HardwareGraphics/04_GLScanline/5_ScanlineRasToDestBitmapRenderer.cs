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
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PixelFarm.Agg
{

    /// <summary
    /// to bitmap
    /// </summary>  
    public class GLScanlineRasToDestBitmapRenderer
    {

        //ArrayList<ColorRGBA> tempSpanColors = new ArrayList<ColorRGBA>();

        ArrayList<VertexC4V2S> mySinglePixelBuffer = new ArrayList<VertexC4V2S>();
        ArrayList<VertexC4V2S> myLineBuffer = new ArrayList<VertexC4V2S>();

        public GLScanlineRasToDestBitmapRenderer()
        {

        }

        /// <summary>
        /// for fill shape
        /// </summary>
        /// <param name="sclineRas"></param>
        /// <param name="scline"></param>
        /// <param name="color"></param>
        /// <param name="shapeHint"></param>
        public void FillWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                LayoutFarm.Drawing.Color color)
        {

            //early exit
            if (color.A == 0) { return; }
            if (!sclineRas.RewindScanlines()) { return; }
            //----------------------------------------------- 

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            //-----------------------------------------------  


            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);


            this.mySinglePixelBuffer.Clear();
            this.myLineBuffer.Clear();

            while (sclineRas.SweepScanline(scline))
            {
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
            }

            //---------------------------------------------
            //points
            int nelements = mySinglePixelBuffer.Count;
            VboC4V2S vbo = GenerateVBOForC4V2I();
            if (nelements > 0)
            {
                vbo.BindBuffer();
                DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements);
                vbo.UnbindBuffer();
            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                vbo.BindBuffer();
                DrawLinesWithVertexBuffer(myLineBuffer, nelements);
                vbo.UnbindBuffer();
            }
            //---------------------------------------------

            vbo.Dispose();
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            //------------------------ 
        }
        /// <summary>
        /// for lines
        /// </summary>
        /// <param name="sclineRas"></param>
        /// <param name="scline"></param>
        /// <param name="color"></param>
        /// <param name="shapeHint"></param>
        public void DrawWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                LayoutFarm.Drawing.Color color)
        {

            //early exit
            if (color.A == 0) { return; }
            if (!sclineRas.RewindScanlines()) { return; }
            //----------------------------------------------- 

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            //-----------------------------------------------  


            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);


            this.mySinglePixelBuffer.Clear();
            this.myLineBuffer.Clear();

            while (sclineRas.SweepScanline(scline))
            {
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
            }

            //---------------------------------------------
            //points
            int nelements = mySinglePixelBuffer.Count;
            VboC4V2S vbo = GenerateVBOForC4V2I();
            if (nelements > 0)
            {
                vbo.BindBuffer();
                DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements);
                vbo.UnbindBuffer();
            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                vbo.BindBuffer();
                DrawLinesWithVertexBuffer(myLineBuffer, nelements);
                vbo.UnbindBuffer();
            }
            //---------------------------------------------

            vbo.Dispose();
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            //------------------------ 
        }

        
        static VboC4V2S GenerateVBOForC4V2I()
        {
            VboC4V2S vboHandle = new VboC4V2S();

            //must open these ... before call this func
            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray);

            GL.GenBuffers(1, out vboHandle.VboID);

            return vboHandle;
        }

        //static void DrawLine(float x1, float y1, float x2, float y2)
        //{
        //    unsafe
        //    {
        //        float* arr = stackalloc float[4];
        //        arr[0] = x1; arr[1] = y1;
        //        arr[2] = x2; arr[3] = y2;

        //        //byte* indices = stackalloc byte[2];
        //        //indices[0] = 0; indices[1] = 1;

        //        GL.EnableClientState(ArrayCap.VertexArray); //***
        //        //vertex
        //        GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
        //        //GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedByte, (IntPtr)indices);
        //        GL.DrawArrays(BeginMode.Lines, 0, 2);
        //        GL.DisableClientState(ArrayCap.VertexArray);
        //    }
        //}
        const int BASE_MASK = 255;


        //======================================================================================
        static void DrawPointsWithVertexBuffer(ArrayList<VertexC4V2S> singlePxBuffer, int nelements)
        {
            unsafe
            {

                //--------------------------------------------- 
                VertexC4V2S[] vpoints = singlePxBuffer.Array;

                IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                // Fill newly allocated buffer
                GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);
                // Only draw particles that are alive
                GL.DrawArrays(BeginMode.Points, 0, nelements);

                //--------------------------------------------- 
            }
        }
        static void DrawLinesWithVertexBuffer(ArrayList<VertexC4V2S> linesBuffer, int nelements)
        {
            unsafe
            {
                VertexC4V2S[] vpoints = linesBuffer.Array;
                IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                // Fill newly allocated buffer
                GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);
                // Only draw particles that are alive
                GL.DrawArrays(BeginMode.Lines, 0, nelements);
            }
        }
        void GLBlendHL(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
        {
            //if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(color.A) * (cover + 1)) >> 8);
            var singlePxBuff = this.mySinglePixelBuffer;
            var lineBuffer = this.myLineBuffer;

            if (alpha == BASE_MASK)
            {

                switch (len)
                {
                    case 0:
                        {
                        } break;
                    case 1:
                        {
                            singlePxBuff.AddVertex(new VertexC4V2S(
                                LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                                x1, y));

                        } break;
                    default:
                        {
                            var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                            lineBuffer.AddVertex(new VertexC4V2S(c, x1, y));
                            lineBuffer.AddVertex(new VertexC4V2S(c, x2 + 1, y));

                            //var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();

                            //for (int i = 0; i < len; ++i)
                            //{
                            //    //var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);
                            //    singlePxBuff.AddVertex(new VertexC4XYZ3I(
                            //        c, x1 + i, y));
                            //}

                        } break;
                }
            }
            else
            {
                int xpos = x1;
                do
                {
                    singlePxBuff.AddVertex(new VertexC4V2S(
                        LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                        xpos, y));
                    xpos++;
                }
                while (--len != 0);
            }
        }
        void GLBlendSolidHSpan(int x, int y, int len,
            LayoutFarm.Drawing.Color sourceColor,
            byte[] covers, int coversIndex)
        {
            //int colorAlpha = sourceColor.A;
            //if (colorAlpha == 0) { return; }

            unchecked
            {

                int xpos = x;
                var pointAndColors = this.mySinglePixelBuffer;

                do
                {
                    //alpha change
                    int alpha = ((sourceColor.A) * ((covers[coversIndex]) + 1)) >> 8;
                    if (alpha == BASE_MASK)
                    {
                        pointAndColors.AddVertex(
                            new VertexC4V2S(sourceColor.ToARGB(), xpos, y));
                    }
                    else
                    {
                        pointAndColors.AddVertex(
                            new VertexC4V2S(LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor).ToARGB(),
                            xpos, y));
                    }
                    xpos++;
                    coversIndex++;
                }
                while (--len != 0);
            }
        }
        //======================================================================================


        //public void RenderWithSpan(IImageReaderWriter dest,
        //         GLScanlineRasterizer sclineRas,
        //         GLScanline scline,
        //        ISpanGenerator spanGenerator)
        //{
        //    if (!sclineRas.RewindScanlines()) { return; } //early exit
        //    //-----------------------------------------------

        //    scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);

        //    spanGenerator.Prepare();


        //    if (dest.Stride / 4 > (tempSpanColors.AllocatedSize))
        //    {
        //        //if not enough -> alloc more
        //        tempSpanColors.Clear(dest.Stride / 4);
        //    }

        //    ColorRGBA[] colorArray = tempSpanColors.Array;

        //    while (sclineRas.SweepScanline(scline))
        //    {

        //        //render single scanline 
        //        int y = scline.Y;
        //        int num_spans = scline.SpanCount;
        //        byte[] covers = scline.GetCovers();

        //        for (int i = 1; i <= num_spans; ++i)
        //        {
        //            ScanlineSpan span = scline.GetSpan(i);
        //            int x = span.x;
        //            int span_len = span.len;
        //            bool firstCoverForAll = false;

        //            if (span_len < 0) { span_len = -span_len; firstCoverForAll = true; } //make absolute value

        //            //1. generate colors -> store in colorArray
        //            spanGenerator.GenerateColors(colorArray, 0, x, y, span_len);

        //            //2. blend color in colorArray to destination image
        //            dest.BlendColorHSpan(x, y, span_len,
        //                colorArray, 0,
        //                covers, span.cover_index,
        //                firstCoverForAll);
        //        }

        //    }
        //}




        //======================================================================================

        //following methods for render with vertext array
        //ArrayList<int> xyPointBuffer = new ArrayList<int>();
        //ArrayList<uint> pointColorBuffer = new ArrayList<uint>();
        //public void RenderWithColor(GLScanlineRasterizer sclineRas,
        //    GLScanline scline,
        //    LayoutFarm.Drawing.Color color)
        //{
        //    if (!sclineRas.RewindScanlines()) { return; } //early exit
        //    //----------------------------------------------- 
        //    scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
        //    //----------------------------------------------- 

        //    GL.EnableClientState(ArrayCap.ColorArray);
        //    GL.EnableClientState(ArrayCap.VertexArray);
        //    while (sclineRas.SweepScanline(scline))
        //    {
        //        this.pointColorBuffer.Clear();
        //        this.xyPointBuffer.Clear();
        //        //render solid single scanline
        //        int y = scline.Y;
        //        int num_spans = scline.SpanCount;
        //        byte[] covers = scline.GetCovers();
        //        for (int i = 1; i <= num_spans; ++i)
        //        {
        //            ScanlineSpan span = scline.GetSpan(i);
        //            if (span.len > 0)
        //            {
        //                //outline
        //                xGLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
        //            }
        //            else
        //            {
        //                //fill
        //                int x = span.x;
        //                int x2 = (x - span.len - 1);
        //                xGLBlendHL(x, y, x2, color, covers[span.cover_index]);
        //            }
        //        }

        //        //DrawPoints(xyPointBuffer, pointColorBuffer);
        //        xDrawPointsWithVertexBuffer(xyPointBuffer, pointColorBuffer);
        //    }
        //    GL.DisableClientState(ArrayCap.ColorArray);
        //    GL.DisableClientState(ArrayCap.VertexArray);
        //    //------------------------

        //}
        //static void xDrawPointsWithVertexBuffer(ArrayList<int> pointsList, ArrayList<uint> colorsList)
        //{
        //    unsafe
        //    {
        //        int n = pointsList.Count / 2;
        //        int[] points = pointsList.Array;
        //        uint[] cbuff = colorsList.Array;

        //        fixed (uint* cbuff0 = &cbuff[0])
        //        fixed (int* arr = &points[0])
        //        {
        //            VertexC4ubV3f[] vpoints = new VertexC4ubV3f[n];
        //            int mm = 0;
        //            //vertices and color
        //            for (int i = 0; i < n; ++i)
        //            {
        //                vpoints[i] = new VertexC4ubV3f(cbuff0[i], pointsList[mm], pointsList[mm + 1]);
        //                mm += 2;
        //            }

        //            var vbo = LoadVBO(vpoints);
        //            int stride = BlittableValueType.StrideOf(vpoints);

        //            int nelements = vpoints.Length;
        //            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(stride * nelements), IntPtr.Zero, BufferUsageHint.StreamDraw);
        //            // Fill newly allocated buffer
        //            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(stride * nelements), vpoints, BufferUsageHint.StreamDraw);
        //            // Only draw particles that are alive
        //            GL.DrawArrays(BeginMode.Points, 0, vpoints.Length);

        //        }
        //    }
        //}
        //void xGLBlendHL(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
        //{
        //    if (color.A == 0) { return; }

        //    int len = x2 - x1 + 1;
        //    int alpha = (((int)(color.A) * (cover + 1)) >> 8);

        //    var colors = this.pointColorBuffer;
        //    var points = this.xyPointBuffer;
        //    if (alpha == BASE_MASK)
        //    {

        //        switch (len)
        //        {
        //            case 0:
        //                {
        //                } break;
        //            case 1:
        //                {
        //                    //colors.AddVertex(LayoutFarm.Drawing.Color.FromArgb(alpha, color)); 
        //                    var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);
        //                    colors.AddVertex(c.ToARGB());
        //                    points.AddVertex(x1);
        //                    points.AddVertex(y);
        //                } break;
        //            default:
        //                {
        //                    for (int i = 0; i < len; ++i)
        //                    {
        //                        //colors.AddVertex(LayoutFarm.Drawing.Color.FromArgb(alpha, color)); 
        //                        var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);
        //                        colors.AddVertex(c.ToARGB());
        //                        points.AddVertex(x1 + i);
        //                        points.AddVertex(y);
        //                    }
        //                    //DrawLine(x1, y, x2 + 1, y);
        //                } break;
        //        }
        //    }
        //    else
        //    {
        //        int xpos = x1;
        //        do
        //        {

        //            var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color);
        //            colors.AddVertex(c.ToARGB());
        //            points.AddVertex(xpos);
        //            points.AddVertex(y);

        //            xpos++;
        //        }
        //        while (--len != 0);
        //    }
        //}
        //void xGLBlendSolidHSpan(int x, int y, int len,
        //    LayoutFarm.Drawing.Color sourceColor,
        //    byte[] covers, int coversIndex)
        //{
        //    int colorAlpha = sourceColor.A;
        //    if (colorAlpha == 0) { return; }

        //    unchecked
        //    {

        //        int xpos = x;
        //        var colors = this.pointColorBuffer;
        //        var points = this.xyPointBuffer;
        //        do
        //        {
        //            //foreach single pixel
        //            int alpha = ((colorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
        //            if (alpha == BASE_MASK)
        //            {
        //                colors.AddVertex(sourceColor.ToARGB());
        //                points.AddVertex(xpos);
        //                points.AddVertex(y);
        //                xpos++;
        //            }
        //            else
        //            {

        //                var c = LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor);
        //                colors.AddVertex(c.ToARGB());
        //                points.AddVertex(xpos);
        //                points.AddVertex(y);
        //                xpos++;
        //            }
        //            coversIndex++;
        //        }
        //        while (--len != 0);
        //    }
        //} 
    }

}
