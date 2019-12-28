using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;


namespace LayoutFarm.UI
{

    public static class PI
    {

        //platform invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hWnd, short cmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, short cmdShow);
        //        BOOL WINAPI SetWindowPos(
        //  _In_ HWND hWnd,
        //  _In_opt_ HWND hWndInsertAfter,
        //  _In_     int X,
        //  _In_     int Y,
        //  _In_     int cx,
        //  _In_     int cy,
        //  _In_ UINT uFlags
        //);
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_MAXIMIZE = 0x01000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_BORDER = 0x00800000;
        public const int PRF_CLIENT = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_CLOSE = 0xF060;
        public const int SC_RESTORE = 0xF120;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int WM_DESTROY = 0x0002;
        public const int WM_NCDESTROY = 0x0082;
        public const int WM_MOVE = 0x0003;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;
        public const int WM_SETREDRAW = 0x000B;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_PAINT = 0x000F;
        public const int WM_PRINTCLIENT = 0x0318;
        public const int WM_CTLCOLOR = 0x0019;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_HELP = 0x0053;
        public const int WM_NCCALCSIZE = 0x0083;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCPAINT = 0x0085;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_DEADCHAR = 0x0103;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_SYSDEADCHAR = 0x0107;
        public const int WM_KEYLAST = 0x0108;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_INITMENU = 0x0116;
        public const int WM_CTLCOLOREDIT = 0x0133;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_NCMOUSELEAVE = 0x02A2;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_PRINT = 0x0317;
        public const int WM_CONTEXTMENU = 0x007B;
        public const int MA_NOACTIVATE = 0x03;
        public const int EM_FORMATRANGE = 0x0439;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_FRAME = 0x0400;
        public const int DCX_WINDOW = 0x01;
        public const int DCX_CACHE = 0x02;
        public const int DCX_CLIPSIBLINGS = 0x10;
        public const int DCX_INTERSECTRGN = 0x80;
        public const int TME_LEAVE = 0x0002;
        public const int TME_NONCLIENT = 0x0010;
        public const int HTNOWHERE = 0x00;
        public const int HTCLIENT = 0x01;
        public const int HTCAPTION = 0x02;
        public const int HTSYSMENU = 0x03;
        public const int HTGROWBOX = 0x04;
        public const int HTSIZE = 0x04;
        public const int HTMENU = 0x05;
        public const int HTLEFT = 0x0A;
        public const int HTRIGHT = 0x0B;
        public const int HTTOP = 0x0C;
        public const int HTTOPLEFT = 0x0D;
        public const int HTTOPRIGHT = 0x0E;
        public const int HTBOTTOM = 0x0F;
        public const int HTBOTTOMLEFT = 0x10;
        public const int HTBOTTOMRIGHT = 0x11;
        public const int HTBORDER = 0x12;
        public const int HTHELP = 0x15;
        public const int HTIGNORE = 0xFF;
        public const int HTTRANSPARENT = -1;
        public const int ULW_ALPHA = 0x00000002;
        public const int DEVICE_BITSPIXEL = 12;
        public const int DEVICE_PLANES = 14;
        public const int SRCCOPY = 0xCC0020;
        public const int GWL_STYLE = -16;
        public const int DTM_SETMCCOLOR = 0x1006;
        public const int DTT_COMPOSITED = 8192;
        public const int DTT_GLOWSIZE = 2048;
        public const int DTT_TEXTCOLOR = 1;
        public const int MCSC_BACKGROUND = 0;
        public const int PLANES = 14;
        public const int BITSPIXEL = 12;
        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;
        public const uint GW_HWNDFIRST = 0;
        public const uint GW_HWNDLAST = 1;
        public const uint GW_HWNDNEXT = 2;
        public const uint GW_HWNDPREV = 3;
        public const uint GW_OWNER = 4;
        public const uint GW_CHILD = 5;
        public const uint GW_ENABLEDPOPUP = 6;
    }

    public static class CommonHelper
    {
        //BSD-3 clauses, 2016, https://github.com/ComponentFactory/Krypton
        // <summary>
        /// Create a graphics path that describes a rounded rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to become rounded.</param>
        /// <param name="rounding">The rounding factor to apply.</param>
        /// <returns>GraphicsPath instance.</returns>
        public static GraphicsPath RoundedRectanglePath(Rectangle rect,
                                                        int rounding)
        {
            GraphicsPath roundedPath = new GraphicsPath();

            // Only use a rounding that will fit inside the rect
            rounding = Math.Min(rounding, Math.Min(rect.Width / 2, rect.Height / 2) - rounding);

            // If there is no room for any rounding effect...
            if (rounding <= 0)
            {
                // Just add a simple rectangle as a quick way of adding four lines
                roundedPath.AddRectangle(rect);
            }
            else
            {
                // We create the path using a floating point rectangle
                RectangleF rectF = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);

                // The border is made of up a quarter of a circle arc, in each corner
                int arcLength = rounding * 2;
                roundedPath.AddArc(rectF.Left, rectF.Top, arcLength, arcLength, 180f, 90f);
                roundedPath.AddArc(rectF.Right - arcLength, rectF.Top, arcLength, arcLength, 270f, 90f);
                roundedPath.AddArc(rectF.Right - arcLength, rectF.Bottom - arcLength, arcLength, arcLength, 0f, 90f);
                roundedPath.AddArc(rectF.Left, rectF.Bottom - arcLength, arcLength, arcLength, 90f, 90f);

                // Make the last and first arc join up
                roundedPath.CloseFigure();
            }

            return roundedPath;
        }
    }
}