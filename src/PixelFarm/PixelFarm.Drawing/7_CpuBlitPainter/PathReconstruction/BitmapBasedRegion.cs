//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

namespace PixelFarm.PathReconstruction
{
    public class BitmapBasedRegion : CpuBlitRegion
    {
        MemBitmap _bmp;
        ReconstructedRegionData _reconRgnData;
        Rectangle _bounds;

        /// <summary>
        /// we STORE membitmap inside this rgn, 
        /// </summary>
        /// <param name="bmp"></param>
        public BitmapBasedRegion(CpuBlit.MemBitmap bmp)
        {
            _bmp = bmp;
            _bounds = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
        }

        public BitmapBasedRegion(ReconstructedRegionData reconRgnData)
        {
            _reconRgnData = reconRgnData;
            _bounds = reconRgnData.GetBounds();
        }
        public override CpuBlitRegionKind Kind => CpuBlitRegionKind.BitmapBasedRegion;

        public override Region CreateComplement(Region another)
        {
            CpuBlitRegion rgnB = another as CpuBlitRegion;
            if (rgnB == null) return null;
            //
            //
            switch (rgnB.Kind)
            {
                default: throw new System.NotSupportedException();
                case CpuBlitRegionKind.BitmapBasedRegion:
                    {
                        BitmapBasedRegion bmpRgn = (BitmapBasedRegion)rgnB;
                    }
                    break;
                case CpuBlitRegionKind.MixedRegion:
                    break;
                case CpuBlitRegionKind.VxsRegion:
                    //TODO: review complement
                    break;
            }
            return null;
        }

        public override Region CreateExclude(Region another)
        {
            CpuBlitRegion rgnB = another as CpuBlitRegion;
            if (rgnB == null) return null;
            //
            switch (rgnB.Kind)
            {
                default: throw new System.NotSupportedException();
                case CpuBlitRegionKind.BitmapBasedRegion:
                    {
                        BitmapBasedRegion bmpRgn = (BitmapBasedRegion)rgnB;
                    }
                    break;
                case CpuBlitRegionKind.MixedRegion:
                    break;
                case CpuBlitRegionKind.VxsRegion:
                    //TODO: review complement
                    break;
            }

            return null;
        }

        public override Region CreateIntersect(Region another)
        {
            CpuBlitRegion rgnB = another as CpuBlitRegion;
            if (rgnB == null) return null;
            //
            switch (rgnB.Kind)
            {
                default: throw new System.NotSupportedException();
                case CpuBlitRegionKind.BitmapBasedRegion:
                    {
                        BitmapBasedRegion bmpRgn = (BitmapBasedRegion)rgnB;
                    }
                    break;
                case CpuBlitRegionKind.MixedRegion:
                    break;
                case CpuBlitRegionKind.VxsRegion:
                    //TODO: review complement
                    break;
            }
            return null;
        }

        BitmapBasedRegion CreateNewUnion(BitmapBasedRegion another)
        {

            if (another._bmp != null)
            {
                //check if we have bmp or not 
                if (this._bmp != null)
                {
                    //do bitmap union
                    //2 rgn merge may 
                    Rectangle r1 = this.GetRectBounds();
                    Rectangle r2 = another.GetRectBounds();
                    Rectangle r3 = Rectangle.Union(r1, r2);




                }
                else if (_reconRgnData != null)
                {

                }
                else
                {
                    //?
                }
            }
            else if (another._reconRgnData != null)
            {

            }
            return null;
        }

        public override Region CreateUnion(Region another)
        {
            CpuBlitRegion rgnB = another as CpuBlitRegion;
            if (rgnB == null) return null;
            //
            switch (rgnB.Kind)
            {
                default: throw new System.NotSupportedException();
                case CpuBlitRegionKind.BitmapBasedRegion:
                    return CreateNewUnion((BitmapBasedRegion)rgnB);
                case CpuBlitRegionKind.MixedRegion:
                    break;
                case CpuBlitRegionKind.VxsRegion:
                    //TODO: review complement
                    break;
            }
            return null;
        }

        public override Region CreateXor(Region another)
        {
            CpuBlitRegion rgnB = another as CpuBlitRegion;
            if (rgnB == null) return null;
            //
            switch (rgnB.Kind)
            {
                default: throw new System.NotSupportedException();
                case CpuBlitRegionKind.BitmapBasedRegion:
                    {
                        BitmapBasedRegion bmpRgn = (BitmapBasedRegion)rgnB;
                    }
                    break;
                case CpuBlitRegionKind.MixedRegion:
                    break;
                case CpuBlitRegionKind.VxsRegion:
                    //TODO: review complement
                    break;
            }
            return null;
        }
        public override void Dispose()
        {
            if (_bmp != null)
            {
                _bmp.Dispose();
            }
            _bmp = null;
        }
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

        public MemBitmap GetRegionBitmap()
        {
            if (_bmp != null)
            {
                return _bmp;
            }
            else if (_reconRgnData != null)
            {
                //
                return _bmp = _reconRgnData.CreateMaskBitmap();
            }
            return null;
        }
    }



}