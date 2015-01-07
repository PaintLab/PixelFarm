//BSD 2014, WinterDev

/*
Copyright (c) 2013, Lars Brubaker
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
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;

using Mini;
namespace PixelFarm.Agg.Sample_Images
{
    [Info(OrderCode = "41")]
    [Info("Basic Bitmap Rendering")]
    public class BasicBitmapRendering : DemoBase
    {
        ActualImage actualImage;
        public override void Init()
        {
            this.actualImage = LoadImage("../../../SampleImages/plain01.png");

        }

        static ActualImage LoadImage(string filename)
        {

            //read sample image
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename))
            {
                //read to image buffer 
                int bmpW = bmp.Width;
                int bmpH = bmp.Height;
                PixelFormat pixFormat = PixelFormat.Rgba32;
                ActualImage actualImage = new ActualImage(bmpW, bmpH, pixFormat);
                PixelFarm.Agg.Image.BitmapHelper.CopyFromWindowsBitmapSameSize(bmp, actualImage);
                
                return actualImage;
            }
        }
        public override void Draw(Graphics2D g)
        {

            CanvasPainter painter = new CanvasPainter(g);

            painter.DrawImage(actualImage, 0, 0);

            painter.DrawImage(actualImage,
                    AffinePlan.Translate(actualImage.Width *2, actualImage.Height*2),
                    AffinePlan.Scale(0.5));


            painter.FillColor = ColorRGBA.Blue;
            painter.FillRectangle(0, 0, 5, 5);
            painter.FillColor = ColorRGBA.Green;
            painter.FillRectangle(actualImage.Width, actualImage.Height,
                actualImage.Width + 5, actualImage.Height + 5);

        }
        public override void MouseDrag(int x, int y)
        {

        }


    }


}

