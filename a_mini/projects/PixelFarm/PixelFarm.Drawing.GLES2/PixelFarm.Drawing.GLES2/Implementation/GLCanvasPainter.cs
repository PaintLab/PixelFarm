//MIT, 2016-2017, WinterDev

using System;
using PixelFarm.Agg;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.DrawingGL
{
    public class GLCanvasPainter : GLCanvasPainterBase
    {

        ITextPrinter _textPriner;
        RequestFont _requestFont;
        TextureFont _textureFont;
        public GLCanvasPainter(CanvasGL2d canvas, int w, int h)
            : base(canvas, w, h)
        {
            //_win32GdiPrinter = new WinGdiFontPrinter(w, h);
        }
        public ITextPrinter TextPrinter
        {
            get { return _textPriner; }
            set { _textPriner = value; }
        }
        public override RequestFont CurrentFont
        {
            get
            {
                return _requestFont;
            }
            set
            {

                _requestFont = value;
                _textureFont = null;

                if (_requestFont.SizeInPoints > 10)
                {
                    if (UseTextureFontIfAvailable)
                    {
                        //try resolve this font
                        _textureFont = GLES2PlatformFontMx.Default.ResolveForTextureFont(value) as TextureFont;
                        if (_textureFont != null)
                        {
                            //found and can use
                            //this 
                            SetCurrentTextureFont(_textureFont);
                        }
                    }
                }
            }
        }
        public override void DrawString(string text, double x, double y)
        {

            //TODO: review here
            //for small font size we use gdi+ 
            //for large font size we use msdf font
            if (_textureFont != null)
            {
                base.DrawString(text, x, y);
            }
            else
            {
                if (_textPriner != null)
                {
                    _textPriner.DrawString(text, x, y);
                }
                
            }
        }
        public bool UseTextureFontIfAvailable { get; set; }
    }
}