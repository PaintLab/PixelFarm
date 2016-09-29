//MIT, 2014-2016, WinterDev

using System;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing
{

    public sealed class Font : IDisposable
    {
        //--------------------------
        //in our lib
        //1 font may has more than 1 actual impl
        //--------------------------
        ActualFont _actualFont;
        //--------------------------
        NativeFont _nativeFont;
        OutlineFont _outlineFont;
        PlatformFont _platformFont;
        TextureFont _textureFont;
        //--------------------------


        public string FileName { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// emheight in point unit
        /// </summary>
        public float EmSize { get; set; }
        public float EmSizeInPixels { get; set; }
        public FontStyle Style { get; set; }
        public FontFace FontFace { get; set; }

        /// <summary>
        /// canvas specific presentation
        /// </summary>
        public ActualFont ActualFont
        {
            get { return _actualFont; }
        }
        public NativeFont NativeFont
        {
            get { return _nativeFont; }
        }
        public OutlineFont OutlineFont
        {
            get { return _outlineFont; }
        }
        public PlatformFont PlatformFont
        {
            get { return _platformFont; }
        }
        public TextureFont TextureFont
        {
            get { return _textureFont; }
        }
        //--------------------------
        public void SetOutlineFont(OutlineFont outlineFont, bool forceSetToPrimaryActualFont = false)
        {
            _outlineFont = outlineFont;
            if (_actualFont == null || forceSetToPrimaryActualFont)
            {
                _actualFont = outlineFont;
            }

        }
        public void SetTextureFont(TextureFont textureFont, bool forceSetToPrimaryActualFont = false)
        {
            _textureFont = textureFont;
            if (_actualFont == null || forceSetToPrimaryActualFont)
            {
                _actualFont = textureFont;
            }
        }
        public void SetPlatformFont(PlatformFont platformFont, bool forceSetToPrimaryActualFont = false)
        {
            _platformFont = platformFont;
            if (_actualFont == null || forceSetToPrimaryActualFont)
            {
                _actualFont = platformFont;
            }
        }
        public void SetNativeFont(NativeFont nativeFont, bool forceSetToPrimaryActualFont = false)
        {
            _nativeFont = nativeFont;
            if (_actualFont == null || forceSetToPrimaryActualFont)
            {
                _actualFont = nativeFont;
            }
        }
        public void Dispose()
        {
        }
    }





    public interface IFonts
    {
        Font GetFont(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f);
        Size MeasureString(char[] str, int startAt, int len, Font font);
        Size MeasureString(char[] str, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth);
        void Dispose();
    }


    public abstract class StringFormat
    {
        public abstract object InnerFormat { get; }
    }


}