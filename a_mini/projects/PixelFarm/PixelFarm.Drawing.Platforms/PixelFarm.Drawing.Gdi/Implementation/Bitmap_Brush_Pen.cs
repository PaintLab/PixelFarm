//MIT, 2014-2016, WinterDev   

using System;

using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.WinGdi
{
    //*** this class need System.Drawing , because 
    class WinGdiPlusFont : PlatformFont
    {
        System.Drawing.Font myFont;
        System.IntPtr hFont;
        float emSize;
        float emSizeInPixels;
        static BasicGdi32FontHelper basGdi32FontHelper = new BasicGdi32FontHelper();

        int[] charWidths;
        Win32.NativeTextWin32.FontABC[] charAbcWidths;


        public WinGdiPlusFont(System.Drawing.Font f)
        {
            this.myFont = f;
            this.hFont = f.ToHfont();
            //
            this.emSize = f.SizeInPoints;
            this.emSizeInPixels = Font.ConvEmSizeInPointsToPixels(this.emSize);
            //
            //build font matrix
            basGdi32FontHelper.MeasureCharWidths(hFont, out charWidths, out charAbcWidths);
            //--------------
        }
      
         
        public System.IntPtr ToHfont()
        {   /// <summary>
            /// Set a resource (e.g. a font) for the specified device context.
            /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
            /// </summary>
            return this.hFont;
        }
        public override float EmSize
        {
            get { return emSize; }
        }
        public override float EmSizeInPixels
        {
            get { return emSizeInPixels; }
        }

         
        protected override void OnDispose()
        {
            if (myFont != null)
            {
                myFont.Dispose();
                myFont = null;
            }
        }


        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }

        public override FontGlyph GetGlyph(char c)
        {
            throw new NotImplementedException();
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }

        public override float GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }

        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Font InnerFont
        {
            get { return this.myFont; }
        }

        public override FontFace FontFace
        {
            get
            {
                throw new NotImplementedException();
            }
        }



        public override float AscentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override float DescentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }



    }
}