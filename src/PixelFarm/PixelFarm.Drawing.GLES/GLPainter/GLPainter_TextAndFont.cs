//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        GLBitmapGlyphTextPrinter _bmpTextPrinter;
        GlyphTexturePrinterDrawingTechnique _drawingTech;
        public Color FontFillColor
        {
            get => _pcx.FontFillColor;
            set => _pcx.FontFillColor = value;
        }
        public GlyphTexturePrinterDrawingTechnique DrawingTechnique
        {
            get => _drawingTech;
            set
            {
                _drawingTech = value;
                if (_bmpTextPrinter != null)
                {
                    _bmpTextPrinter.DrawingTechnique = value;
                }
            }
        }

        internal void SetCurrentCanvasForTextPrinter(PixelFarm.Drawing.GLES2.MyGLDrawBoard drawboard)
        {
            //temp fix
            if (_bmpTextPrinter != null)
            {
                _bmpTextPrinter._tmpDrawBoard = drawboard;
            }
        }
        public ITextPrinter TextPrinter
        {
            get => _textPrinter;
            set
            {
                _textPrinter = value;
                _bmpTextPrinter = value as GLBitmapGlyphTextPrinter;
                if (value != null && _requestFont != null)
                {
                    _textPrinter.ChangeFont(_requestFont);
                }
            }
        }
        public override RequestFont CurrentFont
        {
            get => _requestFont;
            set
            {
                _requestFont = value;
                if (_textPrinter != null)
                {
                    _textPrinter.ChangeFont(value);
                }
            }
        }
        public override void DrawString(string text, double left, double top)
        {
            _textPrinter?.DrawString(text, left, top);
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {

            if (_textPrinter != null)
            {
                char[] buffer = textspan.ToCharArray();
                var renderVxFmtStr = new GLRenderVxFormattedString();
                
#if DEBUG
                renderVxFmtStr.dbugText = textspan;
#endif
                _textPrinter?.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);
                return renderVxFmtStr;
            }
            else
            {
                return null;
            }
        }
        public override RenderVxFormattedString CreateRenderVx(char[] textspanBuff, int startAt, int len)
        {
            if (_textPrinter != null)
            {
                var renderVxFmtStr = new GLRenderVxFormattedString();
#if DEBUG
                renderVxFmtStr.dbugText = new string(textspanBuff, startAt, len);
#endif
                _textPrinter?.PrepareStringForRenderVx(renderVxFmtStr, textspanBuff, startAt, len);
                return renderVxFmtStr;
            }
            else
            {
                return null;
            }
        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            // 
            _textPrinter?.DrawString(renderVx, x, y);
        }

    }
}