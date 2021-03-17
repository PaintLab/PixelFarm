//MIT, 2014-present, WinterDev
using System;
using System.Runtime.InteropServices;


namespace Win32
{
    [StructLayout(LayoutKind.Sequential)]
    struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
        public BlendFunction(byte alpha)
        {
            BlendOp = 0;
            BlendFlags = 0;
            AlphaFormat = 0;
            SourceConstantAlpha = alpha;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BitMapInfo
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        public byte bmiColors_rgbBlue;
        public byte bmiColors_rgbGreen;
        public byte bmiColors_rgbRed;
        public byte bmiColors_rgbReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public void* bmBits;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct RGBQUAD
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public IntPtr bmBits;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int W;
        public int H;

        public int Right { get { return X + W; } }
        public int Bottom { get { return Y + H; } }


        public Rectangle(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        private bool IntersectsWithInclusive(Rectangle r)
        {
            return !((X > r.Right) || (Right < r.X) ||
                (Y > r.Bottom) || (Bottom < r.Y));
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return new Rectangle();//empty

            return new Rectangle()
            {
                X = Math.Max(a.X, b.X),
                Y = Math.Max(a.Y, b.Y),
                W = Math.Min(a.Right, b.Right),
                H = Math.Min(a.Bottom, b.Bottom)
            };
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public int W;
        public int H;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }


    [System.Security.SuppressUnmanagedCodeSecurity]
    public static partial class MyWin32
    {
        //this is platform specific ***
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void memset(byte* dest, byte c, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void memcpy(byte* dest, byte* src, int byteCount);
       

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();


        //----------
        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp; //the only source and destination blend operation that has been defined is AC_SRC_OVER
            public byte BlendFlags; //Must be zero.
            public byte SourceConstantAlpha;
            public byte AlphaFormat; //AC_SRC_ALPHA

            //Specifies an alpha transparency value to be used on the entire source bitmap. 
            //The SourceConstantAlpha value is combined with any per-pixel alpha values in the source bitmap. 
            //If you set SourceConstantAlpha to 0, it is assumed that your image is transparent. 
            //Set the SourceConstantAlpha value to 255 (opaque) when you only want to use per-pixel alpha values.
        }

        [DllImport("Msimg32.dll", EntryPoint = "AlphaBlend", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AlphaBlend(
            IntPtr hdcDest,
            int xoriginDest,
            int yoriginDest,
            int wDest,
            int hDest,
            IntPtr hdcSrc,
            int xoriginSrc,
            int yoriginSrc,
            int wSrc,
            int hSrc,
            BLENDFUNCTION ftn
            );
        //----------
        //DC
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern bool DeleteObject(IntPtr obj);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr obj);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref Win32.BitMapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        [DllImport("gdi32.dll")]
        public static extern unsafe int GetObject(
            IntPtr hgdiobj,
            int cbBuffer,
            void* lpvObject
        );
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr GetStockObject(int index);
        // 
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int left, int top, int right, int bottom);
        [DllImport("gdi32.dll")]
        public static extern int OffsetRgn(IntPtr hdc, int xoffset, int yoffset);
        [DllImport("gdi32.dll")]
        public static extern bool SetRectRgn(IntPtr hrgn, int left, int top, int right, int bottom);
        [DllImport("gdi32.dll")]
        public static extern int GetRgnBox(IntPtr hrgn, ref Rectangle lprc);
        [DllImport("gdi32.dll")]
        public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);
        public const int NULLREGION = 1;
        public const int SIMPLEREGION = 2;
        public const int COMPLEXREGION = 3;
        [DllImport("gdi32.dll")]
        public static unsafe extern bool SetViewportOrgEx(IntPtr hdc,
            int x, int y, IntPtr expoint);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSource, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern bool PatBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, int dwRop);


        //
        public const int AC_SRC_OVER = 0x00;
        //
        public const int AC_SRC_ALPHA = 0x01;
        [DllImport("gdi32.dll")]
        public static extern bool OffsetViewportOrgEx(IntPtr hdc, int nXOffset, int nYOffset, out IntPtr lpPoint);
        [DllImport("gdi32.dll")]
        public static unsafe extern bool GetViewportOrgEx(IntPtr hdc, Point* p);



        public const int SRCCOPY = 0x00CC0020;/* dest = source                   */
        public const int SRCPAINT = 0x00EE0086;/* dest = source OR dest           */
        public const int SRCAND = 0x008800C6; /* dest = source AND dest          */
        public const int SRCINVERT = 0x008800C6;/* dest = source XOR dest          */
        public const int SRCERASE = 0x00440328; /* dest = source AND (NOT dest )   */
        public const int NOTSRCCOPY = 0x00330008; /* dest = (NOT source)             */
        public const int NOTSRCERASE = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */
        public const int MERGECOPY = 0x00C000CA;/* dest = (source AND pattern)     */
        public const int MERGEPAINT = 0x00BB0226; /* dest = (NOT source) OR dest     */
        public const int PATCOPY = 0x00F00021; /* dest = pattern                  */
        public const int PATPAINT = 0x00FB0A09; /* dest = DPSnoo                   */
        public const int PATINVERT = 0x005A0049; /* dest = pattern XOR dest         */
        public const int DSTINVERT = 0x00550009; /* dest = (NOT dest)               */
        public const int BLACKNESS = 0x00000042;/* dest = BLACK                    */
        public const int WHITENESS = 0x00FF0062;/* dest = WHITE                    */
        public const int CBM_Init = 0x04;
        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hDC, int l, int t, int r, int b);

        [DllImport("gdi32.dll")]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpstring, int c, out Size size);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(int crColor);
        [DllImport("gdi32.dll")]
        public extern static int SetTextColor(IntPtr hdc, int newcolorRef);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /// <summary>
        /// request font 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            public fixed char lfFaceName[32];//[LF_FACESIZE = 32];
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)] //need -> unicode
        public extern static IntPtr CreateFontIndirect(ref LOGFONT logFont);

        public static unsafe void SetFontName(ref LOGFONT logFont, string fontName)
        {
            //font name not longer than 32 chars
            char[] fontNameChars = fontName.ToCharArray();
            int j = Math.Min(fontNameChars.Length, 31);
            fixed (char* c = logFont.lfFaceName)
            {
                char* c1 = c;
                for (int i = 0; i < j; ++i)
                {
                    *c1 = fontNameChars[i];
                    c1++;
                }
            }
        }
        //LOGFONT's  font weight
        public enum LOGFONT_FontWeight
        {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_ULTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_REGULAR = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_DEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_ULTRABOLD = 800,
            FW_HEAVY = 900,
            FW_BLACK = 900,
        }
        public const int TA_LEFT = 0;
        public const int TA_RIGHT = 2;
        public const int TA_CENTER = 6;
        public const int TA_TOP = 0;
        public const int TA_BOTTOM = 8;
        public const int TA_BASELINE = 24;
        [DllImport("gdi32.dll")]
        public extern static uint SetTextAlign(IntPtr hdc, uint fMode);
        [DllImport("gdi32.dll")]
        public extern static uint GetTextAlign(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public extern static int SetBkMode(IntPtr hdc, int mode);
        /* Background Modes */
        public const int _SetBkMode_TRANSPARENT = 1;
        public const int _SetBkMode_OPAQUE = 2;

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern int LineTo(IntPtr hdc, int nXEnd, int nYEnd);
        [DllImport("gdi32.dll")]
        public static extern bool MoveToEx(IntPtr hdc, int X, int Y, int lpPoint);


        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hWnd, string text);
        [DllImport("user32.dll")]
        public static extern void ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtualKey);
        [DllImport("user32.dll")]
        public static extern bool GetUpdateRect(IntPtr hWnd, ref RECT rect, bool bErase);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, ref RECT rect, bool bErase);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
           IntPtr handle,
           IntPtr insertAfter,
           int x, int y, int cx, int cy,
           SetWindowPosFlags flags
        );

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
          IntPtr hwndChild,
          IntPtr hwndParent
         );

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(
              IntPtr hWnd,
              int X,
              int Y,
              int nWidth,
              int nHeight,
              bool bRepaint
            );
        [Flags]
        public enum SetWindowPosFlags : int
        {
            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            NOSIZE = 0x0001,
            /// <summary>
            /// Retains the current position (ignores the x and y parameters).
            /// </summary>
            NOMOVE = 0x0002,
            /// <summary>
            /// Retains the current Z order (ignores the hwndInsertAfter parameter).
            /// </summary>
            NOZORDER = 0x0004,
            /// <summary>
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs.
            /// This applies to the client area, the nonclient area (including the title bar and scroll bars),
            /// and any part of the parent window uncovered as a result of the window being moved.
            /// When this flag is set, the application must explicitly invalidate or redraw any parts
            /// of the window and parent window that need redrawing.
            /// </summary>
            NOREDRAW = 0x0008,
            /// <summary>
            /// Does not activate the window. If this flag is not set,
            /// the window is activated and moved to the top of either the topmost or non-topmost group
            /// (depending on the setting of the hwndInsertAfter member).
            /// </summary>
            NOACTIVATE = 0x0010,
            /// <summary>
            /// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed.
            /// If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            FRAMECHANGED = 0x0020, /* The frame changed: send WM_NCCALCSIZE */
                                   /// <summary>
                                   /// Displays the window.
                                   /// </summary>
            SHOWWINDOW = 0x0040,
            /// <summary>
            /// Hides the window.
            /// </summary>
            HIDEWINDOW = 0x0080,
            /// <summary>
            /// Discards the entire contents of the client area. If this flag is not specified,
            /// the valid contents of the client area are saved and copied back into the client area
            /// after the window is sized or repositioned.
            /// </summary>
            NOCOPYBITS = 0x0100,
            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            NOOWNERZORDER = 0x0200, /* Don't do owner Z ordering */
                                    /// <summary>
                                    /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
                                    /// </summary>
            NOSENDCHANGING = 0x0400, /* Don't send WM_WINDOWPOSCHANGING */

            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            DRAWFRAME = FRAMECHANGED,
            /// <summary>
            /// Same as the NOOWNERZORDER flag.
            /// </summary>
            NOREPOSITION = NOOWNERZORDER,

            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WndClass
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr RegisterClassEx(ref WndClass wndClass);


        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormet, IntPtr hMem);



        //from WinUser.h
        public const int WM_GETDLGCODE = 0x0087;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_DEADCHAR = 0x0103;

        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_SYSDEADCHAR = 0x0107;



        //-------------
        public const int WM_SIZE = 0x0005;

        public const int SIZE_RESTORED = 0;
        public const int SIZE_MINIMIZED = 1;
        public const int SIZE_MAXIMIZED = 2;
        public const int SIZE_MAXSHOW = 3;
        public const int SIZE_MAXHIDE = 4;
        //-------------

        public const int WM_ACTIVATE = 0x0006;
        /*
       * WM_ACTIVATE state values
       */
        public const int WA_INACTIVE = 0;
        public const int WA_ACTIVE = 1;
        public const int WA_CLICKACTIVE = 2;


        /// <summary>
        /// Sent to a window after it has gained the keyboard focus.
        /// </summary>
        /// <remarks>
        /// To display a caret, an application should call the appropriate caret functions when it receives the WM_SETFOCUS message.
        /// </remarks>
        public const int WM_SETFOCUS = 0x0007;
        /// <summary>
        /// Sent to a window immediately before it loses the keyboard focus.     
        /// </summary> 
        //If an application is displaying a caret, the caret should be destroyed at this point.
        //  While processing this message, do not make any function calls that display or
        //  activate a window. 
        //This causes the thread to yield control and can cause the application
        //to stop responding to messages.
        //For more information, see Message Deadlocks.

        public const int WM_KILLFOCUS = 0x0008;



        public const int WM_SHOWWINDOW = 0x0018;


        //#define WM_KEYDOWN                      0x0100
        //#define WM_KEYUP                        0x0101
        //#define WM_CHAR                         0x0102
        //#define WM_DEADCHAR                     0x0103
        //#define WM_SYSKEYDOWN                   0x0104
        //#define WM_SYSKEYUP                     0x0105
        //#define WM_SYSCHAR                      0x0106
        //#define WM_SYSDEADCHAR                  0x0107
        //#if (_WIN32_WINNT >= 0x0501)
        //#define WM_UNICHAR                      0x0109
        //#define WM_KEYLAST                      0x0109
        //#define UNICODE_NOCHAR                  0xFFFF
        //#else
        //#define WM_KEYLAST                      0x0108
        //#endif /* _WIN32_WINNT >= 0x0501 */

        public const int WM_MOUSEMOVE = 0x0200;

        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;

        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;

        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;

        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHWHEEL = 0x020E;

        public const int WM_MOUSELEAVE = 0x02A3;


        //#if (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
        //#define WM_MOUSEWHEEL                   0x020A
        //#endif
        //#if (_WIN32_WINNT >= 0x0500)
        //#define WM_XBUTTONDOWN                  0x020B
        //#define WM_XBUTTONUP                    0x020C
        //#define WM_XBUTTONDBLCLK                0x020D
        //#endif
        //#if (_WIN32_WINNT >= 0x0600)
        //#define WM_MOUSEHWHEEL                  0x020E
        //#endif



        public const int WM_PAINT = 0x000F;
        public const int WM_ERASEBKGND = 0x0014;

        public const int WM_SETCURSOR = 0x0020;

        //#define WM_DEVMODECHANGE                0x001B
        //#define WM_ACTIVATEAPP                  0x001C
        //#define WM_FONTCHANGE                   0x001D
        //#define WM_TIMECHANGE                   0x001E
        //#define WM_CANCELMODE                   0x001F
        //#define WM_SETCURSOR                    0x0020
        //#define WM_MOUSEACTIVATE                0x0021
        //#define WM_CHILDACTIVATE                0x0022
        //#define WM_QUEUESYNC                    0x0023


        //#define VK_SHIFT          0x10
        //#define VK_CONTROL        0x11
        //#define VK_MENU           0x12
        //#define VK_PAUSE          0x13
        //#define VK_CAPITAL        0x14

        public const int VK_SHIFT = 0x10;
        public const int VK_CONTROL = 0x11;
        public const int VK_MENU = 0x12;


        public const int VK_F2 = 0x71;
        public const int VK_F3 = 0x72;
        public const int VK_LEFT = 0x25;
        public const int VK_RIGHT = 0x27;
        public const int VK_F4 = 0x73;
        public const int VK_F5 = 0x74;
        public const int VK_F6 = 0x75;
        public const int VK_ESCAPE = 0x1B;
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        public const int CB_SETCURSEL = 0x014E;
        public const int CB_SHOWDROPDOWN = 0x014F;
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int MK_LBUTTON = 0x0001;
        public const int MK_RBUTTON = 0x0002;
        public const int MK_SHIFT = 0x0004;
        public const int MK_CONTROL = 0x0008;
        public const int MK_MBUTTON = 0x0010;
        public const int WM_NCMOUSEHOVER = 0x02A0;
        public const int WM_NCMOUSELEAVE = 0x02A2;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCPAINT = 0x0085;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_MOVING = 0x0216;
        public const int WM_ACTIVATEAPP = 0x001C;
        public static int SignedLOWORD(int n)
        {
            return (short)(n & 0xffff);
        }
        public static int SignedHIWORD(int n)
        {
            return (short)((n >> 0x10) & 0xffff);
        }
        public static int MAKELONG(int low, int high)
        {
            return ((high << 0x10) | (low & 0xffff));
        }

        /// <summary>
        /// Create a compatible memory HDC from the given HDC.<br/>
        /// The memory HDC can be rendered into without effecting the original HDC.<br/>
        /// The returned memory HDC and <paramref name="dib"/> must be released using <see cref="ReleaseMemoryHdc"/>.
        /// </summary>
        /// <param name="hdc">the HDC to create memory HDC from</param>
        /// <param name="width">the width of the memory HDC to create</param>
        /// <param name="height">the height of the memory HDC to create</param>
        /// <param name="dib">returns used bitmap memory section that must be released when done with memory HDC</param>
        /// <returns>memory HDC</returns>
        public static IntPtr CreateMemoryHdc(IntPtr hdc, int width, int height, out IntPtr dib, out IntPtr ppvBits)
        {
            // Create a memory DC so we can work off-screen
            IntPtr memoryHdc = MyWin32.CreateCompatibleDC(hdc);
            MyWin32.SetBkMode(memoryHdc, 1);
            // Create a device-independent bitmap and select it into our DC
            var info = new Win32.BitMapInfo();
            info.biSize = Marshal.SizeOf(info);
            info.biWidth = width;
            info.biHeight = -height;
            info.biPlanes = 1;
            info.biBitCount = 32;
            info.biCompression = 0; // BI_RGB 
            dib = MyWin32.CreateDIBSection(hdc, ref info, 0, out ppvBits, IntPtr.Zero, 0);
            MyWin32.SelectObject(memoryHdc, dib);
            return memoryHdc;
        }
        /// <summary>
        /// Release the given memory HDC and dib section created from <see cref="CreateMemoryHdc"/>.
        /// </summary>
        /// <param name="memoryHdc">Memory HDC to release</param>
        /// <param name="dib">bitmap section to release</param>
        public static void ReleaseMemoryHdc(IntPtr memoryHdc, IntPtr dib)
        {
            MyWin32.DeleteObject(dib);
            MyWin32.DeleteDC(memoryHdc);
        }
        internal const int s_POINTS_PER_INCH = 72;
        internal static float ConvEmSizeInPointsToPixels(float emsizeInPoint, float pixels_per_inch)
        {
            return (int)(((float)emsizeInPoint / (float)s_POINTS_PER_INCH) * pixels_per_inch);
        }

    }

    [System.Security.SuppressUnmanagedCodeSecurity]
    class NativeTextWin32
    {
        const string GDI32 = "gdi32.dll";
        [DllImport(GDI32)]
        public static extern bool GetCharWidth32(IntPtr hdc, uint uFirstChar, uint uLastChar, ref int width);

        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
            [MarshalAs(UnmanagedType.LPWStr)]string charBuffer, int cbstring);
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, char[] charBuffer, int cbstring);
        [DllImport(GDI32, EntryPoint = "TextOutW")]
        public static unsafe extern bool TextOutUnsafe(IntPtr hdc, int x, int y, char* s, int len);
        [DllImport(GDI32)]
        public static unsafe extern bool ExtTextOut(IntPtr hdc, int x, int y, uint fuOptions,
            Rectangle* lpRect, char[] charBuffer, int cbCount, object arrayOfSpaceValues);
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, char[] charBuffer, int c, out Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32", CharSet = CharSet.Unicode)]
        public static unsafe extern bool GetTextExtentPoint32Char(IntPtr hdc, char* ch, int c, out Size size);
        public const int ETO_OPAQUE = 0x0002;
        public const int ETO_CLIPPED = 0x0004;
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32W", CharSet = CharSet.Unicode)]
        public static extern int GetTextExtentPoint32(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32W", CharSet = CharSet.Unicode)]
        public static unsafe extern int UnsafeGetTextExtentPoint32(
            IntPtr hdc, char* str, int len, ref Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentExPointW", CharSet = CharSet.Unicode)]
        public static extern bool GetTextExtentExPoint(IntPtr hDc, [MarshalAs(UnmanagedType.LPWStr)]string str, int nLength, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentExPointW", CharSet = CharSet.Unicode)]
        public static unsafe extern bool UnsafeGetTextExtentExPoint(
            IntPtr hDc, char* str, int len, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);
        /// <summary>
        /// translates a string into an array of glyph indices. The function can be used to determine whether a glyph exists in a font.
        /// This function attempts to identify a single-glyph representation for each character in the string pointed to by lpstr. 
        /// While this is useful for certain low-level purposes (such as manipulating font files), higher-level applications that wish to map a string to glyphs will typically wish to use the Uniscribe functions.
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="text"></param>
        /// <param name="c">The length of both the length of the string pointed to by lpstr and the size (in WORDs) of the buffer pointed to by pgi.</param>
        /// <param name="buffer">This buffer must be of dimension c. On successful return, contains an array of glyph indices corresponding to the characters in the string</param>
        /// <param name="fl">(0 | GGI_MARK_NONEXISTING_GLYPHS) Specifies how glyphs should be handled if they are not supported. This parameter can be the following value.</param>
        /// <returns>If the function succeeds, it returns the number of bytes (for the ANSI function) or WORDs (for the Unicode function) converted.</returns>
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static extern unsafe int GetGlyphIndices(IntPtr hdc, char* text, int c, ushort* glyIndexBuffer, int fl);

        [DllImport(GDI32)]
        public static unsafe extern int GetCharABCWidths(IntPtr hdc, uint uFirstChar, uint uLastChar, void* lpabc);
        [DllImport(GDI32)]
        public static unsafe extern int GetCharABCWidthsFloat(IntPtr hdc, uint uFirstChar, uint uLastChar, void* lpabc);

        public const int GGI_MARK_NONEXISTING_GLYPHS = 0X0001;

        [StructLayout(LayoutKind.Sequential)]
        public struct FontABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;
            public int Sum
            {
                get
                {
                    return abcA + (int)abcB + abcC;
                }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct ABCFloat
        {
            /// <summary>Specifies the A spacing of the character. The A spacing is the distance to add to the current
            /// position before drawing the character glyph.</summary>
            public float abcfA;
            /// <summary>Specifies the B spacing of the character. The B spacing is the width of the drawn portion of
            /// the character glyph.</summary>
            public float abcfB;
            /// <summary>Specifies the C spacing of the character. The C spacing is the distance to add to the current
            /// position to provide white space to the right of the character glyph.</summary>
            public float abcfC;
        }
        [DllImport(GDI32, CharSet = CharSet.Unicode)]
        public static unsafe extern int GetCharacterPlacement(IntPtr hdc, char* str, int nCount,
            int nMaxExtent, ref GCP_RESULTS lpResults, int dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct GCP_RESULTS
        {
            public int lStructSize;
            public char* lpOutString;
            public uint* lpOrder;
            public int* lpDx;
            public int* lpCaretPos;
            public char* lpClass;
            public char* lpGlyphs;
            public uint nGlyphs;
            public int nMaxFit;
        }
        //            DWORD GetCharacterPlacement(
        //  _In_    HDC           hdc,
        //  _In_    LPCTSTR       lpString,
        //  _In_    int           nCount,
        //  _In_    int           nMaxExtent,
        //  _Inout_ LPGCP_RESULTS lpResults,
        //  _In_    DWORD         dwFlags
        //);

        //    typedef struct _OUTLINETEXTMETRICW {
        //UINT    otmSize;
        //TEXTMETRICW otmTextMetrics;
        //BYTE    otmFiller;
        //PANOSE  otmPanoseNumber;
        //UINT    otmfsSelection;
        //UINT    otmfsType;
        // int    otmsCharSlopeRise;
        // int    otmsCharSlopeRun;
        // int    otmItalicAngle;
        //UINT    otmEMSquare;
        // int    otmAscent;
        // int    otmDescent;
        //UINT    otmLineGap;
        //UINT    otmsCapEmHeight;
        //UINT    otmsXHeight;
        //RECT    otmrcFontBox;
        // int    otmMacAscent;
        // int    otmMacDescent;
        //UINT    otmMacLineGap;
        //UINT    otmusMinimumPPEM;
        //POINT   otmptSubscriptSize;
        //POINT   otmptSubscriptOffset;
        //POINT   otmptSuperscriptSize;
        //POINT   otmptSuperscriptOffset;
        //UINT    otmsStrikeoutSize;
        // int    otmsStrikeoutPosition;
        // int    otmsUnderscoreSize;
        // int    otmsUnderscorePosition;
        //PSTR    otmpFamilyName;
        //PSTR    otmpFaceName;
        //PSTR    otmpStyleName;
        //PSTR    otmpFullName;
        //}

        [StructLayout(LayoutKind.Sequential)]
        public struct PANOSE
        {
            byte bFamilyType;
            byte bSerifStyle;
            byte bWeight;
            byte bProportion;
            byte bContrast;
            byte bStrokeVariation;
            byte bArmStyle;
            byte bLetterform;
            byte bMidline;
            byte bXHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct _OUTLINETEXTMETRICW
        {
            uint otmSize;
            TEXTMETRICW otmTextMetrics;
            byte otmFiller;
            PANOSE otmPanoseNumber;
            uint otmfsSelection;
            uint otmfsType;
            int otmsCharSlopeRise;
            int otmsCharSlopeRun;
            int otmItalicAngle;
            uint otmEMSquare;
            int otmAscent;
            int otmDescent;
            uint otmLineGap;
            uint otmsCapEmHeight;
            uint otmsXHeight;
            Rectangle otmrcFontBox;
            int otmMacAscent;
            int otmMacDescent;
            uint otmMacLineGap;
            uint otmusMinimumPPEM;
            Point otmptSubscriptSize;
            Point otmptSubscriptOffset;
            Point otmptSuperscriptSize;
            Point otmptSuperscriptOffset;
            uint otmsStrikeoutSize;
            int otmsStrikeoutPosition;
            int otmsUnderscoreSize;
            int otmsUnderscorePosition;
            char* otmpFamilyName;
            char* otmpFaceName;
            char* otmpStyleName;
            char* otmpFullName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TEXTMETRICW
        {
            int tmHeight;
            int tmAscent;
            int tmDescent;
            int tmInternalLeading;
            int tmExternalLeading;
            int tmAveCharWidth;
            int tmMaxCharWidth;
            int tmWeight;
            int tmOverhang;
            int tmDigitizedAspectX;
            int tmDigitizedAspectY;
            char tmFirstChar;
            char tmLastChar;
            char tmDefaultChar;
            char tmBreakChar;
            byte tmItalic;
            byte tmUnderlined;
            byte tmStruckOut;
            byte tmPitchAndFamily;
            byte tmCharSet;
        }



#if DEBUG
        public static void dbugDrawTextOrigin(IntPtr hdc, int x, int y)
        {
            MyWin32.Rectangle(hdc, x, y, x + 20, y + 20);
            MyWin32.MoveToEx(hdc, x, y, 0);
            MyWin32.LineTo(hdc, x + 20, y + 20);
            MyWin32.MoveToEx(hdc, x, y + 20, 0);
            MyWin32.LineTo(hdc, x + 20, y);
        }
#endif


    }
}