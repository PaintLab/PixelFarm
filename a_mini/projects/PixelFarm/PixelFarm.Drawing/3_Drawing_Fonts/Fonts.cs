//MIT, 2014-2016, WinterDev

using System;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing
{
    public sealed class Font : IDisposable
    {
        
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
        public ActualFont InnerFont
        {
            get { return _actualFont; }
        }
        public ActualFont NativeFont
        {
            get { return _nativeFont; }
        }
        public ActualFont OutlineFont
        {
            get { return _outlineFont; }
        }

        public void SetOutlineFont(OutlineFont outlineFont)
        {
            _outlineFont = outlineFont;
            if (_actualFont == null)
            {
                _actualFont = outlineFont;
            }
        }
        public void SetTextureFont(TextureFont textureFont)
        {
            _textureFont = textureFont;
            if (_actualFont == null)
            {
                _actualFont = textureFont;
            }
        }
        public void SetPlatformFont(PlatformFont platformFont)
        {
            _platformFont = platformFont;
            if (_actualFont == null)
            {
                _actualFont = platformFont;
            }
        }
        internal void SetNativeFont(NativeFont nativeFont)
        {
            _nativeFont = nativeFont;
            if (_actualFont == null)
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