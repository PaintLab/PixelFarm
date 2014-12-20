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
using OpenTK.Graphics.ES20;
using LayoutFarm.DrawingGL;

namespace PixelFarm.Agg
{

    /// <summary
    /// to bitmap
    /// </summary>  
    public class GLScanlineRasToDestBitmapRenderer
    {

        ScanlineShader scanlineShader = new ScanlineShader();
        ArrayList<VertexV2S1Cvr> mySinglePixelBuffer = new ArrayList<VertexV2S1Cvr>();
        ArrayList<VertexV2S1Cvr> myLineBuffer = new ArrayList<VertexV2S1Cvr>();

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

            throw new NotSupportedException();
            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray);


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

            //----------------------------------------
            //draw ...
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
            //GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.VertexArray);
            ////------------------------ 
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


            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray);


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
            //GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.VertexArray);
            //------------------------ 
        }

        static void DrawPoint(float x1, float y1)
        {
            unsafe
            {
                float* arr = stackalloc float[2];
                arr[0] = x1; arr[1] = y1;


                throw new NotSupportedException();
                //GL.EnableClientState(ArrayCap.VertexArray); //***
                ////vertex
                //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr); 
                //GL.DrawArrays(BeginMode.Points, 0, 1);
                //GL.DisableClientState(ArrayCap.VertexArray);
            }
        }

        static void DrawPoints(ArrayList<int> pointsList, ArrayList<uint> colorsList)
        {
            unsafe
            {
                int n = pointsList.Count / 2;
                int[] points = pointsList.Array;
                uint[] cbuff = colorsList.Array;

                fixed (uint* cbuff0 = &cbuff[0])
                fixed (int* arr = &points[0])
                {

                    throw new NotSupportedException();
                    //GL.EnableClientState(ArrayCap.ColorArray);
                    //GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, (IntPtr)cbuff0); 
                    //GL.EnableClientState(ArrayCap.VertexArray); //***
                    ////vertex
                    //GL.VertexPointer(2, VertexPointerType.Int, 0, (IntPtr)arr); 
                    //GL.DrawArrays(BeginMode.Points, 0, n);
                    //GL.DisableClientState(ArrayCap.VertexArray);
                    //GL.DisableClientState(ArrayCap.ColorArray);
                }
            }
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

        static void DrawLine(float x1, float y1, float x2, float y2)
        {
            unsafe
            {
                float* arr = stackalloc float[4];
                arr[0] = x1; arr[1] = y1;
                arr[2] = x2; arr[3] = y2;

                throw new NotSupportedException();
                //GL.EnableClientState(ArrayCap.VertexArray); //***
                ////vertex
                //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);

                //GL.DrawArrays(BeginMode.Lines, 0, 2);
                //GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        const int BASE_MASK = 255;


        //======================================================================================
        static void DrawPointsWithVertexBuffer(ArrayList<VertexV2S1Cvr> singlePxBuffer, int nelements)
        {
            unsafe
            {

                ////--------------------------------------------- 
                //VertexC4V2S[] vpoints = singlePxBuffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                //// Fill newly allocated buffer
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsage.StreamDraw);
                //// Only draw particles that are alive
                //GL.DrawArrays(BeginMode.Points, 0, nelements);

                throw new NotSupportedException();
                //--------------------------------------------- 
            }
        }
        static void DrawLinesWithVertexBuffer(ArrayList<VertexV2S1Cvr> linesBuffer, int nelements)
        {
            unsafe
            {   
                //VertexC4V2S[] vpoints = linesBuffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                //// Fill newly allocated buffer
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsage.StreamDraw);
                //// Only draw particles that are alive
                //GL.DrawArrays(BeginMode.Lines, 0, nelements);

                throw new NotSupportedException();
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
                            //singlePxBuff.AddVertex(new VertexV2S1Cvr(
                            //    LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                            //    x1, y));
                            singlePxBuff.AddVertex(new VertexV2S1Cvr(x1, y, alpha));
                            //LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                            //x1, y));
                        } break;
                    default:
                        {
                            //var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                            lineBuffer.AddVertex(new VertexV2S1Cvr(x1, y, alpha));
                            lineBuffer.AddVertex(new VertexV2S1Cvr(x2 + 1, y, alpha));

                        } break;
                }
            }
            else
            {
                int xpos = x1;
                do
                {
                    singlePxBuff.AddVertex(new VertexV2S1Cvr(xpos, y, alpha));
                    xpos++;
                }
                while (--len != 0);
            }
        }
        void GLBlendSolidHSpan(int x, int y, int len,
            LayoutFarm.Drawing.Color sourceColor,
            byte[] covers, int coversIndex)
        {

            unchecked
            {

                int xpos = x;
                var pointAndColors = this.mySinglePixelBuffer;

                do
                {
                    //alpha change
                    int alpha = ((sourceColor.A) * ((covers[coversIndex]) + 1)) >> 8;
                    pointAndColors.AddVertex(new VertexV2S1Cvr(xpos, y, alpha));

                    //if (alpha == BASE_MASK)
                    //{
                    //    pointAndColors.AddVertex(
                    //        new VertexV2S1Cvr(xpos, y, alpha));
                    //}
                    //else
                    //{
                    //    pointAndColors.AddVertex(
                    //        new VertexC4V2S(LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor).ToARGB(),
                    //        xpos, y));
                    //}
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


    }

}
