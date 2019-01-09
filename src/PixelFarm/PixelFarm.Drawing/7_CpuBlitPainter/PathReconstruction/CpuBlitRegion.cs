//MIT, 2019-present, WinterDev

using System.Collections.Generic;
using ClipperLib;
using PixelFarm.Drawing; 

namespace PixelFarm.CpuBlit.VertexProcessing
{
    public abstract class CpuBlitRegion : Region
    {
        object _innerObj;
        public override object InnerRegion => _innerObj;
        internal void SetInnerObject(object value) => _innerObj = value;

        public override void Dispose()
        {
        }
        public Rectangle GetRectBounds() => new Rectangle();

    }
}