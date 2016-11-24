//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo


using System;
namespace PixelFarm.Drawing.WinGdi
{
    partial class MyGdiPlusCanvas
    {
        RequestFont currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        //public override float GetCharWidth(RequestFont f, char c)
        //{
        //    WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(f);
        //    return winFont.GetGlyph(c).horiz_adv_x >> 6;
        //}
        public override void DrawText(char[] buffer, int x, int y)
        {

            var clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            gx.SetClip(clipRect);
            gx.DrawString(buffer, x, y);
            gx.ClearClip();
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {

            var clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            gx.SetClip(clipRect);
            gx.DrawString(buffer, logicalTextBox);
            gx.ClearClip();
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //this is the most common used function for text drawing
            //return;
#if DEBUG
            dbugDrawStringCount++;
#endif
            var color = this.CurrentTextColor;
            if (color.A == 255)
            {
                //1. find clip rect
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle((int)currentClipRect.Left,
                        (int)currentClipRect.Top,
                        (int)currentClipRect.Width,
                        (int)currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rect rgn

                clipRect.Offset(canvasOriginX, canvasOriginY);
                gx.SetClip(clipRect);
                gx.DrawString(str, logicalTextBox);
                gx.ClearClip();
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif

            }
            else
            {

                //1. find clip rect
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle((int)currentClipRect.Left,
                        (int)currentClipRect.Top,
                        (int)currentClipRect.Width,
                        (int)currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rect rgn

                clipRect.Offset(canvasOriginX, canvasOriginY);
                gx.SetClip(clipRect);
                gx.DrawString(str, logicalTextBox);
                gx.ClearClip();
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif 
            }
        }
        //====================================================
        public override RequestFont CurrentFont
        {
            get
            {
                return currentTextFont;
            }
            set
            {

                this.currentTextFont = value;
                gx.CurrentFont = value;
                //win32MemDc.SetFont(WinGdiFontSystem.GetWinGdiFont(value).ToHfont());
            }
        }
        public override Color CurrentTextColor
        {
            get
            {
                return mycurrentTextColor;
            }
            set
            {
                mycurrentTextColor = value;
                //win32MemDc.SetSolidTextColor(value.R, value.G, value.B);
                gx.CurrentTextColor = value;
            }
        }
    }
}