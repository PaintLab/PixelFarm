//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

namespace PixelFarm.PathReconstruction
{

    public class MixedRegion : CpuBlitRegion
    {
        internal MixedRegion()
        {

        }

        public override CpuBlitRegionKind Kind => CpuBlitRegionKind.MixedRegion;

        public override Region CreateComplement(Region another)
        {
            throw new NotImplementedException();
        }

        public override Region CreateExclude(Region another)
        {
            throw new NotImplementedException();
        }

        public override Region CreateIntersect(Region another)
        {
            throw new NotImplementedException();
        }

        public override Region CreateUnion(Region another)
        {
            throw new NotImplementedException();
        }

        public override Rectangle GetRectBounds()
        {
            throw new NotImplementedException();
        }
    }
}