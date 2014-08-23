//------------------------------------------------------scanline_hit_test
using System;

namespace MatterHackers.Agg
{
    public class scanline_hit_test : IScanline
    {
        //see pattern fill example  
        private int m_x;
        private bool m_hit;

        public scanline_hit_test(int x)
        {
            m_x = x;
            m_hit = false;
        }

        public void ResetSpans() { }
        public void CloseLine(int nothing) { }
        public void AddCell(int x, int nothing)
        {
            if (m_x == x) m_hit = true;
        }
        public void AddSpan(int x, int len, int nothing)
        {
            if (m_x >= x && m_x < x + len)
            {
                m_hit = true;
            }
        }
        public int num_spans() { return 1; }
        public bool hit() { return m_hit; }

        public ScanlineSpan GetSpan(int index)
        {
            //empty scanline
            return new ScanlineSpan();
        }
        public int SpanCount { get { return num_spans(); } }
        public void ResetSpans(int min_x, int max_x)
        {
            throw new System.NotImplementedException();
        }
        public ScanlineSpan begin()
        {
            throw new System.NotImplementedException();
        }
        public ScanlineSpan GetNextScanlineSpan()
        {
            throw new System.NotImplementedException();
        }
        public int Y
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        public byte[] GetCovers()
        {
            throw new System.NotImplementedException();
        }


    }

}