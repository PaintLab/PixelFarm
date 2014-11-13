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

        public GLScanlineRasToDestBitmapRenderer()
        {

        }
        protected bool UseCustomRenderSingleScanLine
        {
            get;
            set;
        }

        public void RenderWithColor(IImageReaderWriter dest,
                GLScanlineRasterizer sclineRas,
                GLScanline scline,
                ColorRGBA color)
        {
            if (!sclineRas.RewindScanlines()) { return; } //early exit
            //-----------------------------------------------

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);

            if (this.UseCustomRenderSingleScanLine)
            {
                while (sclineRas.SweepScanline(scline))
                {
                    CustomRenderSingleScanLine(dest, scline, color);
                }
            }
            else
            {

                while (sclineRas.SweepScanline(scline))
                {
                    //render solid single scanline
                    int y = scline.Y;
                    int num_spans = scline.SpanCount;
                    byte[] covers = scline.GetCovers();
                    for (int i = 1; i <= num_spans; ++i)
                    {
                        ScanlineSpan span = scline.GetSpan(i);
                        if (span.len > 0)
                        {
                            dest.BlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                        }
                        else
                        {
                            int x = span.x;
                            int x2 = (x - span.len - 1);
                            dest.BlendHL(x, y, x2, color, covers[span.cover_index]);
                        }
                    }
                }

            }
        }
        public void RenderWithColor2(GLScanlineRasterizer sclineRas,
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

                while (sclineRas.SweepScanline(scline))
                {
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
                            //dest.BlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                        }
                        else
                        {
                            //fill
                            int x = span.x;
                            int x2 = (x - span.len - 1);
                            //dest.BlendHL(x, y, x2, color, covers[span.cover_index]);
                            GLBlendHL(x, y, x2, color, covers[span.cover_index]);
                        }
                    }
                }

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
            if (alpha == BASE_MASK)
            {
                GL.Color4(color); 
                DrawLine(x1, y, x2 + 1, y);

            }
            else
            {
                int xpos = x1;
                do
                {
                    GL.Color4(LayoutFarm.Drawing.Color.FromArgb(alpha, color));  
                    DrawPoint(xpos, y);
                    xpos++;
                }
                while (--len != 0);
            }

        }

        void GLBlendSolidHSpan(int x, int y, int len, LayoutFarm.Drawing.Color sourceColor, byte[] covers, int coversIndex)
        {
            int colorAlpha = sourceColor.A;
            if (colorAlpha == 0) { return; }

            unchecked
            {

                int xpos = x;
                do
                {
                    //foreach single pixel
                    int alpha = ((colorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                    if (alpha == BASE_MASK)
                    {
                        GL.Color4(sourceColor); 
                        DrawPoint(xpos, y);
                        xpos++;
                    }
                    else
                    {
                        GL.Color4(LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor)); 
                        DrawPoint(xpos, y);
                        xpos++;
                    }
                    coversIndex++;
                }
                while (--len != 0);
            }

        }
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
