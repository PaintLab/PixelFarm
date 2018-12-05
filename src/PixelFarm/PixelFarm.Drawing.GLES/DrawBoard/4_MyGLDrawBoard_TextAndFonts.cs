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
                _gpuPainter.FillColor = PixelFarm.Drawing.Color.Black;
                _gpuPainter.FontFillColor = PixelFarm.Drawing.Color.Black;
                _gpuPainter.TextPrinter.DrawString(formattedString, x, y);
                _gpuPainter.FillColor = prevColor;
            }
        }

        public override void DrawText(char[] buffer, int left, int top)
        {
            Color prevColor = _gpuPainter.FillColor;

            //TODO: review here
            //use font color for fill the glyphs
            _gpuPainter.FontFillColor = PixelFarm.Drawing.Color.Black;//tmp
            _gpuPainter.FillColor = PixelFarm.Drawing.Color.Black;//tmp
            _gpuPainter.TextPrinter.DrawString(buffer, 0, buffer.Length, left, top);
            _gpuPainter.FillColor = prevColor;
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            //if (buffer.Length > 0 && buffer[0] == '2')
            //{

            //}
            Color prevColor = _gpuPainter.FillColor;
            _gpuPainter.FillColor = PixelFarm.Drawing.Color.Black;//tmp
            _gpuPainter.FontFillColor = PixelFarm.Drawing.Color.Black;//tmp
            _gpuPainter.TextPrinter.DrawString(buffer, 0, buffer.Length, logicalTextBox.X, logicalTextBox.Y);
            _gpuPainter.FillColor = prevColor;
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //TODO: review again
            Color prevColor = _gpuPainter.FillColor;
            _gpuPainter.FillColor = PixelFarm.Drawing.Color.Black;//tmp
            _gpuPainter.FontFillColor = PixelFarm.Drawing.Color.Black;//tmp
            _gpuPainter.TextPrinter.DrawString(str, startAt, len, logicalTextBox.X, logicalTextBox.Y);
            _gpuPainter.FillColor = prevColor;
            //TODO: review here
            //painter1.FillRectangle(0, 0, 20, 20, Color.Red);

            //painter1.FillColor = Color.Blue;
            //painter1.FillRectangle(0, 0, 20, 20);
            ////var intersectRect = Rectangle.Intersect(logicalTextBox,
            ////    new Rectangle(currentClipRect.Left,
            ////        currentClipRect.Top,
            ////        currentClipRect.Width,
            ////        currentClipRect.Height));
            ////intersectRect.Offset(canvasOriginX, canvasOriginY);
            ////MyWin32.SetRectRgn(hRgn,
            //// intersectRect.Left,
            //// intersectRect.Top,
            //// intersectRect.Right,
            //// intersectRect.Bottom);
            ////MyWin32.SelectClipRgn(tempDc, hRgn);



            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(str, startAt, len),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush,
            //    logicalTextBox.X,
            //    logicalTextBox.Y);
            ////new System.Drawing.RectangleF(
            ////    logicalTextBox.X,
            ////    logicalTextBox.Y,
            ////    logicalTextBox.Width,
            ////    logicalTextBox.Height));
            //internalSolidBrush.Color = tmpColor;
            ////var str= new string(
            ////fixed (char* startAddr = &str[0])
            ////{
            ////    Win32.Win32Utils.TextOut2(tempDc,
            ////        (int)logicalTextBox.X + canvasOriginX,
            ////        (int)logicalTextBox.Y + canvasOriginY,
            ////        (startAddr + startAt), len);
            ////}


        }
    }
}