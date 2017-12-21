namespace System.Drawing
{
    using ImageTools.IO.Jpeg;
    using ImageTools.IO.Png;
    using ImageTools.IO.Gif;
    using System.Drawing.Imaging;
    using System.IO;

    public class Bitmap : Image
    {
        int _width, _height;
        double _vResolution, _hResolution;

        byte[] buffer;
        public Bitmap(int width, int height)
        {
            _width = width;
            _height = height;
        }
        internal Bitmap(int w, int h, byte[] buffer, double xRes, double yRes)
        {
            this.buffer = buffer;
            this._width = w;
            this._height = h;
            this._hResolution = xRes;
            this._vResolution = yRes;
        }
        public override int Height
        {
            get { return _height; }
        }

        public override int Width
        {
            get { return _width; }
        }

        public override double VerticalResolution
        {
            get { return _vResolution; }
        }

        public override double HorizontalResolution
        {
            get { return _hResolution; }
        }

        public override void Dispose()
        {
            //TODO: dispose the unmanaged resource
        }
        public override void Save(Stream filename, ImageFormat format)
        {
            //write raw buffer***
            filename.Write(buffer, 0, buffer.Length);
        }
    }


    public enum FontStyle
    {
        Normal,
        Bold,
        Italic,

    }
    public class Font
    {


        public Font(string family, float emSize, FontStyle style, GraphicsUnit unit)
        {
            this.Name = family;
            this.Size = emSize;
            this.Style = style;
            this.Unit = unit;
            this.FontFamily = new FontFamily(family);
        }
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
        {
            this.FontFamily = family;
            this.Size = emSize;
            this.Style = style;
            this.Unit = unit;
            this.FontFamily = family;
        }
        public string OriginalFontName { get; set; }
        public string SystemFontName { get; set; }
        public string Name { get; set; }
        public GraphicsUnit Unit { get; set; }
        public FontFamily FontFamily { get; set; }
        public float Size { get; }
        public IntPtr ToHfont()
        {
            return IntPtr.Zero;
        }
        public bool Bold { get; set; }
        public bool Strikeout { get; set; }
        public bool Underline { get; set; }
        public FontStyle Style { get; set; }
        public bool Italic { get; set; }
    }

    public class FontFamily
    {
        public FontFamily(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
    public abstract class Image : IDisposable
    {
        public abstract void Dispose();


        static bool s_ini_decoder = false;
        static void InitDecoders()
        {
            if (!s_ini_decoder)
            {

                s_ini_decoder = true;
            }
        }
        public static Image FromFile(string filename)
        {
            //TODO: review here
            //should not depend on the extension
            string fileext = IO.Path.GetExtension(filename).ToLower();

            switch (fileext)
            {
                default:
                    throw new NotSupportedException();
                case ".jpg":
                    {
                        JpegDecoder jpegDec = new JpegDecoder();
                        ImageTools.ExtendedImage outputImg = new ImageTools.ExtendedImage();
                        using (System.IO.FileStream fs = new IO.FileStream(filename, IO.FileMode.Open))
                        {
                            jpegDec.Decode(outputImg, fs);
                        }
                        //return bitmap
                        return new Bitmap(outputImg.PixelWidth, outputImg.PixelHeight, outputImg.Pixels,
                            outputImg.DensityXInt32, outputImg.DensityYInt32);

                    }
                case ".gif":
                    {

                        GifDecoder gifDec = new GifDecoder();
                        ImageTools.ExtendedImage outputImg = new ImageTools.ExtendedImage();
                        using (System.IO.FileStream fs = new IO.FileStream(filename, IO.FileMode.Open))
                        {
                            gifDec.Decode(outputImg, fs);
                        }
                        //return bitmap
                        return new Bitmap(outputImg.PixelWidth, outputImg.PixelHeight, outputImg.Pixels,
                            outputImg.DensityXInt32, outputImg.DensityYInt32);

                    }

                case ".png":
                    {
                        HjgPngDecoder pngDecoder = new HjgPngDecoder();
                        //PngDecoder pngDecoder = new PngDecoder();
                        ImageTools.ExtendedImage outputImg = new ImageTools.ExtendedImage();
                        using (System.IO.FileStream fs = new IO.FileStream(filename, IO.FileMode.Open))
                        {
                            pngDecoder.Decode(outputImg, fs);
                        }

                        Bitmap bmp = new Bitmap(outputImg.PixelWidth,
                            outputImg.PixelHeight, outputImg.Pixels,
                            outputImg.DensityXInt32, outputImg.DensityYInt32);
                        bmp.PixelFormat = Imaging.PixelFormat.Format32bppArgb;
                        return bmp;

                    }
            }
            return null;
        }
        public static Image FromStream(System.IO.Stream stream)
        {
            throw new NotSupportedException();
        }
        public Imaging.RawFormat RawFormat { get; set; }
        public abstract int Height { get; }
        public abstract int Width { get; }
        public abstract double VerticalResolution { get; }
        public abstract double HorizontalResolution { get; }
        public Imaging.PixelFormat PixelFormat { get; set; }
        public virtual void Save(string filename)
        {
        }
        public virtual void Save(string filename, Imaging.ImageFormat format)
        {
        }
        public virtual void Save(System.IO.Stream filename, Imaging.ImageFormat format)
        {
        }
        public int Flags { get; set; }
    }
    public enum GraphicsUnit
    {
        World
    }
    namespace Imaging
    {
        public class RawFormat
        {
            public Guid Guid { get; private set; }

        }
        public class ImageFormat
        {
            public static ImageFormat Png { get; set; }
            public static ImageFormat Jpeg { get; set; }
            public static ImageFormat Bmp { get; set; }
        }
        public enum ImageFlags
        {
            ColorSpaceYcck,
            ColorSpaceGray,
            ColorSpaceCmyk,

        }
        public enum PixelFormat
        {
            Format1bppIndexed,
            Format4bppIndexed,
            Format8bppIndexed,
            Format24bppRgb,
            Format32bppRgb,
            Format32bppArgb,
            Format32bppPArgb
        }
    }
    
}
