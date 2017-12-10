//MIT, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing
{

    /// <summary>
    /// user request for font
    /// </summary>
    public sealed class RequestFont
    {
        //each platform/canvas has its own representation of this Font
        //actual font will be resolved by the platform.
        /// <summary>
        /// font size in points unit
        /// </summary>
        float sizeInPoints;
        int _fontKey;
        public RequestFont(string facename, float fontSizeInPts, FontStyle style = FontStyle.Regular)
        {
            //WriteDirection = WriteDirection.LTR;
            //ScriptLang = ScriptLangs.Latin;//default

            //Lang = "en";//default
            Name = facename;
            SizeInPoints = fontSizeInPts;
            Style = style;

            this._fontKey = (new InternalFontKey(facename, fontSizeInPts, style)).GetHashCode();
            //TODO: review here ***
            //temp fix 
            //we need font height*** 
            //this.Height = SizeInPixels;
        }
        public int FontKey
        {
            get { return this._fontKey; }
        }

        /// <summary>
        /// font's face name
        /// </summary>
        public string Name { get; private set; }
        public FontStyle Style { get; private set; }

        /// <summary>
        /// emheight in point unit
        /// </summary>
        public float SizeInPoints
        {
            get { return sizeInPoints; }
            private set
            {
                sizeInPoints = value;
            }
        }



        struct InternalFontKey
        {

            public readonly int FontNameIndex;
            public readonly float FontSize;
            public readonly FontStyle FontStyle;

            public InternalFontKey(string fontname, float fontSize, FontStyle fs)
            {
                //font name/ not filename
                this.FontNameIndex = RegisterFontName(fontname.ToLower());
                this.FontSize = fontSize;
                this.FontStyle = fs;
            }

            static Dictionary<string, int> registerFontNames = new Dictionary<string, int>();
            static InternalFontKey()
            {
                RegisterFontName(""); //blank font name
            }
            static int RegisterFontName(string fontName)
            {
                fontName = fontName.ToUpper();
                int found;
                if (!registerFontNames.TryGetValue(fontName, out found))
                {
                    int nameIndex = registerFontNames.Count;
                    registerFontNames.Add(fontName, nameIndex);
                    return nameIndex;
                }
                return found;
            }
            public override int GetHashCode()
            {
                return CalculateGetHasCode(this.FontNameIndex, this.FontSize, (int)this.FontStyle);
            }
            static int CalculateGetHasCode(int nameIndex, float fontSize, int fontstyle)
            {
                //modified from https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + nameIndex.GetHashCode();
                    hash = hash * 31 + fontSize.GetHashCode();
                    hash = hash * 31 + fontstyle.GetHashCode();
                    return hash;
                }
            }
        }

        static int s_POINTS_PER_INCH = 72; //default value
        static int s_PIXELS_PER_INCH = 96; //default value

        //public WriteDirection WriteDirection { get; set; }
        //public ScriptLang ScriptLang { get; set; }
        public static float ConvEmSizeInPointsToPixels(float emsizeInPoint)
        {
            return (int)(((float)emsizeInPoint / (float)s_POINTS_PER_INCH) * (float)s_PIXELS_PER_INCH);
        }
    }

}