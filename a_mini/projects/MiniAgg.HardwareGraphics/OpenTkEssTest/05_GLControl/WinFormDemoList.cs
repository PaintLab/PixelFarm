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

                canvas.FillColor = LayoutFarm.Drawing.Color.Green;
                //draw line test
                canvas.DrawLine(1, 1, 100f, 500);

                canvas.DrawLineAggAA(20, 20, 120f, 525);
                canvas.FillColor = LayoutFarm.Drawing.Color.Red;

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