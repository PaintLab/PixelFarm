/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using MatterHackers.Agg;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.RasterizerScanline;

namespace Mini
{
    internal class WindowsFormsBitmapBackBuffer
    {
        internal ImageBuffer backingImageBufferByte;
        internal ImageBufferFloat backingImageBufferFloat;
        internal Bitmap windowsBitmap;

         
        public WindowsFormsBitmapBackBuffer()
        {

        }
        

        int numInFunction = 0;
        internal void UpdateHardwareSurface(RectangleInt rect)
        {

            numInFunction++;
            if (backingImageBufferByte != null)
            {

                BitmapHelper.CopyToWindowsBitmap(
                    this.backingImageBufferByte,
                    this.windowsBitmap,
                    rect);
            }
            else
            {
                switch (backingImageBufferFloat.BitDepth)
                {
                    case 128:
                        {
                            BitmapData bitmapData1 = windowsBitmap.LockBits(new Rectangle(0, 0, windowsBitmap.Width, windowsBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, windowsBitmap.PixelFormat);
                            int index = 0;
                            unsafe
                            {
                                unchecked
                                {
                                    int offset;
                                    float[] buffer = backingImageBufferFloat.GetBuffer(out offset);
                                    fixed (float* pSource = &buffer[offset])
                                    {
                                        for (int y = 0; y < backingImageBufferFloat.Height; y++)
                                        {
                                            byte* pDestBuffer = (byte*)bitmapData1.Scan0 + (bitmapData1.Stride * (backingImageBufferFloat.Height - 1 - y));
                                            for (int x = 0; x < backingImageBufferFloat.Width; x++)
                                            {
#if true
                                                pDestBuffer[x * 4 + 0] = (byte)(pSource[index * 4 + 0] * 255);
                                                pDestBuffer[x * 4 + 1] = (byte)(pSource[index * 4 + 1] * 255);
                                                pDestBuffer[x * 4 + 2] = (byte)(pSource[index * 4 + 2] * 255);
                                                pDestBuffer[x * 4 + 3] = (byte)(pSource[index * 4 + 3] * 255);
                                                index++;
#else
                                                pDestBuffer[x * 4 + 0] = (byte)255;
                                                pDestBuffer[x * 4 + 1] = (byte)0;
                                                pDestBuffer[x * 4 + 2] = (byte)128;
                                                pDestBuffer[x * 4 + 3] = (byte)255;
#endif
                                            }
                                        }
                                    }
                                }
                            }

                            windowsBitmap.UnlockBits(bitmapData1);
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            numInFunction--;
        }

        internal void Initialize(int width, int height, int bitDepth)
        {
            if (width > 0 && height > 0)
            {
                switch (bitDepth)
                {
                    case 24:
                        windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        backingImageBufferByte = new ImageBuffer(width, height, 24, new BlenderBGR());
                        break;

                    case 32:
                        windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        //widowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        //widowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        //32bppPArgb 
                        backingImageBufferByte = new ImageBuffer(width, height, 32, new BlenderBGRA());
                        break;

                    case 128:
                        windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        backingImageBufferByte = null;
                        backingImageBufferFloat = new ImageBufferFloat(width, height, 128, new BlenderBGRAFloat());
                        break;

                    default:
                        throw new NotImplementedException("Don't support this bit depth yet.");
                }
            }
        }
    }
}
