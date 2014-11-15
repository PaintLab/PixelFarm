//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Tesselate;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

namespace OpenTkEssTest
{

    public partial class CanvasGL2d
    {
        LayoutFarm.Drawing.Color fillColor = LayoutFarm.Drawing.Color.Black;

        Tesselator tess = new Tesselator();
        TessListener2 tessListener = new TessListener2();
        //tools---------------------------------
        Ellipse ellipse = new Ellipse();
        PathStorage ps = new PathStorage();
        Stroke stroke1 = new Stroke(1);
        GLScanlineRasterizer sclineRas;
        GLScanlineRasToDestBitmapRenderer sclineRasToGL;
        GLScanlinePacked8 sclinePack8;


        public CanvasGL2d()
        {
            sclineRas = new GLScanlineRasterizer();
            sclineRasToGL = new GLScanlineRasToDestBitmapRenderer();
            sclinePack8 = new GLScanlinePacked8();
            tessListener.Connect(tess, Tesselate.Tesselator.WindingRuleType.Odd, true);
        }

        public CanvasSmoothMode SmoothMode
        {
            get;
            set;
        }

        public void Clear(LayoutFarm.Drawing.Color c)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(c);
        }
        public double StrokeWidth
        {
            get { return this.stroke1.Width; }
            set { this.stroke1.Width = value; }
        }


        public void DrawLine(float x1, float y1, float x2, float y2)
        {

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //--------------------------------------
                        ps.Clear();
                        ps.MoveTo(x1, y1);
                        ps.LineTo(x2, y2);
                        VertexStore vxs = stroke1.MakeVxs(ps.Vxs);
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
                        //--------------------------------------
                    } break;
                default:
                    {
                        unsafe
                        {
                            float* arr = stackalloc float[4];
                            arr[0] = x1; arr[1] = y1;
                            arr[2] = x2; arr[3] = y2;
                            //byte* indices = stackalloc byte[2];
                            //indices[0] = 0; indices[1] = 1;

                            GL.EnableClientState(ArrayCap.VertexArray); //***
                            //vertex
                            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                            //GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedByte, (IntPtr)indices);
                            GL.DrawArrays(BeginMode.Lines, 0, 2);
                            GL.DisableClientState(ArrayCap.VertexArray);
                        }
                    } break;

            }
        }


        public void DrawImage(GLBitmapTexture bmp, float x, float y)
        {
            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.TextureId);
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    float* arr = stackalloc float[8];
                    arr[0] = 0; arr[1] = 1;
                    arr[2] = 1; arr[3] = 1;
                    arr[4] = 1; arr[5] = 0;
                    arr[6] = 0; arr[7] = 0;

                    byte* indices = stackalloc byte[6];
                    indices[0] = 0; indices[1] = 1; indices[2] = 2;
                    indices[3] = 2; indices[4] = 3; indices[5] = 0;

                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //------------------------------------------ 
                    //fill rect with texture
                    FillRect(x, y, bmp.Width, bmp.Height);
                    GL.DisableClientState(ArrayCap.TextureCoordArray);

                } GL.Disable(EnableCap.Texture2D);
            }
        }
        public void DrawImage(GLBitmapTexture bmp, float x, float y, float w, float h)
        {
            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.TextureId);
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    float* arr = stackalloc float[8];
                    arr[0] = 0; arr[1] = 1;
                    arr[2] = 1; arr[3] = 1;
                    arr[4] = 1; arr[5] = 0;
                    arr[6] = 0; arr[7] = 0;

                    byte* indices = stackalloc byte[6];
                    indices[0] = 0; indices[1] = 1; indices[2] = 2;
                    indices[3] = 2; indices[4] = 3; indices[5] = 0;

                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //------------------------------------------ 
                    //fill rect with texture
                    FillRect(x, y, w, h);
                    GL.DisableClientState(ArrayCap.TextureCoordArray);

                } GL.Disable(EnableCap.Texture2D);
            }
        }


        public void DrawPolygon(float[] polygon2dVertices, int npoints)
        {
            //closed polyline
            //draw polyline
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //draw polyon

                        ps.Clear();
                        //closed polygon
                        int j = npoints / 2;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(polygon2dVertices[0], polygon2dVertices[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(polygon2dVertices[nn++],
                                polygon2dVertices[nn++]);
                        }
                        //close
                        ps.ClosePolygon();

                        VertexStore vxs = stroke1.MakeVxs(ps.Vxs);
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);
                        //--------------------------------------


                    } break;
                default:
                    {
                        unsafe
                        {
                            fixed (float* arr = &polygon2dVertices[0])
                            {
                                DrawPolygonUnsafe(arr, npoints);
                            }
                        }
                    } break;

            }

        }

        public void DrawEllipse(float x, float y, double rx, double ry)
        {

            ellipse.Reset(x, y, rx, ry);

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {

                        VertexStore vxs = stroke1.MakeVxs(ellipse.MakeVxs());
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.fillColor);

                    } break;
                default:
                    {
                        VertexStore vxs = ellipse.MakeVxs();
                        int n = vxs.Count;
                        unsafe
                        {
                            float* coords = stackalloc float[n * 2];
                            int i = 0;
                            int nn = 0;
                            double vx, vy;
                            var cmd = vxs.GetVertex(i, out vx, out vy);
                            while (i < n)
                            {
                                switch (cmd)
                                {
                                    case ShapePath.FlagsAndCommand.CommandMoveTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        } break;
                                    case ShapePath.FlagsAndCommand.CommandLineTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        } break;
                                    case ShapePath.FlagsAndCommand.CommandStop:
                                        {
                                        } break;
                                    default:
                                        {

                                        } break;
                                }
                                i++;
                                cmd = vxs.GetVertex(i, out vx, out vy);
                            }
                            //-------------------------------------- 
                            DrawPolygonUnsafe(coords, nn / 2);

                        }
                    } break;
            }

        }
        public void DrawCircle(float x, float y, double radius)
        {

            DrawEllipse(x, y, radius, radius);
        }

        //==================================================================================
        public LayoutFarm.Drawing.Color FillColor
        {
            get
            {
                return this.fillColor;
            }
            set
            {
                this.fillColor = value;
                GL.Color4(value);
            }
        }
        public void FillRect(float x, float y, float w, float h)
        {
            //2d
            unsafe
            {
                float* arr = stackalloc float[8];
                byte* indices = stackalloc byte[6];
                CreateRectCoords(arr, indices, x, y, w, h);
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        public void FillEllipse(float x, float y, double rx, double ry)
        {
            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs();
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {   
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, this.fillColor);
                        return;
                    }
            }

            int n = vxs.Count;
            //make triangular fan*** 
            unsafe
            {
                float* coords = stackalloc float[(n * 2) + 4];

                int i = 0;
                int nn = 0;
                int npoints = 0;
                double vx, vy;
                //center
                coords[nn++] = (float)x;
                coords[nn++] = (float)y;
                npoints++;
                var cmd = vxs.GetVertex(i, out vx, out vy);

                while (i < n)
                {
                    switch (cmd)
                    {
                        case ShapePath.FlagsAndCommand.CommandMoveTo:
                            {
                                coords[nn++] = (float)vx;
                                coords[nn++] = (float)vy;
                                npoints++;
                            } break;
                        case ShapePath.FlagsAndCommand.CommandLineTo:
                            {
                                coords[nn++] = (float)vx;
                                coords[nn++] = (float)vy;
                                npoints++;
                            } break;
                        case ShapePath.FlagsAndCommand.CommandStop:
                            {
                            } break;
                        default:
                            {

                            } break;
                    }
                    i++;
                    cmd = vxs.GetVertex(i, out vx, out vy);
                }

                //close circle
                coords[nn++] = coords[2];
                coords[nn++] = coords[3];
                npoints++;

                //int* indx = stackalloc int[npoints];
                //for (i = 0; i < npoints; ++i)
                //{
                //    indx[i] = i;
                //}

                //fill triangular fan
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex 2d
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                //GL.DrawElements(BeginMode.TriangleFan, npoints, DrawElementsType.UnsignedInt, (IntPtr)indx);
                GL.DrawArrays(BeginMode.TriangleFan, 0, npoints);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        public void FillCircle(float x, float y, double radius)
        {
            FillEllipse(x, y, radius, radius);
        }

        public void FillPolygon(float[] vertex2dCoords)
        {
            FillPolygon(vertex2dCoords, vertex2dCoords.Length);
        }
        public void FillPolygon(float[] vertex2dCoords, int npoints)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point 
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //closed polygon

                        //closed polygon
                        int j = npoints / 2;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(vertex2dCoords[0], vertex2dCoords[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(vertex2dCoords[nn++],
                                vertex2dCoords[nn++]);
                        }
                        //close
                        ps.ClosePolygon();
                        VertexStore vxs = ps.Vxs;
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, this.fillColor);
                        //-------------------------------------- 

                    } break;
                default:
                    {
                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------
                        FillTriangles(vertextList);
                        //-----------------------------
                    } break;
            }

        }

    }
}