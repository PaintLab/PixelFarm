//MIT, 2019-present, WinterDev

using System.Collections.Generic;
using ClipperLib;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.PathReconstruction
{
    public abstract class CpuBlitRegion : PixelFarm.Drawing.Region
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