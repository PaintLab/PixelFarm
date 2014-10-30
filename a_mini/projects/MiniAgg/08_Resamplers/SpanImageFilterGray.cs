//2014 BSD,WinterDev   
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
#define USE_UNSAFE_CODE

using System;

using MatterHackers.Agg.Image;

using image_subpixel_scale_e = MatterHackers.Agg.ImageFilterLookUpTable.image_subpixel_scale_e;
using image_filter_scale_e = MatterHackers.Agg.ImageFilterLookUpTable.image_filter_scale_e;

namespace MatterHackers.Agg
{
    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
    class SpanImageFilterGray_NNStepXby1 : SpanImageFilter
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        public SpanImageFilterGray_NNStepXby1(IImageBufferAccessor sourceAccessor, ISpanInterpolator spanInterpolator)
            : base(sourceAccessor, spanInterpolator, null)
        {
        }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageBase SourceRenderingBuffer = (ImageBase)GetImageBufferAccessor().SourceImage;
            int bytesBetweenPixelsInclusive = SourceRenderingBuffer.GetBytesBetweenPixelsInclusive();
            if (SourceRenderingBuffer.BitDepth != 8)
            {
                throw new NotSupportedException("The source is expected to be 32 bit.");
            }
            ISpanInterpolator spanInterpolator = interpolator();
            spanInterpolator.Begin(x + filter_dx_dbl(), y + filter_dy_dbl(), len);
            int x_hr;
            int y_hr;
            spanInterpolator.GetCoord(out x_hr, out y_hr);
            int x_lr = x_hr >> (int)image_subpixel_scale_e.image_subpixel_shift;
            int y_lr = y_hr >> (int)image_subpixel_scale_e.image_subpixel_shift;
            int bufferIndex;
            bufferIndex = SourceRenderingBuffer.GetBufferOffsetXY(x_lr, y_lr);

            byte[] fg_ptr = SourceRenderingBuffer.GetBuffer();
#if USE_UNSAFE_CODE
            unsafe
            {
                fixed (byte* pSource = fg_ptr)
                {
                    do
                    {
                        span[spanIndex].red = pSource[bufferIndex];
                        span[spanIndex].green = pSource[bufferIndex];
                        span[spanIndex].blue = pSource[bufferIndex];
                        span[spanIndex].alpha = 255;
                        spanIndex++;
                        bufferIndex += bytesBetweenPixelsInclusive;
                    } while (--len != 0);
                }
            }
#else
            do
            {
                throw new Exception("this code is for 32 bit");
                color.m_B = fg_ptr[bufferIndex++];
                color.m_G = fg_ptr[bufferIndex++];
                color.m_R = fg_ptr[bufferIndex++];
                color.m_A = fg_ptr[bufferIndex++];
                span[spanIndex++] = color;
            } while (--len != 0);
#endif
        }
    }

    
}
