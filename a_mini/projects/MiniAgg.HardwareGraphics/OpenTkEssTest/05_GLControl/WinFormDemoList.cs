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

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(LayoutFarm.Drawing.Color.White);

                canvas.FillColor = LayoutFarm.Drawing.Color.Blue;
                canvas.FillRect(1, 1, 1f, 1f);

                canvas.FillColor = LayoutFarm.Drawing.Color.Green;

                //rect polygon
                canvas.DrawPolygon(
                    new float[]{
                        3,3,
                        4,3,
                        4.5f,4,
                        3.5f,4});

                //fill polygon test
                canvas.FillPolygon(
                  new float[]{
                        3,3,
                        4,3,
                        4.5f,4,
                        3.5f,4});


                canvas.DrawLine(1, 1, 1.5f, 3);


                //GL.Begin(BeginMode.Triangles);
                //GL.Vertex2(0, 1f);
                //GL.Vertex2(-1f, -1f);
                //GL.Vertex2(1f, -1f); 
                ////GL.Color3(LayoutFarm.Drawing.Color.Red); GL.Vertex2(0, 1f);  // GL.Vertex3(0.0f, 1.0f, 0.0f);
                ////GL.Color3(LayoutFarm.Drawing.Color.Green); GL.Vertex2(-1f, -1f); //GL.Vertex3(-1f, -1f, 0.0f);
                ////GL.Color3(LayoutFarm.Drawing.Color.Blue); GL.Vertex2(1f, -1f);  // GL.Vertex3(1f, -1f, 0.0f);
                ////GL.Color3(LayoutFarm.Drawing.Color.Blue); GL.Vertex2(1f, -1f); 

                //////GL.Vertex2(0, 1f); ;// GL.Vertex3(0.0f, 1.0f, 0.0f);
                //////GL.Vertex2(-1f, -1f); //GL.Vertex3(-1f, -1f, 0.0f);
                //////GL.Vertex2(1f, -1f); ;// GL.Vertex3(1f, -1f, 0.0f);

                //GL.End();
            });
            form.Show();
        }
    }

  

}