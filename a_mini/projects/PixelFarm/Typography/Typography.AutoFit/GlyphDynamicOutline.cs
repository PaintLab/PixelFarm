//MIT, 2017, WinterDev
using System;
using System.Numerics;
using System.Collections.Generic;
using Typography.OpenFont;
namespace Typography.Rendering
{
    public class GlyphDynamicOutline
    {

        List<CentroidLineHub> _centroidHubs;
        internal GlyphDynamicOutline(Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHubs)
        {
            _centroidHubs = new List<CentroidLineHub>(centroidLineHubs.Count);
            foreach (CentroidLineHub centroidHub in centroidLineHubs.Values)
            {
                _centroidHubs.Add(centroidHub);
            }
        }
        public void GenerateOutput(IGlyphTranslator tx, float pxScale)
        {



        }
    }
}