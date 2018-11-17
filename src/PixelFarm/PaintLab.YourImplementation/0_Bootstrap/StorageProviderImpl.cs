//MIT, 2018-present, WinterDev

using System;
using System.Collections.Generic;
using System.IO;

using PixelFarm.CpuBlit;

namespace YourImplementation
{
    class LocalFileStorageProvider : PixelFarm.Platforms.StorageServiceProvider
    {
        public override bool DataExists(string dataName)
        {
            //implement with file
            return System.IO.File.Exists(dataName);
        }
        public override byte[] ReadData(string dataName)
        {
            return System.IO.File.ReadAllBytes(dataName);
        }
        public override void SaveData(string dataName, byte[] content)
        {
            System.IO.File.WriteAllBytes(dataName, content);
        }
        public override MemBitmap ReadPngBitmap(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return PngIOStorage.Read(fs);
            }
        }
        public override void SavePngBitmap(MemBitmap bmp, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                PngIOStorage.Save(bmp, fs);
            }
        }
    }




    static class PngIOStorage
    {

        public static MemBitmap Read(Stream strm)
        {

            Hjg.Pngcs.PngReader reader = new Hjg.Pngcs.PngReader(strm);
            Hjg.Pngcs.ImageInfo imgInfo = reader.ImgInfo;
            Hjg.Pngcs.ImageLine iline2 = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);

            int imgH = imgInfo.Rows;
            int imgW = imgInfo.Cols;
            int stride = imgInfo.BytesPerRow;
            int widthPx = imgInfo.Cols;

            int[] buffer = new int[(stride / 4) * imgH];
            bool isInverted = false;
            if (isInverted)
            {
                //read each row 
                //and fill the glyph image 
                int startWriteAt = (imgW * (imgH - 1));
                int destIndex = startWriteAt;
                for (int row = 0; row < imgH; row++)
                {
                    Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                    byte[] scline = iline.ScanlineB;

                    int b_src = 0;
                    destIndex = startWriteAt;

                    for (int mm = 0; mm < imgW; ++mm)
                    {
                        byte b = scline[b_src];
                        byte g = scline[b_src + 1];
                        byte r = scline[b_src + 2];
                        byte a = scline[b_src + 3];
                        b_src += 4;
                        buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                        destIndex++;
                    }
                    startWriteAt -= imgW;
                }
                return MemBitmap.CreateFromCopy(imgW, imgH, buffer);
            }
            else
            {
                //read each row 
                //and fill the glyph image 
                int startWriteAt = 0;
                int destIndex = startWriteAt;
                for (int row = 0; row < imgH; row++)
                {
                    Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                    byte[] scline = iline.ScanlineB;

                    int b_src = 0;
                    destIndex = startWriteAt;

                    for (int mm = 0; mm < imgW; ++mm)
                    {

                        byte b = scline[b_src];
                        byte g = scline[b_src + 1];
                        byte r = scline[b_src + 2];
                        byte a = scline[b_src + 3];
                        b_src += 4;
                        buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                        destIndex++;
                    }
                    startWriteAt += imgW;
                }
                return MemBitmap.CreateFromCopy(imgW, imgH, buffer);
            }


        }
        public static void Save(MemBitmap bmp, Stream strm)
        {
            //-------------
            unsafe
            {
                PixelFarm.CpuBlit.Imaging.TempMemPtr tmp = MemBitmap.GetBufferPtr(bmp);
                int* intBuffer = (int*)tmp.Ptr;

                int imgW = bmp.Width;
                int imgH = bmp.Height;

                Hjg.Pngcs.ImageInfo imgInfo = new Hjg.Pngcs.ImageInfo(imgW, imgH, 8, true); //8 bits per channel with alpha
                Hjg.Pngcs.PngWriter writer = new Hjg.Pngcs.PngWriter(strm, imgInfo);
                Hjg.Pngcs.ImageLine iline = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);
                int startReadAt = 0;

                int imgStride = imgW * 4;

                int srcIndex = 0;
                int srcIndexRowHead = (tmp.LengthInBytes / 4) - imgW;

                for (int row = 0; row < imgH; row++)
                {
                    byte[] scanlineBuffer = iline.ScanlineB;
                    srcIndex = srcIndexRowHead;
                    for (int b = 0; b < imgStride;)
                    {
                        int srcInt = intBuffer[srcIndex];
                        srcIndex++;
                        scanlineBuffer[b] = (byte)((srcInt >> 16) & 0xff);
                        scanlineBuffer[b + 1] = (byte)((srcInt >> 8) & 0xff);
                        scanlineBuffer[b + 2] = (byte)((srcInt) & 0xff);
                        scanlineBuffer[b + 3] = (byte)((srcInt >> 24) & 0xff);
                        b += 4;
                    }
                    srcIndexRowHead -= imgW;
                    startReadAt += imgStride;
                    writer.WriteRow(iline, row);
                }
                writer.End();
            }


        }


    }

}