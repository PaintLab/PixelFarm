//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Text;
using PixelFarm.Agg;

namespace OpenTkEssTest
{
    [Info(OrderCode = "405")]
    [Info("T405_DrawString")]
    public class T405_DrawString : DemoBase
    {


        HarfBuzzShapingService hbShapingService;
        protected override void OnReadyForInitGLShaderProgram()
        {

        }
        public override void Draw(CanvasPainter p)
        {
            //canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            //canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            //canvas2d.ClearColorBuffer();
            //if (!resInit)
            //{
            //    resInit = true;
            //}
            //painter.Clear(PixelFarm.Drawing.Color.White);
            //painter.DrawString("hello world!", 100, 100);            
            //painter.DrawString("กิ่น", 100, 100);
            //string test_str = "อูญูอุบ่ป่กินกิ่นก็โก้"; 
            p.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 10); 
            p.Clear(PixelFarm.Drawing.Color.White);

            string test_str = "012345";
            p.FillColor = PixelFarm.Drawing.Color.Red;
            //string test_str = "กิน";
            int n = 50;
            for (int i = 0; i < n; i++)
            {
                //  p.DrawString(test_str, i * 10, i * 10);
                float x_pos = i * 10;
                float y_pos = i * 10;
                p.FillRectangle(x_pos, y_pos, x_pos + 5, y_pos + 5);
            }

            p.FillColor = PixelFarm.Drawing.Color.Black;
            for (int i = 0; i < n; i++)
            {

                float x_pos = i * 10;
                float y_pos = i * 10;
                p.DrawString("(" + x_pos + "," + y_pos + ")", x_pos, y_pos);
            }
            //PixelFarm.Drawing.GLES2.GLES2Platform.AddTextureFont("tahoma",
            //     "d:\\WImageTest\\a_total.xml",
            //     "d:\\WImageTest\\a_total.png"); 
            //////------------------------------------------------
            //hbShapingService = new HarfBuzzShapingService();
            //hbShapingService.SetAsCurrentImplementation(); 
            //------------------------------------------------ 
            //string fontName = "tahoma";
            //float fontSize = 24;
            //GlyphImage glypImage = null;
            //using (var nativeImg = new PixelFarm.Drawing.Imaging.NativeImage("d:\\WImageTest\\a_total.png"))
            //{
            //    glypImage = new GlyphImage(nativeImg.Width, nativeImg.Height);
            //    var buffer = new int[nativeImg.Width * nativeImg.Height];
            //    System.Runtime.InteropServices.Marshal.Copy(nativeImg.GetNativeImageHandle(), buffer, 0, buffer.Length);
            //    glypImage.SetImageBuffer(buffer, true);
            //}

            //textureFont = TextureFont.CreateFont(fontName, fontSize,
            //    "d:\\WImageTest\\a_total.xml",
            //    glypImage);

            ////PixelFarm.Drawing.RequestFont f = new PixelFarm.Drawing.RequestFont(fontName, fontSize); 
            ////canvas2d.TextureFontStore = textureFonts;
            ////painter.CurrentFont = textureFont;
            //painter.ActualFont = textureFont;
        }
    }
}

