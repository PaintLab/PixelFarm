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


    public struct PatternFilterBilnearColorRGBA : IPatternFilter
    {
        public int Dilation { get { return 1; } }

        public void SetPixelLowRes(ColorRGBA[][] buf, ColorRGBA[] p, int offset, int x, int y)
        {
            p[offset] = buf[y][x];
        }

        public void SetPixelHighRes(ImageBase sourceImage, 
            ColorRGBA[] destBuffer,
            int destBufferOffset, 
            int x, 
            int y)
        {
            int r, g, b, a;
            r = g = b = a = LineAABasics.SUBPIXEL_SCALE * LineAABasics.SUBPIXEL_SCALE / 2;

            int weight;
            int x_lr = x >> LineAABasics.SUBPIXEL_SHIFT;
            int y_lr = y >> LineAABasics.SUBPIXEL_SHIFT;

            x &= LineAABasics.SUBPIXEL_MARK;
            y &= LineAABasics.SUBPIXEL_MARK;
            int sourceOffset;
            byte[] ptr = sourceImage.GetPixelPointerXY(x_lr, y_lr, out sourceOffset);

            weight = (LineAABasics.SUBPIXEL_SCALE - x) *
                     (LineAABasics.SUBPIXEL_SCALE - y);
            r += weight * ptr[sourceOffset + ImageBase.OrderR];
            g += weight * ptr[sourceOffset + ImageBase.OrderG];
            b += weight * ptr[sourceOffset + ImageBase.OrderB];
            a += weight * ptr[sourceOffset + ImageBase.OrderA];

            sourceOffset += sourceImage.GetBytesBetweenPixelsInclusive();

            weight = x * (LineAABasics.SUBPIXEL_SCALE - y);
            r += weight * ptr[sourceOffset + ImageBase.OrderR];
            g += weight * ptr[sourceOffset + ImageBase.OrderG];
            b += weight * ptr[sourceOffset + ImageBase.OrderB];
            a += weight * ptr[sourceOffset + ImageBase.OrderA];

            ptr = sourceImage.GetPixelPointerXY(x_lr, y_lr + 1, out sourceOffset);

            weight = (LineAABasics.SUBPIXEL_SCALE - x) * y;
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

            destBuffer[destBufferOffset].red = (byte)(r >> LineAABasics.SUBPIXEL_SHIFT * 2);
            destBuffer[destBufferOffset].green = (byte)(g >> LineAABasics.SUBPIXEL_SHIFT * 2);
            destBuffer[destBufferOffset].blue = (byte)(b >> LineAABasics.SUBPIXEL_SHIFT * 2);
            destBuffer[destBufferOffset].alpha = (byte)(a >> LineAABasics.SUBPIXEL_SHIFT * 2);
        }
    } 
}
