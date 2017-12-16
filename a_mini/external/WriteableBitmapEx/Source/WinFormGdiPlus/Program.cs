//MIT, 2017, WinterDev
//example and test for WritableBitmap (https://github.com/teichgraf/WriteableBitmapEx) on Gdi+

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Drawing;
using System.Drawing.Imaging;

namespace WinFormGdiPlus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    struct LockBmp : IDisposable
    {
        Bitmap _bmp;
        BitmapData _bmpdata;
        System.Windows.Media.Imaging.WriteableBitmap _writeableBitmap;
        int bufferLenInBytes;
        public LockBmp(Bitmap bmp)
        {
            _bmp = bmp;
            _bmpdata = _bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            _writeableBitmap = null;
            bufferLenInBytes = 0;
        }
        public System.Windows.Media.Imaging.WriteableBitmap GetWritableBitmap()
        {
            if (_writeableBitmap != null) return _writeableBitmap;
            //
            //create
            bufferLenInBytes = _bmpdata.Stride * _bmpdata.Height;
            int[] buffer = new int[bufferLenInBytes / 4];
            System.Runtime.InteropServices.Marshal.Copy(_bmpdata.Scan0, buffer, 0, bufferLenInBytes / 4);

            return _writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(_bmp.Width, _bmp.Height, buffer);
        }
        public void WriteAndUnlock()
        {
            Write();
            Unlock();
        }

        public void Write()
        {
            //write back

            if (_writeableBitmap == null) return;

            //write data back
            System.Runtime.InteropServices.Marshal.Copy(_writeableBitmap.Pixels,
                0, _bmpdata.Scan0, bufferLenInBytes / 4);

        }
        public void Unlock()
        {
            if (_bmp != null)
            {
                _bmp.UnlockBits(_bmpdata);
                _bmp = null;
                _bmpdata = null;
            }
        }
        public void Dispose()
        {
            Unlock();
        }
    }

    static class BitmapExtension2
    {
        public static LockBmp Lock(this Bitmap bitmap)
        {
            return new LockBmp(bitmap);
        }
    }
}


