//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Fonts;
namespace OpenTkEssTest
{
    [Info(OrderCode = "405")]
    [Info("T405_DrawString")]
    public class T405_DrawString : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        bool resInit;

        GLCanvasPainter painter;
        TextureFont textureFont;

        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);

            textureFont = TextureFontBuilder.CreateFont("tahoma",
                "d:\\WImageTest\\a_total.xml",
                "d:\\WImageTest\\a_total.png");

            PixelFarm.Drawing.Font f = new PixelFarm.Drawing.Font();
            f.Name = "tahoma";
            f.EmSize = 24;
            f.SetTextureFont(textureFont);
            painter.CurrentFont = f;
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            if (!resInit)
            {
                resInit = true;
            }
            painter.Clear(PixelFarm.Drawing.Color.White);
            //painter.DrawString("hello world!", 100, 100);            
            //painter.DrawString("กิ่น", 100, 100);
            string test_str = "อูญูอุบ่ป่กินกิ่นก็โก้";
            //string test_str = "กิน";
            painter.DrawString(test_str, 100, 100);
            //painter.DrawString("hello world! กิ่น", 100, 100);
            miniGLControl.SwapBuffers();
        }
    }
}

