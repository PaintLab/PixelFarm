//MIT, 2016-2017, WinterDev
using System;
using System.Numerics;

namespace Typography.Rendering
{

    public class GlyphEdge
    {
        internal readonly EdgeLine _edgeLine;
        public readonly GlyphPoint _P;
        public readonly GlyphPoint _Q;

#if DEBUG
        public static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        internal GlyphEdge(GlyphPoint p0, GlyphPoint p1, EdgeLine edgeLine)
        {
#if DEBUG
            edgeLine.dbugGlyphEdge = this;
#endif
            this._P = p0;
            this._Q = p1;
            this._edgeLine = edgeLine;


        }


#if DEBUG
        public EdgeLine dbugGetInternalEdgeLine()
        {
            return this._edgeLine;
        }
        public override string ToString()
        {
            return this._P + "=>" + this._Q;
        }
#endif
    }

}