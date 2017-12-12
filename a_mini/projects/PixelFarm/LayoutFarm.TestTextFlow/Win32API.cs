using System;
using System.Runtime.InteropServices;

using LayoutFarm.Text;
namespace Win32
{
    public class NativeWin32MemoryDc : IDisposable
    {
        int _width;
        int _height;
        IntPtr memHdc;
        IntPtr dib;
        IntPtr ppvBits;
        IntPtr hRgn = IntPtr.Zero;

        bool isDisposed;
        public NativeWin32MemoryDc(int w, int h, bool invertImage = false)
        {
            this._width = w;
            this._height = h;

            memHdc = MyWin32.CreateMemoryHdc(
                IntPtr.Zero,
                w,
                invertImage ? -h : h, //***
                out dib,
                out ppvBits);
        }
        public IntPtr DC
        {
            get { return this.memHdc; }
        }
        public IntPtr PPVBits
        {
            get { return this.ppvBits; }
        }
        public void SetTextColor(int win32Color)
        {
            Win32.MyWin32.SetTextColor(memHdc, win32Color);
        }
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            if (hRgn != IntPtr.Zero)
            {
                MyWin32.DeleteObject(hRgn);
                hRgn = IntPtr.Zero;
            }


            MyWin32.ReleaseMemoryHdc(memHdc, dib);
            dib = IntPtr.Zero;
            memHdc = IntPtr.Zero;
            isDisposed = true;
        }
        public void PatBlt(PatBltColor color)
        {
            MyWin32.PatBlt(memHdc, 0, 0, _width, _height, (int)color);
        }
        public void SetBackTransparent(bool value)
        {
            //public const int _SetBkMode_TRANSPARENT = 1;
            //public const int _SetBkMode_OPAQUE = 2;
            MyWin32.SetBkMode(memHdc, value ? 1 : 2);
        }
        public enum PatBltColor
        {
            Black = MyWin32.BLACKNESS,
            White = MyWin32.WHITENESS
        }
        public IntPtr SetFont(IntPtr hFont)
        {
            return MyWin32.SelectObject(memHdc, hFont);
        }
        /// <summary>
        /// set solid text color
        /// </summary>
        /// <param name="r">0-255</param>
        /// <param name="g">0-255</param>
        /// <param name="b">0-255</param>
        public void SetSolidTextColor(byte r, byte g, byte b)
        {
            //convert to win32 colorv
            MyWin32.SetTextColor(memHdc, (b & 0xFF) << 16 | (g & 0xFF) << 8 | r);
        }
        public void SetClipRect(Rectangle r)
        {
            SetClipRect(r.Left, r.Top, r.Width, r.Height);
        }
        public void SetClipRect(int x, int y, int w, int h)
        {
            if (hRgn == IntPtr.Zero)
            {
                //create
                hRgn = MyWin32.CreateRectRgn(0, 0, w, h);
            }
            MyWin32.SetRectRgn(hRgn,
            x,
            y,
            x + w,
            y + h);
            MyWin32.SelectObject(memHdc, hRgn);
        }
        public void ClearClipRect()
        {
            MyWin32.SelectClipRgn(memHdc, IntPtr.Zero);

        }
    }
    public struct Rectangle
    {
        private int x, y, width, height;
        /// <summary>
        ///	Empty Shared Field
        /// </summary>
        ///
        /// <remarks>
        ///	An uninitialized Rectangle Structure.
        /// </remarks>

        public static readonly Rectangle Empty;

        /// <summary>
        ///	FromLTRB Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Rectangle structure from left, top, right,
        ///	and bottom coordinates.
        /// </remarks>

        public static Rectangle FromLTRB(int left, int top,
                          int right, int bottom)
        {
            return new Rectangle(left, top, right - left,
                          bottom - top);
        }

        /// <summary>
        ///	Inflate Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new Rectangle by inflating an existing 
        ///	Rectangle by the specified coordinate values.
        /// </remarks>

        public static Rectangle Inflate(Rectangle rect, int x, int y)
        {
            Rectangle r = new Rectangle(rect.Location, rect.Size);
            r.Inflate(x, y);
            return r;
        }

        /// <summary>
        ///	Inflate Method
        /// </summary>
        ///
        /// <remarks>
        ///	Inflates the Rectangle by a specified width and height.
        /// </remarks>

        public void Inflate(int width, int height)
        {
            Inflate(new Size(width, height));
        }

        /// <summary>
        ///	Inflate Method
        /// </summary>
        ///
        /// <remarks>
        ///	Inflates the Rectangle by a specified Size.
        /// </remarks>

        public void Inflate(Size size)
        {
            x -= size.Width;
            y -= size.Height;
            Width += size.Width * 2;
            Height += size.Height * 2;
        }

        /// <summary>
        ///	Intersect Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new Rectangle by intersecting 2 existing 
        ///	Rectangles. Returns null if there is no	intersection.
        /// </remarks>

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return Empty;
            return Rectangle.FromLTRB(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom));
        }

        /// <summary>
        ///	Intersect Method
        /// </summary>
        ///
        /// <remarks>
        ///	Replaces the Rectangle with the intersection of itself
        ///	and another Rectangle.
        /// </remarks>

        public void Intersect(Rectangle rect)
        {
            this = Rectangle.Intersect(this, rect);
        }

        ///// <summary>
        /////	Round Shared Method
        ///// </summary>
        /////
        ///// <remarks>
        /////	Produces a Rectangle structure from a RectangleF by
        /////	rounding the X, Y, Width, and Height properties.
        ///// </remarks>

        //public static Rectangle Round(RectangleF value)
        //{
        //    int x, y, w, h;
        //    checked
        //    {
        //        x = (int)Math.Round(value.X);
        //        y = (int)Math.Round(value.Y);
        //        w = (int)Math.Round(value.Width);
        //        h = (int)Math.Round(value.Height);
        //    }

        //    return new Rectangle(x, y, w, h);
        //}

        /// <summary>
        ///	Truncate Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Rectangle structure from a RectangleF by
        ///	truncating the X, Y, Width, and Height properties.
        /// </remarks>

        // LAMESPEC: Should this be floor, or a pure cast to int?

        //public static Rectangle Truncate(RectangleF value)
        //{
        //    int x, y, w, h;
        //    checked
        //    {
        //        x = (int)value.X;
        //        y = (int)value.Y;
        //        w = (int)value.Width;
        //        h = (int)value.Height;
        //    }

        //    return new Rectangle(x, y, w, h);
        //}

        /// <summary>
        ///	Union Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new Rectangle from the union of 2 existing 
        ///	Rectangles.
        /// </remarks>

        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            return FromLTRB(Math.Min(a.Left, b.Left),
                     Math.Min(a.Top, b.Top),
                     Math.Max(a.Right, b.Right),
                     Math.Max(a.Bottom, b.Bottom));
        }

        /// <summary>
        ///	Equality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Rectangle objects. The return value is
        ///	based on the equivalence of the Location and Size 
        ///	properties of the two Rectangles.
        /// </remarks>

        //public static bool operator ==(Rectangle left, Rectangle right)
        //{
        //    return ((left.Location == right.Location) &&
        //        (left.Size == right.Size));
        //}

        /// <summary>
        ///	Inequality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Rectangle objects. The return value is
        ///	based on the equivalence of the Location and Size 
        ///	properties of the two Rectangles.
        /// </remarks>

        //public static bool operator !=(Rectangle left, Rectangle right)
        //{
        //    return ((left.Location != right.Location) ||
        //        (left.Size != right.Size));
        //}


        // -----------------------
        // Public Constructors
        // -----------------------

        /// <summary>
        ///	Rectangle Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a Rectangle from Point and Size values.
        /// </remarks>

        public Rectangle(Point location, Size size)
        {
            x = location.X;
            y = location.Y;
            width = size.Width;
            height = size.Height;
        }

        /// <summary>
        ///	Rectangle Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a Rectangle from a specified x,y location and
        ///	width and height values.
        /// </remarks>

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }



        /// <summary>
        ///	Bottom Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the bottom edge of the Rectangle.
        ///	Read only.
        /// </remarks>


        public int Bottom
        {
            get
            {
                return y + height;
            }
        }

        /// <summary>
        ///	Height Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Height of the Rectangle.
        /// </remarks>

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        ///	IsEmpty Property
        /// </summary>
        ///
        /// <remarks>
        ///	Indicates if the width or height are zero. Read only.
        /// </remarks>		

        public bool IsEmpty
        {
            get
            {
                return ((x == 0) && (y == 0) && (width == 0) && (height == 0));
            }
        }

        /// <summary>
        ///	Left Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the left edge of the Rectangle.
        ///	Read only.
        /// </remarks>

        public int Left
        {
            get
            {
                return X;
            }
        }

        /// <summary>
        ///	Location Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Location of the top-left corner of the Rectangle.
        /// </remarks>


        public Point Location
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }

        /// <summary>
        ///	Right Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the right edge of the Rectangle.
        ///	Read only.
        /// </remarks>


        public int Right
        {
            get
            {
                return X + Width;
            }
        }

        /// <summary>
        ///	Size Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Size of the Rectangle.
        /// </remarks>


        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        ///	Top Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the top edge of the Rectangle.
        ///	Read only.
        /// </remarks>


        public int Top
        {
            get
            {
                return y;
            }
        }

        /// <summary>
        ///	Width Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Width of the Rectangle.
        /// </remarks>

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        ///	X Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the Rectangle.
        /// </remarks>

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        /// <summary>
        ///	Y Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the Rectangle.
        /// </remarks>

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if an x,y coordinate lies within this Rectangle.
        /// </remarks>

        public bool Contains(int x, int y)
        {
            return ((x >= Left) && (x < Right) &&
                (y >= Top) && (y < Bottom));
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Point lies within this Rectangle.
        /// </remarks>

        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Rectangle lies entirely within this 
        ///	Rectangle.
        /// </remarks>

        //public bool Contains(Rectangle rect)
        //{
        //    return (rect == Intersect(this, rect));
        //}

        /// <summary>
        ///	Equals Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks equivalence of this Rectangle and another object.
        /// </remarks>

        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Rectangle))
        //        return false;
        //    return (this == (Rectangle)obj);
        //}

        /// <summary>
        ///	GetHashCode Method
        /// </summary>
        ///
        /// <remarks>
        ///	Calculates a hashing value.
        /// </remarks>

        public override int GetHashCode()
        {
            return (height + width) ^ x + y;
        }

        /// <summary>
        ///	IntersectsWith Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Rectangle intersects with this one.
        /// </remarks>

        public bool IntersectsWith(Rectangle rect)
        {
            return !((Left >= rect.Right) || (Right <= rect.Left) ||
                (Top >= rect.Bottom) || (Bottom <= rect.Top));
        }
        public bool IntersectsWith(int left, int top, int right, int bottom)
        {
            if (((this.Left <= left) && (this.Right > left)) || ((this.Left >= left) && (this.Left < right)))
            {
                if (((this.Top <= top) && (this.Bottom > top)) || ((this.Top >= top) && (this.Top < bottom)))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IntersectsWithInclusive(Rectangle r)
        {
            return !((Left > r.Right) || (Right < r.Left) ||
                (Top > r.Bottom) || (Bottom < r.Top));
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the Rectangle a specified distance.
        /// </remarks>

        public void Offset(int x, int y)
        {
            this.x += x;
            this.y += y;
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the Rectangle a specified distance.
        /// </remarks>

        public void Offset(Point pos)
        {
            x += pos.X;
            y += pos.Y;
        }
        public void OffsetX(int dx)
        {
            x += dx;
        }
        public void OffsetY(int dy)
        {
            y += dy;
        }
        /// <summary>
        ///	ToString Method
        /// </summary>
        ///
        /// <remarks>
        ///	Formats the Rectangle as a string in (x,y,w,h) notation.
        /// </remarks>

        public override string ToString()
        {
            return String.Format("{{X={0},Y={1},Width={2},Height={3}}}",
                         x, y, width, height);
        }
    }
    public class NativeTextWin32
    {

        const string GDI32 = "gdi32.dll";
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
        public static extern bool GetTextExtentPoint32(IntPtr hdc, char[] charBuffer, int c, out WIN32SIZE size);
        [DllImport(GDI32, EntryPoint = "GetTextExtentPoint32", CharSet = CharSet.Unicode)]
        public static unsafe extern bool GetTextExtentPoint32Char(IntPtr hdc, char* ch, int c, out WIN32SIZE size);
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
        [DllImport(GDI32)]
        public static unsafe extern int GetOutlineTextMetrics(IntPtr hdc, uint cbData, uint uLastChar, void* lp_outlineTextMatrix);

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
        public static unsafe extern int
            GetCharacterPlacement(IntPtr hdc, char* str, int nCount,
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
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
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
            RECT otmrcFontBox;
            int otmMacAscent;
            int otmMacDescent;
            uint otmMacLineGap;
            uint otmusMinimumPPEM;
            POINT otmptSubscriptSize;
            POINT otmptSubscriptOffset;
            POINT otmptSuperscriptSize;
            POINT otmptSuperscriptOffset;
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

        [StructLayout(LayoutKind.Sequential)]
        public struct WIN32SIZE
        {
            public int Width;
            public int Height;
            public WIN32SIZE(int w, int h)
            {
                this.Width = w;
                this.Height = h;
            }
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
