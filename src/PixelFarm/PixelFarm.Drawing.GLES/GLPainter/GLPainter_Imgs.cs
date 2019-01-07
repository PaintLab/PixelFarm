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
        public override void ApplyFilter(PixelFarm.Drawing.IImageFilter imgFilter)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(Image actualImage, params AffinePlan[] affinePlans)
        {
            //create gl bmp
            //TODO: affinePlans***
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp != null)
            {
                _pcx.DrawImage(glBmp, 0, 0);
            }
        }
        public override void DrawImage(Image actualImage, double left, double top, ICoordTransformer coordTx)
        {
            //TODO: implement transformation matrix
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp != null)
            {
                if (this.OriginX != 0 || this.OriginY != 0)
                {
                    //TODO: review here
                }

                //  coordTx = aff = aff * Affine.NewTranslation(this.OriginX, this.OriginY);

                Affine aff = coordTx as Affine;
                if (aff != null)
                {
                    _pcx.DrawImageToQuad(glBmp, aff);
                }
                else
                {

                }

                //_pcx.DrawImage(glBmp, (float)left, (float)top);
            }
        }
        //public override void DrawImage(Image actualImage, double left, double top, ICoordTransformer coordTx)
        //{
        //    //draw img with transform coord
        //    //
        //    MemBitmap memBmp = actualImage as MemBitmap;
        //    if (memBmp == null)
        //    {
        //        //? TODO
        //        return;
        //    }

        //    if (_renderQuality == RenderQuality.Fast)
        //    {
        //        //todo, review here again
        //        //TempMemPtr tmp = ActualBitmap.GetBufferPtr(actualImg);
        //        //BitmapBuffer srcBmp = new BitmapBuffer(actualImage.Width, actualImage.Height, tmp.Ptr, tmp.LengthInBytes); 
        //        //_bxt.BlitRender(srcBmp, false, 1, new BitmapBufferEx.MatrixTransform(affinePlans));

        //        //if (affinePlans != null && affinePlans.Length > 0)
        //        //{
        //        //    _bxt.BlitRender(srcBmp, false, 1, new BitmapBufferEx.MatrixTransform(affinePlans));
        //        //}
        //        //else
        //        //{
        //        //    //_bxt.BlitRender(srcBmp, false, 1, null);
        //        //    _bxt.Blit(0, 0, srcBmp.PixelWidth, srcBmp.PixelHeight, srcBmp, 0, 0, srcBmp.PixelWidth, srcBmp.PixelHeight,
        //        //        ColorInt.FromArgb(255, 255, 255, 255),
        //        //        BitmapBufferExtensions.BlendMode.Alpha);
        //        //}
        //        return;
        //    }

        //    bool useSubPix = UseSubPixelLcdEffect; //save, restore later... 
        //                                           //before render an image we turn off vxs subpixel rendering
        //    this.UseSubPixelLcdEffect = false;

        //    if (coordTx is Affine)
        //    {
        //        Affine aff = (Affine)coordTx;
        //        if (this.OriginX != 0 || this.OriginY != 0)
        //        {
        //            coordTx = aff = aff * Affine.NewTranslation(this.OriginX, this.OriginY);
        //        }
        //    }

        //    //_aggsx.SetScanlineRasOrigin(OriginX, OriginY);

        //    _aggsx.Render(memBmp, coordTx);

        //    //_aggsx.SetScanlineRasOrigin(xx, yy);
        //    //restore...
        //    this.UseSubPixelLcdEffect = useSubPix;
        //}
        public override void DrawImage(Image actualImage)
        {
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;
            _pcx.DrawImage(glBmp, 0, 0);
        }
        public override void DrawImage(Image actualImage, double left, double top)
        {
            GLBitmap glBmp = _pcx.ResolveForGLBitmap(actualImage);
            if (glBmp == null) return;
            _pcx.DrawImage(glBmp, (float)left, (float)top);
        }
        public override void DrawImage(Image actualImage, double left, double top, int srcX, int srcY, int srcW, int srcH)
        {
            throw new NotImplementedException();
        }

    }
}