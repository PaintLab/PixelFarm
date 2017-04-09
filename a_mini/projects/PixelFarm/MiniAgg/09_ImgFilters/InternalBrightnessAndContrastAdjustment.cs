//BSD, 
namespace PixelFarm.Agg
{

    /////////////////////////////////////////////////////////////////////////////////
    // Paint.NET                                                                   //
    // Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
    // Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
    // See src/Resources/Files/License.txt for full licensing and attribution      //
    // details.                                                                    //
    // .                                                                           //
    /////////////////////////////////////////////////////////////////////////////////


    class InternalBrightnessAndContrastAdjustment
    {
        int brightness;
        int contrast;
        byte[] rgbTable;

        public InternalBrightnessAndContrastAdjustment()
        {
        }
        public void SetParameters(int brightness, int contrast)
        {
            int multiply;
            int divide;
            this.brightness = brightness;
            this.contrast = contrast;
            if (this.contrast < 0)
            {
                multiply = this.contrast + 100;
                divide = 100;
            }
            else if (this.contrast > 0)
            {
                multiply = 100;
                divide = 100 - this.contrast;
            }
            else
            {
                multiply = 1;
                divide = 1;
            }

            if (this.rgbTable == null)
            {
                this.rgbTable = new byte[65536];//256*256
            }
            if (divide == 0)
            {
                for (int intensity = 0; intensity < 256; ++intensity)
                {
                    if (intensity + brightness < 128)
                    {
                        this.rgbTable[intensity] = 0;
                    }
                    else
                    {
                        this.rgbTable[intensity] = 255;
                    }
                }
            }
            else if (divide == 100)
            {
                for (int intensity = 0; intensity < 256; ++intensity)
                {
                    int shift = (intensity - 127) * multiply / divide + 127 - intensity + brightness;

                    for (int col = 0; col < 256; ++col)
                    {
                        int index = (intensity * 256) + col;
                        this.rgbTable[index] = PixelUtils.ClampToByte(col + shift);
                    }
                }
            }
            else
            {
                for (int intensity = 0; intensity < 256; ++intensity)
                {
                    int shift = (intensity - 127 + brightness) * multiply / divide + 127 - intensity;

                    for (int col = 0; col < 256; ++col)
                    {
                        int index = (intensity * 256) + col;
                        this.rgbTable[index] = PixelUtils.ClampToByte(col + shift);
                    }
                }
            }


            //if (divide == 0)
            //{
            //    for (int intensity = 0; intensity < 256; ++intensity)
            //    {
            //        if (intensity + brightness < 128)
            //        {
            //            this.rgbTable[intensity] = 0;
            //        }
            //        else
            //        {
            //            this.rgbTable[intensity] = 255;
            //        }
            //    }
            //}
            //else if (divide == 100)
            //{
            //    for (int intensity = 0; intensity < 256; ++intensity)
            //    {
            //        int shift = (intensity - 127) * multiply / divide + 127 - intensity + brightness;

            //        for (int col = 0; col < 256; ++col)
            //        {
            //            int index = (intensity * 256) + col;
            //            this.rgbTable[index] = PixelUtils.ClampToByte(col + shift);
            //        }
            //    }
            //}
            //else
            //{
            //    for (int intensity = 0; intensity < 256; ++intensity)
            //    {
            //        int shift = (intensity - 127 + brightness) * multiply / divide + 127 - intensity;

            //        for (int col = 0; col < 256; ++col)
            //        {
            //            int index = (intensity * 256) + col;
            //            this.rgbTable[index] = PixelUtils.ClampToByte(col + shift);
            //        }
            //    }
            //}
        }
        public void ApplyBytes(byte r0, byte g0, byte b0, out byte r1, out byte g1, out byte b1)
        {
            //this is my extension, to subpixel lut  

            int intensity = GetIntensityByte(b0, g0, r0);
            int shiftIndex = intensity * 256;

            r1 = this.rgbTable[shiftIndex + r0];
            g1 = this.rgbTable[shiftIndex + g0];
            b1 = this.rgbTable[shiftIndex + b0];


        }
        public void ApplyGrayScale(byte[] srcBuffer, byte[] destBuffer)
        {
            //this is my extension, to subpixel lut 

            int j = srcBuffer.Length;
            for (int i = 0; i < j; ++i)
            {
                byte level = srcBuffer[i];
                int intensity = GetIntensityByte(level, level, level);
                int shiftIndex = intensity * 256;
                destBuffer[i] = rgbTable[shiftIndex + level];
            }

        }
        public void Apply32BGRA(byte[] srcBuffer, byte[] destBuffer, int stride, int h)
        {


            int p = 0;
            for (int row = 0; row < h; ++row)
            {
                p = row * stride;
                for (int i = 0; i < stride;)
                {
                    //get color from src
                    //pass to contrast filter
                    //put value to dest**
                    byte b = srcBuffer[p];
                    byte g = srcBuffer[p + 1];
                    byte r = srcBuffer[p + 2];
                    byte a = srcBuffer[i + 3];
                    //------------------------
                    int intensity = GetIntensityByte(b, g, r);
                    int shiftIndex = intensity * 256;

                    byte new_r = this.rgbTable[shiftIndex + r];
                    byte new_g = this.rgbTable[shiftIndex + g];
                    byte new_b = this.rgbTable[shiftIndex + b];

                    //save to dest
                    destBuffer[p] = new_b;
                    destBuffer[p + 1] = new_g;
                    destBuffer[p + 2] = new_r;
                    destBuffer[p + 3] = a;
                    i += 4;
                    p += 4;
                }
            }
        }
        static byte GetIntensityByte(byte b, byte g, byte r)
        {
            return (byte)((7471 * b + 38470 * g + 19595 * r) >> 16);
        }
        static class PixelUtils
        {
            public static byte ClampToByte(int x)
            {
                if (x > 255)
                {
                    return 255;
                }
                else if (x < 0)
                {
                    return 0;
                }
                else
                {
                    return (byte)x;
                }
            }
        }
    }
}