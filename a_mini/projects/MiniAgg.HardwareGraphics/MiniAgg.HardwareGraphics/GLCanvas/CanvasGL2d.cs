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

    public class CanvasGL2d
    {
        LayoutFarm.Drawing.Color fillColor = LayoutFarm.Drawing.Color.Black;
        //tools
        Ellipse ellipse = new Ellipse();
        PathStorage ps = new PathStorage();
        Stroke stroke1 = new Stroke(1);
        GLScanlineRasterizer sclineRas;
        GLScanlineRasToDestBitmapRenderer sclineRasToBmp;
        GLScanlinePacked8 sclinePack8;

        public CanvasGL2d()
        {
            sclineRas = new GLScanlineRasterizer();
            sclineRasToBmp = new GLScanlineRasToDestBitmapRenderer();
            sclinePack8 = new GLScanlinePacked8();

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
        static unsafe void CreateRectCoords(float* arr, byte* indices,
                float x, float y, float w, float h)
        {
            arr[0] = x; arr[1] = y;
            arr[2] = x + w; arr[3] = y;
            arr[4] = x + w; arr[5] = y + h;
            arr[6] = x; arr[7] = y + h;

            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 2; indices[4] = 3; indices[5] = 0;
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
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            unsafe
            {
                switch (this.SmoothMode)
                {
                    case CanvasSmoothMode.AggSmooth:
                        {
                            DrawLineAggAA(x1, y1, x2, y2);
                        } break;
                    default:
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
                        } break;
                }

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
        public void DrawPolygon(float[] polygon2dVertices)
        {
            DrawPolygon(polygon2dVertices, polygon2dVertices.Length / 2);
        }
        public void DrawPolygon(float[] polygon2dVertices, int npoints)
        {
            //closed polyline
            //draw polyline
            unsafe
            {
                //crete indices
                int* indices = stackalloc int[npoints * 2];
                int nn = 0;
                for (int i = 1; i < npoints; ++i)
                {
                    indices[nn++] = i - 1;
                    indices[nn++] = i;
                }
                //------------------
                //close polygon
                indices[nn++] = npoints - 1;
                indices[nn++] = 0;

                fixed (float* arr = &polygon2dVertices[0])
                {
                    GL.EnableClientState(ArrayCap.VertexArray); //***
                    //vertex 2d
                    GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                    GL.DrawElements(BeginMode.LineLoop, npoints * 2, DrawElementsType.UnsignedInt, (IntPtr)indices);
                    GL.DisableClientState(ArrayCap.VertexArray);
                }
            }
        }

        public void FillPolygon(float[] vertex2dCoords)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point 
            var vertextList = TessealatePolygon(vertex2dCoords);
            //-----------------------------
            FillTriangles(vertextList);
            //-----------------------------
        }


        static List<Vertex> TessealatePolygon(float[] vertex2dCoords)
        {
            TessListener t01 = new TessListener();
            Tesselator tess = new Tesselator();
            int ncoords = vertex2dCoords.Length / 2;
            List<Vertex> vertexts = new List<Vertex>(ncoords);
            int nn = 0;
            for (int i = 0; i < ncoords; ++i)
            {
                vertexts.Add(new Vertex(vertex2dCoords[nn++], vertex2dCoords[nn++]));
            }
            t01.Connect(vertexts, tess, Tesselate.Tesselator.WindingRuleType.Odd, true);
            tess.BeginPolygon();
            tess.BeginContour();
            int j = vertexts.Count;
            for (int i = 0; i < j; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            tess.EndPolygon();
            return t01.resultVertexList;
        }


        void FillTriangles(List<Vertex> m_VertexList)
        {
            //convert vertex to float array
            {
                unsafe
                {
                    int j = m_VertexList.Count;
                    int j2 = j * 2;
                    float* vertices = stackalloc float[j2];
                    //float[] vx2 = new float[j2];
                    int nn = 0;
                    for (int i = 0; i < j; ++i)
                    {
                        var v = m_VertexList[i];
                        vertices[nn] = (float)v.m_X;
                        vertices[nn + 1] = (float)v.m_Y;

                        //vx2[nn] = (float)v.m_X;
                        //vx2[nn + 1] = (float)v.m_Y;


                        nn += 2;
                    }
                    //--------------------------------------
                    int num_indices = j - 2;
                    int* indx = stackalloc int[j];
                    nn = 0;//reset
                    for (int i = 0; i < num_indices; )
                    {
                        indx[nn] = i;
                        indx[nn + 1] = i + 1;
                        indx[nn + 2] = i + 2;
                        nn += 3;
                        i += 3;
                    }
                    //--------------------------------------
                    GL.EnableClientState(ArrayCap.VertexArray); //***
                    //vertex 2d
                    GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)vertices);
                    GL.DrawElements(BeginMode.Triangles, j, DrawElementsType.UnsignedInt, (IntPtr)indx);
                    GL.DisableClientState(ArrayCap.VertexArray);

                }
            }
            {

                //var currentColor = this.fillColor;
                //this.FillColor = LayoutFarm.Drawing.Color.Black;

                //int j = m_VertexList.Count;
                //int lim = j - 2;
                //for (int i = 0; i < lim; )
                //{
                //    var v0 = m_VertexList[i];
                //    var v1 = m_VertexList[i + 1];
                //    var v2 = m_VertexList[i + 2]; 

                //    DrawLine((float)v0.m_X, (float)v0.m_Y,
                //            (float)v1.m_X, (float)v1.m_Y);
                //    DrawLine((float)v1.m_X, (float)v1.m_Y,
                //          (float)v2.m_X, (float)v2.m_Y);
                //    DrawLine((float)v2.m_X, (float)v2.m_Y,
                //         (float)v0.m_X, (float)v0.m_Y);

                //    i += 3;
                //}
                //this.FillColor = currentColor;
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
                GL.Color4(value);
            }
        }


        public void DrawEllipse(float x, float y, double rx, double ry)
        {

            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs();
            int n = vxs.Count;
            //iterate
            float[] coords = new float[n * 2];
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
            DrawPolygon(coords, nn / 2);
        }
        public void DrawCircle(float x, float y, double radius)
        {

            DrawEllipse(x, y, radius, radius);
        }

        public void FillEllipse(float x, float y, double rx, double ry)
        {
            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs();
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

                int* indx = stackalloc int[npoints];
                for (i = 0; i < npoints; ++i)
                {
                    indx[i] = i;
                }



                //fill triangular fan
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex 2d
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                GL.DrawElements(BeginMode.TriangleFan, npoints, DrawElementsType.UnsignedInt, (IntPtr)indx);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        public void FillCircle(float x, float y, double radius)
        {
            FillEllipse(x, y, radius, radius);
        }

        //---test only ----
        void DrawLineAgg(float x1, float y1, float x2, float y2)
        {

            ps.Clear();
            ps.MoveTo(x1, y1);
            ps.LineTo(x2, y2);
            VertexStore vxs = stroke1.MakeVxs(ps.Vxs);
            int n = vxs.Count;

            unsafe
            {
                float* coords = stackalloc float[(n * 2)];
                int i = 0;
                int nn = 0;
                int npoints = 0;
                double vx, vy;

                var cmd = vxs.GetVertex(i, out vx, out vy);
                while (i < n)
                {
                    switch (cmd)
                    {
                        case ShapePath.FlagsAndCommand.CommandMoveTo:
                            {
                                coords[nn] = (float)vx;
                                coords[nn + 1] = (float)vy;
                                nn += 2;
                                npoints++;
                            } break;
                        case ShapePath.FlagsAndCommand.CommandLineTo:
                            {
                                coords[nn] = (float)vx;
                                coords[nn + 1] = (float)vy;
                                nn += 2;
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

                int num_indices = npoints;
                int* indx = stackalloc int[num_indices];
                nn = 0;//reset
                for (i = 0; i < num_indices; ++i)
                {
                    indx[nn++] = i;
                }

                //--------------------------------------
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex 2d
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                GL.DrawElements(BeginMode.LineLoop, num_indices, DrawElementsType.UnsignedInt, (IntPtr)indx);
                GL.DisableClientState(ArrayCap.VertexArray);
                //--------------------------------------
            }
        }
        void DrawLineAggAA(float x1, float y1, float x2, float y2)
        {
            //--------------------------------------
            ps.Clear();
            ps.MoveTo(x1, y1);
            ps.LineTo(x2, y2);
            VertexStore vxs = stroke1.MakeVxs(ps.Vxs);
            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            sclineRasToBmp.RenderWithColor2(sclineRas, sclinePack8, this.fillColor);
            //--------------------------------------
        }
    }
}