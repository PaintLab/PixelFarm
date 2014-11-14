using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using Mini;
using Tesselate;


namespace OpenTkEssTest
{
    [Info(OrderCode = "21")]
    [Info("T21_TestWinGLControl")]
    public class T21_TestWinGLControl : DemoBase
    {
        public override void Init()
        {
            FormTestWinGLControl form = new FormTestWinGLControl();
            form.Show();

        }
    }
    [Info(OrderCode = "22")]
    [Info("T22_DemoWinGLControl")]
    public class T22_FormTestWinGLControlDemo2 : DemoBase
    {
        public override void Init()
        {
            FormGLControlSimple form = new FormGLControlSimple();
            form.Show();
        }
    }

    [Info(OrderCode = "23")]
    [Info("T23_FormMultipleGLControlsFormDemo")]
    public class T23_FormMultipleGLControlsFormDemo : DemoBase
    {
        public override void Init()
        {
            FormMultipleGLControlsForm form = new FormMultipleGLControlsForm();
            form.Show();
        }
    }
    [Info(OrderCode = "24")]
    [Info("T24_FormMultipleGLControlsFormDemo2")]
    public class T24_FormMultipleGLControlsFormDemo2 : DemoBase
    {
        public override void Init()
        {
            FormTestWinGLControl2 form = new FormTestWinGLControl2();
            CanvasGL2d canvas = new CanvasGL2d();
            GLBitmapTexture hwBmp = null;

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(LayoutFarm.Drawing.Color.White);


                //canvas.FillColor = LayoutFarm.Drawing.Color.Blue;
                //canvas.FillRect(1, 1, 1f, 1f); 
                if (hwBmp == null)
                {
                    using (Bitmap bitmap = new Bitmap("../../Data/Textures/logo-dark.jpg"))
                    {
                        hwBmp = new GLBitmapTexture(bitmap);
                    }
                }

                canvas.DrawImage(hwBmp, 10, 10);
                canvas.DrawImage(hwBmp, 300, 300, hwBmp.Width / 4, hwBmp.Height / 4);

                canvas.FillColor = new LayoutFarm.Drawing.Color(50, 255, 0, 0);  //  LayoutFarm.Drawing.Color.Red;
                //rect polygon
                var polygonCoords = new float[]{
                        5,300,
                        40,300,
                        50,340,
                        10f,340};
                //canvas.DrawPolygon(polygonCoords);


                //fill polygon test
                canvas.FillPolygon(polygonCoords);

                var polygonCoords2 = new float[]{
                        5+10,300,
                        40+10,300,
                        50+10,340,
                        10f +10,340};
                canvas.FillColor = new LayoutFarm.Drawing.Color(100, 0, 255, 0);  //  L
                canvas.FillPolygon(polygonCoords2);

                canvas.FillColor = LayoutFarm.Drawing.Color.Magenta;
                //draw line test

                canvas.DrawLine(20, 20, 600, 200);
                int strkW = 10;

                canvas.FillColor = LayoutFarm.Drawing.Color.Black;
                canvas.SmoothMode = CanvasSmoothMode.AggSmooth;
                for (int i = 1; i < 90; i += 10)
                {
                    canvas.StrokeWidth = strkW;
                    double angle = OpenTK.MathHelper.DegreesToRadians(i);
                    canvas.DrawLine(20, 20, (float)(600 * Math.Cos(angle)), (float)(600 * Math.Sin(angle)));

                    strkW--;
                    if (strkW < 1)
                    {
                        strkW = 1;
                    }
                }
                canvas.SmoothMode = CanvasSmoothMode.No;
                canvas.FillColor = LayoutFarm.Drawing.Color.Green;

                ////---------------------------------------------
                ////draw ellipse and circle
                canvas.DrawCircle(400, 500, 50);

                canvas.FillCircle(450, 550, 25);

                canvas.FillColor = LayoutFarm.Drawing.Color.White;
                //--------------------------------------------- 


            });
            form.Show();
        }
    }



}