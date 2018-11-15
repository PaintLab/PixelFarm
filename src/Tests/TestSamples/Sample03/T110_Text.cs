//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{

    [Info(OrderCode = "110")]
    [Info("T110_DrawText")]
    public class T110_DrawText : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
#if DEBUG
            ShowGlyphTexture = ShowMarkers = true;
#endif
        }

        [DemoConfig]
        public GlyphTexturePrinterDrawingTechnique DrawTextTechnique
        {
            get;
            set;
        }
        [DemoConfig]
        public bool UseVbo
        {
            get;
            set;
        }
        [DemoConfig]
        public bool ShowMarkers
        {
            get;
            set;
        }
        [DemoConfig]
        public bool ShowGlyphTexture
        {
            get;
            set;
        }

        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
            _glsx.Clear(PixelFarm.Drawing.Color.White);


#if DEBUG
            //test only
            GLBitmapGlyphTextPrinter.s_dbugDrawTechnique = DrawTextTechnique;
            GLBitmapGlyphTextPrinter.s_dbugUseVBO = UseVbo;
            GLBitmapGlyphTextPrinter.s_dbugShowGlyphTexture = ShowGlyphTexture;
            GLBitmapGlyphTextPrinter.s_dbugShowMarkers = ShowMarkers;
#endif

            //-------------------------------
            int line_top = 500;
            painter.FontFillColor = PixelFarm.Drawing.Color.Blue;
            painter.DrawString("aftjypqkx", 0, line_top);
            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
           
            //
            line_top = 550;
            painter.DrawString("1234567890 ABCD", 0, line_top);
            painter.StrokeColor = PixelFarm.Drawing.Color.Blue;
            
            //-------------------------------
            SwapBuffers();
        }
    }
}

