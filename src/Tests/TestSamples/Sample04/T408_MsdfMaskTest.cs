//MIT, 2014-present,WinterDev

using System;
using Mini;

using PixelFarm.Drawing;
using PixelFarm.DrawingGL;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.CpuBlit.VertexProcessing;

namespace OpenTkEssTest
{

    public enum T408_DrawSet
    {
        A,
        B,
        C,
        D,
    }

    [Info(OrderCode = "408", AvailableOn = AvailableOn.GLES)]
    [Info("T408_MsdfMaskTest")]
    public class T408_MsdfMaskTest : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;

        //
        MemBitmap _colorBmp;
        MemBitmap _msdfMaskBmp;
        GLBitmap _colorGLBmp;
        GLBitmap _msdfMaskGLBmp;



        [DemoConfig]
        public T408_DrawSet DrawSet { get; set; }

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            //1. create color bmp  
            _colorBmp = new MemBitmap(30, 30);
            using (AggPainterPool.Borrow(_colorBmp, out AggPainter painter))
            {
                painter.Clear(Color.White);
                painter.FillRect(2, 2, 30, 30, Color.Red);
            }
            //2. create mask bmp

            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                var msdf = new Msdfgen.MsdfGen3();

                v1.AddMoveTo(0, 0);
                v1.AddLineTo(20, 0);
                v1.AddLineTo(10, 15);
                v1.AddCloseFigure();

                v1.ScaleToNewVxs(1, -1, v2); //flipY

                //msdf.MsdfGenParams = new Msdfgen.MsdfGenParams() { UseCustomImageSize = true, CustomWidth = 64, CustomHeight = 64 };
                BitmapAtlasItemSource itemSrc = msdf.GenerateMsdfTexture(v2);
                _msdfMaskBmp = MemBitmap.CreateFromCopy(itemSrc.Width, itemSrc.Height, itemSrc.Source);
#if DEBUG
                //_msdfMaskBmp.SaveImage("mask_msdf.png");
#endif
            }
            _colorGLBmp = new GLBitmap(_colorBmp);
            _msdfMaskGLBmp = new GLBitmap(_msdfMaskBmp);

        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            //reset
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.ClearColorBuffer();
            _painter.Clear(Color.Yellow);

            switch (DrawSet)
            {
                case T408_DrawSet.A:
                    {
                        RectangleF maskSrc = new RectangleF(0, 0, _msdfMaskBmp.Width, _msdfMaskBmp.Height);
                        //_pcx.DrawImageWithMsdfMask(_msdfMaskGLBmp, _colorGLBmp, maskSrc,
                        //    0, 0,
                        //    20, 60);


                        Rectangle rect = new Rectangle(10, 10, 120, 120);
                        Quad2f quad = new Quad2f();
                        quad.SetCornersFromRect(rect);

                        _pcx.DrawImageWithMsdfMask(_msdfMaskGLBmp, _colorGLBmp,
                          quad,
                          maskSrc,
                          0, 0,
                          20, 60);
                    }
                    break;
                case T408_DrawSet.B:
                    {
                        RectangleF maskSrc = new RectangleF(0, 0, _msdfMaskBmp.Width, _msdfMaskBmp.Height);


                        Rectangle rect = new Rectangle(10, 10, 120, 120);
                        Quad2f quad = new Quad2f();
                        quad.SetCornersFromRect(rect);

                        AffineMat mat1 = AffineMat.Iden;
                        mat1.Translate(-rect.Width / 2, -rect.Height / 2);
                        mat1.RotateDeg(45);
                        mat1.Translate(rect.Width / 2, rect.Height / 2); 
                        quad.Transform(mat1);//***test transform

                        _pcx.DrawImageWithMsdfMask(_msdfMaskGLBmp, _colorGLBmp,
                          quad,
                          maskSrc,
                          0, 0,
                          20, 60);
                    }
                    break;
                case T408_DrawSet.C:
                    {
                        RectangleF maskSrc = new RectangleF(0, 0, _msdfMaskBmp.Width, _msdfMaskBmp.Height);

                        Rectangle rect = new Rectangle(10, 10, 120, 120);
                        Quad2f quad = new Quad2f();
                        quad.SetCornersFromRect(rect);

                        _pcx.DrawImageWithMsdfMaskV2(_msdfMaskGLBmp, _colorGLBmp,
                          quad,
                          maskSrc,
                          Color.Red,
                          20, 60);
                    }
                    break;
                case T408_DrawSet.D:
                    {
                        RectangleF maskSrc = new RectangleF(0, 0, _msdfMaskBmp.Width, _msdfMaskBmp.Height);

                        Rectangle rect = new Rectangle(10, 10, 120, 120);
                        Quad2f quad = new Quad2f();
                        quad.SetCornersFromRect(rect);


                        AffineMat mat1 = AffineMat.Iden;
                        mat1.Translate(-rect.Width / 2, -rect.Height / 2);
                        mat1.RotateDeg(45);
                        mat1.Translate(rect.Width / 2, rect.Height / 2);
                        quad.Transform(mat1);//***test transform

                        _pcx.DrawImageWithMsdfMaskV2(_msdfMaskGLBmp, _colorGLBmp,
                          quad,
                          maskSrc,
                          Color.Red,
                          20, 60);
                    }
                    break;
            }

            SwapBuffers();
        }

    }
}

