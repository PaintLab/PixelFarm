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
    /// <summary>
    /// sub-image reader /writer
    /// </summary>
    public class SubImageRW : ImageReaderWriterBase
    {
        public SubImageRW(IImageReaderWriter image,
            int bufferOffsetToFirstPixel,
            int width,
            int height)
        {
            SetRecieveBlender(image.GetRecieveBlender());
            AttachBuffer(image.GetInt32Buffer(),
                bufferOffsetToFirstPixel,
                width,
                height,
                image.Stride,
                image.BitDepth,
                image.BytesBetweenPixelsInclusive);
        }

        public SubImageRW(int[] buffer,
            int bufferOffsetToFirstPixel,
            int width,
            int height,
            int strideInBytes,
            int bitDepth,
            int distanceInBytesBetweenPixelsInclusive)
        {
            AttachBuffer(buffer,
                bufferOffsetToFirstPixel,
                width,
                height,
                strideInBytes, bitDepth,
                distanceInBytesBetweenPixelsInclusive);
        }
        public SubImageRW(IImageReaderWriter image,
            IPixelBlender blender,
            int distanceBetweenPixelsInclusive,
            int bufferOffset,
            int bitsPerPixel)
        {
            SetRecieveBlender(blender);
            Attach(image, blender, distanceBetweenPixelsInclusive, bufferOffset, bitsPerPixel);
        }
        public SubImageRW(IImageReaderWriter image, IPixelBlender blender)
        {
            Attach(image, blender, image.BytesBetweenPixelsInclusive, 0, image.BitDepth);
        }
        public SubImageRW(IImageReaderWriter image, IPixelBlender blender, int x1, int y1, int x2, int y2)
        {
            SetRecieveBlender(blender);
            Attach(image, x1, y1, x2, y2);
        }
        public override void ReplaceBuffer(int[] newbuffer)
        {
            if (_sourceImage != null)
            {
                _sourceImage.ReplaceBuffer(newbuffer);
            }

        }

        void AttachBuffer(int[] buffer,
          int bufferOffset,
          int width,
          int height,
          int strideInBytes,
          int bitDepth,
          int distanceInBytesBetweenPixelsInclusive)
        {

            SetBufferToNull();
            SetDimmensionAndFormat(width, height, strideInBytes, bitDepth,
                distanceInBytesBetweenPixelsInclusive);
            SetBuffer(buffer, bufferOffset);

        }

        IImageReaderWriter _sourceImage;
        void Attach(IImageReaderWriter sourceImage,
          IPixelBlender recieveBlender,
          int distanceBetweenPixelsInclusive,
          int bufferOffset,
          int bitsPerPixel)
        {
            _sourceImage = sourceImage;
            SetDimmensionAndFormat(sourceImage.Width,
                sourceImage.Height,
                sourceImage.Stride,
                bitsPerPixel,
                distanceBetweenPixelsInclusive);
            int offset = sourceImage.GetByteBufferOffsetXY(0, 0);
            int[] buffer = sourceImage.GetInt32Buffer();
            SetBuffer(buffer, offset + bufferOffset);
            SetRecieveBlender(recieveBlender);
        }
        bool Attach(IImageReaderWriter sourceImage, int x1, int y1, int x2, int y2)
        {
            _sourceImage = sourceImage;
            SetBufferToNull();
            if (x1 > x2 || y1 > y2)
            {
                throw new Exception("You need to have your x1 and y1 be the lower left corner of your sub image.");
            }
            RectInt boundsRect = new RectInt(x1, y1, x2, y2);
            if (boundsRect.Clip(new RectInt(0, 0, sourceImage.Width - 1, sourceImage.Height - 1)))
            {
                SetDimmensionAndFormat(boundsRect.Width, boundsRect.Height, sourceImage.Stride, sourceImage.BitDepth, sourceImage.BytesBetweenPixelsInclusive);
                int bufferOffset = sourceImage.GetByteBufferOffsetXY(boundsRect.Left, boundsRect.Bottom) / 4;
                int[] buffer = sourceImage.GetInt32Buffer();
                SetBuffer(buffer, bufferOffset);
                return true;
            }

            return false;
        }

        void SetBuffer(int[] int32Buffer, int arrayElemOffset)
        {
            int height = this.Height;

            if (int32Buffer.Length < height * Width)
            {
                throw new Exception("Your buffer does not have enough room it it for your height and strideInBytes.");
            }

            SetBuffer(int32Buffer);
            int32ArrayStartPixelAt = arrayElemOffset;

            if (this.Stride < 0) //stride in bytes
            {
                //TODO: review here 
                int addAmount = -((height - 1) * Width);
                int32ArrayStartPixelAt = addAmount + arrayElemOffset;
            }
            SetUpLookupTables();
        }
    }
}