//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit; 

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        ClipingTechnique _currentClipTech;
        RectInt _clipBox;

        public bool EnableBuiltInMaskComposite { get; set; }
        public override RectInt ClipBox
        {
            get => _clipBox;
            set => _clipBox = value;
        }

        public override bool EnableMask
        {
            get => _currentClipTech == ClipingTechnique.ClipMask;
            set
            {
                //review here again
                if (value)
                {
                    //NOT READY FOR Mask
                    //_pcx.EnableMask(pathRenderVx);
                    _currentClipTech = ClipingTechnique.ClipMask;
                }
                else
                {
                    _currentClipTech = ClipingTechnique.None;
                }
            }
        }
        public override void SetClipRgn(VertexStore vxs)
        {

            //TODO: review mask combine mode
            //1. Append
            //2. Replace 

            //this version we use replace 
            //clip rgn implementation
            //this version replace only
            //TODO: add append clip rgn 

            //TODO: implement complex framebuffer-based mask

            if (vxs != null)
            {
                if (PixelFarm.Drawing.SimpleRectClipEvaluator.EvaluateRectClip(vxs, out RectangleF clipRect))
                {
                    this.SetClipBox(
                        (int)Math.Floor(clipRect.X), (int)Math.Floor(clipRect.Y),
                        (int)Math.Ceiling(clipRect.Right), (int)Math.Ceiling(clipRect.Bottom));

                    _currentClipTech = ClipingTechnique.ClipSimpleRect;
                }
                else
                {
                    //not simple rect => 
                    //use mask technique
                    _currentClipTech = ClipingTechnique.ClipMask;
                    //1. switch to mask buffer  
                    using (PathRenderVx pathRenderVx = PathRenderVx.Create(_pathRenderVxBuilder.Build(vxs)))
                    {
                        _pcx.EnableMask(pathRenderVx);
                    }
                }
            }
            else
            {
                //remove clip rgn if exists**
                switch (_currentClipTech)
                {
                    case ClipingTechnique.ClipMask:
                        _pcx.DisableMask();
                        break;
                    case ClipingTechnique.ClipSimpleRect:
                        this.SetClipBox(0, 0, this.Width, this.Height);
                        break;
                }
                _currentClipTech = ClipingTechnique.None;
            }
        }
        public override void SetClipBox(int left, int top, int right, int bottom)
        {
            _pcx.SetClipRect(left, top, right - left, bottom - top);
        }
        public override void Fill(Region rgn)
        {
            var region = rgn as CpuBlitRegion;
            if (region == null) return;
            switch (region.Kind)
            {
                case CpuBlitRegion.CpuBlitRegionKind.BitmapBasedRegion:
                    {
                        //set bitmap 
                        var bmpRgn = (PixelFarm.PathReconstruction.BitmapBasedRegion)region;
                        //for bitmap that is used to be a region...
                        //our convention is ...
                        //  non-region => black
                        //  region => white                        
                        //(same as the Typography GlyphTexture)
                        MemBitmap rgnBitmap = bmpRgn.GetRegionBitmap();
                        DrawImage(rgnBitmap);
                    }
                    break;
                case CpuBlitRegion.CpuBlitRegionKind.VxsRegion:
                    {
                        //fill 'hole' of the region
                        var vxsRgn = (PixelFarm.PathReconstruction.VxsRegion)region;
                        Fill(vxsRgn.GetVxs());
                    }
                    break;
            }

        }
        public override void Draw(Region rgn)
        {
            var region = rgn as CpuBlitRegion;
            if (region == null) return;
            switch (region.Kind)
            {
                case CpuBlitRegion.CpuBlitRegionKind.BitmapBasedRegion:
                    {
                        var bmpRgn = (PixelFarm.PathReconstruction.BitmapBasedRegion)region;
                        //check if it has outline data or not
                        //if not then just return
                    }
                    break;
                case CpuBlitRegion.CpuBlitRegionKind.VxsRegion:
                    {
                        //draw outline of the region
                        var vxsRgn = (PixelFarm.PathReconstruction.VxsRegion)region;
                        Draw(vxsRgn.GetVxs());
                    }
                    break;
            }
        }

    }
}