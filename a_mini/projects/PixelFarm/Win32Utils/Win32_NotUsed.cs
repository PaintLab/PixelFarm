//MIT, 2014-2017, WinterDev
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Win32
{
    partial class MyWin32
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalAlloc(int flags, int size);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalLock(IntPtr handle);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern bool GlobalUnlock(IntPtr handle);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalFree(IntPtr handle);

        //
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int GetMessagePos();
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int GetMessageTime();
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, object dwProcessId);

        [DllImport("user32.dll")]
        public static extern bool FrameRect(IntPtr hDC, ref Rectangle rect, IntPtr hBrush);

        //
        [DllImport("gdi32.dll")]
        public static extern int SetDCBrushColor(IntPtr hdc, int crColor);
        [DllImport("gdi32.dll")]
        public static extern int SetDCPenColor(IntPtr hdc, int crColor);
        [DllImport("gdi32.dll")]
        public static extern int GetDCBrushColor(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern int GetDCPenColor(IntPtr hdc);
        //

        [DllImport("gdi32.dll")]
        public static extern int SaveDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern bool RestoreDC(IntPtr hdc, int nSaveDC);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        //
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateDC(string szdriver, string szdevice, string szoutput, IntPtr devmode);
        //
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern int SetDIBitsToDevice(IntPtr hdc, int xdst, int ydst,
                                                int width, int height, int xsrc, int ysrc, int start, int lines,
                                                IntPtr bitsptr, IntPtr bmiptr, int color);
        //
        //TODO: review these 2 functions
        [DllImport("gdi32.dll")]
        public static extern int SetDIBits(IntPtr hdc, IntPtr hBitmap, uint uStartScan, uint cScanLines, IntPtr lpbitmapArray, IntPtr lpBitmapData, uint fuColorUse);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBitmap(IntPtr hdc, IntPtr lpBitmapInfo, int fdwInt, IntPtr lpbInit, IntPtr BitmapInfo, uint fuUsage);
        //
        [DllImport("gdi32.dll")]
        public static extern bool RoundRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nButtomRect, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hdc, ref Rectangle rect, IntPtr hBrush);
        [DllImport("gdi32.dll")]
        public static extern bool FillRgn(IntPtr hdc, IntPtr hRgn, IntPtr hBrush);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string windowName);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder winText, int maxCount);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int wCmd);
        public static string GetWinTitle(IntPtr handle)
        {
            int winTxtLength = GetWindowTextLength(handle);
            StringBuilder stBuilder = new StringBuilder(winTxtLength + 1);
            GetWindowText(handle, stBuilder, stBuilder.Capacity);
            return stBuilder.ToString();
        }
        public static string GetClassName(IntPtr handle)
        {
            StringBuilder stBuilder = new StringBuilder(100);
            GetClassName(handle, stBuilder, stBuilder.Capacity);
            return stBuilder.ToString();
        }
        [DllImport("gdi32.dll")]
        public static unsafe extern int GetOutlineTextMetrics(IntPtr hdc, uint cbData, uint uLastChar, void* lp_outlineTextMatrix);



        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int LoadKeyboardLayout(string pwszKLID, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point point);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int flags);


        //[DllImport("user32.dll")]
        //public static extern bool IsWindowVisible(IntPtr hWnd);
        //[DllImport("user32.dll")]
        //public static extern IntPtr WindowFromDC(IntPtr hdc);
        ///// <summary>
        ///// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        ///// </summary>
        ///// <remarks>
        ///// In conformance with conventions for the RECT structure, the bottom-right coordinates of the returned rectangle are exclusive. In other words, 
        ///// the pixel at (right, bottom) lies immediately outside the rectangle.
        ///// </remarks>
        ///// <param name="hWnd">A handle to the window.</param>
        ///// <param name="lpRect">A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
        ///// <returns>If the function succeeds, the return value is nonzero.</returns>
        //[DllImport("User32", SetLastError = true)]
        //public static extern int GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
        ///// <summary>
        ///// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
        ///// </summary>
        ///// <remarks>
        ///// In conformance with conventions for the RECT structure, the bottom-right coordinates of the returned rectangle are exclusive. In other words, 
        ///// the pixel at (right, bottom) lies immediately outside the rectangle.
        ///// </remarks>
        ///// <param name="handle">A handle to the window.</param>
        ///// <returns>RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</returns>
        //public static Rectangle GetWindowRectangle(IntPtr handle)
        //{
        //    Rectangle rect;
        //    GetWindowRect(handle, out rect);
        //    return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        //}

        //[DllImport("User32.dll")]
        //public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);
        //[DllImport("gdi32.dll")]
        //public static extern int SetBkMode(IntPtr hdc, int mode);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);
        //[DllImport("gdi32.dll")]
        //public static extern int SetTextColor(IntPtr hdc, int color);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        //[DllImport("gdi32.dll")]
        //public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);
        //[DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);
        //[DllImport("gdi32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        //[DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
        //public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BlendFunction blendFunction);
        //[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        //public static extern bool DeleteDC(IntPtr hdc);
        //[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        //public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        ///// <summary>
        ///// Const for BitBlt copy raster-operation code.
        ///// </summary>
        //public const int BitBltCopy = 0x00CC0020;
        ///// <summary>
        ///// Const for BitBlt paint raster-operation code.
        ///// </summary>
        //public const int BitBltPaint = 0x00EE0086;
    }
}