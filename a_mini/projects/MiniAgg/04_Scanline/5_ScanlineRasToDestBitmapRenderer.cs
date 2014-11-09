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
        protected bool UseCustomRenderSingleScanLine
        {
            get;
            set;
        }

        public void RenderWithColor(IImageReaderWriter dest,
                ScanlineRasterizer sclineRas,
                Scanline scline,
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

        public void RenderWithSpan(IImageReaderWriter dest,
                ScanlineRasterizer sclineRas,
                Scanline scline,
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
                //if (y >= 239)
                //{
                //    break;
                //}

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
            Scanline scline,
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
