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
using MatterHackers.Agg;
using MatterHackers.VectorMath;

using MatterHackers.Agg.Image;

namespace MatterHackers.Agg
{

    public interface IImage
    {
      
        int BitDepth { get; }
        int Width { get; }
        int Height { get; }
        RectangleInt GetBounds();

        int GetBufferOffsetXY(int x, int y);
        void MarkImageChanged();

        int StrideInBytes();
        int GetBytesBetweenPixelsInclusive();

        IRecieveBlenderByte GetRecieveBlender();
        void SetRecieveBlender(IRecieveBlenderByte value);


        byte[] GetBuffer();

        ColorRGBA GetPixel(int x, int y);
     
        void CopyFrom(IImage sourceImage);
        void CopyFrom(IImage sourceImage, RectangleInt sourceImageRect, int destXOffset, int destYOffset);

        void SetPixel(int x, int y, ColorRGBA color);
         
        // line stuff
        void CopyHL(int x, int y, int len, ColorRGBA sourceColor);
        void CopyVL(int x, int y, int len, ColorRGBA sourceColor);

        void BlendHL(int x, int y, int x2, ColorRGBA sourceColor, byte cover);
        void BlendVL(int x, int y1, int y2, ColorRGBA sourceColor, byte cover);

        // color stuff
        void CopyColorHSpan(int x, int y, int len, ColorRGBA[] colors, int colorIndex);
        void CopyColorVSpan(int x, int y, int len, ColorRGBA[] colors, int colorIndex);

        void BlendSolidHSpan(int x, int y, int len, ColorRGBA sourceColor, byte[] covers, int coversIndex);
        void BlendSolidVSpan(int x, int y, int len, ColorRGBA sourceColor, byte[] covers, int coversIndex);

        void BlendColorHSpan(int x, int y, int len, ColorRGBA[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll);
        void BlendColorVSpan(int x, int y, int len, ColorRGBA[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll);
    }

}
