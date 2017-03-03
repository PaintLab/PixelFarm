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

        //bool resInit;
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        HarfBuzzShapingService hbShapingService;

        
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            var prebuiltContext = sender as PrebuiltContext;
            if (prebuiltContext != null)
            {
                canvas2d = prebuiltContext.gl2dCanvas;
                painter = prebuiltContext.glCanvasPainter;
                painter.FillColor = PixelFarm.Drawing.Color.Black;
                painter.StrokeColor = PixelFarm.Drawing.Color.Black;
            }
            else
            {
                canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                painter = new GLCanvasPainter(canvas2d, max, max);
                painter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);
                //choose printer type
                //1. agg 
                painter.TextPrinter = new AggFontPrinter(painter, 200, 50);
            }
         
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
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
            p.Clear(PixelFarm.Drawing.Color.White);
            string test_str = "AAAAA";
            //string test_str = "กิน";
            p.DrawString(test_str, 300, 400);


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

