//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    public enum T110_DrawTextColor
    {
        //for debug only
        Black,
        Red,
        Green,
        Blue,
        Yellow,
        Magenta
    }

    [Info(OrderCode = "110", SupportedOn = AvailableOn.GLES)]
    [Info("T110_DrawText")]
    public class T110_DrawText : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
            UserText = "";
#if DEBUG
            ShowGlyphTexture = ShowMarkers = true;
#endif
        }
        [DemoConfig]
        public T110_DrawTextColor DrawTextColor { get; set; }
        [DemoConfig]
        public GlyphTexturePrinterDrawingTechnique DrawTextTechnique { get; set; }
        [DemoConfig]
        public bool UseVbo { get; set; }
        [DemoConfig]
        public bool ShowMarkers { get; set; }
        [DemoConfig]
        public bool ShowGlyphTexture { get; set; }
        [DemoConfig]
        public string UserText { get; set; }
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
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            _pcx.Clear(PixelFarm.Drawing.Color.White);


#if DEBUG
            //test only
            GLBitmapGlyphTextPrinter.s_dbugDrawTechnique = DrawTextTechnique;
            GLBitmapGlyphTextPrinter.s_dbugUseVBO = UseVbo;
            GLBitmapGlyphTextPrinter.s_dbugShowGlyphTexture = ShowGlyphTexture;
            GLBitmapGlyphTextPrinter.s_dbugShowMarkers = ShowMarkers;
#endif

            //-------------------------------

            PixelFarm.Drawing.Color fillColor = PixelFarm.Drawing.Color.Black;
            switch (DrawTextColor)
            {
                case T110_DrawTextColor.Blue:
                    fillColor = PixelFarm.Drawing.Color.Blue;
                    break;
                case T110_DrawTextColor.Green:
                    fillColor = PixelFarm.Drawing.Color.Green;
                    break;
                case T110_DrawTextColor.Magenta:
                    fillColor = PixelFarm.Drawing.Color.Magenta;
                    break;
                case T110_DrawTextColor.Red:
                    fillColor = PixelFarm.Drawing.Color.Red;
                    break;
                case T110_DrawTextColor.Yellow:
                    fillColor = PixelFarm.Drawing.Color.Yellow;
                    break;
            }


            int line_top = 500;
            _painter.FontFillColor = fillColor;


            _painter.DrawString("aftjypqkx", 0, line_top);

            //
            line_top = 550;
            _painter.DrawString("1234567890 ABCD", 0, line_top);
            //-------------------------------
            line_top = 570;
            if (!string.IsNullOrEmpty(UserText))
            {
                _painter.DrawString(UserText, 0, line_top);
            }

            SwapBuffers();
        }
    }
}

