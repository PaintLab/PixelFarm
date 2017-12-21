#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;

using PdfSharp.Pdf;


using PdfSharp.Drawing.Internal;
using PdfSharp.Internal;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;

// WPFHACK
#pragma warning disable 0169
#pragma warning disable 0649

namespace PdfSharp.Drawing
{
    [Flags]
    internal enum XImageState
    {
        UsedInDrawingContext = 0x00000001,

        StateMask = 0x0000FFFF,
    }

    /// <summary>
    /// Defines an object used to draw image files (bmp, png, jpeg, gif) and PDF forms.
    /// An abstract base class that provides functionality for the Bitmap and Metafile descended classes.
    /// </summary>
    public class XImage : IDisposable
    {
        // The hierarchy is adapted to WPF/Silverlight/WinRT
        //
        // XImage                           <-- ImageSource
        //   XForm
        //   PdfForm
        //   XBitmapSource               <-- BitmapSource
        //     XBitmapImage             <-- BitmapImage

        // ???
        //public bool Disposed
        //{
        //    get { return _disposed; }
        //    set { _disposed = value; }
        //}


        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class.
        /// </summary>
        protected XImage()
        { }

#if GDI || CORE || WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class from an image read by ImageImporter.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="System.ArgumentNullException">image</exception>
        XImage(ImportedImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            _importedImage = image;
            Initialize();
        }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class from a GDI+ image.
        /// </summary>
        XImage(Image image)
        {
            _gdiImage = image;
#if WPF  // Is defined in hybrid build.
            _wpfImage = ImageHelper.CreateBitmapSource(image);
#endif
            Initialize();
        }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class from a WPF image.
        /// </summary>
        XImage(BitmapSource image)
        {
            _wpfImage = image;
            Initialize();
        }
#endif

#if WPF
        XImage(Uri uri)
        {
            //var uriSource = new Uri(@"/WpfApplication1;component/Untitled.png", UriKind.Relative); foo.Source = new BitmapImage(uriSource);

            _wpfImage = BitmapFromUri(uri);

            //throw new NotImplementedException("XImage from Uri.");
            // WPF
            //Image finalImage = new Image();
            //finalImage.Width = 80;
            //...BitmapImage logo = new BitmapImage()
            //logo.BeginInit();logo.UriSource = new Uri("pack://application:,,,/ApplicationName;component/Resources/logo.png");
            //logo.EndInit();
            //...finalImage.Source = logo;
        }

        /// <summary>
        /// Creates an BitmapImage from URI. Sets BitmapCacheOption.OnLoad for WPF to prevent image file from being locked.
        /// </summary>
        public static BitmapImage BitmapFromUri(Uri uri)
        {
#if !SILVERLIGHT
            // Using new BitmapImage(uri) will leave a lock on the file, leading to problems with temporary image files in server environments.
            // We use BitmapCacheOption.OnLoad to prevent this lock.
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = uri;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
#else
            return new BitmapImage(uri);
#endif
        }
#endif

#if NETFX_CORE
        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class from a WinRT image.
        /// </summary>
        XImage(BitmapSource image)
        {
            _wrtImage = image;
            Initialize();
        }
#endif

        // Useful stuff here: http://stackoverflow.com/questions/350027/setting-wpf-image-source-in-code
        XImage(string path)
        {
#if !NETFX_CORE && !UWP
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
                throw new FileNotFoundException(PSSR.FileNotFound(path));
            //throw new FileNotFoundException(PSSR.FileNotFound(path), path);
#endif
            _path = path;

            //FileStream file = new FileStream(filename, FileMode.Open);
            //BitsLength = (int)file.Length;
            //Bits = new byte[BitsLength];
            //file.Read(Bits, 0, BitsLength);
            //file.Close();
#if CORE_WITH_GDI || GDI
            try
            {
                Lock.EnterGdiPlus();
                _gdiImage = Image.FromFile(path);
            }
            finally { Lock.ExitGdiPlus(); }
#endif
#if WPF && !SILVERLIGHT
            //BitmapSource.Create()
            // BUG: BitmapImage locks the file
            //_wpfImage = new BitmapImage(new Uri(path));  // AGHACK
            // Suggested change from forum to prevent locking.
            _wpfImage = BitmapFromUri(new Uri(path));
#endif
#if WPF && SILVERLIGHT
            //BitmapSource.Create()
            // BUG: BitmapImage locks the file
            //_wpfImage = new BitmapImage(new Uri(path));  // AGHACK
            //Debug-Break.Break();
#endif

#if true_
            float vres = image.VerticalResolution;
            float hres = image.HorizontalResolution;
            SizeF size = image.PhysicalDimension;
            int flags = image.Flags;
            Size sz = image.Size;
            GraphicsUnit units = GraphicsUnit.Millimeter;
            RectangleF rect = image.GetBounds(ref units);
            int width = image.Width;
#endif
            Initialize();
        }

        XImage(Stream stream)
        {
            // Create a dummy unique path.
            _path = "*" + Guid.NewGuid().ToString("B");

            // TODO: Create a fingerprint of the bytes in the stream to identify identical images.
            // TODO: Merge code for CORE_WITH_GDI and GDI.
#if CORE_WITH_GDI
            // Create a GDI+ image.
            try
            {
                Lock.EnterGdiPlus();
                _gdiImage = Image.FromStream(stream);
            }
            finally { Lock.ExitGdiPlus(); }
#endif
#if GDI
            // Create a GDI+ image.
            try
            {
                Lock.EnterGdiPlus();
                _gdiImage = Image.FromStream(stream);
            }
            finally { Lock.ExitGdiPlus(); }
#endif
#if WPF && !SILVERLIGHT
            // Create a WPF BitmapImage.
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.StreamSource = stream;
            bmi.EndInit();
            _wpfImage = bmi;
#endif
#if SILVERLIGHT
            int length = (int)stream.Length;
            stream.Seek(0, SeekOrigin.Begin);
            //_bytes = new byte[length];
            //stream.Read(_bytes, 0, length);
            //stream.Seek(0, SeekOrigin.Begin);

            // Create a Silverlight BitmapImage.
            _wpfImage = new BitmapImage();
            _wpfImage.SetSource(stream);
#endif

#if true_
            float vres = image.VerticalResolution;
            float hres = image.HorizontalResolution;
            SizeF size = image.PhysicalDimension;
            int flags = image.Flags;
            Size sz = image.Size;
            GraphicsUnit units = GraphicsUnit.Millimeter;
            RectangleF rect = image.GetBounds(ref units);
            int width = image.Width;
#endif
            // Must assign _stream before Initialize().
            _stream = stream;
            Initialize();
        }

#if GDI //|| CORE
#if UseGdiObjects
        /// <summary>
        /// Implicit conversion from Image to XImage.
        /// </summary>
        public static implicit operator XImage(Image image)
        {
            return new XImage(image);
        }
#endif

        /// <summary>
        /// Conversion from Image to XImage.
        /// </summary>
        public static XImage FromGdiPlusImage(Image image)
        {
            return new XImage(image);
        }
#endif

#if WPF
        /// <summary>
        /// Conversion from BitmapSource to XImage.
        /// </summary>
        public static XImage FromBitmapSource(BitmapSource image)
        {
            return new XImage(image);
        }
#endif

#if NETFX_CORE
        /// <summary>
        /// Conversion from BitmapSource to XImage.
        /// </summary>
        public static XImage FromBitmapSource(BitmapSource image)
        {
            return new XImage(image);
        }
#endif

        /// <summary>
        /// Creates an image from the specified file.
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static XImage FromFile(string path)
        {
            if (PdfReader.TestPdfFile(path) > 0)
                return new XPdfForm(path);
            return new XImage(path);
        }

        /// <summary>
        /// Creates an image from the specified stream.<br/>
        /// Silverlight supports PNG and JPEF only.
        /// </summary>
        /// <param name="stream">The stream containing a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static XImage FromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // TODO: Check PDF stream.
            //if (PdfReader.TestPdfFile(path) > 0)
            //  return new XPdfForm(path);
            return new XImage(stream);
        }

#if DEBUG
#if CORE || GDI || WPF
        /// <summary>
        /// Creates an image from the specified file.
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        /// <param name="platformIndependent">Uses an platform-independent implementation if set to true.
        /// The platform-dependent implementation, if available, will support more image formats.</param>
        /// <param name="document">The document used to obtain the options.</param>
        internal static XImage FromFile(string path, bool platformIndependent, PdfDocument document)
        {
            if (!platformIndependent)
                return FromFile(path);

            // TODO: Check PDF file.

            ImageImporter ii = ImageImporter.GetImageImporter();
            ImportedImage i = ii.ImportImage(path, document);

            if (i == null)
                throw new InvalidOperationException("Unsupported image format.");

            XImage image = new XImage(i);
            image._path = path;
            return image;
        }

        /// <summary>
        /// Creates an image from the specified stream.<br/>
        /// Silverlight supports PNG and JPEF only.
        /// </summary>
        /// <param name="stream">The stream containing a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        /// <param name="platformIndependent">Uses an platform-independent implementation if set to true.
        /// The platform-dependent implementation, if available, will support more image formats.</param>
        /// <param name="document">The document used to obtain the options.</param>
        internal static XImage FromStream(Stream stream, bool platformIndependent, PdfDocument document)
        {
            if (!platformIndependent)
                return FromStream(stream);

            // TODO: Check PDF file.

            ImageImporter ii = ImageImporter.GetImageImporter();
            ImportedImage i = ii.ImportImage(stream, document);

            if (i == null)
                throw new InvalidOperationException("Unsupported image format.");

            XImage image = new XImage(i);
            image._stream = stream;
            return image;
        }
#endif
#endif

#if DEBUG
#if CORE || GDI || WPF
        /// <summary>
        /// Creates an image.
        /// </summary>
        /// <param name="image">The imported image.</param>
        [Obsolete("THHO4THHO Internal test code.")]
        internal static XImage FromImportedImage(ImportedImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            return new XImage(image);
        }
#endif
#endif

        /// <summary>
        /// Tests if a file exist. Supports PDF files with page number suffix.
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static bool ExistsFile(string path)
        {
            // Support for "base64:" pseudo protocol is a MigraDoc feature, currently completely implemented in MigraDoc files. TODO: Does support for "base64:" make sense for PDFsharp? Probably not as PDFsharp can handle images from streams.
            //if (path.StartsWith("base64:")) // The Image is stored in the string here, so the file exists.
            //    return true;

            if (PdfReader.TestPdfFile(path) > 0)
                return true;
#if !NETFX_CORE && !UWP
            return File.Exists(path);
#else
            return false;
#endif
        }

        internal XImageState XImageState
        {
            get { return _xImageState; }
            set { _xImageState = value; }
        }
        XImageState _xImageState;

        internal void Initialize()
        {
#if CORE || GDI || WPF
            if (_importedImage != null)
            {
                ImportedImageJpeg iiJpeg = _importedImage as ImportedImageJpeg;
                // In PDF there are two formats: JPEG and PDF bitmap.
                if (iiJpeg != null)
                    _format = XImageFormat.Jpeg;
                else
                    _format = XImageFormat.Png;
                return;
            }
#endif

#if CORE_WITH_GDI
            if (_gdiImage != null)
            {
                // ImageFormat has no overridden Equals function.

                //....
                if (this._path != null)
                {
                    switch (System.IO.Path.GetExtension(_path))
                    {
                        case ".jpg":
                            _format = XImageFormat.Jpeg;
                            break;
                        case ".png":
                            _format = XImageFormat.Png;
                            break;
                        case ".gif":
                            _format = XImageFormat.Gif;
                            break;
                    }
                }

                //string guid;
                //try
                //{
                //    Lock.EnterGdiPlus();
                //    guid = _gdiImage.RawFormat.Guid.ToString("B").ToUpper();
                //}
                //finally
                //{
                //    Lock.ExitGdiPlus();
                //}

                //switch (guid)
                //{
                //    case "{B96B3CAA-0728-11D3-9D7B-0000F81EF32E}":  // memoryBMP
                //    case "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}":  // bmp
                //    case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":  // png
                //        _format = XImageFormat.Png;
                //        break;

                //    case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":  // jpeg
                //        _format = XImageFormat.Jpeg;
                //        break;

                //    case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":  // gif
                //        _format = XImageFormat.Gif;
                //        break;

                //    case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":  // tiff
                //        _format = XImageFormat.Tiff;
                //        break;

                //    case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":  // icon
                //        _format = XImageFormat.Icon;
                //        break;

                //    case "{B96B3CAC-0728-11D3-9D7B-0000F81EF32E}":  // emf
                //    case "{B96B3CAD-0728-11D3-9D7B-0000F81EF32E}":  // wmf
                //    case "{B96B3CB2-0728-11D3-9D7B-0000F81EF32E}":  // exif
                //    case "{B96B3CB3-0728-11D3-9D7B-0000F81EF32E}":  // photoCD
                //    case "{B96B3CB4-0728-11D3-9D7B-0000F81EF32E}":  // flashPIX

                //    default:
                //        throw new InvalidOperationException("Unsupported image format.");
                //}
                return;
            }
#endif

        }

        /// <summary>
        /// Under construction
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes underlying GDI+ object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                _disposed = true;

#if CORE || GDI || WPF
            //if (_importedImage != null)
            {
                _importedImage = null;
            }
#endif

#if CORE_WITH_GDI || GDI
            if (_gdiImage != null)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    _gdiImage.Dispose();
                    _gdiImage = null;
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            _wpfImage = null;
#endif
        }
        bool _disposed;

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelWidth, but will become PointWidth in future releases of PDFsharp.")]
        public virtual double Width
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    return _importedImage.Information.Width;
                }
#endif

#if (CORE_WITH_GDI || GDI)  && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
#endif

            }
        }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF. Currently same as PixelHeight, but will become PointHeight in future releases of PDFsharp.")]
        public virtual double Height
        {
            get
            {
#if CORE_WITH_GDI || GDI || WPF
                if (_importedImage != null)
                {
                    return _importedImage.Information.Height;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
#endif

            }
        }

#if CORE || GDI || WPF
        /// <summary>
        /// The factor for conversion from DPM to PointWidth or PointHeight.
        /// 72 points per inch, 1000 mm per meter, 25.4 mm per inch => 72 * 1000 / 25.4.
        /// </summary>
        private const decimal FactorDPM72 = 72000 / 25.4m;

        /// <summary>
        /// The factor for conversion from DPM to PointWidth or PointHeight.
        /// 1000 mm per meter, 25.4 mm per inch => 1000 / 25.4.
        /// </summary>
        private const decimal FactorDPM = 1000 / 25.4m;
#endif

        /// <summary>
        /// Gets the width of the image in point.
        /// </summary>
        public virtual double PointWidth
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.HorizontalDPM > 0)
                        return (double)(_importedImage.Information.Width * FactorDPM72 / _importedImage.Information.HorizontalDPM);
                    if (_importedImage.Information.HorizontalDPI > 0)
                        return (double)(_importedImage.Information.Width * 72 / _importedImage.Information.HorizontalDPI);
                    // Assume 72 DPI if information not available.
                    return _importedImage.Information.Width;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width * 72 / _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif

            }
        }

        /// <summary>
        /// Gets the height of the image in point.
        /// </summary>
        public virtual double PointHeight
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.VerticalDPM > 0)
                        return (double)(_importedImage.Information.Height * FactorDPM72 / _importedImage.Information.VerticalDPM);
                    if (_importedImage.Information.VerticalDPI > 0)
                        return (double)(_importedImage.Information.Height * 72 / _importedImage.Information.VerticalDPI);
                    // Assume 72 DPI if information not available.
                    return _importedImage.Information.Width;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height * 72 / _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif

            }
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public virtual int PixelWidth
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                    return (int)_importedImage.Information.Width;
#endif

#if CORE_WITH_GDI
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                int gdiWidth = _gdiImage.Width;
                int wpfWidth = _wpfImage.PixelWidth;
                Debug.Assert(gdiWidth == wpfWidth);
                return wpfWidth;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.Width;
                //#endif
#if WPF && !GDI
                return _wpfImage.PixelWidth;
#endif
#if NETFX_CORE || UWP
                return _wrtImage.PixelWidth;
#endif
            }
        }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public virtual int PixelHeight
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                    return (int)_importedImage.Information.Height;
#endif

#if CORE_WITH_GDI
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                int gdiHeight = _gdiImage.Height;
                int wpfHeight = _wpfImage.PixelHeight;
                Debug.Assert(gdiHeight == wpfHeight);
                return wpfHeight;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.Height;
                //#endif
#if WPF && !GDI
                return _wpfImage.PixelHeight;
#endif
#if NETFX_CORE || UWP
                return _wrtImage.PixelHeight;
#endif
            }
        }

        /// <summary>
        /// Gets the size in point of the image.
        /// </summary>
        public virtual XSize Size
        {
            get { return new XSize(PointWidth, PointHeight); }
        }

        /// <summary>
        /// Gets the horizontal resolution of the image.
        /// </summary>
        public virtual double HorizontalResolution
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.HorizontalDPI > 0)
                        return (double)_importedImage.Information.HorizontalDPI;
                    if (_importedImage.Information.HorizontalDPM > 0)
                        return (double)(_importedImage.Information.HorizontalDPM / FactorDPM);
                    return 72;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif

            }
        }

        /// <summary>
        /// Gets the vertical resolution of the image.
        /// </summary>
        public virtual double VerticalResolution
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.VerticalDPI > 0)
                        return (double)_importedImage.Information.VerticalDPI;
                    if (_importedImage.Information.VerticalDPM > 0)
                        return (double)(_importedImage.Information.VerticalDPM / FactorDPM);
                    return 72;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.VerticalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif

            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether image interpolation is to be performed. 
        /// </summary>
        public virtual bool Interpolate
        {
            get { return _interpolate; }
            set { _interpolate = value; }
        }
        bool _interpolate = true;

        /// <summary>
        /// Gets the format of the image.
        /// </summary>
        public XImageFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }
        XImageFormat _format;


        internal void AssociateWithGraphics(XGraphics gfx)
        {
            if (_associatedGraphics != null)
                throw new InvalidOperationException("XImage already associated with XGraphics.");
            _associatedGraphics = null;
        }

        internal void DisassociateWithGraphics()
        {
            if (_associatedGraphics == null)
                throw new InvalidOperationException("XImage not associated with XGraphics.");
            _associatedGraphics.DisassociateImage();

            Debug.Assert(_associatedGraphics == null);
        }

        internal void DisassociateWithGraphics(XGraphics gfx)
        {
            if (_associatedGraphics != gfx)
                throw new InvalidOperationException("XImage not associated with XGraphics.");
            _associatedGraphics = null;
        }

        internal XGraphics AssociatedGraphics
        {
            get { return _associatedGraphics; }
            set { _associatedGraphics = value; }
        }
        XGraphics _associatedGraphics;

#if CORE || GDI || WPF
        internal ImportedImage _importedImage;
#endif

#if CORE_WITH_GDI || GDI
        internal Image _gdiImage;
#endif
 

        /// <summary>
        /// If path starts with '*' the image is created from a stream and the path is a GUID.
        /// </summary>
        internal string _path;

        /// <summary>
        /// Contains a reference to the original stream if image was created from a stream.
        /// </summary>
        internal Stream _stream;

        /// <summary>
        /// Cache PdfImageTable.ImageSelector to speed up finding the right PdfImage
        /// if this image is used more than once.
        /// </summary>
        internal PdfImageTable.ImageSelector _selector;
    }
}
