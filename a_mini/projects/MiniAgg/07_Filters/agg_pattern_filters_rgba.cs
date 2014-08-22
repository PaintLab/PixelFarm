//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
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
using MatterHackers.Agg.Image;

namespace MatterHackers.Agg
{
   

     

    public struct pattern_filter_bilinear_RGBA_Bytes : IPatternFilter
    {
        public int dilation() { return 1; }

        public void pixel_low_res(ColorRGBA[][] buf, ColorRGBA[] p, int offset, int x, int y)
        {
            p[offset] = buf[y][x];
        }

        public void pixel_high_res(ImageBase sourceImage, ColorRGBA[] destBuffer, int destBufferOffset, int x, int y)
        {
            int r, g, b, a;
            r = g = b = a = LineAABasics.line_subpixel_scale * LineAABasics.line_subpixel_scale / 2;

            int weight;
            int x_lr = x >> LineAABasics.line_subpixel_shift;
            int y_lr = y >> LineAABasics.line_subpixel_shift;

            x &= LineAABasics.line_subpixel_mask;
            y &= LineAABasics.line_subpixel_mask;
            int sourceOffset;
            byte[] ptr = sourceImage.GetPixelPointerXY(x_lr, y_lr, out sourceOffset);

            weight = (LineAABasics.line_subpixel_scale - x) *
                     (LineAABasics.line_subpixel_scale - y);
            r += weight * ptr[sourceOffset + ImageBase.OrderR];
            g += weight * ptr[sourceOffset + ImageBase.OrderG];
            b += weight * ptr[sourceOffset + ImageBase.OrderB];
            a += weight * ptr[sourceOffset + ImageBase.OrderA];

            sourceOffset += sourceImage.GetBytesBetweenPixelsInclusive();

            weight = x * (LineAABasics.line_subpixel_scale - y);
            r += weight * ptr[sourceOffset + ImageBase.OrderR];
            g += weight * ptr[sourceOffset + ImageBase.OrderG];
            b += weight * ptr[sourceOffset + ImageBase.OrderB];
            a += weight * ptr[sourceOffset + ImageBase.OrderA];

            ptr = sourceImage.GetPixelPointerXY(x_lr, y_lr + 1, out sourceOffset);

            weight = (LineAABasics.line_subpixel_scale - x) * y;
            r += weight * ptr[sourceOffset + ImageBase.OrderR];
            g += weight * ptr[sourceOffset + ImageBase.OrderG];
            b += weight * ptr[sourceOffset + ImageBase.OrderB];
            a += weight * ptr[sourceOffset + ImageBase.OrderA];

            sourceOffset += sourceImage.GetBytesBetweenPixelsInclusive();

            weight = x * y;
            r += weight * ptr[sourceOffset + ImageBase.OrderR];
            g += weight * ptr[sourceOffset + ImageBase.OrderG];
            b += weight * ptr[sourceOffset + ImageBase.OrderB];
            a += weight * ptr[sourceOffset + ImageBase.OrderA];

            destBuffer[destBufferOffset].red = (byte)(r >> LineAABasics.line_subpixel_shift * 2);
            destBuffer[destBufferOffset].green = (byte)(g >> LineAABasics.line_subpixel_shift * 2);
            destBuffer[destBufferOffset].blue = (byte)(b >> LineAABasics.line_subpixel_shift * 2);
            destBuffer[destBufferOffset].alpha = (byte)(a >> LineAABasics.line_subpixel_shift * 2);
        }
    };
}
