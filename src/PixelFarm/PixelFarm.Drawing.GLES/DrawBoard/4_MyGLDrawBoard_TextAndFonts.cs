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
                return painter1.CurrentFont;
            }
            set
            {
                painter1.CurrentFont = value;
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
                this._currentTextColor = value;
                //set this to 
                painter1.TextPrinter.ChangeFillColor(value);
            }
        }
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len)
        {
            char[] copy = new char[len];
            System.Array.Copy(buffer, startAt, copy, 0, len);

            var renderVxFmtStr = new DrawingGL.GLRenderVxFormattedString(copy);
            if (painter1.TextPrinter != null)
            {
                painter1.TextPrinter.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);

            }
            return renderVxFmtStr;
        }
        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            if (renderVx is DrawingGL.GLRenderVxFormattedString)
            {
                DrawingGL.GLRenderVxFormattedString formattedString = (DrawingGL.GLRenderVxFormattedString)renderVx;
               
                var prevColor = painter1.FillColor;
                painter1.FillColor = PixelFarm.Drawing.Color.Black; 
                painter1.TextPrinter.DrawString(formattedString, x, this.Height - y);
                painter1.FillColor = prevColor;
            }
        }
       
        public override void DrawText(char[] buffer, int x, int y)
        {
            var prevColor = painter1.FillColor;

            //TODO: review here
            //use font color for fill the glyphs

            painter1.FillColor = PixelFarm.Drawing.Color.Black;
            painter1.TextPrinter.DrawString(buffer, 0, buffer.Length, x, this.Height - y);
            painter1.FillColor = prevColor;
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            var prevColor = painter1.FillColor;
            painter1.FillColor = PixelFarm.Drawing.Color.Black;
            painter1.TextPrinter.DrawString(buffer, 0, buffer.Length, logicalTextBox.X, this.Height - logicalTextBox.Y);
            painter1.FillColor = prevColor;
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //TODO: review again
            var prevColor = painter1.FillColor;
            painter1.FillColor = PixelFarm.Drawing.Color.Black;
            painter1.TextPrinter.DrawString(str, startAt, len, logicalTextBox.X, this.Height - logicalTextBox.Y);
            painter1.FillColor = prevColor;
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