using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
namespace Mini2
{
    [Info(OrderCode = "02")]
    [Info("Drawing")]
    public class DrawSample02 : DemoBase
    {

        public override void Load()
        {
            //draw 1

            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            GLBitmap hwBmp = null;

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(PixelFarm.Drawing.Color.White);
                canvas.StrokeColor = PixelFarm.Drawing.Color.DeepPink;
                canvas.DrawLine(0, 300, 500, 300);

                //if (hwBmp == null)
                //{
                //    string app = Application.ExecutablePath;
                //    hwBmp = GLBitmapTextureHelper.CreateBitmapTexture(new Bitmap("../../../Data/Textures/logo-dark.jpg"));

                //}

                ////canvas.DrawImage(hwBmp, 10, 10);
                //canvas.DrawImage(hwBmp, 300, 300, hwBmp.Width / 4, hwBmp.Height / 4); 
                //-----------------------------------------------------
                canvas.StrokeColor = PixelFarm.Drawing.Color.Magenta;
                //draw line test 
                canvas.DrawLine(20, 20, 600, 200);
                //----------------------------------------------------- 
                var color = new PixelFarm.Drawing.Color(50, 255, 0, 0);  //  PixelFarm.Drawing.Color.Red;
                //rect polygon
                var polygonCoords = new float[]{
                        5,300,
                        40,300,
                        50,340,
                        10f,340};
                canvas.DrawPolygon(polygonCoords, 4);
                //fill polygon test                
                canvas.FillPolygon(color, polygonCoords);

                var polygonCoords2 = new float[]{
                        5+10,300,
                        40+10,300,
                        50+10,340,
                        10f +10,340};

                canvas.SmoothMode = CanvasSmoothMode.AggSmooth;
                canvas.StrokeColor = new PixelFarm.Drawing.Color(100, 0, 255, 0);  //  L
                canvas.DrawPolygon(polygonCoords2, 4);

                int strkW = 10;
                canvas.StrokeColor = PixelFarm.Drawing.Color.LightGray;

                for (int i = 1; i < 90; i += 10)
                {
                    canvas.StrokeWidth = strkW;
                    double angle = OpenTK.MathHelper.DegreesToRadians(i);
                    canvas.DrawLine(20, 400, (float)(600 * Math.Cos(angle)), (float)(600 * Math.Sin(angle)));

                    strkW--;
                    if (strkW < 1)
                    {
                        strkW = 1;
                    }
                }


                canvas.StrokeColor = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Green);

                //////---------------------------------------------
                //////draw ellipse and circle

                canvas.StrokeWidth = 0.75f;
                canvas.DrawCircle(400, 500, 50);
                canvas.FillCircle(PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Green), 450, 550, 25);

                canvas.StrokeWidth = 3;
                canvas.DrawRoundRect(500, 450, 100, 100, 10, 10);


                canvas.StrokeWidth = 3;
                canvas.StrokeColor = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Blue);

                //canvas.DrawBezierCurve(0, 0, 500, 500, 0, 250, 500, 250);
                canvas.DrawBezierCurve(120, 500 - 160, 220, 500 - 40, 35, 500 - 200, 220, 500 - 260);
                canvas.SmoothMode = CanvasSmoothMode.No;

                //canvas.DrawArc(150, 200, 300, 50, 0, 150, 150, SvgArcSize.Large, SvgArcSweep.Negative);
                canvas.DrawArc(100, 200, 300, 200, 30, 30, 50, SvgArcSize.Large, SvgArcSweep.Negative);

                canvas.StrokeColor = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Green);
                // canvas.DrawArc(100, 200, 300, 200, 0, 100, 100, SvgArcSize.Large, SvgArcSweep.Negative);

                canvas.StrokeColor = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Black);
                canvas.DrawLine(100, 200, 300, 200);


                //////load font data
                ////var font = PixelFarm.Agg.Fonts.NativeFontStore.LoadFont("c:\\Windows\\Fonts\\Tahoma.ttf", 64);
                ////var fontGlyph = font.GetGlyph('{');
                //////PixelFarm.Font2.MyFonts.SetShapingEngine();

                ////canvas.FillVxs(PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Black), fontGlyph.flattenVxs);

                ////canvas.StrokeColor = PixelFarm.Drawing.Color.White;
                ////canvas.CurrentFont = font;

                ////canvas.StrokeColor = PixelFarm.Drawing.Color.Black;
                ////canvas.DrawLine(0, 200, 500, 200);

                //////test Thai words
                ////canvas.DrawString("ดุดีดำด่าด่ำญญู", 80, 200);
                ////canvas.DrawString("1234567890", 80, 200);
                ////GLBitmap bmp = new GLBitmap(new LazyAggBitmapBufferProvider(fontGlyph.glyphImage32));
                ////canvas.DrawImage(bmp, 50, 50);
                ////bmp.Dispose();


            });
            form.Show();
        }
    }
}