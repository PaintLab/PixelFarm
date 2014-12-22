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
                        GLBlendHLine(x, y, x2, color, covers[span.cover_index]);
                    }
                }
            }


            //GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            //---------------------------------------------
            //points
            int nelements = mySinglePixelBuffer.Count;
            // VboC4V2S vbo = GenerateVBOForC4V2I();
            if (nelements > 0)
            {
                //vbo.BindBuffer();
                DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements);
                // vbo.UnbindBuffer();
            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                //vbo.BindBuffer();
                DrawLinesWithVertexBuffer(myLineBuffer, nelements);
                //vbo.UnbindBuffer();
            }
            //---------------------------------------------

            //vbo.Dispose();
            //GL.DisableClientState(ArrayCap.ColorArray);
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
                        GLBlendHLine(x, y, x2, color, covers[span.cover_index]);
                    }
                }
            }


            //single color***
            GL.Color4(color.R, color.G, color.B, color.A);
            //GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            //---------------------------------------------
            //points
            int nelements = mySinglePixelBuffer.Count;
            // VboC4V2S vbo = GenerateVBOForC4V2I();
            if (nelements > 0)
            {
                //vbo.BindBuffer();
                DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements);
                //vbo.UnbindBuffer();
            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                //vbo.BindBuffer();
                DrawLinesWithVertexBuffer(myLineBuffer, nelements);
                //vbo.UnbindBuffer();
            }
            //---------------------------------------------

            //vbo.Dispose();
            //GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            //------------------------ 
        } 
        const int BASE_MASK = 255; 
        //======================================================================================
        static void DrawPointsWithVertexBuffer(ArrayList<VertexC4V2S> singlePxBuffer, int nelements)
        {
            unsafe
            {

                //--------------------------------------------- 
                VertexC4V2S[] vpoints = singlePxBuffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                ////GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                //// Fill newly allocated buffer                
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);

                fixed (VertexC4V2S* vp = &vpoints[0])
                {
                    GL.VertexPointer(2,
                        VertexPointerType.Short,
                        0,
                        (IntPtr)(vp));

                    //GL.VertexAttribPointer(0, 3,
                    //    VertexAttribPointerType.Float,
                    //    false, VertexC4V2S.SIZE_IN_BYTES * nelements, (IntPtr)vp);
                }
                //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride_size, 0);
                // Only draw particles that are alive
                GL.DrawArrays(BeginMode.Points, 0, nelements);

                //--------------------------------------------- 
            }
        }
        static void DrawLinesWithVertexBuffer(ArrayList<VertexC4V2S> linesBuffer, int nelements)
        {
            unsafe
            {
                //VertexC4V2S[] vpoints = linesBuffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                ////GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                //// Fill newly allocated buffer
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);

                //GL.DrawArrays(BeginMode.Lines, 0, nelements);

                //--------------------------------------------- 
                VertexC4V2S[] vpoints = linesBuffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V2S.SIZE_IN_BYTES * nelements);
                ////GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                //// Fill newly allocated buffer                
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);

                fixed (VertexC4V2S* vp = &vpoints[0])
                {
                    GL.VertexPointer(2,
                        VertexPointerType.Short,
                        0,
                        (IntPtr)(vp));
                    //GL.VertexAttribPointer(0, 3,
                    //    VertexAttribPointerType.Float,
                    //    false, VertexC4V2S.SIZE_IN_BYTES * nelements, (IntPtr)vp);
                }
                //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride_size, 0);
                // Only draw particles that are alive
                GL.DrawArrays(BeginMode.Lines, 0, nelements);

            }
        }
        void GLBlendHLine(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
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
    }

}
