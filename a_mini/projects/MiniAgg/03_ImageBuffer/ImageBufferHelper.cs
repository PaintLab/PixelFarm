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
using System.Runtime;

using MatterHackers.Agg;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Agg.RasterizerScanline;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.Image
{
    public static class ImageBufferHelper
    {
        public static void SetAlpha(this ImageBuffer imgBuffer, byte value)
        {
            if (imgBuffer.BitDepth != 32)
            {
                throw new Exception("You don't have alpha channel to set.  Your image has a bit depth of " + imgBuffer.BitDepth.ToString() + ".");
            }
            int numPixels = imgBuffer.Width * imgBuffer.Height;
            int offset;
            byte[] buffer = imgBuffer.GetBuffer(out offset);
            for (int i = 0; i < numPixels; i++)
            {
                buffer[offset + i * 4 + 3] = value;
            }
        }
        public static void CropToVisible(this ImageBuffer imgBuffer)
        {
            Vector2 OldOriginOffset = imgBuffer.OriginOffset;

            //Move the HotSpot to 0, 0 so PPoint will work the way we want
            imgBuffer.OriginOffset = new Vector2(0, 0);

            RectangleInt visibleBounds;
            imgBuffer.GetVisibleBounds(out visibleBounds);

            if (visibleBounds.Width == imgBuffer.Width
                && visibleBounds.Height == imgBuffer.Height)
            {
                imgBuffer.OriginOffset = OldOriginOffset;
                return;
            }

            // check if the Not0Rect has any size
            if (visibleBounds.Width > 0)
            {
                ImageBuffer tempImage = new ImageBuffer(); 
                // set TempImage equal to the Not0Rect
                tempImage.Initialize(imgBuffer, visibleBounds);

                // set the frame equal to the TempImage
                imgBuffer.Initialize(tempImage); 
                imgBuffer.OriginOffset = new Vector2(-visibleBounds.Left + OldOriginOffset.x, -visibleBounds.Bottom + OldOriginOffset.y);
            }
            else
            {
                imgBuffer.Deallocate();
            }
        }
        //public void FlipY(ImageBuffer imgBuffer)
        //{
        //    strideInBytes *= -1;
        //    bufferFirstPixel = bufferOffset;
        //    if (strideInBytes < 0)
        //    {
        //        int addAmount = -((int)((int)height - 1) * strideInBytes);
        //        bufferFirstPixel = addAmount + bufferOffset;
        //    }

        //    SetUpLookupTables();
        //} 


        //public static bool operator ==(ImageBuffer a, ImageBuffer b)
        //{
        //    if ((object)a == null || (object)b == null)
        //    {
        //        if ((object)a == null && (object)b == null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    return a.Equals(b, 0);
        //}

        //public static bool operator !=(ImageBuffer a, ImageBuffer b)
        //{
        //    bool areEqual = a == b;
        //    return !areEqual;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() == typeof(ImageBuffer))
        //    {
        //        return this == (ImageBuffer)obj;
        //    }
        //    return false;
        //}

        //public bool Equals(ImageBuffer b, int maxError = 0)
        //{
        //    if (Width == b.Width
        //        && Height == b.Height
        //        && BitDepth == b.BitDepth
        //        && StrideInBytes() == b.StrideInBytes()
        //        && m_OriginOffset == b.m_OriginOffset)
        //    {
        //        int bytesPerPixel = BitDepth / 8;
        //        int aDistanceBetweenPixels = GetBytesBetweenPixelsInclusive();
        //        int bDistanceBetweenPixels = b.GetBytesBetweenPixelsInclusive();
        //        byte[] aBuffer = GetBuffer();
        //        byte[] bBuffer = b.GetBuffer();
        //        for (int y = 0; y < Height; y++)
        //        {
        //            int aBufferOffset = GetBufferOffsetY(y);
        //            int bBufferOffset = b.GetBufferOffsetY(y);
        //            for (int x = 0; x < Width; x++)
        //            {
        //                for (int byteIndex = 0; byteIndex < bytesPerPixel; byteIndex++)
        //                {
        //                    byte aByte = aBuffer[aBufferOffset + byteIndex];
        //                    byte bByte = bBuffer[bBufferOffset + byteIndex];
        //                    if (aByte < (bByte - maxError) || aByte > (bByte + maxError))
        //                    {
        //                        return false;
        //                    }
        //                }
        //                aBufferOffset += aDistanceBetweenPixels;
        //                bBufferOffset += bDistanceBetweenPixels;
        //            }
        //        }
        //        return true;
        //    }

        //    return false;
        //}

        //public bool Contains(ImageBuffer imageToFind, int maxError = 0)
        //{
        //    int matchX;
        //    int matchY;
        //    return Contains(imageToFind, out matchX, out matchY, maxError);
        //}

        //public bool Contains(ImageBuffer imageToFind, out int matchX, out int matchY, int maxError = 0)
        //{
        //    matchX = 0;
        //    matchY = 0;
        //    if (Width >= imageToFind.Width
        //        && Height >= imageToFind.Height
        //        && BitDepth == imageToFind.BitDepth)
        //    {
        //        int bytesPerPixel = BitDepth / 8;
        //        int aDistanceBetweenPixels = GetBytesBetweenPixelsInclusive();
        //        int bDistanceBetweenPixels = imageToFind.GetBytesBetweenPixelsInclusive();
        //        byte[] thisBuffer = GetBuffer();
        //        byte[] containedBuffer = imageToFind.GetBuffer();
        //        for (matchY = 0; matchY <= Height - imageToFind.Height; matchY++)
        //        {
        //            for (matchX = 0; matchX <= Width - imageToFind.Width; matchX++)
        //            {
        //                bool foundBadMatch = false;
        //                for (int imageToFindY = 0; imageToFindY < imageToFind.Height; imageToFindY++)
        //                {
        //                    int thisBufferOffset = GetBufferOffsetXY(matchX, matchY + imageToFindY);
        //                    int imageToFindBufferOffset = imageToFind.GetBufferOffsetY(imageToFindY);
        //                    for (int imageToFindX = 0; imageToFindX < imageToFind.Width; imageToFindX++)
        //                    {
        //                        for (int byteIndex = 0; byteIndex < bytesPerPixel; byteIndex++)
        //                        {
        //                            byte aByte = thisBuffer[thisBufferOffset + byteIndex];
        //                            byte bByte = containedBuffer[imageToFindBufferOffset + byteIndex];
        //                            if (aByte < (bByte - maxError) || aByte > (bByte + maxError))
        //                            {
        //                                foundBadMatch = true;
        //                                byteIndex = bytesPerPixel;
        //                                imageToFindX = imageToFind.Width;
        //                                imageToFindY = imageToFind.Height;
        //                            }
        //                        }
        //                        thisBufferOffset += aDistanceBetweenPixels;
        //                        imageToFindBufferOffset += bDistanceBetweenPixels;
        //                    }
        //                }
        //                if (!foundBadMatch)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public bool FindLeastSquaresMatch(ImageBuffer imageToFind, double maxError)
        //{
        //    Vector2 bestPosition;
        //    double bestLeastSquares;
        //    return FindLeastSquaresMatch(imageToFind, out bestPosition, out bestLeastSquares, maxError);
        //}

        //public bool FindLeastSquaresMatch(ImageBuffer imageToFind, out Vector2 bestPosition, out double bestLeastSquares, double maxError = double.MaxValue)
        //{
        //    bestPosition = Vector2.Zero;
        //    bestLeastSquares = double.MaxValue;

        //    if (Width >= imageToFind.Width
        //        && Height >= imageToFind.Height
        //        && BitDepth == imageToFind.BitDepth)
        //    {
        //        int bytesPerPixel = BitDepth / 8;
        //        int aDistanceBetweenPixels = GetBytesBetweenPixelsInclusive();
        //        int bDistanceBetweenPixels = imageToFind.GetBytesBetweenPixelsInclusive();
        //        byte[] thisBuffer = GetBuffer();
        //        byte[] containedBuffer = imageToFind.GetBuffer();
        //        for (int matchY = 0; matchY <= Height - imageToFind.Height; matchY++)
        //        {
        //            for (int matchX = 0; matchX <= Width - imageToFind.Width; matchX++)
        //            {
        //                double currentLeastSquares = 0;

        //                for (int imageToFindY = 0; imageToFindY < imageToFind.Height; imageToFindY++)
        //                {
        //                    int thisBufferOffset = GetBufferOffsetXY(matchX, matchY + imageToFindY);
        //                    int imageToFindBufferOffset = imageToFind.GetBufferOffsetY(imageToFindY);
        //                    for (int imageToFindX = 0; imageToFindX < imageToFind.Width; imageToFindX++)
        //                    {
        //                        for (int byteIndex = 0; byteIndex < bytesPerPixel; byteIndex++)
        //                        {
        //                            byte aByte = thisBuffer[thisBufferOffset + byteIndex];
        //                            byte bByte = containedBuffer[imageToFindBufferOffset + byteIndex];
        //                            int difference = (int)aByte - (int)bByte;
        //                            currentLeastSquares += difference * difference;
        //                        }
        //                        thisBufferOffset += aDistanceBetweenPixels;
        //                        imageToFindBufferOffset += bDistanceBetweenPixels;
        //                    }
        //                    if (currentLeastSquares > maxError)
        //                    {
        //                        // stop checking we have too much error.
        //                        imageToFindY = imageToFind.Height;
        //                    }
        //                }
        //                if (currentLeastSquares < bestLeastSquares)
        //                {
        //                    bestPosition = new Vector2(matchX, matchY);
        //                    bestLeastSquares = currentLeastSquares;
        //                }
        //            }
        //        }
        //    }

        //    return bestLeastSquares <= maxError;
        //}
    }
}