//MIT, 2017-present, WinterDev
using System;
namespace PixelFarm.CpuBlit.Imaging
{
    public delegate void SaveImageBufferToFileDel(IntPtr imgBuffer, int stride, int width, int height, string filename);

    public static class PngImageReader
    {
        public static void SaveImgBufferToPngFile(int[] imgBuffer, int stride, int width, int height, string filename)
        {
            if (s_saveToPngFileDel != null)
            {
                unsafe
                {
                    fixed (int* head = &imgBuffer[0])
                    {
                        s_saveToPngFileDel((IntPtr)head, stride, width, height, filename);
                    }
                }
            }
        }
        static SaveImageBufferToFileDel s_saveToPngFileDel;

        public static bool HasDefaultSavePngToFileDelegate()
        {
            return s_saveToPngFileDel != null;
        }


        public static void InstallImageSaveToFileService(SaveImageBufferToFileDel saveToPngFileDelegate)
        {
            s_saveToPngFileDel = saveToPngFileDelegate;
        }


#if DEBUG
        public static void dbugSaveToPngFile(this ActualBitmap bmp, string filename)
        {

            SaveImgBufferToPngFile(ActualBitmap.GetBuffer(bmp),
                bmp.Stride,
                bmp.Width,
                bmp.Height,
                filename);
        }
#endif
    }
    public class PngImageWriter
    {

    }
    //---------------------------------

}