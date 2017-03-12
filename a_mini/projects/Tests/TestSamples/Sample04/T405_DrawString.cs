//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.Drawing.Text;
using PixelFarm.Agg;

namespace OpenTkEssTest
{
    [Info(OrderCode = "405")]
    [Info("T405_DrawString")]
    public class T405_DrawString : DemoBase
    {
        HarfBuzzShapingService hbShapingService;
        PixelFarm.Drawing.RequestFont font1;
        PixelFarm.Drawing.RequestFont font2;
        protected override void OnReadyForInitGLShaderProgram()
        {

        }
        protected override void OnPainterReady(CanvasPainter painter)
        {
            font1 = new PixelFarm.Drawing.RequestFont("tahoma", 11);
            font1.ScriptLang = PixelFarm.Drawing.Fonts.ScriptLangs.Thai; //for test complex script
            //
            font2 = new PixelFarm.Drawing.RequestFont("tahoma", 16);
            font2.ScriptLang = PixelFarm.Drawing.Fonts.ScriptLangs.Thai; //for test complex script
            painter.UseSubPixelRendering = true;
            painter.CurrentFont = font1;
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
            // string test_str = "อูญูอุบ่ป่กินกิ่นก็โก้";
            //string test_str = "ปู่";
            //string test_str = "ก็";
            string test_str = "abcd";
            //string test_str = "A";
            //string test_str = "012345";
            //string test_str = "กิน";
            p.Clear(PixelFarm.Drawing.Color.White);
            // p.Clear(PixelFarm.Drawing.Color.Blue);

            p.FillColor = PixelFarm.Drawing.Color.Red;
            p.UseSubPixelRendering = false;
            p.UseSubPixelRendering = true;
            int n = 50;
            for (int i = 0; i < n; i++)
            {
                //  p.DrawString(test_str, i * 10, i * 10);
                float x_pos = i * 20;
                float y_pos = i * 20;
                p.FillRectangle(x_pos, y_pos, x_pos + 5, y_pos + 5);
            }
            
            p.FillColor = PixelFarm.Drawing.Color.Black;
            for (int i = 0; i < n; i++)
            {
                float x_pos = i * 20;
                float y_pos = i * 20;
                //p.DrawString("(" + x_pos + "," + y_pos + ")", x_pos, y_pos);
                if ((i % 2) == 0)
                {
                    p.CurrentFont = font1;
                }
                else
                {
                    p.CurrentFont = font2;
                }
                p.DrawString(test_str, x_pos, y_pos);
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

