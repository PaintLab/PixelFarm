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
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;

using MatterHackers.Agg;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.Image
{
    public abstract class ProxyImage : IImage
    {
        protected IImage linkedImage;
        public ProxyImage(IImage linkedImage)
        {
            this.linkedImage = linkedImage;
        }
         
        public void SetOriginOffset(double x, double y)
        {
            linkedImage.SetOriginOffset(x, y);
        }
        public void GetOriginOffset(out double x, out double y)
        {
            linkedImage.GetOriginOffset(out x, out y);
        }
        public virtual int Width
        {
            get
            {
                return linkedImage.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                return linkedImage.Height;
            }
        }

        public virtual int StrideInBytes()
        {
            return linkedImage.StrideInBytes();
        }


        public virtual RectangleInt GetBounds()
        {
            return linkedImage.GetBounds();
        }

        public IRecieveBlenderByte GetRecieveBlender()
        {
            return linkedImage.GetRecieveBlender();
        }

        public void SetRecieveBlender(IRecieveBlenderByte value)
        {
            linkedImage.SetRecieveBlender(value);
        }

        public virtual ColorRGBA GetPixel(int x, int y)
        {
            return linkedImage.GetPixel(x, y);
        }

        //public virtual void CopyPixel(int x, int y, byte[] c, int byteOffset)
        //{
        //    linkedImage.CopyPixel(x, y, c, byteOffset);
        //}

        public virtual void CopyFrom(IImage sourceRaster)
        {
            linkedImage.CopyFrom(sourceRaster);
        }

        public virtual void CopyFrom(IImage sourceImage, RectangleInt sourceImageRect, int destXOffset, int destYOffset)
        {
            linkedImage.CopyFrom(sourceImage, sourceImageRect, destXOffset, destYOffset);
        }

        public virtual void SetPixel(int x, int y, ColorRGBA color)
        {
            linkedImage.SetPixel(x, y, color);
        }

        //public virtual void BlendPixel(int x, int y, ColorRGBA sourceColor, byte cover)
        //{
        //    linkedImage.BlendPixel(x, y, sourceColor, cover);
        //}

        public virtual void CopyHL(int x, int y, int len, ColorRGBA sourceColor)
        {
            linkedImage.CopyHL(x, y, len, sourceColor);
        }

        public virtual void CopyVL(int x, int y, int len, ColorRGBA sourceColor)
        {
            linkedImage.CopyVL(x, y, len, sourceColor);
        }

        public virtual void BlendHL(int x1, int y, int x2, ColorRGBA sourceColor, byte cover)
        {
            linkedImage.BlendHL(x1, y, x2, sourceColor, cover);
        }

        public virtual void BlendVL(int x, int y1, int y2, ColorRGBA sourceColor, byte cover)
        {
            linkedImage.BlendVL(x, y1, y2, sourceColor, cover);
        }

        public virtual void BlendSolidHSpan(int x, int y, int len, ColorRGBA c, byte[] covers, int coversIndex)
        {
            linkedImage.BlendSolidHSpan(x, y, len, c, covers, coversIndex);
        }

        public virtual void CopyColorHSpan(int x, int y, int len, ColorRGBA[] colors, int colorIndex)
        {
            linkedImage.CopyColorHSpan(x, y, len, colors, colorIndex);
        }

        public virtual void CopyColorVSpan(int x, int y, int len, ColorRGBA[] colors, int colorIndex)
        {
            linkedImage.CopyColorVSpan(x, y, len, colors, colorIndex);
        }

        public virtual void BlendSolidVSpan(int x, int y, int len, ColorRGBA c, byte[] covers, int coversIndex)
        {
            linkedImage.BlendSolidVSpan(x, y, len, c, covers, coversIndex);
        }

        public virtual void BlendColorHSpan(int x, int y, int len, ColorRGBA[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll)
        {
            linkedImage.BlendColorHSpan(x, y, len, colors, colorsIndex, covers, coversIndex, firstCoverForAll);
        }

        public virtual void BlendColorVSpan(int x, int y, int len, ColorRGBA[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll)
        {
            linkedImage.BlendColorVSpan(x, y, len, colors, colorsIndex, covers, coversIndex, firstCoverForAll);
        }

        public byte[] GetBuffer()
        {
            return linkedImage.GetBuffer();
        }

        public int GetBufferOffsetXY(int x, int y)
        {
            return linkedImage.GetBufferOffsetXY(x, y);
        }



        public virtual int GetBytesBetweenPixelsInclusive()
        {
            return linkedImage.GetBytesBetweenPixelsInclusive();
        }

        public virtual int BitDepth
        {
            get
            {
                return linkedImage.BitDepth;
            }
        }

        public void MarkImageChanged()
        {
            linkedImage.MarkImageChanged();
        }
    }

}
