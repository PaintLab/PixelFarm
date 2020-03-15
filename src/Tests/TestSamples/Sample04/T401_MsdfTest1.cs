//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

namespace OpenTkEssTest
{
    public enum MsdfTest1Mode
    {
        Test1_Sdf_Msdf,
        Test2_Cache,
        Test3_Realtime,
    }
    [Info(OrderCode = "401", AvailableOn = AvailableOn.GLES)]
    [Info("T401_MsdfTest1")]
    public class T401_MsdfTest1 : DemoBase
    {
        GLPainterContext _pcx;
        bool _resInit;
        GLBitmap _msdf_bmp;
        GLBitmap _sdf_bmp;
        float _scale = 1.0f;
        bool _showMsdf;

        PixelFarm.CpuBlit.BitmapAtlas.SpriteTextureMapData<MemBitmap> _spriteMapData;
        public T401_MsdfTest1()
        {
            Mode = MsdfTest1Mode.Test2_Cache;
        }
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
            if (!_showMsdf) return;
            //
            switch (Mode)
            {
                case MsdfTest1Mode.Test1_Sdf_Msdf:
                    TestMsdf1();
                    break;
                case MsdfTest1Mode.Test2_Cache:
                    TestMsdf2();
                    break;
                case MsdfTest1Mode.Test3_Realtime:
                    TestMsdf3();
                    break;
            }
        }
        [DemoConfig]
        public MsdfTest1Mode Mode { get; set; }
        [DemoAction]
        public void ShowMsdf()
        {
            _showMsdf = !_showMsdf;
        }

        void TestMsdf2()
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();

            if (!_resInit)
            {
                //you can create msdf_shape.png with FormTestMsdfGen
                _msdf_bmp = DemoHelper.LoadTexture("msdf_shape.png");
                //_msdf_bmp = DemoHelper.LoadTexture("msdf_fake1_1.png");
                _resInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.White);

            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 400, 1 * _scale, PixelFarm.Drawing.Color.FromArgb(80, PixelFarm.Drawing.Color.Red));


            //for (int y = 400; y >= 0; --y)
            //{
            //    _pcx.DrawImageWithMsdf(_msdf_bmp, 0, y, 0.25f * _scale, PixelFarm.Drawing.Color.FromArgb(100, PixelFarm.Drawing.Color.Black));
            //    _pcx.DrawImageWithMsdf(_msdf_bmp, 5, y, 0.5f * _scale, PixelFarm.Drawing.Color.FromArgb(100, PixelFarm.Drawing.Color.Blue));
            //    _pcx.DrawImageWithMsdf(_msdf_bmp, 100, y, 1 * _scale, PixelFarm.Drawing.Color.FromArgb(80, PixelFarm.Drawing.Color.Red));
            //    _pcx.DrawImageWithMsdf(_msdf_bmp, 150, y, 2 * _scale, PixelFarm.Drawing.Color.FromArgb(50, PixelFarm.Drawing.Color.Green));
            //    y -= 20;
            //}


            //_pcx.DrawImageWithMsdf(_msdf_bmp, 100, 500, 0.5f);

        }
        void TestMsdf1()
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            if (_msdf_bmp == null)
            {

                _msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png");
                _sdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\sdf_75.png");
                _resInit = true;
            }
            _pcx.Clear(PixelFarm.Drawing.Color.White);


            _pcx.DrawImageWithMsdf(_msdf_bmp, 0, 400, 6, Color.Red);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 500, 0.5f, Color.Red);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 520, 0.4f, Color.Red);
            _pcx.DrawImageWithMsdf(_msdf_bmp, 100, 550, 0.3f, Color.Red);
            _pcx.DrawImage(_msdf_bmp, 150, 400);

            //_pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 200, 400, 6);
            //_pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 500, 0.5f);
            //_pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 520, 0.4f);
            //_pcx.DrawImageWithSubPixelRenderingMsdf(_msdf_bmp, 300, 550, 0.3f);

            //
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 400, 6, Color.Red);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 500, 0.5f, Color.Red);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 520, 0.4f, Color.Red);
            _pcx.DrawImageWithMsdf(_sdf_bmp, 400, 550, 0.3f, Color.Red);
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
                    //if v1 is too large we can scale down to ? some degree 
                    //that still preserve original detail for reconstruction 
                    Msdfgen.MsdfGen3 gen3 = new Msdfgen.MsdfGen3();

                    PixelFarm.CpuBlit.BitmapAtlas.BitmapAtlasItem msdf = gen3.GenerateMsdfTexture(v1);
                    var map = new PixelFarm.CpuBlit.BitmapAtlas.SpriteTextureMapData<MemBitmap>(msdf.Left, msdf.Top, msdf.Width, msdf.Height);
                    map.TextureXOffset = msdf.TextureXOffset;
                    map.TextureYOffset = msdf.TextureYOffset;
                    map.Source = MemBitmap.CreateFromCopy(msdf.Width, msdf.Height, msdf.Source);

                    _spriteMapData = map;
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

