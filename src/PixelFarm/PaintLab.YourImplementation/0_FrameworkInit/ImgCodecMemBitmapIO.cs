//MIT, 2016-present, WinterDev
using System;
using PixelFarm.CpuBlit;
using System.IO;
using BitMiracle.LibJpeg;

namespace YourImplementation
{

    public class ImgCodecMemBitmapIO : PixelFarm.CpuBlit.MemBitmapIO
    {
        public override MemBitmap LoadImage(string filename)
        {
            OutputImageFormat format;
            //TODO: review img loading, we should not use only its extension 
            string fileExt = System.IO.Path.GetExtension(filename).ToLower();
            switch (fileExt)
            {
                case ".pngx":
                case ".png":
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Open))
                        {
                            return PngIOStorage.Read(fs);
                        }
                    }
                case ".jpg":
                    {
                        format = OutputImageFormat.Jpeg;
                    }
                    break;
                default:
                    throw new System.NotSupportedException();
            }

            //TODO: don't directly access file here
            //we should access file from host request
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return LoadImage(fs, format);
            }
        }

        public override MemBitmap LoadImage(Stream input)
        {
            //try get type of img from input stream
            byte[] buff = new byte[4];
            input.Read(buff, 0, 4);

            //convert to chars
#if DEBUG
            char c0 = (char)buff[0];
            char c1 = (char)buff[1];
            char c2 = (char)buff[2];
            char c3 = (char)buff[3];
#endif
            if ((char)buff[1] == 'P' &&
                (char)buff[2] == 'N' &&
                (char)buff[3] == 'G')
            {
                //try read as png
                input.Seek(-4, SeekOrigin.Current);
                return LoadImage(input, OutputImageFormat.Png);
            }
            else
            {
                //jpeg?
                //throw new NotImplementedException();
                input.Seek(-4, SeekOrigin.Current);
                return LoadImage(input, OutputImageFormat.Jpeg);
            }
        }


        class JpegDecoderDst : BitMiracle.LibJpeg.IDecompressDestination
        {
            MemBitmap _memBmp;
            int _imgWidth;
            int _imgHeight;
            int _component;
            BitStream _bitStream;

            int _currentRowNo;
            TempMemPtr _tempMemPtr;

            public JpegDecoderDst() { }
            public Stream Output => null;
            public MemBitmap MemBitmapOutput => _memBmp;
            public void BeginWrite()
            {
                _currentRowNo = 0;
            }
            public void EndWrite()
            {
                _tempMemPtr.Dispose();
            }
            public void SetImageAttributes(LoadedImageAttributes parameters)
            {
                _imgWidth = parameters.Width;
                _imgHeight = parameters.Height;
                _component = parameters.Components;
                _bitStream = new BitStream();

                _memBmp = new MemBitmap(_imgWidth, _imgHeight);
                _tempMemPtr = MemBitmap.GetBufferPtr(_memBmp);
            }
            public void ProcessPixelsRow(byte[] row)
            {
                //data row is decoded 
                _bitStream.ResetInput(row);
                //create long buffer for a single line 
                int index = 0;
                unsafe
                {
                    int* head = (int*)_tempMemPtr.Ptr;
                    head += (_currentRowNo * _imgWidth);//

                    //....

                    switch (_component)
                    {
                        case 3:
                            {
                                for (int col = 0; col < _imgWidth; ++col)
                                {
                                    byte b = row[index];
                                    byte g = row[index + 1];
                                    byte r = row[index + 2];
                                    index += 3;

                                    //store value
                                    *head = ((255 << 24) | (b << 16) | (g << 8) | r);
                                    head++;
                                }
                            }
                            break;
                        default: throw new NotSupportedException();
                    }
                }
                _currentRowNo++;
                //for (int i = 0; i < _imgWidth; ++i)
                //{
                //    //each component
                //    //eg. 1,2,3,4 
                //    switch (_component)
                //    {
                //        case 1:
                //            {

                //            }
                //            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                //            byteIndex++;
                //            break;
                //        case 2:
                //            {

                //            }
                //            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                //            _lineBuffer16[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                //            byteIndex += 2;
                //            break;
                //        case 3:
                //            {

                //            }
                //            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                //            _lineBuffer16[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                //            _lineBuffer16[byteIndex + 2] = (short)bitStream.Read(bitsPerComponent);
                //            byteIndex += 3;
                //            break;
                //        case 4:
                //            {

                //            }
                //            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                //            _lineBuffer16[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                //            _lineBuffer16[byteIndex + 2] = (short)bitStream.Read(bitsPerComponent);
                //            _lineBuffer16[byteIndex + 4] = (short)bitStream.Read(bitsPerComponent);
                //            byteIndex += 4;
                //            break;
                //        default:
                //            throw new NotSupportedException();
                //    }
                //}


            }

        }

        public MemBitmap LoadImage(Stream input, OutputImageFormat format)
        {
            ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
            //TODO: review img loading, we should not use only its extension
            switch (format)
            {
                case OutputImageFormat.Jpeg:
                    {

                        var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
                        var dst = new JpegDecoderDst();
                        extendedImg.JpegDecompressDest = dst;
                        extendedImg.Load(input, decoder);
                        //copy from 

                        return dst.MemBitmapOutput;

                    }
                    break;
                case OutputImageFormat.Png:
                    {
                        var decoder = new ImageTools.IO.Png.PngDecoder();
                        extendedImg.Load(input, decoder);
                    }
                    break;

                default:
                    throw new System.NotSupportedException();

            }

            //assume 32 bit ?? 
            byte[] pixels = extendedImg.Pixels;
            unsafe
            {
                fixed (byte* p_src = &pixels[0])
                {
                    PixelFarm.CpuBlit.MemBitmap memBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
                       extendedImg.PixelWidth,
                       extendedImg.PixelHeight,
                       (IntPtr)p_src,
                       pixels.Length,
                       false
                       );

                    memBmp.IsBigEndian = true;
                    return memBmp;
                }
            }

            ////PixelFarm.CpuBlit.MemBitmap memBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
            ////    extendedImg.PixelWidth,
            ////    extendedImg.PixelHeight,
            ////    extendedImg.PixelWidth * 4, //assume
            ////    32, //assume?
            ////    extendedImg.Pixels,
            ////    false
            ////    );
            ////the imgtools load data as BigEndian
            //memBmp.IsBigEndian = true;
            //return memBmp;
        }

        public override void SaveImage(MemBitmap bitmap, Stream output, OutputImageFormat outputFormat, object saveParameters)
        {
            switch (outputFormat)
            {
                case OutputImageFormat.Png:
                    PngIOStorage.Save(bitmap, output);
                    break;
                default:
                    throw new NotSupportedException();
            }

            //throw new NotImplementedException();
        }

        public override void SaveImage(MemBitmap bitmap, string filename, OutputImageFormat outputFormat, object saveParameters)
        {
            //using (FileStream fs = new FileStream(filename, FileMode.Create))
            //{
            //    SaveImage(bitmap, fs, outputFormat, saveParameters);
            //}
        }

        public override MemBitmap ScaleImage(MemBitmap bmp, float x_scale, float y_scale)
        {
            //TODO: implement this ...
            throw new NotImplementedException();
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

            int widthPx = imgInfo.Cols;
            int stride = widthPx * 4;
            //expand to 32 bits 
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


                    if (imgInfo.BitspPixel == 32)
                    {
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

                    }
                    else if (imgInfo.BitspPixel == 24)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            b_src += 3;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (255 << 24);
                            destIndex++;
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
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


                    if (imgInfo.BitspPixel == 32)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            byte a = scline[b_src + 3];
                            b_src += 4;
                            if (a > 0)
                            {

                            }
                            //buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                            buffer[destIndex] = -1;
                            destIndex++;
                        }
                        startWriteAt += imgW;
                    }
                    else if (imgInfo.BitspPixel == 24)
                    {
                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            if (g == 0)
                            {

                            }
                            b_src += 3;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (255 << 24);
                            destIndex++;
                        }
                        startWriteAt += imgW;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                }
                return MemBitmap.CreateFromCopy(imgW, imgH, buffer);
            }


        }
        public static void Save(MemBitmap bmp, Stream strm)
        {
            //-------------
            unsafe
            {
                PixelFarm.CpuBlit.TempMemPtr tmp = MemBitmap.GetBufferPtr(bmp);
                int* intBuffer = (int*)tmp.Ptr;

                int imgW = bmp.Width;
                int imgH = bmp.Height;

                Hjg.Pngcs.ImageInfo imgInfo = new Hjg.Pngcs.ImageInfo(imgW, imgH, 8, true); //8 bits per channel with alpha
                Hjg.Pngcs.PngWriter writer = new Hjg.Pngcs.PngWriter(strm, imgInfo);
                Hjg.Pngcs.ImageLine iline = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);



                bool flipYImg = true;


                int imgStride = imgW * 4;
                int srcIndex = 0;
                int srcIndexRowHead = (tmp.LengthInBytes / 4) - imgW;
                int startReadAt = 0;
                if (flipYImg)
                {
                    srcIndexRowHead = 0;
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
                        srcIndexRowHead += imgW;
                        startReadAt += imgStride;
                        writer.WriteRow(iline, row);
                    }
                }
                else
                {
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
                }

                writer.End();
            }


        }


    }


}