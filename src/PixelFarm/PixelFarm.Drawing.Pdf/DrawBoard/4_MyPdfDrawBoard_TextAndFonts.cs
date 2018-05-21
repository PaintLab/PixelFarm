//BSD, 2014-2018, WinterDev 

namespace PixelFarm.Drawing.Pdf
{
    partial class MyPdfDrawBoard
    {
        RequestFont currentTextFont = null;
        Color mycurrentTextColor = Color.Black;
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len)
        {
            throw new System.NotImplementedException();
        }
        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            throw new System.NotImplementedException();
        }
        //public override float GetCharWidth(RequestFont f, char c)
        //{
        //    WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(f);
        //    return winFont.GetGlyph(c).horiz_adv_x >> 6;
        //}
        public override void DrawText(char[] buffer, int x, int y)
        {

            //var clipRect = currentClipRect;
            //clipRect.Offset(canvasOriginX, canvasOriginY);
            ////1.
            //win32MemDc.SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);
            ////2.
            //NativeTextWin32.TextOut(win32MemDc.DC, CanvasOrgX + x, CanvasOrgY + y, buffer, buffer.Length);
            ////3
            //win32MemDc.ClearClipRect();
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {


            //var clipRect = System.Drawing.Rectangle.Intersect(logicalTextBox.ToRect(), currentClipRect);
            ////1.
            //clipRect.Offset(canvasOriginX, canvasOriginY);
            ////2.
            //win32MemDc.SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);
            ////3.
            //NativeTextWin32.TextOut(win32MemDc.DC, CanvasOrgX + logicalTextBox.X, CanvasOrgY + logicalTextBox.Y, buffer, buffer.Length);
            ////4.
            //win32MemDc.ClearClipRect();


        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //this is the most common used function for text drawing
            //return;
#if DEBUG
            dbugDrawStringCount++;
#endif
            //            var color = this.CurrentTextColor;
            //            if (color.A == 255)
            //            {
            //                //1. find clip rect
            //                var clipRect = Rectangle.Intersect(logicalTextBox,
            //                    new Rectangle(currentClipRect.Left,
            //                        currentClipRect.Top,
            //                        currentClipRect.Width,
            //                        currentClipRect.Height));
            //                //2. offset to canvas origin 
            //                clipRect.Offset(canvasOriginX, canvasOriginY);
            //                //3. set rect rgn
            //                win32MemDc.SetClipRect(clipRect);

            //                unsafe
            //                {
            //                    fixed (char* startAddr = &str[0])
            //                    {
            //                        //4.
            //                        NativeTextWin32.TextOutUnsafe(originalHdc,
            //                            (int)logicalTextBox.X + canvasOriginX,
            //                            (int)logicalTextBox.Y + canvasOriginY,
            //                            (startAddr + startAt), len);
            //                    }
            //                }
            //                //5. clear rect rgn
            //                win32MemDc.ClearClipRect();
            //#if DEBUG
            //                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
            //                //        logicalTextBox.X + canvasOriginX,
            //                //        logicalTextBox.Y + canvasOriginY);
            //#endif

            //            }
            //            else
            //            {

            //                //-------------------------------------------
            //                //not support translucent text in this version,
            //                //so=> draw opaque (like above)
            //                //-------------------------------------------
            //                //1. find clip rect
            //                var clipRect = Rectangle.Intersect(logicalTextBox,
            //                    new Rectangle(currentClipRect.Left,
            //                        currentClipRect.Top,
            //                        currentClipRect.Width,
            //                        currentClipRect.Height));
            //                //2. offset to canvas origin 
            //                clipRect.Offset(canvasOriginX, canvasOriginY);
            //                //3. set rect rgn
            //                win32MemDc.SetClipRect(clipRect);

            //                unsafe
            //                {
            //                    fixed (char* startAddr = &str[0])
            //                    {
            //                        //4.
            //                        NativeTextWin32.TextOutUnsafe(originalHdc,
            //                            (int)logicalTextBox.X + canvasOriginX,
            //                            (int)logicalTextBox.Y + canvasOriginY,
            //                            (startAddr + startAt), len);
            //                    }
            //                }
            //                //5. clear rect rgn
            //                win32MemDc.ClearClipRect();
            //#if DEBUG
            //                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
            //                //        logicalTextBox.X + canvasOriginX,
            //                //        logicalTextBox.Y + canvasOriginY);
            //#endif



            //            }
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
                throw new System.NotSupportedException();
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