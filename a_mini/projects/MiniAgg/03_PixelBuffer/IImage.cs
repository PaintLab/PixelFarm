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
        Vector2 OriginOffset
        {
            get;
            set;
        }
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
        void copy_pixel(int x, int y, byte[] c, int ByteOffset);
        
        void CopyFrom(IImage sourceImage);
        void CopyFrom(IImage sourceImage, RectangleInt sourceImageRect, int destXOffset, int destYOffset);

        void SetPixel(int x, int y, ColorRGBA color);
        void BlendPixel(int x, int y, ColorRGBA sourceColor, byte cover);

        // line stuff
        void copy_hline(int x, int y, int len, ColorRGBA sourceColor);
        void copy_vline(int x, int y, int len, ColorRGBA sourceColor);

        void blend_hline(int x, int y, int x2, ColorRGBA sourceColor, byte cover);
        void blend_vline(int x, int y1, int y2, ColorRGBA sourceColor, byte cover);

        // color stuff
        void copy_color_hspan(int x, int y, int len, ColorRGBA[] colors, int colorIndex);
        void copy_color_vspan(int x, int y, int len, ColorRGBA[] colors, int colorIndex);

        void blend_solid_hspan(int x, int y, int len, ColorRGBA sourceColor, byte[] covers, int coversIndex);
        void blend_solid_vspan(int x, int y, int len, ColorRGBA sourceColor, byte[] covers, int coversIndex);

        void blend_color_hspan(int x, int y, int len, ColorRGBA[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll);
        void blend_color_vspan(int x, int y, int len, ColorRGBA[] colors, int colorsIndex, byte[] covers, int coversIndex, bool firstCoverForAll);
    }
     
}
