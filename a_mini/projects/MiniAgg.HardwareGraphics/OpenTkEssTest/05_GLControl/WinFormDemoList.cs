using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using Mini;

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
                        4,4,
                        3,4});


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

    public class CanvasGL2d
    {
        LayoutFarm.Drawing.Color fillColor = LayoutFarm.Drawing.Color.Black;
        public void Clear(LayoutFarm.Drawing.Color c)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(c);
        }
        public void FillPolygon(float[] vertex2dCoords)
        {

            //2d coods lis
            //n point 


        }
        public void FillRect(float x, float y, float w, float h)
        {

            //2d
            unsafe
            {
                float* arr = stackalloc float[8];
                arr[0] = x; arr[1] = y;
                arr[2] = x + w; arr[3] = y;
                arr[4] = x + w; arr[5] = y + h;
                arr[6] = x; arr[7] = y + h;

                byte* indices = stackalloc byte[6];
                indices[0] = 0; indices[1] = 1; indices[2] = 2;
                indices[3] = 2; indices[4] = 3; indices[5] = 0;

                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
            //GL.Begin(BeginMode.Triangles); 
            //GL.Vertex3(x, y, 0);//1
            //GL.Vertex3(x + w, y, 0);//2
            //GL.Vertex3(x + w, y + h, 0);//3 
            //GL.Vertex3(x + w, y + h, 0);//3
            //GL.Vertex3(x, y + h, 0);//4
            //GL.Vertex3(x, y, 0);//1 
            //GL.End();
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            unsafe
            {
                float* arr = stackalloc float[4];
                arr[0] = x1; arr[1] = y1;
                arr[2] = x2; arr[3] = y2;

                byte* indices = stackalloc byte[2];
                indices[0] = 0; indices[1] = 1;


                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        public void DrawPolygon(float[] polygon2dVertices)
        {
            //closed polyline
            //draw polyline
            unsafe
            {

                int npoints = polygon2dVertices.Length / 2;
                //crete indices
                int* indices = stackalloc int[npoints * 2]; 
                int nn = 0;
                for (int i = 1; i < npoints; ++i)
                {
                    indices[nn++] = i - 1;
                    indices[nn++] = i; 
                }
                //------------------
                indices[nn++] = npoints - 1;
                indices[nn++] = 0; 

                fixed (float* arr = &polygon2dVertices[0])
                {

                    GL.EnableClientState(ArrayCap.VertexArray); //***
                    //vertex 2d
                    GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                    GL.DrawElements(BeginMode.Lines, npoints * 2, DrawElementsType.UnsignedInt, (IntPtr)indices);
                    GL.DisableClientState(ArrayCap.VertexArray);
                }
            }
        }
        
        
        public LayoutFarm.Drawing.Color FillColor
        {
            get
            {
                return this.fillColor;
            }
            set
            {
                this.fillColor = value;
                GL.Color3(value);
            }
        }
    }


}