//MIT 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;
using Tesselate;
namespace PixelFarm.DrawingGL
{
    public class CanvasGL2d
    {
        SmoothLineShader smoothLineShader;
        InvertAlphaLineSmoothShader invertAlphaFragmentShader;
        BasicFillShader basicFillShader;
        RectFillShader rectFillShader;
        SimpleTextureShader textureShader;
        //-----------------------------------------------------------
        CanvasToShaderSharedResource shaderRes;
        //tools---------------------------------

        int canvasOriginX = 0;
        int canvasOriginY = 0;
        int canvasW;
        int canvasH;
        MyMat4 orthoView;
        TessTool tessTool;
        public CanvasGL2d(int canvasW, int canvasH)
        {
            this.canvasW = canvasW;
            this.canvasH = canvasH;
            ////setup viewport size
            int max = Math.Max(canvasW, canvasH);
            ////square viewport 
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //-----------------------------------------------------------------------
            shaderRes = new CanvasToShaderSharedResource();
            shaderRes.OrthoView = orthoView;
            //-----------------------------------------------------------------------
            smoothLineShader = new SmoothLineShader(shaderRes);
            basicFillShader = new BasicFillShader(shaderRes);
            rectFillShader = new RectFillShader(shaderRes);
            textureShader = new SimpleTextureShader(shaderRes);
            invertAlphaFragmentShader = new InvertAlphaLineSmoothShader(shaderRes); //used with stencil  ***
                                                                                    // tessListener.Connect(tess,          
                                                                                    //Tesselate.Tesselator.WindingRuleType.Odd, true);


            Tesselator tess = new Tesselator();
            tess.WindingRule = Tesselator.WindingRuleType.Odd;
            tessTool = new TessTool(tess);
            //-----------------------------------------------------------------------


            //GL.Enable(EnableCap.CullFace);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //-------------------------------------------------------------------------------

            GL.Viewport(0, 0, canvasW, canvasH);
        }
        public void Dispose()
        {
        }

        public CanvasSmoothMode SmoothMode
        {
            get;
            set;
        }

        public void Clear(PixelFarm.Drawing.Color c)
        {
            GL.ClearColor(
               (float)c.R / 255f,
               (float)c.G / 255f,
               (float)c.B / 255f,
               (float)c.A / 255f);
            GL.ClearStencil(0);
            //actual clear here !
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);
        }
        public void ClearColorBuffer()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
        public float StrokeWidth
        {
            get { return shaderRes._strokeWidth; }
            set
            {
                shaderRes._strokeWidth = value;
            }
        }
        public Drawing.Color StrokeColor
        {
            get { return shaderRes.StrokeColor; }
            set { shaderRes.StrokeColor = value; }
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        this.smoothLineShader.DrawLine(x1, y1, x2, y2);
                    }
                    break;
                default:
                    {
                        this.basicFillShader.DrawLine(x1, y1, x2, y2, StrokeColor);
                    }
                    break;
            }
        }

        //-------------------------------------------------------------------------------
        public void DrawImage(GLBitmap bmp, float x, float y)
        {
            DrawImage(bmp,
                   new Drawing.RectangleF(0, 0, bmp.Width, bmp.Height),
                   x, y, bmp.Width, bmp.Height);
        }
        public void DrawImage(GLBitmap bmp, float x, float y, float w, float h)
        {
            DrawImage(bmp,
                new Drawing.RectangleF(0, 0, bmp.Width, bmp.Height),
                x, y, w, h);
        }
        public void DrawImage(GLBitmap bmp,
            Drawing.RectangleF srcRect,
            float x, float y, float w, float h)
        {
            this.textureShader.Render(bmp, x, y, w, h);
        }
        //-------------------------------------------------------------------------------
        public void DrawImage(GLBitmapReference bmp, float x, float y)
        {
            this.DrawImage(bmp.OwnerBitmap,
                 bmp.GetRectF(),
                 x, y, bmp.Width, bmp.Height);
        }
        //-------------------------------------------------------------------------------
        public void FillTriangleStrip(Drawing.Color color, float[] coords, int n)
        {
            basicFillShader.FillTriangleStripWithVertexBuffer(coords, n, color);
        }
        public void FillTriangleFan(Drawing.Color color, float[] coords, int n)
        {
            unsafe
            {
                fixed (float* head = &coords[0])
                {
                    basicFillShader.FillTriangleFan(head, n, color);
                }
            }
        }
        //-------------------------------------------------------------------------------
        //RenderVx
        public void FillRenderVx(Drawing.Brush brush, Drawing.RenderVx renderVx)
        {
            GLRenderVx glRenderVx = (GLRenderVx)renderVx;
            FillGfxPath(brush, glRenderVx.gxpth);
        }
        public void FillRenderVx(Drawing.Color color, Drawing.RenderVx renderVx)
        {
            GLRenderVx glRenderVx = (GLRenderVx)renderVx;
            FillGfxPath(color, glRenderVx.gxpth);
        }
        public void DrawRenderVx(Drawing.Color color, Drawing.RenderVx renderVx)
        {
            GLRenderVx glRenderVx = (GLRenderVx)renderVx;
            DrawGfxPath(color, glRenderVx.gxpth);
        }
        //-------------------------------------------------------------------------------
        //InternalGraphicsPath
        internal void FillGfxPath(Drawing.Color color, InternalGraphicsPath igpth)
        {
            switch (SmoothMode)
            {
                case CanvasSmoothMode.No:
                    {
                        List<Figure> figures = igpth.figures;
                        int subPathCount = figures.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = figures[i];
                            this.basicFillShader.FillTriangles(f.GetAreaTess(ref this.tessTool), f.TessAreaTriangleCount, color);
                        }
                    }
                    break;
                case CanvasSmoothMode.Smooth:
                    {
                        List<Figure> figures = igpth.figures;
                        int subPathCount = figures.Count;
                        float prevWidth = StrokeWidth;
                        StrokeColor = color;
                         
                        StrokeWidth = 0.5f;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = figures[i];
                            basicFillShader.FillTriangles(f.GetAreaTess(ref this.tessTool), f.TessAreaTriangleCount, color);
                            smoothLineShader.DrawTriangleStrips(f.GetSmoothBorders(), f.BorderTriangleStripCount);
                        }
                        StrokeWidth = prevWidth;
                    }
                    break;
            }
        }
        internal void FillGfxPath(Drawing.Brush brush, InternalGraphicsPath igpth)
        {
            switch (brush.BrushKind)
            {
                case Drawing.BrushKind.Solid:
                    {
                        var solidBrush = brush as PixelFarm.Drawing.SolidBrush;
                        FillGfxPath(solidBrush.Color, igpth);
                    }
                    break;
                case Drawing.BrushKind.LinearGradient:
                case Drawing.BrushKind.Texture:
                    {
                        List<Figure> figures = igpth.figures;
                        int m = figures.Count;
                        for (int b = 0; b < m; ++b)
                        {
                            Figure fig = figures[b];
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

                            float[] tessArea = fig.GetAreaTess(ref this.tessTool);
                            //-------------------------------------   
                            this.basicFillShader.FillTriangles(tessArea, fig.TessAreaTriangleCount, PixelFarm.Drawing.Color.Black);
                            //-------------------------------------- 
                            //render color
                            //--------------------------------------  
                            //reenable color buffer 
                            GL.ColorMask(true, true, true, true);
                            //where a 1 was not rendered
                            GL.StencilFunc(StencilFunction.Equal, 1, 1);
                            //freeze stencill buffer
                            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                            //------------------------------------------
                            //we already have valid ps from stencil step
                            //------------------------------------------

                            //-------------------------------------------------------------------------------------
                            //1.  we draw only alpha chanel of this black color to destination color
                            //so we use  BlendFuncSeparate  as follow ... 
                            //-------------------------------------------------------------------------------------
                            //1.  we draw only alpha channel of this black color to destination color
                            //so we use  BlendFuncSeparate  as follow ... 
                            GL.ColorMask(false, false, false, true);
                            //GL.BlendFuncSeparate(
                            //     BlendingFactorSrc.DstColor, BlendingFactorDest.DstColor, //the same
                            //     BlendingFactorSrc.One, BlendingFactorDest.Zero);

                            //use alpha chanel from source***
                            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
                            float[] smoothBorder = fig.GetSmoothBorders();
                            invertAlphaFragmentShader.DrawTriangleStrips(smoothBorder, fig.BorderTriangleStripCount);
                            //at this point alpha component is fill in to destination 
                            //-------------------------------------------------------------------------------------
                            //2. then fill again!, 
                            //we use alpha information from dest, 
                            //so we set blend func to ... GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha)    
                            GL.ColorMask(true, true, true, true);
                            GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);
                            {
                                //draw box*** of gradient color
                                switch (brush.BrushKind)
                                {
                                    case Drawing.BrushKind.LinearGradient:
                                        {
                                            var linearGradientBrush = brush as PixelFarm.Drawing.LinearGradientBrush;
                                            var colors = linearGradientBrush.GetColors();
                                            var points = linearGradientBrush.GetStopPoints();
                                            float[] v2f, color4f;
                                            GLGradientColorProvider.CalculateLinearGradientVxs2(
                                                 points[0].X, points[0].Y,
                                                 points[1].X, points[1].Y,
                                                 colors[0],
                                                 colors[1], out v2f, out color4f);
                                            rectFillShader.Render(v2f, color4f);
                                        }
                                        break;
                                    case Drawing.BrushKind.Texture:
                                        {
                                            //draw texture image ***
                                            PixelFarm.Drawing.TextureBrush tbrush = (PixelFarm.Drawing.TextureBrush)brush;
                                            GLImage img = tbrush.TextureImage as GLImage;
                                            GLBitmap bmpTexture = (GLBitmap)img.InnerImage;
                                            //TODO: review here 
                                            //where text start?
                                            this.DrawImage(bmpTexture, 0, 300);
                                        }
                                        break;
                                }
                            }
                            //restore back 
                            //3. switch to normal blending mode 
                            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                            GL.Disable(EnableCap.StencilTest);
                        }
                    }
                    break;
            }
        }
        internal void DrawGfxPath(Drawing.Color color, InternalGraphicsPath igpth)
        {
            switch (SmoothMode)
            {
                case CanvasSmoothMode.No:
                    {
                        List<Figure> figures = igpth.figures;
                        int subPathCount = figures.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = figures[i];
                            float[] coordXYs = f.coordXYs;
                            unsafe
                            {
                                fixed (float* head = &coordXYs[0])
                                {
                                    basicFillShader.DrawLineLoopWithVertexBuffer(head, coordXYs.Length / 2, StrokeColor);
                                }
                            }
                        }
                    }
                    break;
                case CanvasSmoothMode.Smooth:
                    {
                        StrokeColor = color;
                        StrokeWidth = 1f;
                        List<Figure> figures = igpth.figures;
                        int subPathCount = figures.Count;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = figures[i];
                            smoothLineShader.DrawTriangleStrips(f.GetSmoothBorders(), f.BorderTriangleStripCount);
                        }
                    }
                    break;
            }
        }
        //-------------------------------------------------------------------------------

        public void DrawRect(float x, float y, float w, float h)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        int borderTriAngleCount;
                        float[] triangles = InternalGraphicsPath.BuildSmoothBorders(
                            CreatePolyLineRectCoords(x, y, w, h), out borderTriAngleCount);
                        smoothLineShader.DrawTriangleStrips(triangles, borderTriAngleCount);
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }

        public int CanvasOriginX
        {
            get { return this.canvasOriginX; }
        }
        public int CanvasOriginY
        {
            get { return this.canvasOriginY; }
        }

        public void SetCanvasOrigin(int x, int y)
        {
            //int originalW = 800;
            //set new viewport
            GL.Viewport(x, y, canvasW, canvasH);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, originalW, 0, originalW, 0.0, 100.0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
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

        static float[] CreatePolyLineRectCoords(
               float x, float y, float w, float h)
        {
            return new float[]
            {
                x,y,
                x+w,y,
                x+w,y+h,
                x,x+h
            };
        }
    }
}