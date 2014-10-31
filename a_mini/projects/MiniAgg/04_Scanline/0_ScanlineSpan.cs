//2014 BSD,WinterDev   
using System;
namespace MatterHackers.Agg
{
    public struct ScanlineSpan
    {
        public int x;
        public int len;
        public int cover_index;

#if DEBUG
        public override string ToString()
        {
            return "x:" + x + ",len:" + len + ",cover:" + cover_index;
        }
#endif 
    }
}