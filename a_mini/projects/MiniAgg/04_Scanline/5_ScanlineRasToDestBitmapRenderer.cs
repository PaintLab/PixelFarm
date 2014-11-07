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

namespace PixelFarm.Agg
{
    /// <summary>
    /// to bitmap
    /// </summary>  
    public class ScanlineRasToDestBitmapRenderer
    {

        ArrayList<ColorRGBA> tempSpanColors = new ArrayList<ColorRGBA>();

        internal ScanlineRasToDestBitmapRenderer()
        {

        }
        protected bool UseCustomSolidSingleLineMethod
        {
            get;
            set;
        }

        public void RenderScanlineSolidAA(IImageReaderWriter destImage,
            ScanlineRasterizer rasterizer,
            Scanline scline,
            ColorRGBA color)
        {
            if (rasterizer.RewindScanlines())
            {
                scline.ResetSpans(rasterizer.MinX, rasterizer.MaxX);

                if (this.UseCustomSolidSingleLineMethod)
                {
                    while (rasterizer.SweepScanline(scline))
                    {
                        CustomRenderSolidSingleScanLine(destImage, scline, color);
                    }
                }
                else
                {

                    while (rasterizer.SweepScanline(scline))
                    {
                        RenderSolidSingleScanLine(destImage, scline, color);
                    }
                }
            }
        }

        public void GenerateAndRender(IImageReaderWriter destImage,
             ScanlineRasterizer sclineRas,
             Scanline scline,
             ISpanGenerator spanGenerator)
        {
            if (sclineRas.RewindScanlines())
            {
                scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
                spanGenerator.Prepare();

                if (destImage.Stride > tempSpanColors.AllocatedSize)
                {
                    //if not enough -> alloc more
                    tempSpanColors.Clear(destImage.Stride);
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


                        spanGenerator.GenerateColors(colorArray, 0, x, y, span_len);

                        destImage.BlendColorHSpan(x, y, span_len,
                            colorArray, 0,
                            covers, span.cover_index,
                            firstCoverForAll);
                    } 
                 
                }
            }
        }


        static void RenderSolidSingleScanLine(
             IImageReaderWriter destImage,
             Scanline scline,
             ColorRGBA color)
        {
            int y = scline.Y;
            int num_spans = scline.SpanCount;
            byte[] covers = scline.GetCovers();
            for (int i = 1; i <= num_spans; ++i)
            {
                ScanlineSpan span = scline.GetSpan(i);

                if (span.len > 0)
                {
                    destImage.BlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                }
                else
                {
                    int x = span.x;
                    int x2 = (x - (int)span.len - 1);
                    destImage.BlendHL(x, y, x2, color, covers[span.cover_index]);
                }
            }
        }
        protected virtual void CustomRenderSolidSingleScanLine(
            IImageReaderWriter destImage,
            Scanline scline,
            ColorRGBA color)
        {
            RenderSolidSingleScanLine(destImage, scline, color);
        }

        public void RenderSolidAllPaths(IImageReaderWriter destImage,
            ScanlineRasterizer ras,
            Scanline scline,
            VertexStore vx,
            ColorRGBA[] color_storage,
            int[] path_id,
            int num_paths)
        {
            for (int i = 0; i < num_paths; ++i)
            {
                ras.Reset();
                ras.AddPath(new VertexStoreSnap(vx, path_id[i]));
                RenderScanlineSolidAA(destImage, ras, scline, color_storage[i]);
            }
        }

         
    }


    //----------------------------
    public class CustomScanlineRasToDestBitmapRenderer : ScanlineRasToDestBitmapRenderer
    {

    }
}
