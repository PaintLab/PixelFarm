//BSD, 2014-2017, WinterDev
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

using System;
namespace PixelFarm.Agg.Imaging
{
    public static class ImageHelper
    {
        /// <summary>
        /// This will create a new ImageBuffer that references the same memory as the image that you took the sub image from.
        /// It will modify the original main image when you draw to it.
        /// </summary>
        /// <param name="parentImage"></param>
        /// <param name="subImgBounds"></param>
        /// <returns></returns>
        public static SubImageRW CreateSubImgRW(IImageReaderWriter parentImage, RectInt subImgBounds)
        {
            if (subImgBounds.Left < 0 || subImgBounds.Bottom < 0 || subImgBounds.Right > parentImage.Width || subImgBounds.Top > parentImage.Height
                || subImgBounds.Left >= subImgBounds.Right || subImgBounds.Bottom >= subImgBounds.Top)
            {
                throw new ArgumentException("The subImageBounds must be on the image and valid.");
            }

            int left = Math.Max(0, subImgBounds.Left);
            int bottom = Math.Max(0, subImgBounds.Bottom);
            int width = Math.Min(parentImage.Width - left, subImgBounds.Width);
            int height = Math.Min(parentImage.Height - bottom, subImgBounds.Height);
            int bufferOffsetToFirstPixel = parentImage.GetByteBufferOffsetXY(left, bottom);
            return new SubImageRW(parentImage, bufferOffsetToFirstPixel, width, height);
        }
    }
}