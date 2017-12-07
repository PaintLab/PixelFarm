//MIT, 2014-2017, WinterDev
using System;
using System.Runtime.InteropServices;
using System.Text;

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
    public unsafe struct BITMAP
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
    public struct RGBQUAD
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



    public static partial class MyWin32
    {
        //this is platform specific ***
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void memset(byte* dest, byte c, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void memcpy(byte* dest, byte* src, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int memcmp(byte* dest, byte* src, int byteCount);
        //----------

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
        [DllImport("Msimg32.dll")]
        public static extern bool AlphaBlend(IntPtr hdc, int nXOriginDest,
            int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc,
            int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, ref _BLENDFUNCTION blendFunction);
        [StructLayout(LayoutKind.Sequential)]
        public struct _BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
            public _BLENDFUNCTION(byte alphaValue)
            {
                BlendOp = AC_SRC_OVER;
                BlendFlags = 0;
                SourceConstantAlpha = alphaValue;
                AlphaFormat = 0;
            }
        }
        //
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
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder winText, int maxCount);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);

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
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
         
      

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_PAINT = 0x000F;
        public const int WM_MOUSEMOVE = 0x0200;
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
        const int s_POINTS_PER_INCH = 72;
        static float ConvEmSizeInPointsToPixels(float emsizeInPoint, float pixels_per_inch)
        {
            return (int)(((float)emsizeInPoint / (float)s_POINTS_PER_INCH) * pixels_per_inch);
        }
        public static IntPtr CreateFontHelper(string fontName, float emHeight, bool bold, bool italic, float pixels_per_inch = 96)
        {
            //see: MSDN, LOGFONT structure
            //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
            MyWin32.LOGFONT logFont = new MyWin32.LOGFONT();
            MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = -(int)ConvEmSizeInPointsToPixels(emHeight, pixels_per_inch);//minus **
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default

            //
            MyWin32.LOGFONT_FontWeight weight =
                bold ?
                MyWin32.LOGFONT_FontWeight.FW_BOLD :
                MyWin32.LOGFONT_FontWeight.FW_REGULAR;
            logFont.lfWeight = (int)weight;
            //
            logFont.lfItalic = (byte)(italic ? 1 : 0);
            return MyWin32.CreateFontIndirect(ref logFont);
        }
    }

    public class NativeTextWin32
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