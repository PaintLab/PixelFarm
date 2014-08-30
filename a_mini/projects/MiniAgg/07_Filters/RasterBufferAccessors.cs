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
using System.Runtime.InteropServices;
using MatterHackers.Agg.Image;

namespace MatterHackers.Agg
{
    public interface IImageBufferAccessor
    {
        byte[] span(int x, int y, int len, out int bufferIndex);
        byte[] next_x(out int bufferByteOffset);
        byte[] next_y(out int bufferByteOffset);

        IImage SourceImage
        {
            get;
        }
    }

    public abstract class ImageBufferAccessor : IImageBufferAccessor
    {
        protected IImage m_SourceImage;
        protected int m_x, m_x0, m_y, m_DistanceBetweenPixelsInclusive;
        protected byte[] m_Buffer;
        protected int m_CurrentBufferOffset = -1;
        int m_Width;

        public ImageBufferAccessor(IImage pixf)
        {
            attach(pixf);
        }

        void attach(IImage pixf)
        {
            m_SourceImage = pixf;
            m_Buffer = m_SourceImage.GetBuffer();
            m_Width = m_SourceImage.Width;
            m_DistanceBetweenPixelsInclusive = m_SourceImage.GetBytesBetweenPixelsInclusive();
        }

        public IImage SourceImage
        {
            get
            {
                return m_SourceImage;
            }
        }

        private byte[] pixel(out int bufferByteOffset)
        {
            int x = m_x;
            int y = m_y;
            unchecked
            {
                if ((uint)x >= (uint)m_SourceImage.Width)
                {
                    if (x < 0)
                    {
                        x = 0;
                    }
                    else
                    {
                        x = (int)m_SourceImage.Width - 1;
                    }
                }

                if ((uint)y >= (uint)m_SourceImage.Height)
                {
                    if (y < 0)
                    {
                        y = 0;
                    }
                    else
                    {
                        y = (int)m_SourceImage.Height - 1;
                    }
                }
            }

            bufferByteOffset = m_SourceImage.GetBufferOffsetXY(x, y);
            return m_SourceImage.GetBuffer();
        }

        public byte[] span(int x, int y, int len, out int bufferOffset)
        {
            m_x = m_x0 = x;
            m_y = y;
            unchecked
            {
                if ((uint)y < (uint)m_SourceImage.Height
                    && x >= 0 && x + len <= (int)m_SourceImage.Width)
                {
                    bufferOffset = m_SourceImage.GetBufferOffsetXY(x, y);
                    m_Buffer = m_SourceImage.GetBuffer();
                    m_CurrentBufferOffset = bufferOffset;
                    return m_Buffer;
                }
            }

            m_CurrentBufferOffset = -1;
            return pixel(out bufferOffset);
        }

        public byte[] next_x(out int bufferOffset)
        {
            // this is the code (managed) that the original agg used.  
            // It looks like it doesn't check x but, It should be a bit faster and is valid 
            // because "span" checked the whole length for good x.
            if (m_CurrentBufferOffset != -1)
            {
                m_CurrentBufferOffset += m_DistanceBetweenPixelsInclusive;
                bufferOffset = m_CurrentBufferOffset;
                return m_Buffer;
            }
            ++m_x;
            return pixel(out bufferOffset);
        }

        public byte[] next_y(out int bufferOffset)
        {
            ++m_y;
            m_x = m_x0;
            if (m_CurrentBufferOffset != -1
                && (uint)m_y < (uint)m_SourceImage.Height)
            {
                m_CurrentBufferOffset = m_SourceImage.GetBufferOffsetXY(m_x, m_y);
                m_SourceImage.GetBuffer();
                bufferOffset = m_CurrentBufferOffset; ;
                return m_Buffer;
            }

            m_CurrentBufferOffset = -1;
            return pixel(out bufferOffset);
        }
    };

    public sealed class ImageBufferAccessorClip : ImageBufferAccessor
    {
        byte[] m_OutsideBufferColor;

        public ImageBufferAccessorClip(IImage sourceImage, ColorRGBA bk)
            : base(sourceImage)
        {
            m_OutsideBufferColor = new byte[4];
            m_OutsideBufferColor[0] = bk.red;
            m_OutsideBufferColor[1] = bk.green;
            m_OutsideBufferColor[2] = bk.blue;
            m_OutsideBufferColor[3] = bk.alpha;
        }

        //private byte[] pixel(out int bufferByteOffset)
        //{
        //    unchecked
        //    {
        //        if (((uint)m_x < (uint)m_SourceImage.Width)
        //            && ((uint)m_y < (uint)m_SourceImage.Height))
        //        {
        //            bufferByteOffset = m_SourceImage.GetBufferOffsetXY(m_x, m_y);
        //            return m_SourceImage.GetBuffer();
        //        }
        //    }

        //    bufferByteOffset = 0;
        //    return m_OutsideBufferColor;
        //}
    }  
}