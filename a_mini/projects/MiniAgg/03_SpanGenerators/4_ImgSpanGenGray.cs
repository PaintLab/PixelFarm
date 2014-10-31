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


using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using img_filter_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgFilterConst;

namespace PixelFarm.Agg.Image
{
    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
    class ImgSpanGenGray_NNStepXby1 : ImgSpanGen
    {
        const int BASE_SHIFT = 8;
        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
        const int BASE_MASK = BASE_SCALE - 1;

        public ImgSpanGenGray_NNStepXby1(IImageBufferAccessor sourceAccessor, ISpanInterpolator spanInterpolator)
            : base(sourceAccessor, spanInterpolator, null)
        {
        }

        public override void Generate(ColorRGBA[] span, int spanIndex, int x, int y, int len)
        {
            ImageBase SourceRenderingBuffer = (ImageBase)ImgBuffAccessor.SourceImage;
            int bytesBetweenPixelsInclusive = SourceRenderingBuffer.BytesBetweenPixelsInclusive;
            if (SourceRenderingBuffer.BitDepth != 8)
            {
                throw new NotSupportedException("The source is expected to be 32 bit.");
            }
            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + Dx, y + Dy, len);
            int x_hr;
            int y_hr;
            spanInterpolator.GetCoord(out x_hr, out y_hr);
            int x_lr = x_hr >> (int)img_subpix_const.SHIFT;
            int y_lr = y_hr >> (int)img_subpix_const.SHIFT;
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
