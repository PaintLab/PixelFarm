//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

namespace OpenTkEssTest
{
    [Info(OrderCode = "401", AvailableOn = AvailableOn.GLES)]
    [Info("T401_MsdfTest1")]
    public class T401_MsdfTest1 : DemoBase
    {
        GLPainterContext _pcx;
        bool _resInit;
        GLBitmap _msdf_bmp;
        GLBitmap _sdf_bmp;
        float _scale = 1.0f;
        ExtMsdfGen.SpriteTextureMapData<MemBitmap> _spriteMapData;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
        }
        [DemoConfig(MaxValue = 500, MinValue = 1)]
        public float ZoomLevel
        {
            get => _scale;
            set
            {
                _scale = value / 10;
            }
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            TestMsdf3();
            //TestMsdf2();
            //TestMsdf1();
        }

        void TestMsdf2()
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();

            if (!_resInit)
            {
                _msdf_bmp = DemoHelper.LoadTexture("d:\\WImageTest\\msdf_shape.png");
                //_msdf_bmp = DemoHelper.LoadTexture("d:\\WImageTest\\msdf_fake1_1.png");
                _resInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.White);

            for (int y = 400; y >= 0; --y)
            {
                _pcx.DrawImageWithMsdf(_msdf_bmp, 0, y, 0.25f * _scale, PixelFarm.Drawing.Color.FromArgb(100, PixelFarm.Drawing.Color.Black));
                _pcx.DrawImageWithMsdf(_msdf_bmp, 5, y, 0.5f * _scale, PixelFarm.Drawing.Color.FromArgb(100, PixelFarm.Drawing.Color.Blue));
                _pcx.DrawImageWithMsdf(_msdf_bmp, 100, y, 1 * _scale, PixelFarm.Drawing.Color.FromArgb(80, PixelFarm.Drawing.Color.Red));
                _pcx.DrawImageWithMsdf(_msdf_bmp, 150, y, 2 * _scale, PixelFarm.Drawing.Color.FromArgb(50, PixelFarm.Drawing.Color.Green));
                y -= 20;
            }


            //_pcx.DrawImageWithMsdf(_msdf_bmp, 100, 500, 0.5f);

        }
        void TestMsdf1()
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            if (!_resInit)
            {

                _msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png");
                _sdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\sdf_75.png");
                _resInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.White);

            _pcx.DrawImageWithMsdf(_msdf_bmp, 0, 400, 6);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 500, 0.5f);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 520, 0.4f);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 550, 0.3f);
            _pcx.DrawImage(_msdf_bmp, 150, 400);

            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 200, 400, 6);
            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 500, 0.5f);
            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 520, 0.4f);
            _pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 550, 0.3f);

            //
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 400, 6);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 500, 0.5f);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 520, 0.4f);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 550, 0.3f);
            _pcx.DrawImage(_sdf_bmp, 400, 300);

            SwapBuffers();
        }


        void TestMsdf3()
        {
            //we create vxs shape and then create msdf bitmap 
            //then render to the screen
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();

            if (!_resInit)
            {
                using (VxsTemp.Borrow(out var v1))
                {
                    GetExampleVxs(v1);

                    ExtMsdfGen.MsdfGen3 gen3 = new ExtMsdfGen.MsdfGen3();
                    _spriteMapData = gen3.GenerateMsdfTexture(v1);
                    _msdf_bmp = new GLBitmap(_spriteMapData.Source, true);
                }
                _resInit = true;
            }

            _pcx.Clear(PixelFarm.Drawing.Color.White);

            for (int y = 400; y >= 0; --y)
            {
                _pcx.DrawImageWithMsdf(_msdf_bmp, 0, y, 0.25f * _scale, PixelFarm.Drawing.Color.FromArgb(100, PixelFarm.Drawing.Color.Black));
                _pcx.DrawImageWithMsdf(_msdf_bmp, 5, y, 0.5f * _scale, PixelFarm.Drawing.Color.FromArgb(100, PixelFarm.Drawing.Color.Blue));
                _pcx.DrawImageWithMsdf(_msdf_bmp, 100, y, 1 * _scale, PixelFarm.Drawing.Color.FromArgb(80, PixelFarm.Drawing.Color.Red));
                _pcx.DrawImageWithMsdf(_msdf_bmp, 150, y, 2 * _scale, PixelFarm.Drawing.Color.FromArgb(50, PixelFarm.Drawing.Color.Green));
                y -= 20;
            }

        }
        void GetExampleVxs(VertexStore outputVxs)
        {
            //counter-clockwise 
            //a triangle
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddCloseFigure();

            //a quad
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();



            //curve4
            //outputVxs.AddMoveTo(5, 5);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddCurve4To(70, 20, 50, 10, 10, 5);
            //outputVxs.AddCloseFigure();

            //curve3
            outputVxs.AddMoveTo(5, 5);
            outputVxs.AddLineTo(50, 60);
            outputVxs.AddCurve3To(70, 20, 10, 5);
            outputVxs.AddCloseFigure();


            //a quad with hole
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();

            //outputVxs.AddMoveTo(30, 30);
            //outputVxs.AddLineTo(40, 30);
            //outputVxs.AddLineTo(40, 35);
            //outputVxs.AddLineTo(30, 35);
            //outputVxs.AddCloseFigure();



        }

    }
}

