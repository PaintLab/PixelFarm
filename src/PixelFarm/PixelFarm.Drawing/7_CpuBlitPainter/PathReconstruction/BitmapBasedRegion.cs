//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
namespace PixelFarm.PathReconstruction
{
    public class BitmapBasedRegion : CpuBlitRegion
    {
        CpuBlit.MemBitmap _bmp;
        ReconstructedRegionData _reconRgnData;
        Rectangle _bounds;

        public BitmapBasedRegion(CpuBlit.MemBitmap bmp)
        {
            //create from memBitmap
            _bmp = bmp;
            _bounds = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
        }
        public BitmapBasedRegion(ReconstructedRegionData reconRgnData)
        {
            _reconRgnData = reconRgnData;
            _bounds = reconRgnData.GetBounds();
        }
        public override CpuBlitRegionKind Kind => CpuBlitRegionKind.BitmapBasedRegion;
        public override Rectangle GetRectBounds()
        {
            if (_bmp != null)
            {
                return new Rectangle(0, 0, _bmp.Width, _bmp.Height);
            }
            else
            {
                return _reconRgnData.GetBounds();
            }
        }

    }
}