//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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

using System;
using Win32;
namespace PixelFarm.Drawing.WinGdi
{
    partial class MyGdiPlusCanvas
    {
        RequestFont currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        public override float GetCharWidth(RequestFont f, char c)
        {
            WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(f);
            return winFont.GetGlyph(c).horiz_adv_x >> 6;
        }
        public override void DrawText(char[] buffer, int x, int y)
        {
            
            //draw text to target mem dc?
            IntPtr gxdc = gx.GetHdc();
            var clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            MyWin32.SetRectRgn(hRgn,
             clipRect.Left,
             clipRect.Top,
             clipRect.Right,
             clipRect.Bottom);
            MyWin32.SelectClipRgn(gxdc, hRgn);
            NativeTextWin32.TextOut(gxdc, CanvasOrgX + x, CanvasOrgY + y, buffer, buffer.Length);
            MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);
            gx.ReleaseHdc();
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
           
            IntPtr gxdc = gx.GetHdc();
            var clipRect = System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            clipRect.Offset(canvasOriginX, canvasOriginY);
            MyWin32.SetRectRgn(hRgn,
             clipRect.Left,
             clipRect.Top,
             clipRect.Right,
             clipRect.Bottom);
            MyWin32.SelectClipRgn(gxdc, hRgn);
            NativeTextWin32.TextOut(gxdc, CanvasOrgX + logicalTextBox.X, CanvasOrgY + logicalTextBox.Y, buffer, buffer.Length);
            MyWin32.SelectClipRgn(gxdc, IntPtr.Zero);
            gx.ReleaseHdc();
            //ReleaseHdc();
            //IntPtr gxdc = gx.GetHdc();
            //MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //System.Drawing.Rectangle clipRect =
            //    System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            //clipRect.Offset(CanvasOrgX, CanvasOrgY);
            //MyWin32.SetRectRgn(hRgn, clipRect.X, clipRect.Y, clipRect.Right, clipRect.Bottom);
            //MyWin32.SelectClipRgn(gxdc, hRgn);
            //NativeTextWin32.TextOut(gxdc, logicalTextBox.X, logicalTextBox.Y, buffer, buffer.Length); 
            //MyWin32.SelectClipRgn(gxdc, IntPtr.Zero); 
            //MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero); 
            //gx.ReleaseHdc();

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
                    new Rectangle(currentClipRect.Left,
                        currentClipRect.Top,
                        currentClipRect.Width,
                        currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rgn 
                MyWin32.SetRectRgn(hRgn,
                 clipRect.Left,
                 clipRect.Top,
                 clipRect.Right,
                 clipRect.Bottom);
                MyWin32.SelectClipRgn(originalHdc, hRgn);
                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        NativeTextWin32.TextOutUnsafe(originalHdc,
                            (int)logicalTextBox.X + canvasOriginX,
                            (int)logicalTextBox.Y + canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                MyWin32.SelectClipRgn(originalHdc, IntPtr.Zero);
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif

            }
            else
            {

                //-------------------------------------------
                //not support in this version
                var clipRect = Rectangle.Intersect(logicalTextBox,
                  new Rectangle(currentClipRect.Left,
                      currentClipRect.Top,
                      currentClipRect.Width,
                      currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rgn 
                MyWin32.SetRectRgn(hRgn,
                 clipRect.Left,
                 clipRect.Top,
                 clipRect.Right,
                 clipRect.Bottom);
                MyWin32.SelectClipRgn(originalHdc, hRgn);
                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        NativeTextWin32.TextOutUnsafe(originalHdc,
                            (int)logicalTextBox.X + canvasOriginX,
                            (int)logicalTextBox.Y + canvasOriginY,
                            (startAddr + startAt), len);
                    }
                }
                MyWin32.SelectClipRgn(originalHdc, IntPtr.Zero);
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
                win32MemDc.SetFont(WinGdiFontSystem.GetWinGdiFont(value).ToHfont()); 
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
                win32MemDc.SetSolidTextColor(value.R, value.G, value.B);
                //int rgb = (value.B & 0xFF) << 16 | (value.G & 0xFF) << 8 | value.R;
                //MyWin32.SetTextColor(originalHdc, rgb); 
                //SetTextColor(value);
                //this.currentTextColor = ConvColor(value);
                //IntPtr hdc = gx.GetHdc();
                //MyWin32.SetTextColor(hdc, MyWin32.ColorToWin32(value));
                //gx.ReleaseHdc();
            }
        }
    }
}