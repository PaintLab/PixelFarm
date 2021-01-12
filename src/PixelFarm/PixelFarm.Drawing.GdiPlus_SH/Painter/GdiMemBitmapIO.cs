//Apache2, 2014-present, WinterDev

using System;
using System.IO;

using PixelFarm.CpuBlit;
using MemMx = PixelFarm.Drawing.Internal.MemMx;
namespace PixelFarm.Drawing.WinGdi
{

    public sealed class GdiBitmapIO : MemBitmapIO
    {

        public override MemBitmap ScaleImage(MemBitmap bmp, float x_scale, float y_scale)
        {
            return ScaleImageInternal(bmp, x_scale, y_scale);
        }
        static PixelFarm.CpuBlit.MemBitmap ScaleImageInternal(PixelFarm.CpuBlit.MemBitmap bmp, float x_scale, float y_scale)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(bmp.Width, bmp.Height);
            PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSizeNotFlip(bmp, gdiBmp);
            //save exported img to tmp file system? 
            //--------------
            int paddingLeftRight = 10;
            int paddingTopBottom = 10;
            int newW = (int)(gdiBmp.Width * x_scale);
            int newH = (int)(gdiBmp.Height * y_scale);
            int newBmpW = newW + paddingLeftRight;
            int newBmpH = newH + paddingTopBottom;

            System.Drawing.Bitmap resize = ResizeImage(gdiBmp, newBmpW, newBmpH, 0, 0, newW, newH);


            PixelFarm.CpuBlit.MemBitmap newBmp = new PixelFarm.CpuBlit.MemBitmap(newBmpW, newBmpH);
            PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(resize, newBmp);
            return newBmp;
        }

        static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int newBmpW, int newBmpH, int left, int top, int scaledW, int scaledH)
        {
            //a holder for the result
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(newBmpW, newBmpH);
            //set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.Clear(System.Drawing.Color.White);
                graphics.DrawImage(image, left, top, scaledW, scaledH);
            }

            //return the resulting bitmap
            return result;
        }
        public override MemBitmap LoadImage(string filename)
        {
            //resolve IO dest too!!
            using (System.IO.FileStream fs = new System.IO.FileStream(filename, FileMode.Open))
            {
                return LoadImage(fs);
            }
        }
        public override MemBitmap LoadImage(Stream input)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(input))
            {
                var bmpData2 = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                MemBitmap memBitmap = new MemBitmap(bmp.Width, bmp.Height);
                unsafe
                {
                    byte* dst = (byte*)MemBitmap.GetBufferPtr(memBitmap).Ptr;
                    MemMx.memcpy(dst, (byte*)bmpData2.Scan0, bmpData2.Stride * bmpData2.Height);
                }
                return memBitmap;
            }
        }
        public override void SaveImage(MemBitmap bitmap, Stream output, OutputImageFormat outputFormat, object saveParameters)
        {
            //save img to
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                unsafe
                {
                    byte* ptr = (byte*)MemBitmap.GetBufferPtr(bitmap).Ptr;
                    MemMx.memcpy((byte*)bmpdata.Scan0, ptr, bmpdata.Stride * bmp.Height);
                }
                bmp.UnlockBits(bmpdata);
                //save to stream
                System.Drawing.Imaging.ImageFormat format = null;
                switch (outputFormat)
                {
                    case OutputImageFormat.Default:
                        throw new NotSupportedException();
                    case OutputImageFormat.Jpeg:
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case OutputImageFormat.Png:
                        format = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                }
                bmp.Save(output, format);
            }
        }

        public override void SaveImage(MemBitmap bitmap, string filename, OutputImageFormat outputFormat, object saveParameters)
        {
            //TODO: resolve filename here!!!
            using (FileStream output = new FileStream(filename, FileMode.Create))
            {
                SaveImage(bitmap, output, outputFormat, saveParameters);
                output.Flush();
            }
           
        }
    }
}