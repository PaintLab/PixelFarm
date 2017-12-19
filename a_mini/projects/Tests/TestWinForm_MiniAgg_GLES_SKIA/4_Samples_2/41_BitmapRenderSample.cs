//BSD, 2014-2017, WinterDev

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
using PixelFarm.Drawing;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Imaging;
using Mini;
namespace PixelFarm.Agg.Sample_Images
{
    [Info(OrderCode = "41")]
    [Info("Basic Bitmap Rendering")]
    public class BasicBitmapRendering : DemoBase
    {
        ActualImage actualImage;
        AffinePlan[] affinePlans;
        public override void Init()
        {
            //actualImage2 = LoadImage(RootDemoPath.Path + "\\plain01.png");
            actualImage = LoadImage(RootDemoPath.Path + "\\02.jpg");

            affinePlans = new AffinePlan[] {
                AffinePlan.Translate(50, 50) };
        }

        static ActualImage LoadImage(string filename)
        {
            //read sample image
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename))
            {
                //read to image buffer 
                int bmpW = bmp.Width;
                int bmpH = bmp.Height;
                ActualImage img = new ActualImage(bmpW, bmpH, PixelFormat.ARGB32);
                BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(bmp, img);
                return img;
            }
        }

        AffinePlan[] affinePlan1 = new[] { AffinePlan.Translate(0, 200) }; //test
        public override void Draw(Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.DrawImage(actualImage, 0, 0);
            p.DrawImage(actualImage, 10, 10);
            //p.DrawImage(actualImage, affinePlan1);
            //p.DrawImage(actualImage, affinePlans);
            //p.DrawImage(actualImage, 100,200);

            //p.FillColor = Drawing.Color.Blue;
            //p.FillRectangle(0, 0, 5, 5);
            //p.FillColor = Drawing.Color.Green;
            //p.FillRectangle(actualImage.Width, actualImage.Height,
            //    actualImage.Width + 5, actualImage.Height + 5); 
        }
        public override void MouseDrag(int x, int y)
        {
        }
    }
}

