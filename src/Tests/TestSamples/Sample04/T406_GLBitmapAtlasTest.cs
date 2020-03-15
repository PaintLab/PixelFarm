//MIT, 2019-present,WinterDev

using System;
using System.Collections.Generic;
using Mini;


using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;

using PixelFarm.DrawingGL;
using PixelFarm.Drawing;


namespace OpenTkEssTest
{

    [Info(OrderCode = "406")]
    [Info("T406_GLBitmapAtlas", AvailableOn = AvailableOn.GLES)]
    public class T406_GLBitmapAtlas : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        LayoutFarm.ImageBinder _chk_checked;
        LayoutFarm.ImageBinder _chk_unchecked;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            //example;
            //test1_atlas=> atlas filename
            _chk_checked = new AtlasImageBinder("test1_atlas", "\\chk_checked.png");
            _chk_unchecked = new AtlasImageBinder("test1_atlas", "\\chk_unchecked.png");

            _pcx = painter.PainterContext;
            _painter = painter;
            //
            //string atlasInfoFile = "test1_atlas"; //see SampleFontAtlasBuilder below
            //_bmpAtlasPainter.ChangeBitmapAtlas(atlasInfoFile);

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
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.ClearColorBuffer();

            _painter.DrawImage(_chk_checked, 0, 0);
            _painter.DrawImage(_chk_unchecked, 20, 0);

            SwapBuffers();
        }
    }



}

