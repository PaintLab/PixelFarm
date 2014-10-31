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

using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

namespace MatterHackers.Agg
{
    /// <summary>
    /// to bitmap
    /// </summary>  
    public class ScanlineRasToDestBitmapRenderer
    {

        ArrayList<ColorRGBA> tempSpanColors = new ArrayList<ColorRGBA>();

        public void RenderScanlineSolidAA(IImage destImage,
            ScanlineRasterizer rasterizer,
            Scanline scline,
            ColorRGBA color)
        {
            if (rasterizer.RewindScanlines())
            {
                scline.ResetSpans(rasterizer.MinX, rasterizer.MaxX);
                while (rasterizer.SweepScanline(scline))
                {
                    RenderSolidSingleScanLine(destImage, scline, color);
                }
            }
        }

        public void GenerateAndRender(IImage destImage,
             ScanlineRasterizer rasterizer,
             Scanline scline,
             ISpanGenerator spanGenerator)
        {
            if (rasterizer.RewindScanlines())
            {
                scline.ResetSpans(rasterizer.MinX, rasterizer.MaxX);
                spanGenerator.Prepare();
                while (rasterizer.SweepScanline(scline))
                {
                    GenerateAndRenderSingleScanline(destImage, scline, spanGenerator);
                }
            }
        }


        protected virtual void RenderSolidSingleScanLine(IImage destImage,
            Scanline scline,
            ColorRGBA color)
        {

            int y = scline.Y;
            int num_spans = scline.SpanCount;
            byte[] covers = scline.GetCovers();
            for (int i = 1; i <= num_spans; ++i)
            {
                ScanlineSpan span = scline.GetSpan(i);
                int x = span.x;
                if (span.len > 0)
                {
                    destImage.BlendSolidHSpan(x, y, span.len, color, covers, span.cover_index);
                }
                else
                {
                    int x2 = (x - (int)span.len - 1);
                    destImage.BlendHL(x, y, x2, color, covers[span.cover_index]);
                }
            }
        }


        public void RenderSolidAllPaths(IImage destImage,
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

        void GenerateAndRenderSingleScanline(IImage destImage, Scanline scline, ISpanGenerator span_gen)
        {

            int y = scline.Y;
            int num_spans = scline.SpanCount;
            byte[] covers = scline.GetCovers();

            for (int i = 1; i <= num_spans; ++i)
            {
                ScanlineSpan span = scline.GetSpan(i);
                int x = span.x;
                int len = span.len; 

                if (len < 0) { len = -len; } //make absolute value

                if (len > tempSpanColors.AllocatedSize)
                {
                    //if not enough -> alloc more
                    tempSpanColors.Clear(len);
                }

                var colorArray = tempSpanColors.Array;
                span_gen.Generate(colorArray, 0, x, y, len);

                bool useFirstCoverForAll = span.len < 0;

                destImage.BlendColorHSpan(x, y, len,
                    colorArray, 0,
                    covers, span.cover_index, useFirstCoverForAll);
            }
        }



    }
}
