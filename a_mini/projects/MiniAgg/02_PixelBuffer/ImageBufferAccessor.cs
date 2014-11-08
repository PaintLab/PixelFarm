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

namespace PixelFarm.Agg.Image
{       

    public sealed class ImageBufferAccessor
    {
        IImageReaderWriter m_sourceImage;
        int m_x, m_x0, m_y, m_distanceBetweenPixelsInclusive;
        byte[] m_buffer;
        int m_currentBufferOffset = -1;
        int m_width;

        public ImageBufferAccessor(IImageReaderWriter imgReaderWriter)
        {
            Attach(imgReaderWriter);
        }

        void Attach(IImageReaderWriter pixf)
        {
            m_sourceImage = pixf;
            m_buffer = m_sourceImage.GetBuffer();
            m_width = m_sourceImage.Width;
            m_distanceBetweenPixelsInclusive = m_sourceImage.BytesBetweenPixelsInclusive;
        }

        public IImageReaderWriter SourceImage
        {
            get
            {
                return m_sourceImage;
            }
        }

        byte[] GetPixels(out int bufferByteOffset)
        {
            int x = m_x;
            int y = m_y;
            unchecked
            {
                if ((uint)x >= (uint)m_sourceImage.Width)
                {
                    if (x < 0)
                    {
                        x = 0;
                    }
                    else
                    {
                        x = (int)m_sourceImage.Width - 1;
                    }
                }

                if ((uint)y >= (uint)m_sourceImage.Height)
                {
                    if (y < 0)
                    {
                        y = 0;
                    }
                    else
                    {
                        y = (int)m_sourceImage.Height - 1;
                    }
                }
            }

            bufferByteOffset = m_sourceImage.GetBufferOffsetXY(x, y);
            return m_sourceImage.GetBuffer();
        }

        public byte[] GetSpan(int x, int y, int len, out int bufferOffset)
        {
            m_x = m_x0 = x;
            m_y = y;
            unchecked
            {
                if ((uint)y < (uint)m_sourceImage.Height
                    && x >= 0 && x + len <= (int)m_sourceImage.Width)
                {
                    bufferOffset = m_sourceImage.GetBufferOffsetXY(x, y);
                    m_buffer = m_sourceImage.GetBuffer();
                    m_currentBufferOffset = bufferOffset;
                    return m_buffer;
                }
            }

            m_currentBufferOffset = -1;
            return GetPixels(out bufferOffset);
        }

        public byte[] NextX(out int bufferOffset)
        {
            // this is the code (managed) that the original agg used.  
            // It looks like it doesn't check x but, It should be a bit faster and is valid 
            // because "span" checked the whole length for good x.
            if (m_currentBufferOffset != -1)
            {
                m_currentBufferOffset += m_distanceBetweenPixelsInclusive;
                bufferOffset = m_currentBufferOffset;
                return m_buffer;
            }
            ++m_x;
            return GetPixels(out bufferOffset);
        }

        public byte[] NextY(out int bufferOffset)
        {
            ++m_y;
            m_x = m_x0;
            if (m_currentBufferOffset != -1
                && (uint)m_y < (uint)m_sourceImage.Height)
            {
                m_currentBufferOffset = m_sourceImage.GetBufferOffsetXY(m_x, m_y);
                m_sourceImage.GetBuffer();
                bufferOffset = m_currentBufferOffset; ;
                return m_buffer;
            }

            m_currentBufferOffset = -1;
            return GetPixels(out bufferOffset);
        }
    }

    
}