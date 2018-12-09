//BSD, 2014-present, WinterDev
//ArthurHub, Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

namespace PixelFarm.Drawing.GLES2
{
    partial class MyGLDrawBoard
    {
        //TODO: review drawstring again ***
        Color _currentTextColor;
        public override RequestFont CurrentFont
        {
            get
            {
                return _gpuPainter.CurrentFont;
            }
            set
            {
                _gpuPainter.CurrentFont = value;
            }
        }
        /// <summary>
        /// current text fill color
        /// </summary>
        public override Color CurrentTextColor
        {
            get
            {
                return _currentTextColor;
            }
            set
            {
                _currentTextColor = value;
                //set this to 
                _gpuPainter.TextPrinter.ChangeFillColor(value);
            }
        }
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len)
        {
            char[] copy = new char[len];
            System.Array.Copy(buffer, startAt, copy, 0, len);

            var renderVxFmtStr = new DrawingGL.GLRenderVxFormattedString(copy);
            if (_gpuPainter.TextPrinter != null)
            {
                _gpuPainter.TextPrinter.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);

            }
            return renderVxFmtStr;
        }
        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            if (renderVx is DrawingGL.GLRenderVxFormattedString)
            {
                DrawingGL.GLRenderVxFormattedString formattedString = (DrawingGL.GLRenderVxFormattedString)renderVx;

                var prevColor = _gpuPainter.FillColor;
                //_gpuPainter.FillColor = PixelFarm.Drawing.Color.Black;
                _gpuPainter.FontFillColor = PixelFarm.Drawing.Color.Black;
                _gpuPainter.TextPrinter.DrawString(formattedString, x, y);
                _gpuPainter.FillColor = prevColor;
            }
        }

        public override void DrawText(char[] buffer, int left, int top)
        {
#if DEBUG
            if (_gpuPainter.FontFillColor.A == 0)
            {

            } 
#endif
            _gpuPainter.TextPrinter.DrawString(buffer, 0, buffer.Length, left, top);

        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
#if DEBUG
            if (_gpuPainter.FontFillColor.A == 0)
            {

            }
#endif
            _gpuPainter.TextPrinter.DrawString(buffer, 0, buffer.Length, logicalTextBox.X, logicalTextBox.Y);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
#if DEBUG
            if (_gpuPainter.FontFillColor.A == 0)
            {

            }
#endif

            _gpuPainter.TextPrinter.DrawString(str, startAt, len, logicalTextBox.X, logicalTextBox.Y);
        }
    }
}