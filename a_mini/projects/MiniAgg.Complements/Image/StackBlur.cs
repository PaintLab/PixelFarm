
//ref:  adapt from snippetsfor.net/CSharp/StackBlur

namespace MatterHackers
{

    using System.Runtime.InteropServices;

    public class StackBlur2
    {
        static void CalculateSumRGBHorizontal(byte[] source,
            int width,
            ref int bufferIndex, int radius,
            out int rsum, out int gsum, out int bsum)
        {

            rsum = gsum = bsum = 0;
            int startAt = bufferIndex;

            for (int i = -radius; i <= 0; ++i)
            {
                //negative and zero  
                byte a = source[startAt];
                byte r = source[startAt + 1];
                byte g = source[startAt + 2];
                byte b = source[startAt + 3];

                rsum += r;
                gsum += g;
                bsum += b;

                startAt += 4;
            }

            int widthMinus1 = width - 1;
            for (int i = 1; i <= radius; ++i)
            {
                //positive side
                int mm = (min(widthMinus1, i) * 4);
                //int p = source[startAt + (min(widthMinus1, i) * 4)];
                byte a = source[startAt + mm + 0];
                byte r = source[startAt + mm + 1];
                byte g = source[startAt + mm + 2];
                byte b = source[startAt + mm + 3];

                startAt += 4;
                rsum += r;
                gsum += g;
                bsum += b;
            }

        }

        static void CalculateSumRGBHorizontal2(int[] source,
            int width,
            int pixel_index,
            int radius,
            out int rsum, out int gsum, out int bsum)
        {

            rsum = gsum = bsum = 0;
            for (int i = -radius; i <= 0; ++i)
            {
                //negative and zero  

                int p = source[pixel_index];

                rsum += (p & 0xff0000) >> 16;
                gsum += (p & 0x00ff00) >> 8;
                bsum += p & 0x0000ff;
            }
            int widthMinus1 = width - 1;
            for (int i = 1; i <= radius; ++i)
            {
                //positive side
                int p = source[pixel_index + min(widthMinus1, i)];
                rsum += (p & 0xff0000) >> 16;
                gsum += (p & 0x00ff00) >> 8;
                bsum += p & 0x0000ff;
            }

        }
        static byte[] PrepareLookupTable(int radius)
        {
            int div = radius + radius + 1;
            var dv = new byte[256 * div];
            for (int i = (256 * div) - 1; i >= 0; --i)
            {
                dv[i] = (byte)(i / div);
            }
            return dv;
        }
        static void PrepareHorizontalMinMax(int width, int radius, LimitMinMax[] limitMinMax)
        {
            int widthMinus1 = width - 1;
            for (int x = width - 1; x >= 0; --x)
            {

                limitMinMax[x] = new LimitMinMax(
                    min(x + radius + 1, widthMinus1),
                    max(x - radius, 0));
            }
        }
        static void PrepareVerticalMinMax(int width, int height, int radius, LimitMinMax[] limitMinMax)
        {
            int heightMinus1 = height - 1;
            for (int y = height - 1; y >= 0; --y)
            {
                limitMinMax[y] = new LimitMinMax(
                    min(y + radius + 1, heightMinus1) * width,
                    max(y - radius, 0) * width);
            }
        }

        struct LimitMinMax
        {
            public int Min;
            public int Max;
            public LimitMinMax(int min, int max)
            {
                this.Min = min;
                this.Max = max;
            }
        }
        struct ColorARGB
        {
            public byte a;
            public byte r;
            public byte g;
            public byte b;
            public ColorARGB(byte a, byte r, byte g, byte b)
            {
                this.a = a;
                this.r = r;
                this.g = g;
                this.b = b;
            }
        }

        public static void FastBlur32RGBA(byte[] srcBuffer,
            byte[] dest,
            int srcImageWidth,
            int srcImageHeight,
            int radius)
        {

            if (srcImageWidth < 1)
            {
                return;
            }

            int width = srcImageWidth;
            int height = srcImageHeight;
            int wh = width * height;

            var r_buffer = new byte[wh];
            var g_buffer = new byte[wh];
            var b_buffer = new byte[wh];
            //--------------------------------------------- 

            int p1, p2;

            LimitMinMax[] limitMinMax = new LimitMinMax[max(width, height)];
            //------------------------------
            //look up table : depends on radius,  
            var dvLookup = PrepareLookupTable(radius);
            //------------------------------  

            PrepareHorizontalMinMax(width, radius, limitMinMax);

            int px_row_head = 0;
            int pixel_index = 0;
            for (int y = 0; y < height; y++)
            {
                // blur horizontal
                int rsum, gsum, bsum;
                CalculateSumRGBHorizontal(srcBuffer, width, ref pixel_index, radius, out rsum, out gsum, out bsum);

                for (int x = 0; x < width; x++)
                {
                    r_buffer[pixel_index] = dvLookup[rsum];
                    g_buffer[pixel_index] = dvLookup[gsum];
                    b_buffer[pixel_index] = dvLookup[bsum];

                    LimitMinMax lim = limitMinMax[x];

                    int px_idx = (px_row_head + lim.Min) * 4;
                    p1 =  (srcBuffer[px_idx + 1] << 16) | (srcBuffer[px_idx + 2] << 8) | srcBuffer[px_idx + 3];
                    int px_idx2 = (px_row_head + lim.Max) * 4;
                    p2 =  (srcBuffer[px_idx2 + 1] << 16) | (srcBuffer[px_idx2 + 2] << 8) | srcBuffer[px_idx2 + 3];


                    //p1 = source[px_row_head + lim.Min];
                    //p2 = source[px_row_head + lim.Max];

                    rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
                    gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
                    bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);

                    pixel_index++;
                }
                //go next row
                px_row_head += width;
            }

            PrepareVerticalMinMax(width, height, radius, limitMinMax);
            //-------------------------------------------------------------------
            int dest_buffer_index = 0;

            for (int x = 0; x < width; x++)
            {
                // blur vertical
                int rsum, gsum, bsum;
                rsum = gsum = bsum = 0;
                //-----------------------------
                int yp = -radius * width;
                for (int i = -radius; i <= 0; ++i)
                {
                    pixel_index = x;
                    rsum += r_buffer[pixel_index];
                    gsum += g_buffer[pixel_index];
                    bsum += b_buffer[pixel_index];
                    yp += width;
                }
                for (int i = 1; i <= radius; ++i)
                {
                    pixel_index = yp + x;
                    rsum += r_buffer[pixel_index];
                    gsum += g_buffer[pixel_index];
                    bsum += b_buffer[pixel_index];
                    yp += width;
                }
                //-----------------------------

                pixel_index = x; //first row
                dest_buffer_index = pixel_index * 4;
                for (int y = 0; y < height; y++)
                {
                    //assign pixel value here
                    //dest[pixel_index] = (int)(0xff000000u | (uint)(dvLookup[rsum] << 16) | (uint)(dvLookup[gsum] << 8) | (uint)dvLookup[bsum]);

                    dest[dest_buffer_index] = 0;//a
                    dest[dest_buffer_index++] = dvLookup[rsum];//r
                    dest[dest_buffer_index++] = dvLookup[gsum];//g
                    dest[dest_buffer_index++] = dvLookup[bsum];//b


                    var limit = limitMinMax[y];
                    p1 = x + limit.Min;
                    p2 = x + limit.Max;


                    rsum += r_buffer[p1] - r_buffer[p2]; //diff between 2 pixels
                    gsum += g_buffer[p1] - g_buffer[p2]; //diff between 2 pixels
                    bsum += b_buffer[p1] - b_buffer[p2]; //diff between 2 pixels

                    pixel_index += width;
                    dest_buffer_index += (width * 4);
                }

            }

            //// copy back to image
            //var bits2 = srcImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            //Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
            //srcImage.UnlockBits(bitmapData);
        }




        //public static void FastBlur32RGBA_2(Bitmap SourceImage, int radius)
        //{
        //    var rct = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);

        //    var dest = new int[rct.Width * rct.Height];
        //    var source = new int[rct.Width * rct.Height];
        //    var bits = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, SourceImage.PixelFormat);

        //    Marshal.Copy(bits.Scan0, source, 0, source.Length);
        //    SourceImage.UnlockBits(bits);

        //    if (radius < 1) return;

        //    int w = rct.Width;
        //    int h = rct.Height;
        //    int wm = w - 1;
        //    int hm = h - 1;
        //    int wh = w * h;
        //    int div = radius + radius + 1;
        //    var r = new int[wh];
        //    var g = new int[wh];
        //    var b = new int[wh];
        //    int rsum, gsum, bsum, x, y, i, p1, p2, yi;
        //    var vmin = new int[max(w, h)];
        //    var vmax = new int[max(w, h)];

        //    var dv = new int[256 * div];
        //    for (i = 0; i < 256 * div; i++)
        //    {
        //        dv[i] = (i / div);
        //    }

        //    int yw = yi = 0;

        //    for (y = 0; y < h; y++)
        //    { // blur horizontal
        //        rsum = gsum = bsum = 0;
        //        for (i = -radius; i <= radius; i++)
        //        {
        //            int p = source[yi + min(wm, max(i, 0))];
        //            rsum += (p & 0xff0000) >> 16;
        //            gsum += (p & 0x00ff00) >> 8;
        //            bsum += p & 0x0000ff;
        //        }
        //        for (x = 0; x < w; x++)
        //        {

        //            r[yi] = dv[rsum];
        //            g[yi] = dv[gsum];
        //            b[yi] = dv[bsum];

        //            if (y == 0)
        //            {
        //                vmin[x] = min(x + radius + 1, wm);
        //                vmax[x] = max(x - radius, 0);
        //            }
        //            p1 = source[yw + vmin[x]];
        //            p2 = source[yw + vmax[x]];

        //            rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
        //            gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
        //            bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);
        //            yi++;
        //        }
        //        yw += w;
        //    }

        //    for (x = 0; x < w; x++)
        //    { // blur vertical
        //        rsum = gsum = bsum = 0;
        //        int yp = -radius * w;
        //        for (i = -radius; i <= radius; i++)
        //        {
        //            yi = max(0, yp) + x;
        //            rsum += r[yi];
        //            gsum += g[yi];
        //            bsum += b[yi];
        //            yp += w;
        //        }
        //        yi = x;
        //        for (y = 0; y < h; y++)
        //        {
        //            dest[yi] = (int)(0xff000000u | (uint)(dv[rsum] << 16) | (uint)(dv[gsum] << 8) | (uint)dv[bsum]);
        //            if (x == 0)
        //            {
        //                vmin[y] = min(y + radius + 1, hm) * w;
        //                vmax[y] = max(y - radius, 0) * w;
        //            }
        //            p1 = x + vmin[y];
        //            p2 = x + vmax[y];

        //            rsum += r[p1] - r[p2];
        //            gsum += g[p1] - g[p2];
        //            bsum += b[p1] - b[p2];

        //            yi += w;
        //        }
        //    }

        //    // copy back to image
        //    var bits2 = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
        //    SourceImage.UnlockBits(bits);
        //}
        private static int min(int a, int b) { return System.Math.Min(a, b); }
        private static int max(int a, int b) { return System.Math.Max(a, b); }

        //public static void FastBlur(Bitmap SourceImage, int radius)
        //{
        //    var rct = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
        //    var dest = new byte[rct.Width * rct.Height];
        //    var source = new byte[rct.Width * rct.Height];
        //    var bits = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, SourceImage.PixelFormat);
        //    Marshal.Copy(bits.Scan0, source, 0, source.Length);
        //    SourceImage.UnlockBits(bits);

        //    if (radius < 1) return;

        //    int w = rct.Width;
        //    int h = rct.Height;
        //    int wm = w - 1;
        //    int hm = h - 1;
        //    int wh = w * h;
        //    int div = radius + radius + 1;
        //    var r = new int[wh];
        //    var g = new int[wh];
        //    var b = new int[wh];
        //    int rsum, gsum, bsum, x, y, i, p1, p2, yi;
        //    var vmin = new int[max(w, h)];
        //    var vmax = new int[max(w, h)];

        //    var dv = new int[256 * div];
        //    for (i = 0; i < 256 * div; i++)
        //    {
        //        dv[i] = (i / div);
        //    }

        //    int yw = yi = 0;

        //    for (y = 0; y < h; y++)
        //    { // blur horizontal
        //        rsum = gsum = bsum = 0;
        //        for (i = -radius; i <= radius; i++)
        //        {
        //            int p = source[yi + min(wm, max(i, 0))];
        //            rsum += (p & 0xff0000) >> 16;
        //            gsum += (p & 0x00ff00) >> 8;
        //            bsum += p & 0x0000ff;
        //        }
        //        for (x = 0; x < w; x++)
        //        {

        //            r[yi] = dv[rsum];
        //            g[yi] = dv[gsum];
        //            b[yi] = dv[bsum];

        //            if (y == 0)
        //            {
        //                vmin[x] = min(x + radius + 1, wm);
        //                vmax[x] = max(x - radius, 0);
        //            }
        //            p1 = source[yw + vmin[x]];
        //            p2 = source[yw + vmax[x]];

        //            rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
        //            gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
        //            bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);
        //            yi++;
        //        }
        //        yw += w;
        //    }

        //    for (x = 0; x < w; x++)
        //    { // blur vertical
        //        rsum = gsum = bsum = 0;
        //        int yp = -radius * w;
        //        for (i = -radius; i <= radius; i++)
        //        {
        //            yi = max(0, yp) + x;
        //            rsum += r[yi];
        //            gsum += g[yi];
        //            bsum += b[yi];
        //            yp += w;
        //        }
        //        yi = x;
        //        for (y = 0; y < h; y++)
        //        {
        //            dest[yi] = (int)(0xff000000u | (uint)(dv[rsum] << 16) | (uint)(dv[gsum] << 8) | (uint)dv[bsum]);
        //            if (x == 0)
        //            {
        //                vmin[y] = min(y + radius + 1, hm) * w;
        //                vmax[y] = max(y - radius, 0) * w;
        //            }
        //            p1 = x + vmin[y];
        //            p2 = x + vmax[y];

        //            rsum += r[p1] - r[p2];
        //            gsum += g[p1] - g[p2];
        //            bsum += b[p1] - b[p2];

        //            yi += w;
        //        }
        //    }

        //    // copy back to image
        //    var bits2 = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
        //    SourceImage.UnlockBits(bits);
        //}
    }

}