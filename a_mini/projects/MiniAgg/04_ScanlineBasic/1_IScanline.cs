using System;

namespace MatterHackers.Agg
{
 

    public interface IScanline
    {
        void CloseLine(int y);
        void ResetSpans(int min_x, int max_x);
        void ResetSpans();

        int Y { get; }
        byte[] GetCovers();
        void add_cell(int x, int cover);
        void add_span(int x, int len, int cover);


        ScanlineSpan GetSpan(int index);
        int SpanCount { get; }
        
    }
}
