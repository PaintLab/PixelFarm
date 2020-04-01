//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        GLBitmapAtlasPainter _bmpAtlasPainter = new GLBitmapAtlasPainter();
        public override void ApplyFilter(PixelFarm.Drawing.IImageFilter imgFilter)
        {
            //TODO: 
            throw new NotImplementedException();
        }

        public override void DrawImage(Image img, in AffineMat aff)
        {
            //create gl bmp
            //TODO: affinePlans***

            GLBitmap glBmp = _pcx.ResolveForGLBitmap(img);
            if (glBmp != null)
            {
                _pcx.DrawImage(glBmp, 0, 0);
            }
        }
        public override void DrawImage(Image img, double left, double top, ICoordTransformer coordTx)
        {
            //TODO: implement transformation matrix
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(img);
            if (glBmp != null)
            {
                if (this.OriginX != 0 || this.OriginY != 0)
                {
                    //TODO: review here
                }

                //  coordTx = aff = aff * Affine.NewTranslation(this.OriginX, this.OriginY);

                if (coordTx is Affine aff)
                {
                    _pcx.DrawImageToQuad(glBmp, aff);
                }
                else
                {

                }
            }
        }

        public override void DrawImage(Image img)
        {
            if (img is AtlasImageBinder binder)
            {
                _bmpAtlasPainter.DrawImage(this, binder, 0, 0);
            }
            else
            {
                GLBitmap glBmp = _pcx.ResolveForGLBitmap(img);
                if (glBmp == null) return;
                _pcx.DrawImage(glBmp, 0, 0);
            }

        }
        public override void DrawImage(Image img, double left, double top)
        {
            if (img is AtlasImageBinder binder)
            {
                _bmpAtlasPainter.DrawImage(this, binder, (float)left, (float)top);
            }
            else
            {
                GLBitmap glBmp = _pcx.ResolveForGLBitmap(img);
                if (glBmp == null) return;
                _pcx.DrawImage(glBmp, (float)left, (float)top);
            }
        }
        public override void DrawImage(Image img, double left, double top, int srcX, int srcY, int srcW, int srcH)
        {
            if (img is AtlasImageBinder binder)
            {
                _bmpAtlasPainter.DrawImage(this, binder, (float)left, (float)top);
            }
            else
            {
                GLBitmap glBmp = _pcx.ResolveForGLBitmap(img);
                if (glBmp == null) return;
                _pcx.DrawSubImage(glBmp, srcX, srcY, srcW, srcH, (float)left, (float)top);
            }
        }


        public void DrawImageWithMask(Image img, Image maskImg, double left, double top)
        {
            //experiment, see also our texture brush implementation



        }
    }
}