//BSD, 2014-2016, WinterDev

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
namespace PixelFarm.Agg.Imaging
{


    public class GdiBitmapBackBuffer : IDisposable
    {
        ActualImage actualImage;
        int width;
        int height;
        //------------------------------------
        Win32.NativeWin32MemoryDc nativeWin32Dc;

        public GdiBitmapBackBuffer()
        {
        }
        public void Dispose()
        {
            if (nativeWin32Dc != null)
            {
                nativeWin32Dc.Dispose();
                nativeWin32Dc = null;
            }
        }


        const int SRCCOPY = 0xcc0020;
        /// <summary>
        /// copy buffer to 
        /// </summary>
        /// <param name="dest"></param>
        public void UpdateToHardwareSurface(IntPtr displayHdc)
        {

            BitmapHelper.CopyToWindowsBitmapSameSize(
                this.actualImage,   //src from actual img buffer
                nativeWin32Dc.PPVBits);//dest to buffer bmp     

            bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
                 width,
                 height,
                 nativeWin32Dc.DC, 0, 0, SRCCOPY);
        }
        public void Initialize(int width, int height, int bitDepth, ActualImage actualImage)
        {
            if (width > 0 && height > 0)
            {
                this.width = width;
                this.height = height;

                //if (bitDepth != 32)
                //{
                //    throw new NotImplementedException("Don't support this bit depth yet.");
                //}
                //else
                //{
                //    actualImage = new ActualImage(width, height, PixelFormat.ARGB32);
                this.actualImage = actualImage;
                nativeWin32Dc = new Win32.NativeWin32MemoryDc(width, height, true);
                //    return Graphics2D.CreateFromImage(actualImage);
                //}
                return;
            }
            throw new NotSupportedException();
        }


    }
}
