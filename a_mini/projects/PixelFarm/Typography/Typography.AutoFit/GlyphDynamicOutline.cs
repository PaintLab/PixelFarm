//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;

namespace Typography.Rendering
{
    public class GlyphDynamicOutline
    {
        Dictionary<GlyphTriangle, CentroidLineHub> _centroidLineHubs;
        internal GlyphDynamicOutline(Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs)
        {
            this._centroidLineHubs = centroidLineHubs;
        }
    }
}