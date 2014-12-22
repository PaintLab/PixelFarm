//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Text;
using OpenTK.Graphics.OpenGL;
using Tesselate;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

namespace LayoutFarm.DrawingGL
{

    public partial class CanvasGL2d
    {   

        public void Clear(LayoutFarm.Drawing.Color c)
        {
            //set value for clear color buffer
            GL.ClearColor(c);
            GL.ClearStencil(0);
            //actual clear here 
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.AccumBufferBit |
                ClearBufferMask.StencilBufferBit); 
        }
        public void SetCanvasOrigin(int x, int y)
        {
            int originalW = 800;
            //set new viewport
            GL.Viewport(x, y, originalW, originalW);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, originalW, 0, originalW, 0.0, 100.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }
        public void EnableClipRect()
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        public void DisableClipRect()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
        public void SetClipRect(int x, int y, int w, int h)
        {
            GL.Scissor(x, y, w, h);
        }
        public void FillPolygon(LayoutFarm.Drawing.Brush brush, float[] vertex2dCoords, int npoints)
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
                        ps.CloseFigure();
                        VertexStore vxs = ps.Vxs;
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);

                        switch (brush.BrushKind)
                        {
                            case Drawing.BrushKind.Solid:
                                {
                                    var color = ((LayoutFarm.Drawing.SolidBrush)brush).Color;
                                    sclineRasToGL.FillWithColor(sclineRas, sclinePack8, color);

                                } break;
                            default:
                                {
                                } break;
                        }

                    } break;
                default:
                    {

                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------   
                        //switch how to fill polygon
                        switch (brush.BrushKind)
                        {
                            case Drawing.BrushKind.LinearGradient:
                            case Drawing.BrushKind.Texture:
                                {
                                    var linearGradientBrush = brush as LayoutFarm.Drawing.LinearGradientBrush;
                                    GL.ClearStencil(0); //set value for clearing stencil buffer 
                                    //actual clear here
                                    GL.Clear(ClearBufferMask.StencilBufferBit);
                                    //-------------------
                                    //disable rendering to color buffer
                                    GL.ColorMask(false, false, false, false);
                                    //start using stencil
                                    GL.Enable(EnableCap.StencilTest);
                                    //place a 1 where rendered
                                    GL.StencilFunc(StencilFunction.Always, 1, 1);
                                    //replace where rendered
                                    GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
                                    //render  to stencill buffer
                                    //-----------------
                                    if (this.Note1 == 1)
                                    {
                                        ////create stencil with Agg shape
                                        int j = npoints / 2;
                                        //first point
                                        if (j < 2)
                                        {
                                            return;
                                        }
                                        ps.Clear();
                                        ps.MoveTo(vertex2dCoords[0], vertex2dCoords[1]);
                                        int nn = 2;
                                        for (int i = 1; i < j; ++i)
                                        {
                                            ps.LineTo(vertex2dCoords[nn++],
                                                vertex2dCoords[nn++]);
                                        }
                                        //close
                                        ps.CloseFigure();

                                        VertexStore vxs = ps.Vxs;
                                        sclineRas.Reset();
                                        sclineRas.AddPath(vxs);
                                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, LayoutFarm.Drawing.Color.White);
                                        //create stencil with normal OpenGL 
                                    }
                                    else
                                    {
                                        //create stencil with normal OpenGL
                                        int j = vertextList.Count;
                                        int j2 = j * 2;
                                        //VboC4V3f vbo = GenerateVboC4V3f();
                                        ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
                                        uint color_uint = LayoutFarm.Drawing.Color.Black.ToABGR();   //color.ToABGR();
                                        for (int i = 0; i < j; ++i)
                                        {
                                            var v = vertextList[i];
                                            vrx.AddVertex(new VertexC4V3f(color_uint, (float)v.m_X, (float)v.m_Y));
                                        }

                                        //GL.EnableClientState(ArrayCap.ColorArray);
                                        //GL.EnableClientState(ArrayCap.VertexArray);
                                        int pcount = vrx.Count;
                                        //vbo.BindBuffer();
                                        DrawTrianglesWithVertexBuffer(vrx, pcount);
                                        //vbo.UnbindBuffer();
                                        //GL.DisableClientState(ArrayCap.ColorArray);
                                        //GL.DisableClientState(ArrayCap.VertexArray);
                                    }
                                    //-------------------------------------- 
                                    //render color
                                    //--------------------------------------  
                                    //reenable color buffer 
                                    GL.ColorMask(true, true, true, true);
                                    //where a 1 was not rendered
                                    GL.StencilFunc(StencilFunction.Equal, 1, 1);
                                    //freeze stencill buffer
                                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

                                    if (this.Note1 == 1) //temp
                                    {
                                        //------------------------------------------
                                        //we already have valid ps from stencil step
                                        //------------------------------------------
                                        VertexStore vxs = ps.Vxs;
                                        sclineRas.Reset();
                                        sclineRas.AddPath(vxs);
                                        //-------------------------------------------------------------------------------------
                                        //1.  we draw only alpha channel of this black color to destination color
                                        //so we use  BlendFuncSeparate  as follow ... 
                                        GL.ColorMask(false, false, false, true);
                                        //GL.BlendFuncSeparate(
                                        //     BlendingFactorSrc.DstColor, BlendingFactorDest.DstColor, //the same
                                        //     BlendingFactorSrc.One, BlendingFactorDest.Zero);

                                        //use alpha chanel from source***
                                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
                                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, LayoutFarm.Drawing.Color.Black);

                                        //at this point alpha component is fill in to destination 
                                        //-------------------------------------------------------------------------------------
                                        //2. then fill again!, 
                                        //we use alpha information from dest, 
                                        //so we set blend func to ... GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha)    
                                        GL.ColorMask(true, true, true, true);
                                        GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);
                                        {
                                            //draw box of gradient color
                                            if (brush.BrushKind == Drawing.BrushKind.LinearGradient)
                                            {
                                                var colors = linearGradientBrush.GetColors();
                                                var points = linearGradientBrush.GetStopPoints();
                                                uint c1 = colors[0].ToABGR();
                                                uint c2 = colors[1].ToABGR();
                                                //create polygon for graident bg 

                                                ArrayList<VertexC4V3f>
                                                     vrx = GLGradientColorProvider.CalculateLinearGradientVxs(
                                                     points[0].X, points[0].Y,
                                                     points[1].X, points[1].Y,
                                                     colors[0],
                                                     colors[1]);
                                                int pcount = vrx.Count;
                                                //GL.EnableClientState(ArrayCap.ColorArray);
                                                //GL.EnableClientState(ArrayCap.VertexArray);
                                                ////--- 
                                                //VboC4V3f vbo = GenerateVboC4V3f();
                                                //vbo.BindBuffer();
                                                DrawTrianglesWithVertexBuffer(vrx, pcount);
                                                //vbo.UnbindBuffer();
                                                ////vbo.Dispose();
                                                //GL.DisableClientState(ArrayCap.ColorArray);
                                                //GL.DisableClientState(ArrayCap.VertexArray);
                                            }
                                            else if (brush.BrushKind == Drawing.BrushKind.Texture)
                                            {
                                                //draw texture image 
                                                LayoutFarm.Drawing.TextureBrush tbrush = (LayoutFarm.Drawing.TextureBrush)brush;
                                                LayoutFarm.Drawing.Image img = tbrush.TextureImage;
                                                GLBitmap bmpTexture = (GLBitmap)tbrush.InnerImage2;
                                                this.DrawImage(bmpTexture, 0, 0);
                                                //GLBitmapTexture bmp = GLBitmapTexture.CreateBitmapTexture(fontGlyph.glyphImage32);
                                                //this.DrawImage(bmp, 0, 0);
                                                //bmp.Dispose();

                                            }
                                        }
                                        //restore back 
                                        //3. switch to normal blending mode 
                                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                                    }
                                    else
                                    {
                                        //draw box of gradient color
                                        var colors = linearGradientBrush.GetColors();
                                        var points = linearGradientBrush.GetStopPoints();
                                        uint c1 = colors[0].ToABGR();
                                        uint c2 = colors[1].ToABGR();
                                        //create polygon for graident bg 
                                        var vrx = GLGradientColorProvider.CalculateLinearGradientVxs(
                                             points[0].X, points[0].Y,
                                             points[1].X, points[1].Y,
                                             colors[0],
                                             colors[1]);
                                        int pcount = vrx.Count;
                                        //GL.EnableClientState(ArrayCap.ColorArray);
                                        //GL.EnableClientState(ArrayCap.VertexArray);
                                        //--- 
                                        //VboC4V3f vbo = GenerateVboC4V3f();
                                        //vbo.BindBuffer();
                                        DrawTrianglesWithVertexBuffer(vrx, pcount);
                                        //vbo.UnbindBuffer();

                                    }
                                    GL.Disable(EnableCap.StencilTest);
                                } break;
                            default:
                                {
                                    //unknown brush
                                    //int j = vertextList.Count;
                                    //int j2 = j * 2;
                                    //VboC4V3f vbo = GenerateVboC4V3f();
                                    //ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
                                    //uint color_int = color.ToABGR();
                                    //for (int i = 0; i < j; ++i)
                                    //{
                                    //    var v = vertextList[i];
                                    //    vrx.AddVertex(new VertexC4V3f(color_int, (float)v.m_X, (float)v.m_Y));
                                    //}
                                    ////------------------------------------- 
                                    //GL.EnableClientState(ArrayCap.ColorArray);
                                    //GL.EnableClientState(ArrayCap.VertexArray);
                                    //int pcount = vrx.Count;
                                    //vbo.BindBuffer();
                                    //DrawTrianglesWithVertexBuffer(vrx, pcount);
                                    //vbo.UnbindBuffer();
                                    //GL.DisableClientState(ArrayCap.ColorArray);
                                    //GL.DisableClientState(ArrayCap.VertexArray);
                                    ////-------------------------------------- 
                                } break;
                        }


                    } break;
            }
        }
        public void FillEllipse(LayoutFarm.Drawing.Color color, float x, float y, float rx, float ry)
        {
            ellipse.Reset(x, y, rx, ry);
            var vxs = ellipse.MakeVxs();

            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, color);
                    } break;
                default:
                    {   //other mode
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
                                    case VertexCmd.MoveTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                            npoints++;
                                        } break;
                                    case VertexCmd.LineTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                            npoints++;
                                        } break;
                                    case VertexCmd.Stop:
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

                            //fill triangular fan
                            GL.EnableClientState(ArrayCap.VertexArray); //***
                            //vertex 2d
                            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                            GL.DrawArrays(BeginMode.TriangleFan, 0, npoints);
                            GL.DisableClientState(ArrayCap.VertexArray);
                        }
                    } break;
            }
        }
        public void DrawImages(GLBitmap bmp, LayoutFarm.Drawing.RectangleF[] destAndSrcPairs)
        {

            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    //texture source coord 1= 100% of original width
                    float* arr = stackalloc float[8];
                    float fullsrcW = bmp.Width;
                    float fullsrcH = bmp.Height;

                    int len = destAndSrcPairs.Length;
                    if (len > 1)
                    {
                        if ((len % 2 != 0))
                        {
                            len -= 1;
                        }
                        for (int i = 0; i < len; )
                        {
                            //each 

                            var destRect = destAndSrcPairs[i];
                            var srcRect = destAndSrcPairs[i + 1];
                            i += 2;

                            if (bmp.IsInvert)
                            {

                                ////arr[0] = 0; arr[1] = 0;
                                arr[0] = srcRect.Left / fullsrcW; arr[1] = (srcRect.Top + srcRect.Height) / fullsrcH;
                                //arr[2] = 1; arr[3] = 0;
                                arr[2] = srcRect.Right / fullsrcW; arr[3] = (srcRect.Top + srcRect.Height) / fullsrcH;
                                //arr[4] = 1; arr[5] = 1;
                                arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Top / fullsrcH;
                                //arr[6] = 0; arr[7] = 1;
                                arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Top / fullsrcH;
                            }
                            else
                            {

                                arr[0] = srcRect.Left / fullsrcW; arr[1] = srcRect.Top / fullsrcH;
                                //arr[2] = 1; arr[3] = 1;
                                arr[2] = srcRect.Right / fullsrcW; arr[3] = srcRect.Top / fullsrcH;
                                //arr[4] = 1; arr[5] = 0;
                                arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Bottom / fullsrcH;
                                //arr[6] = 0; arr[7] = 0;
                                arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Bottom / fullsrcH;
                            }
                            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                            //------------------------------------------ 
                            //fill rect with texture                             
                            FillRectWithTexture(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                        }
                    }

                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                }
                GL.Disable(EnableCap.Texture2D);
            }
        }

        public void DrawImage(GLBitmap bmp,
           LayoutFarm.Drawing.RectangleF srcRect,
           float x, float y, float w, float h)
        {
            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    //texture source coord 1= 100% of original width
                    float* arr = stackalloc float[8];
                    float fullsrcW = bmp.Width;
                    float fullsrcH = bmp.Height;
                    if (bmp.IsInvert)
                    {

                        ////arr[0] = 0; arr[1] = 0;
                        arr[0] = srcRect.Left / fullsrcW; arr[1] = (srcRect.Top + srcRect.Height) / fullsrcH;
                        //arr[2] = 1; arr[3] = 0;
                        arr[2] = srcRect.Right / fullsrcW; arr[3] = (srcRect.Top + srcRect.Height) / fullsrcH;
                        //arr[4] = 1; arr[5] = 1;
                        arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Top / fullsrcH;
                        //arr[6] = 0; arr[7] = 1;
                        arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Top / fullsrcH;
                    }
                    else
                    {

                        arr[0] = srcRect.Left / fullsrcW; arr[1] = srcRect.Top / fullsrcH;
                        //arr[2] = 1; arr[3] = 1;
                        arr[2] = srcRect.Right / fullsrcW; arr[3] = srcRect.Top / fullsrcH;
                        //arr[4] = 1; arr[5] = 0;
                        arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Bottom / fullsrcH;
                        //arr[6] = 0; arr[7] = 0;
                        arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Bottom / fullsrcH;
                    }
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //------------------------------------------ 
                    //fill rect with texture 
                    FillRectWithTexture(x, y, w, h);
                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                }
                GL.Disable(EnableCap.Texture2D);
            }
        }




        static void DrawTrianglesWithVertexBuffer(ArrayList<VertexC4V3f> buffer, int nelements)
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            unsafe
            {
                //c4 + vector3

                //VertexC4V3f[] vpoints = buffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V3f.SIZE_IN_BYTES * nelements);
                ////GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                //// Fill newly allocated buffer
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);
                //GL.DrawArrays(BeginMode.Triangles, 0, nelements);
            }
            //vbo.Dispose();
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);

        }
        static void DrawLinesWithVertexBuffer(ArrayList<VertexC4V3f> buffer, int nelements)
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            unsafe
            {
                //VertexC4V3f[] vpoints = buffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V3f.SIZE_IN_BYTES * nelements);
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);
                //GL.DrawArrays(BeginMode.Lines, 0, nelements);
            }
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }
        static void DrawLineStripWithVertexBuffer(ArrayList<VertexC4V3f> buffer, int nelements)
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            unsafe
            {
                //VertexC4V3f[] vpoints = buffer.Array;
                //IntPtr stride_size = new IntPtr(VertexC4V3f.SIZE_IN_BYTES * nelements);
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsageHint.StreamDraw);
                //GL.DrawArrays(BeginMode.LineStrip, 0, nelements);
            }
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }
        void FillRectWithTexture(float x, float y, float w, float h)
        {
            unsafe
            {
                float* arr = stackalloc float[8];
                byte* indices = stackalloc byte[6];
                CreateRectCoords(arr, indices, x, y, w, h);
                GL.EnableClientState(ArrayCap.VertexArray);
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);

            }
        }
    
        static unsafe void CreateRectCoords(float* arr, byte* indices,
                  float x, float y, float w, float h)
        {
            //cartesian 
            arr[0] = x; arr[1] = y;
            arr[2] = x + w; arr[3] = y;
            arr[4] = x + w; arr[5] = y + h;
            arr[6] = x; arr[7] = y + h;

            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 2; indices[4] = 3; indices[5] = 0;
        }
        static unsafe void CreateLineCoords(ArrayList<VertexC4V3f> vrx,
                 LayoutFarm.Drawing.Color color,
                 float x1, float y1, float x2, float y2)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V3f(color_uint, x1, y1));
            vrx.AddVertex(new VertexC4V3f(color_uint, x2, y2));
        }
        static unsafe void CreateRectCoords(ArrayList<VertexC4V3f> vrx,
                   LayoutFarm.Drawing.Color color,
                   float x, float y, float w, float h)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y + h));

            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y));

        }
        static unsafe void CreatePolyLineRectCoords(ArrayList<VertexC4V3f> vrx,
                   LayoutFarm.Drawing.Color color,
                   float x, float y, float w, float h)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y));

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

        //---test only ----
        void DrawLineAgg(float x1, float y1, float x2, float y2)
        {

            ps.Clear();
            ps.MoveTo(x1, y1);
            ps.LineTo(x2, y2);
            VertexStore vxs = aggStroke.MakeVxs(ps.Vxs);
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
                        case VertexCmd.MoveTo:
                            {
                                coords[nn] = (float)vx;
                                coords[nn + 1] = (float)vy;
                                nn += 2;
                                npoints++;
                            } break;
                        case VertexCmd.LineTo:
                            {
                                coords[nn] = (float)vx;
                                coords[nn + 1] = (float)vy;
                                nn += 2;
                                npoints++;

                            } break;
                        case VertexCmd.Stop:
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
                GL.EnableClientState(ArrayCap.VertexArray); //***
                //vertex 2d
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                GL.DrawArrays(BeginMode.LineLoop, 0, npoints);
                //GL.DrawElements(BeginMode.LineLoop, num_indices, DrawElementsType.UnsignedInt, (IntPtr)indx);
                GL.DisableClientState(ArrayCap.VertexArray);
                //--------------------------------------
            }
        }

        unsafe void DrawPolygonUnsafe(float* polygon2dVertices, int npoints)
        {
            //1. enable client side memory
            GL.EnableClientState(ArrayCap.VertexArray); //***
            //2. load data from point to vertex array part
            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)polygon2dVertices);
            //3. draw array
            GL.DrawArrays(BeginMode.LineLoop, 0, npoints);
            //4. disable client side memory
            GL.DisableClientState(ArrayCap.VertexArray);
        }
    }
}