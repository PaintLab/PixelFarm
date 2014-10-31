//MIT 2014, WinterDev
using System;
using System.Drawing;
using System.Drawing.Imaging;

using PixelFarm.Agg;
using PixelFarm.Agg.Image;

namespace Mini
{
    public static class BitmapHelper
    {
        public static void CopyToWindowsBitmap(ImageBase backingImageBufferByte,
            Bitmap windowsBitmap,
            RectangleInt rect)
        {




            //int backBufferStrideInBytes = backingImageBufferByte.StrideInBytes();
            //int backBufferStrideInInts = backBufferStrideInBytes / 4;
            //int backBufferHeight = backingImageBufferByte.Height;
            //int backBufferHeightMinusOne = backBufferHeight - 1;

            int offset = 0;
            byte[] buffer = backingImageBufferByte.GetBuffer();

            BitmapHelper.CopyToWindowsBitmap(buffer,offset,
                backingImageBufferByte.Stride, backingImageBufferByte.Height,
                backingImageBufferByte.BitDepth,
                windowsBitmap, rect);

            //switch (backingImageBufferByte.BitDepth)
            //{
            //    case 24:
            //        {
            //            unsafe
            //            {
            //                byte* bitmapDataScan0 = (byte*)bitmapData1.Scan0;
            //                fixed (byte* pSourceFixed = &buffer[offset])
            //                {
            //                    byte* pSource = pSourceFixed;
            //                    byte* pDestBuffer = bitmapDataScan0 + bitmapDataStride * backBufferHeightMinusOne;
            //                    for (int y = 0; y < backBufferHeight; y++)
            //                    {
            //                        int* pSourceInt = (int*)pSource;
            //                        int* pDestBufferInt = (int*)pDestBuffer;
            //                        for (int x = 0; x < backBufferStrideInInts; x++)
            //                        {
            //                            pDestBufferInt[x] = pSourceInt[x];
            //                        }
            //                        for (int x = backBufferStrideInInts * 4; x < backBufferStrideInBytes; x++)
            //                        {
            //                            pDestBuffer[x] = pSource[x];
            //                        }
            //                        pDestBuffer -= bitmapDataStride;
            //                        pSource += backBufferStrideInBytes;
            //                    }
            //                }
            //            }
            //        }
            //        break;

            //    case 32:
            //        {
            //            unsafe
            //            {
            //                byte* bitmapDataScan0 = (byte*)bitmapData1.Scan0;
            //                fixed (byte* pSourceFixed = &buffer[offset])
            //                {
            //                    byte* pSource = pSourceFixed;
            //                    byte* pDestBuffer = bitmapDataScan0 + bitmapDataStride * backBufferHeightMinusOne;

            //                    int rect_bottom = rect.Bottom;
            //                    int rect_top = rect.Top;
            //                    int rect_left = rect.Left;
            //                    int rect_right = rect.Right;

            //                    for (int y = rect_bottom; y < rect_top; y++)
            //                    {
            //                        int* pSourceInt = (int*)pSource;
            //                        pSourceInt += (backBufferStrideInBytes * y / 4);

            //                        int* pDestBufferInt = (int*)pDestBuffer;
            //                        pDestBufferInt -= (bitmapDataStride * y / 4);

            //                        for (int x = rect_left; x < rect_right; x++)
            //                        {
            //                            pDestBufferInt[x] = pSourceInt[x];
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}

        }
        public static void CopyToWindowsBitmap(byte[] buffer, int offset,
          int sBackBufferStrideInBytes, int sHeight,
          int bitDepth,
          Bitmap windowsBitmap,
          RectangleInt rect)
        {
            BitmapData bitmapData1 = windowsBitmap.LockBits(
                      new Rectangle(0, 0, windowsBitmap.Width, windowsBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, windowsBitmap.PixelFormat);

            int backBufferStrideInInts = sBackBufferStrideInBytes / 4;
            int backBufferHeight = sHeight;
            int backBufferHeightMinusOne = backBufferHeight - 1;
            int bitmapDataStride = bitmapData1.Stride;


            switch (bitDepth)
            {
                case 24:
                    {
                        unsafe
                        {
                            byte* bitmapDataScan0 = (byte*)bitmapData1.Scan0;
                            fixed (byte* pSourceFixed = &buffer[offset])
                            {
                                byte* pSource = pSourceFixed;
                                byte* pDestBuffer = bitmapDataScan0 + bitmapDataStride * backBufferHeightMinusOne;
                                for (int y = 0; y < backBufferHeight; y++)
                                {
                                    int* pSourceInt = (int*)pSource;
                                    int* pDestBufferInt = (int*)pDestBuffer;
                                    for (int x = 0; x < backBufferStrideInInts; x++)
                                    {
                                        pDestBufferInt[x] = pSourceInt[x];
                                    }
                                    for (int x = backBufferStrideInInts * 4; x < sBackBufferStrideInBytes; x++)
                                    {
                                        pDestBuffer[x] = pSource[x];
                                    }
                                    pDestBuffer -= bitmapDataStride;
                                    pSource += sBackBufferStrideInBytes;
                                }
                            }
                        }
                    }
                    break;

                case 32:
                    {
                        unsafe
                        {
                            byte* bitmapDataScan0 = (byte*)bitmapData1.Scan0;
                            fixed (byte* pSourceFixed = &buffer[offset])
                            {
                                byte* pSource = pSourceFixed;
                                byte* pDestBuffer = bitmapDataScan0 + bitmapDataStride * backBufferHeightMinusOne;

                                int rect_bottom = rect.Bottom;
                                int rect_top = rect.Top;
                                int rect_left = rect.Left;
                                int rect_right = rect.Right;

                                for (int y = rect_bottom; y < rect_top; y++)
                                {
                                    int* pSourceInt = (int*)pSource;
                                    pSourceInt += (sBackBufferStrideInBytes * y / 4);

                                    int* pDestBufferInt = (int*)pDestBuffer;
                                    pDestBufferInt -= (bitmapDataStride * y / 4);

                                    for (int x = rect_left; x < rect_right; x++)
                                    {
                                        pDestBufferInt[x] = pSourceInt[x];
                                    }
                                }
                            }
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            windowsBitmap.UnlockBits(bitmapData1);
        }
    }

}
