//MIT, 2011, Inedo, https://github.com/Inedo/iconmaker
//MIT, 2020, WinterDev
//see also https://en.wikipedia.org/wiki/ICO_(file_format)

using System;
using System.Collections.Generic;
using System.IO;

namespace IconMaker
{

    public abstract class Bitmap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int BitPerPixel { get; set; }

        internal int DataToSaveByteCount => DataToSave.Length;
        internal byte[] DataToSave { get; set; }
    }

    public class WindowBitmap : Bitmap
    {
        public int[] RawBitmapData { get; set; }
        public WindowBitmap(int w, int h)
        {
            Width = w;
            Height = h;
            BitPerPixel = 32;
        }
        internal void PrepareOutput()
        {
            DataToSave = GetImageData(this);
        }
        static byte[] GetImageData(WindowBitmap image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(memoryStream);
                if (image.Width < 256)
                {
                    int width = image.Width;
                    int height = width;//***, so please ensure that width== heigth
                    int pixelCount = width * height;

                    int maskWidth = width / 8;//mask is monochrome
                    if ((maskWidth % 4) != 0)
                    {
                        maskWidth += 3 - (maskWidth % 4);
                    }

                    //typedef struct tagBITMAPINFOHEADER {
                    //  DWORD biSize; //4
                    //  LONG  biWidth; //4
                    //  LONG  biHeight;//4
                    //  WORD  biPlanes; //2
                    //  WORD  biBitCount;//2
                    //  DWORD biCompression;//4
                    //  DWORD biSizeImage;//4
                    //  LONG  biXPelsPerMeter;//4
                    //  LONG  biYPelsPerMeter;//4
                    //  DWORD biClrUsed;//4
                    //  DWORD biClrImportant;//4
                    //}


                    writer.Write(40);   // size of BITMAPINFOHEADER
                    writer.Write(width);  // icon width/height
                    writer.Write(height * 2);  // icon height * 2 (AND plane)
                    writer.Write((short)1); // must be 1,Specifies the number of planes for the target device. This value must be set to 1.
                    writer.Write((short)32);    // bits per pixel
                    writer.Write(0);    // biCompression must be 0
                    writer.Write(pixelCount * 4 + maskWidth * height);//size of bitmap data, color =4 bytes per pixel +mask 1 width 8 height
                    writer.Write(new byte[4 * 4]);  //biXPelsPerMeter+biYPelsPerMeter+ biClrUsed+biClrImportant must be 0



                    //https://en.wikipedia.org/wiki/ICO_(file_format)
                    //The XOR mask must precede the AND mask inside the bitmap data;
                    //if the image is stored in bottom - up order(which it most likely is),
                    //the XOR mask would be drawn below the AND mask. 
                    //The AND mask is 1 bit per pixel,
                    //regardless of the color depth specified by the BMP header, 
                    //and specifies which pixels are fully transparent and which are fully opaque.
                    //The XOR mask conforms to the bit depth specified in the BMP header and specifies
                    //the numerical color or palette value for each pixel.

                    //XOR mask
                    int[] pixelData = image.RawBitmapData;

                    for (int y = height - 1; y >= 0; y--)
                    {
                        int src_index = y * width;
                        for (int x = 0; x < width; x++)
                        {
                            uint srcPixel = (uint)pixelData[src_index + x];
                            //check alpha channel
                            if ((srcPixel >> 24) != 0)
                            {
                                //byte r = (byte)(srcPixel & 0xff);
                                //byte g = (byte)((srcPixel >> 8) & 0xff);
                                //byte b = (byte)((srcPixel >> 16) & 0xff);

                                writer.Write((int)srcPixel);
                                //writer.Write(r);
                                //writer.Write(g);
                                //writer.Write(b);
                            }
                            else
                            {
                                //transparent 
                                writer.Write((int)0);
                                //writer.Write((byte)0);
                                //writer.Write((byte)0);
                                //writer.Write((byte)0);
                            }
                        }
                    }
                    //AND mask
                    for (int y = height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < width / 8; x++)
                        {
                            byte maskValue = 0;

                            for (int bit = 0; bit < 8; bit++)
                            {
                                uint srcPixel = (uint)pixelData[(y * width) + (x * 8) + bit];
                                //check alpha channel
                                if ((srcPixel >> 24) < 128)
                                    maskValue |= (byte)(1 << (7 - bit));
                            }

                            writer.Write(maskValue);
                        }

                        //padding per row
                        for (int padding = 0; padding < ((width / 8) % 4); padding++)
                            writer.Write((byte)0);
                    }
                }
                else
                {
                    //in this case, use png  
                    throw new NotSupportedException();
                }
                writer.Flush();
                return memoryStream.ToArray();
            }
        }
    }
    public class PngBitmap : Bitmap
    {
        public PngBitmap()
        {
        }
        public void SetPngFileContent(byte[] pngFileContent)
        {
            DataToSave = pngFileContent;
        }
    }


    public sealed class CursorFile
    {
        public class CursorBitmapInfo
        {
            public Bitmap Bitmap { get; set; }
            public int HotSpotX { get; set; }
            public int HotSpotY { get; set; }
        }
        List<CursorBitmapInfo> _bitmaps = new List<CursorBitmapInfo>();
        public void AddBitmap(CursorBitmapInfo cursor)
        {
            _bitmaps.Add(cursor);
        }

        public void AddBitmap(Bitmap bmp, int hotSpotX, int hotSpotY)
        {
            AddBitmap(new CursorBitmapInfo() { Bitmap = bmp, HotSpotX = hotSpotX, HotSpotY = hotSpotY });
        }

        public void Save(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            using (var stream = File.OpenWrite(fileName))
            {
                this.Save(stream);
            }
        }

        /// <summary>
        /// Saves the icon to a stream.
        /// </summary>
        /// <param name="stream">Stream into which icon is saved.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (_bitmaps.Count > 1)
            {
                _bitmaps.Sort((b1, b2) => b1.Bitmap.Width.CompareTo(b2.Bitmap.Width));
            }


            BinaryWriter writer = new BinaryWriter(stream);
            int offset = (_bitmaps.Count * 16) + 6;

            // Write the icon file header.
            writer.Write((ushort)0);    //reserved, must be 0
            writer.Write((ushort)2);    // 1 = ico file,2 =cur file
            writer.Write((ushort)_bitmaps.Count); // number of sizes          

            foreach (CursorBitmapInfo cursorInfo in _bitmaps)
            {
                Bitmap image = cursorInfo.Bitmap;
                if (image is WindowBitmap windowBmp)
                {
                    windowBmp.PrepareOutput();
                }


                //Offset# 	Size (in bytes) 	Purpose
                //0         1               Specifies image width in pixels.Can be any number between 0 and 255.Value 0 means image width is 256 pixels.
                //1         1               Specifies image height in pixels.Can be any number between 0 and 255.Value 0 means image height is 256 pixels.
                //2         1               Specifies number of colors in the color palette.Should be 0 if the image does not use a color palette.
                //3         1               Reserved.Should be 0.[Notes 2]
                //4         2               In ICO format: Specifies color planes.Should be 0 or 1.[Notes 3]
                //                          In CUR format: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
                //6         2               In ICO format: Specifies bits per pixel. [Notes 4]
                //                          In CUR format: Specifies the vertical coordinates of the hotspot in number of pixels from the top.
                //8 	    4 	            Specifies the size of the image's data in bytes
                //12 	    4 	            Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file

                writer.Write((byte)image.Width);  // width
                writer.Write((byte)image.Height);  // height
                writer.Write((byte)0);  //0=> not use color palette
                writer.Write((byte)0);  // must be 0,Reserved
                writer.Write((ushort)1);    // Specifies the horizontal coordinates of the hotspot in number of pixels from the left
                writer.Write((ushort)1);   // Specifies the vertical coordinates of the hotspot in number of pixels from the top. 
                writer.Write(image.DataToSaveByteCount);  // size of bitmap data in bytes
                writer.Write(offset);   // bitmap data offset in file

                offset += image.DataToSaveByteCount;

            }
            foreach (CursorBitmapInfo cursorInfo in _bitmaps)
            {
                writer.Write(cursorInfo.Bitmap.DataToSave);
            }
        }

    }


    public sealed class IconFile
    {
        List<Bitmap> _bitmaps = new List<Bitmap>();

        /// <summary>
        /// Initializes a new instance of the IconFile class.
        /// </summary>
        public IconFile()
        {
        }
        public void AddBitmap(Bitmap bmp)
        {
            _bitmaps.Add(bmp);
        }
        /// <summary>
        /// Saves the icon to a file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        public void Save(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            using (var stream = File.OpenWrite(fileName))
            {
                this.Save(stream);
            }
        }

        /// <summary>
        /// Saves the icon to a stream.
        /// </summary>
        /// <param name="stream">Stream into which icon is saved.</param>
        public void Save(Stream stream)
        {
            //sort image 
            //we assume bitmap is square,
            //we sort it by width
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (_bitmaps.Count > 1)
            {
                _bitmaps.Sort((b1, b2) => b1.Width.CompareTo(b2.Width));
            }


            BinaryWriter writer = new BinaryWriter(stream);
            var imageData = new Dictionary<int, byte[]>();

            int offset = (_bitmaps.Count * 16) + 6;

            // Write the icon file header.
            writer.Write((ushort)0);    // must be 0
            writer.Write((ushort)1);    // 1 = ico file,2 =cur file
            writer.Write((ushort)_bitmaps.Count); // number of sizes



            foreach (Bitmap image in _bitmaps)
            {
                if (image is WindowBitmap windowBmp)
                {
                    windowBmp.PrepareOutput();
                }
                writer.Write((byte)image.Width);  // width
                writer.Write((byte)image.Height);  // height
                writer.Write((byte)0);  // colors, 0 = more than 256
                writer.Write((byte)0);  // must be 0
                writer.Write((ushort)1);    // color planes, should be 0 or 1
                writer.Write((ushort)32);   // bits per pixel
                writer.Write(image.DataToSaveByteCount);  // size of bitmap data in bytes
                writer.Write(offset);   // bitmap data offset in file

                offset += image.DataToSaveByteCount;
            }

            foreach (Bitmap image in _bitmaps)
            {
                writer.Write(image.DataToSave);
            }
        }
    }
}
