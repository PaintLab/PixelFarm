//MIT, 2014-present, WinterDev

using System;

using System.IO;
using System.Collections.Generic;
using System.Text;

using Typography.FontCollections;
using Typography.OpenFont;
using Typography.OpenFont.Contours;
using Typography.OpenFont.Extensions;
using Typography.Text;
using Typography.TextLayout;

namespace PixelFarm.Drawing.WinGdi
{
    using PixelFarm.Drawing;
    using Win32;

    static class WinGdiTextService
    {
        //TODO: consider use uniscribe 
        static NativeWin32MemoryDC s_win32MemDc;
        //=====================================
        //static 
        static readonly int[] s_charFit = new int[1];
        static readonly int[] s_charFitWidth = new int[1000];

        static float s_whitespaceSize = -1;
        static char[] s_whitespace = new char[1];
        static Encoding s_en;

        static WinGdiTextService()
        {
            s_en = Encoding.Default; //use platform's default encoding
            s_win32MemDc = new NativeWin32MemoryDC(2, 2);
            s_whitespace[0] = ' ';

        }
        public static void SetDefaultEncoding(Encoding encoding)
        {
            s_en = encoding;
        }

        const int MAX_CODEPOINT_NO = 255;
        internal static void MeasureCharWidths(IntPtr hFont,
            out int[] charWidths,
            out NativeTextWin32.FontABC[] abcSizes)
        {

            //only in ascii range
            //current version
            charWidths = new int[MAX_CODEPOINT_NO + 1]; // 
            MyWin32.SelectObject(s_win32MemDc.DC, hFont);
            unsafe
            {
                //see: https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx
                //A code page contains 256 code points and is zero-based.
                //In most code pages, code points 0 through 127 represent the ASCII character set,
                //and code points 128 through 255 differ significantly between code pages
                abcSizes = new NativeTextWin32.FontABC[MAX_CODEPOINT_NO + 1];
                fixed (NativeTextWin32.FontABC* abc = abcSizes)
                {
                    NativeTextWin32.GetCharABCWidths(s_win32MemDc.DC, (uint)0, (uint)MAX_CODEPOINT_NO, abc);
                }
                for (int i = 0; i < (MAX_CODEPOINT_NO + 1); ++i)
                {
                    charWidths[i] = abcSizes[i].Sum;
                }

            }
        }
        public static float MeasureWhitespace(RequestFont f)
        {
            return s_whitespaceSize = MeasureString(s_whitespace, 0, 1, f).Width;
        }
        static WinGdiFont SetFont(RequestFont font)
        {
            WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(font);
            MyWin32.SelectObject(s_win32MemDc.DC, winFont.CachedHFont());
            return winFont;
        }
        /// <summary>
        /// measure blank line height in px
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static int MeasureBlankLineHeight(RequestFont font)
        {
            WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(font);
            return (int)System.Math.Ceiling(winFont.RecommendedLineSpacingInPixels);
        }
        public static PixelFarm.Drawing.Size MeasureString(char[] buff, int startAt, int len, RequestFont font)
        {

            SetFont(font);
            Win32.Size win32_size = new Win32.Size();
            if (buff.Length > 0)
            {
                unsafe
                {
                    fixed (char* startAddr = &buff[0])
                    {
                        NativeTextWin32.UnsafeGetTextExtentPoint32(s_win32MemDc.DC, startAddr + startAt, len, ref win32_size);
                    }
                }
            }
            return new PixelFarm.Drawing.Size(win32_size.W, win32_size.H);

        }
        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.<br/>
        /// Restrict the width of the string and get the number of characters able to fit in the restriction and
        /// the width those characters take.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <param name="maxWidth">the max width to render the string in</param>
        /// <param name="charFit">the number of characters that will fit under <see cref="maxWidth"/> restriction</param>
        /// <param name="charFitWidth"></param>
        /// <returns>the size of the string</returns>
        public static PixelFarm.Drawing.Size MeasureString(
            char[] buff, int startAt, int len, RequestFont font,
            float maxWidth, out int charFit, out int charFitWidth)
        {

            SetFont(font);
            if (buff.Length == 0)
            {
                charFit = 0;
                charFitWidth = 0;
                return PixelFarm.Drawing.Size.Empty;
            }

            var size = new Win32.Size(); //win32
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    NativeTextWin32.UnsafeGetTextExtentExPoint(
                        s_win32MemDc.DC, startAddr + startAt, len,
                        (int)Math.Round(maxWidth), s_charFit, s_charFitWidth, ref size);
                }
            }
            charFit = s_charFit[0];
            charFitWidth = charFit > 0 ? s_charFitWidth[charFit - 1] : 0;
            return new PixelFarm.Drawing.Size(size.W, size.H);
            //}
        }
        //==============================================

        public static void CalculateGlyphAdvancePos(in TextBufferSpan textBufferSpan,
                RequestFont font, int[] outputGlyphAdvances, out int outputTotalW)
        {


            unsafe
            {
                outputTotalW = 0;
                int len = textBufferSpan.len;
                if (len == 0)
                {
                    return;
                }

                WinGdiFont winfont = SetFont(font);
                //ushort* glyhIndics = stackalloc ushort[len];
                //fixed (char* s = &str[startAt])
                //{
                //    NativeTextWin32.GetGlyphIndices(
                //        win32MemDc.DC,
                //        s,
                //        len,
                //        glyhIndics,
                //        0);
                //}


                byte[] encoding = s_en.GetBytes(
                    textBufferSpan.GetRawUtf16Buffer(),
                    textBufferSpan.start,
                    len);

                NativeTextWin32.FontABC[] abcWidths = winfont.GetInteralABCArray();
                int totalW = 0;
                for (int i = 0; i < len; ++i)
                {
                    //ushort glyphIndex = *(glyhIndics + i);
                    int enc_index = encoding[i];
                    if (enc_index == 0)
                    {
                        break;//?
                    }
                    totalW += outputGlyphAdvances[i] = abcWidths[enc_index].Sum;
                }
                outputTotalW = totalW;
            }
            //unsafe
            //{
            //    SetFont(font);
            //    NativeTextWin32.GCP_RESULTS gpcResult = new NativeTextWin32.GCP_RESULTS();
            //    int[] caretpos = new int[len];
            //    uint[] lpOrder = new uint[len];
            //    int[] lpDx = new int[len];
            //    fixed (int* lpdx_h = &lpDx[0])
            //    fixed (uint* lpOrder_h = &lpOrder[0])
            //    fixed (int* caretpos_h = &caretpos[0])
            //    fixed (char* str_h = &str[startAt])
            //    {
            //        gpcResult.lpCaretPos = caretpos_h;
            //        gpcResult.lpOrder = lpOrder_h;
            //        gpcResult.lpDx = lpdx_h;
            //        //gpcResult.
            //        ////gpcResult.lpCaretPos = 
            //        NativeTextWin32.GetCharacterPlacement(
            //            win32MemDc.DC,
            //            str_h,
            //            len,
            //            len, ref gpcResult, 0);

            //    }

            //}

        }
        public static ActualFont GetWinGdiFont(RequestFont f)
        {
            return WinGdiFontSystem.GetWinGdiFont(f);
        }
    }
    class WinGdiFont : ActualFont
    {
        /// <summary>
        /// font 'em' height?
        /// </summary>
        float _fontSizeInPoints;
        float _emSizeInPixels;
        float _ascendInPixels;
        float _descentInPixels;
        float _linegapInPixels;
        float _recommenedLineHeight;

        WinGdiFontFace _fontFace;
        int[] _charWidths;
        NativeTextWin32.FontABC[] _charAbcWidths;

        IntPtr _hfont;
        OldFontStyle _fontStyle;
        public WinGdiFont(WinGdiFontFace fontFace, float sizeInPoints, OldFontStyle style)
        {

            _fontFace = fontFace;
            _fontSizeInPoints = sizeInPoints;
            _fontStyle = style;

            _fontSizeInPoints = sizeInPoints;
            _emSizeInPixels = PixelFarm.Drawing.Len.Pt(_fontSizeInPoints).ToPixels();
            _hfont = InitFont(fontFace.Name, sizeInPoints, style);
            //------------------------------------------------------------------
            //create gdi font from font data
            //build font matrix             ;
            WinGdiTextService.MeasureCharWidths(_hfont, out _charWidths, out _charAbcWidths);
            float scale = fontFace.GetScale(sizeInPoints);
            _ascendInPixels = fontFace.AscentInDzUnit * scale;
            _descentInPixels = fontFace.DescentInDzUnit * scale;
            _linegapInPixels = fontFace.LineGapInDzUnit * scale;
            _recommenedLineHeight = fontFace.RecommendedLineHeight * scale;
            //------------------------------------------------------------------


            //int emHeightInDzUnit = f.FontFamily.GetEmHeight(f.Style);
            //this.ascendInPixels = Font.ConvEmSizeInPointsToPixels((f.FontFamily.GetCellAscent(f.Style) / emHeightInDzUnit));
            //this.descentInPixels = Font.ConvEmSizeInPointsToPixels((f.FontFamily.GetCellDescent(f.Style) / emHeightInDzUnit));

            ////--------------
            ////we build font glyph, this is just win32 glyph
            ////
            //int j = charAbcWidths.Length;
            //fontGlyphs = new FontGlyph[j];
            //for (int i = 0; i < j; ++i)
            //{
            //    FontGlyph glyph = new FontGlyph();
            //    glyph.horiz_adv_x = charWidths[i] << 6;
            //    fontGlyphs[i] = glyph;
            //}

        }
        protected override void OnDispose()
        {

            //TODO: review here 
            MyWin32.DeleteObject(_hfont);
            _hfont = IntPtr.Zero;

        }
        //
        public override string FontName => _fontFace.Name;
        public override OldFontStyle FontStyle => _fontStyle;
        //
        static IntPtr InitFont(string fontName, float emHeight, OldFontStyle style)
        {
            //see: MSDN, LOGFONT structure
            //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
            MyWin32.LOGFONT logFont = new MyWin32.LOGFONT();
            MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = -(int)PixelFarm.Drawing.Len.Pt(emHeight).ToPixels();//minus **
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default

            MyWin32.LOGFONT_FontWeight weight =
                ((style & OldFontStyle.Bold) == OldFontStyle.Bold) ?
                MyWin32.LOGFONT_FontWeight.FW_BOLD :
                MyWin32.LOGFONT_FontWeight.FW_REGULAR;
            logFont.lfWeight = (int)weight;
            //
            logFont.lfItalic = (byte)(((style & OldFontStyle.Italic) == OldFontStyle.Italic) ? 1 : 0);
            return MyWin32.CreateFontIndirect(ref logFont);
        }
        //
        public System.IntPtr CachedHFont() => _hfont;
        public override float SizeInPoints => _fontSizeInPoints;
        public override float SizeInPixels => _emSizeInPixels;
        //

        internal NativeTextWin32.FontABC[] GetInteralABCArray() => _charAbcWidths;
        //
        public override FontGlyph GetGlyphByIndex(ushort glyphIndex)
        {
            throw new NotImplementedException();
        }
        public override FontGlyph GetGlyph(char c)
        {
            //convert c to glyph index
            //temp fix  
            throw new NotImplementedException();
        }
        public override FontFace FontFace
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override float AscentInPixels => _ascendInPixels;
        public override float LineGapInPixels => _linegapInPixels;
        public override float RecommendedLineSpacingInPixels => _recommenedLineHeight;
        public override float DescentInPixels => _descentInPixels;
        public void AssignToRequestFont(RequestFont r)
        {
            SetCacheActualFont(r, this);
        }
        public static WinGdiFont GetCacheFontAsWinGdiFont(RequestFont r)
        {
            return GetCacheActualFont(r) as WinGdiFont;
        }
    }

    class Gdi32TextService
    {
        public Gdi32TextService()
        {
        }

        public float MeasureWhitespace(RequestFont f)
        {
            return WinGdiTextService.MeasureWhitespace(f);
        }
        public PixelFarm.Drawing.Size MeasureString(in TextBufferSpan textBufferSpan, RequestFont font)
        {
            //TODO: review here*** 
            return WinGdiTextService.MeasureString(textBufferSpan.GetRawUtf16Buffer(), textBufferSpan.start, textBufferSpan.len, font);
        }
        public void MeasureString(in TextBufferSpan textBufferSpan, RequestFont font, int maxWidth, out int charFit, out int charFitWidth)
        {
            //TODO: review here***
            WinGdiTextService.MeasureString(textBufferSpan.GetRawUtf16Buffer(), textBufferSpan.start, textBufferSpan.len, font, maxWidth, out charFit, out charFitWidth);
        }

        public void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan,
            RequestFont font, ref TextSpanMeasureResult measureResult)
        {
            WinGdiTextService.CalculateGlyphAdvancePos(textBufferSpan, font, measureResult.outputXAdvances, out measureResult.outputTotalW);
            measureResult.lineHeight = (ushort)WinGdiTextService.MeasureBlankLineHeight(font);
        }

        public float MeasureBlankLineHeight(RequestFont f) => WinGdiTextService.MeasureBlankLineHeight(f);


        public void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan,
            ILineSegmentList lineSegs,
            RequestFont font,
            ref TextSpanMeasureResult measureResult)
        {
            throw new NotImplementedException();
        }

        public bool SupportsWordBreak => false;

    }



    class WinGdiFontSystem
    {

        struct FontFaceKey
        {
            //internal use only
            public readonly int FontNameIndex;
            public FontFaceKey(int fontNameIndex)
            {
                this.FontNameIndex = fontNameIndex;
            }

        }
        static RequestFont s_latestFont;
        static WinGdiFont s_latestWinFont;
        static Dictionary<int, WinGdiFont> s_registerFonts = new Dictionary<int, WinGdiFont>();
        static Dictionary<FontFaceKey, WinGdiFontFace> s_winGdiFonFaces = new Dictionary<FontFaceKey, WinGdiFontFace>();

        public static WinGdiFont GetWinGdiFont(RequestFont f)
        {
            if (f == null)
            {
                throw new NotSupportedException();
            }
            if (f == s_latestFont)
            {
                return s_latestWinFont;
            }
            WinGdiFont actualFontInside = WinGdiFont.GetCacheFontAsWinGdiFont(f);
            if (actualFontInside != null)
            {
                return actualFontInside;
            }
            //-----
            //need to create a new one
            //get register font or create the new one
            int key = f.GetReqKey();
            if (!s_registerFonts.TryGetValue(key, out WinGdiFont found))
            {
                //create the new one and register                  
                //create fontface
                FontFaceKey fontfaceKey = new FontFaceKey(key);
                WinGdiFontFace fontface;
                if (!s_winGdiFonFaces.TryGetValue(fontfaceKey, out fontface))
                {
                    //create new 
                    fontface = new WinGdiFontFace(f);
                    s_winGdiFonFaces.Add(fontfaceKey, fontface);
                }

                found = (WinGdiFont)fontface.GetFontAtPointSize(f.SizeInPoints);
                s_registerFonts.Add(key, found);//cache here
            }
            s_latestFont = f;
            found.AssignToRequestFont(f);
            return s_latestWinFont = found;
        }
    }
    class WinGdiFontFace : FontFace
    {
        FontFace _nopenTypeFontFace;
        OldFontStyle _style;
        static IInstalledTypefaceProvider s_installedTypefaceProvider;

        public WinGdiFontFace(RequestFont f)
        {

            //resolve

            InstalledTypeface foundInstalledFont = s_installedTypefaceProvider.GetInstalledTypeface(f.Name, ConvToInstalledFontStyle(f.Style), 400);
            //TODO: review 
            if (foundInstalledFont == null)
            {
                //not found

            }
            _nopenTypeFontFace = OpenFontLoader.LoadFont(foundInstalledFont.FontPath);
        }
        static Typography.FontCollections.TypefaceStyle ConvToInstalledFontStyle(RequestFontStyle style)
        {
            Typography.FontCollections.TypefaceStyle installedStyle = Typography.FontCollections.TypefaceStyle.Regular;//regular
            switch (style)
            {
                case RequestFontStyle.Italic:
                    installedStyle = Typography.FontCollections.TypefaceStyle.Italic;
                    break;
            }
            return installedStyle;
        }
        //
        public override int RecommendedLineHeight => _nopenTypeFontFace.RecommendedLineHeight;
        //
        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider provider)
        {
            //warning if duplicate
            if (s_installedTypefaceProvider != null)
            {
                //TODO: review here again
                return;
            }
            s_installedTypefaceProvider = provider;
        }
        protected override void OnDispose()
        {
            s_installedTypefaceProvider = null;
        }
        //
        public override string FontPath => _nopenTypeFontFace.FontPath;
        //
        public override string Name => _nopenTypeFontFace.Name;
        //
        public override ActualFont GetFontAtPointSize(float pointSize) => new WinGdiFont(this, pointSize, _style);
        //
        public override float GetScale(float pointSize) => _nopenTypeFontFace.GetScale(pointSize);

        public override int AscentInDzUnit => _nopenTypeFontFace.AscentInDzUnit;
        public override int DescentInDzUnit => _nopenTypeFontFace.DescentInDzUnit;
        public override int LineGapInDzUnit => _nopenTypeFontFace.LineGapInDzUnit;
        public override object GetInternalTypeface()
        {
            throw new NotImplementedException();
        }
    }
    public static class OpenFontLoader
    {
        public static FontFace LoadFont(Typeface typeface)
        {
            //read font file 
            //TODO:...
            //set shape engine ***  
            return new NOpenFontFace(typeface, typeface.Name, typeface.Filename);
        }
        public static FontFace LoadFont(string fontpath)
        {
            using (FileStream fs = new FileStream(fontpath, FileMode.Open, FileAccess.Read))
            {
                var reader = new OpenFontReader();
                Typeface t = reader.Read(fs);
                if (t == null)
                {
                    return null;
                }

                t.Filename = fontpath;
                return LoadFont(t);
            }
        }
    }

    class NOpenFontFace : FontFace
    {
        readonly string _name;
        readonly string _path;
        Typeface _typeface;

        //TODO: review again,
        //remove ...
        GlyphOutlineBuilder _glyphPathBuilder;

        public NOpenFontFace(Typeface typeface, string fontName, string fontPath)
        {
            _typeface = typeface;
            _name = fontName;
            _path = fontPath;

            _glyphPathBuilder = new GlyphOutlineBuilder(typeface);
        }
        public override string Name => _name;

        public override string FontPath => _path;

        protected override void OnDispose() { }
        public override ActualFont GetFontAtPointSize(float pointSize)
        {
            return new NOpenFont(this, pointSize, OldFontStyle.Regular);
        }
        public Typeface Typeface => _typeface;

        //TODO: review again,
        //remove ...
        internal GlyphOutlineBuilder VxsBuilder => _glyphPathBuilder;

        public override float GetScale(float pointSize) => _typeface.CalculateScaleToPixelFromPointSize(pointSize);

        public override int AscentInDzUnit => _typeface.Ascender;

        public override int DescentInDzUnit => _typeface.Descender;

        public override int LineGapInDzUnit => _typeface.LineGap;

        public override int RecommendedLineHeight => _typeface.CalculateRecommendLineSpacing();

        public override object GetInternalTypeface() => _typeface;

    }

    class NOpenFont : ActualFont
    {
        NOpenFontFace _ownerFace;
        readonly float _sizeInPoints;
        readonly OldFontStyle _style;
        readonly Typeface _typeFace;
        readonly float _scale;
        readonly Dictionary<uint, VertexStore> _glyphVxs = new Dictionary<uint, VertexStore>();

        float _recommendLineSpacing;

        public NOpenFont(NOpenFontFace ownerFace, float sizeInPoints, OldFontStyle style)
        {
            _ownerFace = ownerFace;
            _sizeInPoints = sizeInPoints;
            _style = style;
            _typeFace = ownerFace.Typeface;
            //calculate scale *** 
            _scale = _typeFace.CalculateScaleToPixelFromPointSize(sizeInPoints);
            _recommendLineSpacing = _typeFace.CalculateRecommendLineSpacing() * _scale;

        }
        public override float SizeInPoints => _sizeInPoints;

        public override float SizeInPixels => _sizeInPoints * _scale;

        public override float AscentInPixels => _typeFace.Ascender * _scale;

        public override float DescentInPixels => _typeFace.Descender * _scale;

        public override float LineGapInPixels => _typeFace.LineGap * _scale;

        public override float RecommendedLineSpacingInPixels => _recommendLineSpacing;

        public override FontFace FontFace => _ownerFace;

        public override string FontName => _typeFace.Name;

        public override OldFontStyle FontStyle => _style;

        public override FontGlyph GetGlyph(char c) => GetGlyphByIndex(_typeFace.GetGlyphIndex(c));

        public override FontGlyph GetGlyphByIndex(ushort glyphIndex)
        {
            //1.  
            FontGlyph fontGlyph = new FontGlyph();
            fontGlyph.flattenVxs = GetGlyphVxs(glyphIndex);
            fontGlyph.horiz_adv_x = _typeFace.GetAdvanceWidthFromGlyphIndex(glyphIndex);

            return fontGlyph;
        }
        protected override void OnDispose()
        {

        }
        public VertexStore GetGlyphVxs(uint codepoint)
        {
            if (_glyphVxs.TryGetValue(codepoint, out VertexStore found))
            {
                return found;
            }
            //not found
            //then build it
            _ownerFace.VxsBuilder.BuildFromGlyphIndex((ushort)codepoint, _sizeInPoints);

            var txToVxs = new GlyphTranslatorToVxs();
            _ownerFace.VxsBuilder.ReadShapes(txToVxs);
            //
            //create new one
            found = new VertexStore();
            txToVxs.WriteOutput(found);
            _glyphVxs.Add(codepoint, found);
            return found;
        }

    }




}