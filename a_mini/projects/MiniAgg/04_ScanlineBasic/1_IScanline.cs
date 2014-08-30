using System;

namespace MatterHackers.Agg
{   
    public interface IScanline
    {   
        int Y { get; }
        
        void AddCell(int x, int cover);
        void AddSpan(int x, int len, int cover);
        int SpanCount { get; }

        byte[] GetCovers();
        ScanlineSpan GetSpan(int index);
         
        void CloseLine(int y);
        void ResetSpans(int min_x, int max_x);
        void ResetSpans(); 
    }
}
