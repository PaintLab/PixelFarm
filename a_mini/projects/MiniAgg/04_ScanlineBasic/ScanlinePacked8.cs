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
//
// Class scanline_p - a general purpose scanline container with packed spans.
//
//----------------------------------------------------------------------------
//
// Adaptation for 32-bit screen coordinates (scanline32_p) has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
using System;

namespace MatterHackers.Agg
{
    //=============================================================scanline_p8
    // 
    // This is a general purpose scanline container which supports the interface 
    // used in the rasterizer::render(). See description of scanline_u8
    // for details.
    // 
    //------------------------------------------------------------------------
    public sealed class ScanlinePacked8 : IScanline
    {
        int m_last_x;
        int m_y;

        byte[] m_covers;
        int m_cover_index;
        ScanlineSpan[] m_spans;

        int m_span_index; 


        public ScanlinePacked8()
        {
            m_last_x = 0x7FFFFFF0;
            m_covers = new byte[1000];
            m_spans = new ScanlineSpan[1000];
        }

        public ScanlineSpan GetSpan(int index)
        {
            return m_spans[index];
        }

        public int SpanCount
        {
            get { return  m_span_index; }
        }
        //--------------------------------------------------------------------
        public void reset(int min_x, int max_x)
        {
            int max_len = max_x - min_x + 3;
            if (max_len > m_spans.Length)
            {
                m_spans = new ScanlineSpan[max_len];
                m_covers = new byte[max_len];
            }
            m_last_x = 0x7FFFFFF0;
            m_cover_index = 0;
            m_span_index = 0;
            m_spans[m_span_index].len = 0;
        }

        //--------------------------------------------------------------------
        public void add_cell(int x, int cover)
        {
            m_covers[m_cover_index] = (byte)cover;
            if (x == m_last_x + 1 && m_spans[m_span_index].len > 0)
            {
                m_spans[m_span_index].len++;
            }
            else
            {
                m_span_index++;

                m_spans[m_span_index].cover_index = m_cover_index;
                m_spans[m_span_index].x = (short)x;
                m_spans[m_span_index].len = 1;
            }
            m_last_x = x;
            m_cover_index++;
        }
        //--------------------------------------------------------------------
        public void add_span(int x, int len, int cover)
        {
            if (x == m_last_x + 1
                && m_spans[m_span_index].len < 0
                && cover == m_spans[m_span_index].cover_index)
            {
                m_spans[m_span_index].len -= (short)len;
            }
            else
            {
                m_covers[m_cover_index] = (byte)cover;
                m_span_index++;

                m_spans[m_span_index].cover_index = m_cover_index++;
                m_spans[m_span_index].x = (short)x;
                m_spans[m_span_index].len = (short)(-(int)(len));
            }
            m_last_x = x + (int)len - 1;
        }

        //--------------------------------------------------------------------
        public void CloseLine(int y)
        {
            m_y = y;
        }

        //--------------------------------------------------------------------
        public void ResetSpans()
        {
            m_last_x = 0x7FFFFFF0;
            m_cover_index = 0;
            m_span_index = 0;
            m_spans[m_span_index].len = 0;
        }

        public int y() { return m_y; } 
        public byte[] GetCovers()
        {
            return m_covers;
        }
    };
}
