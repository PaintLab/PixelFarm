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
        List<Vertex> TessPolygon(float[] vertex2dCoords)
        {
            int ncoords = vertex2dCoords.Length / 2;
            List<Vertex> vertexts = new List<Vertex>(ncoords);
            int nn = 0;
            for (int i = 0; i < ncoords; ++i)
            {
                vertexts.Add(new Vertex(vertex2dCoords[nn++], vertex2dCoords[nn++]));
            }
            //-----------------------
            tessListener.Reset(vertexts);
            //-----------------------
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
            return tessListener.resultVertexList;
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

                        nn += 2;
                    }
                    //--------------------------------------
                    //int num_indices = j - 2;
                    //int* indx = stackalloc int[j];
                    //nn = 0;//reset
                    //for (int i = 0; i < num_indices; )
                    //{
                    //    indx[nn] = i;
                    //    indx[nn + 1] = i + 1;
                    //    indx[nn + 2] = i + 2;
                    //    nn += 3;
                    //    i += 3;
                    //}
                    //--------------------------------------
                    GL.EnableClientState(ArrayCap.VertexArray); //***
                    //vertex 2d
                    GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)vertices);
                    //GL.DrawElements(BeginMode.Triangles, j, DrawElementsType.UnsignedInt, (IntPtr)indx);
                    GL.DrawArrays(BeginMode.Triangles, 0, nn);
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
            sclineRasToBmp.RenderWithColor(sclineRas, sclinePack8, this.fillColor);
            //--------------------------------------
        }
    }
}