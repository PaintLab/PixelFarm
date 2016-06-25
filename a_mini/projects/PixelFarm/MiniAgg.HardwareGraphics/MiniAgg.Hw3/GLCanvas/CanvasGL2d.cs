//MIT 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;
using Tesselate;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
namespace PixelFarm.DrawingGL
{
    public partial class CanvasGL2d
    {
        BasicShader basicShader;
        SmoothLineShader smoothLineShader;
        InvertAlphaFragmentShader invertAlphaFragmentShader;
        BasicFillShader basicFillShader;
        RectFillShader rectFillShader;
        SimpleTextureShader textureShader;
        //-----------------------------------------------------------

        PixelFarm.Drawing.Color strokeColor = PixelFarm.Drawing.Color.Black;
        //tools---------------------------------
        PathWriter ps = new PathWriter();
        Stroke aggStroke = new Stroke(1);
        Arc arcTool = new Arc();
        CurveFlattener curveFlattener = new CurveFlattener();
        GLTextPrinter textPriner;
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
            basicShader = new BasicShader();
            smoothLineShader = new SmoothLineShader();
            basicFillShader = new BasicFillShader();
            rectFillShader = new RectFillShader();
            textureShader = new SimpleTextureShader();
            invertAlphaFragmentShader = new InvertAlphaFragmentShader(); //used with stencil  ***
                                                                         // tessListener.Connect(tess,          
                                                                         //Tesselate.Tesselator.WindingRuleType.Odd, true);

            //
            Tesselator tess = new Tesselator();
            tess.WindingRule = Tesselator.WindingRuleType.Odd;
            tessTool = new TessTool(tess);
            //-----------------------------------------------------------------------
            textPriner = new GLTextPrinter(this);
            SetupFonts();
            ////--------------------------------------------------------------------------------
            //GL.Enable(EnableCap.CullFace);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            ////setup viewport size
            int max = Math.Max(canvasW, canvasH);
            ////square viewport 
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //-------------------------------------------------------------------------------

            smoothLineShader.OrthoView = orthoView;
            basicFillShader.OrthoView = orthoView;
            textureShader.OrthoView = orthoView;
            basicFillShader.OrthoView = orthoView;
            invertAlphaFragmentShader.OrthoView = orthoView;
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

        internal Stroke StrokeGen { get { return this.aggStroke; } }
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
        public double StrokeWidth
        {
            get { return this.aggStroke.Width; }
            set
            {
                //agg stroke
                this.aggStroke.Width = value;
            }
        }
        public Drawing.Color StrokeColor
        {
            get { return this.strokeColor; }
            set { this.strokeColor = value; }
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.StrokeWidth = (float)this.StrokeWidth;
                        this.smoothLineShader.DrawLine(x1, y1, x2, y2);
                    }
                    break;
                default:
                    {
                        this.basicShader.DrawLine(x1, y1, x2, y2, this.strokeColor);
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
        public void FillVxsSnap(Drawing.Color color, VertexStoreSnap snap)
        {
            FillGfxPath(color, InternalGraphicsPath.CreateGraphicsPath(snap));
        }

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
                        strokeColor = color;
                        StrokeWidth = 0.5f;
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.StrokeWidth = (float)this.StrokeWidth;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = figures[i];
                            this.basicFillShader.FillTriangles(f.GetAreaTess(ref this.tessTool), f.TessAreaTriangleCount, color);
                            smoothLineShader.DrawTriangleStrips(f.GetSmoothBorders(), f.BorderTriangleStripCount);
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
                                    DrawPolygonUnsafe(head, coordXYs.Length / 2);
                                }
                            }
                        }
                    }
                    break;
                case CanvasSmoothMode.Smooth:
                    {
                        strokeColor = color;
                        StrokeWidth = 1f;
                        List<Figure> figures = igpth.figures;
                        int subPathCount = figures.Count;
                        strokeColor = color;
                        StrokeWidth = 1f;
                        smoothLineShader.StrokeWidth = 1;
                        smoothLineShader.StrokeColor = color;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = figures[i];
                            smoothLineShader.DrawTriangleStrips(f.GetSmoothBorders(), f.BorderTriangleStripCount);
                        }
                    }
                    break;
            }
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
                            var linearGradientBrush = brush as PixelFarm.Drawing.LinearGradientBrush;
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

                            //switch how to fill polygon ***
                            //int j = vertextList.Count;
                            //float[] vtx = new float[j * 2];
                            //int n = 0;
                            //for (int i = 0; i < j; ++i)
                            //{
                            //    var v = vertextList[i];
                            //    vtx[n] = (float)v.m_X;
                            //    vtx[n + 1] = (float)v.m_Y;
                            //    n += 2;
                            //}
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
                            invertAlphaFragmentShader.OrthoView = orthoView;
                            invertAlphaFragmentShader.StrokeColor = PixelFarm.Drawing.Color.Black;
                            invertAlphaFragmentShader.DrawPolygon(fig.coordXYs, fig.coordXYs.Length);
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
                                            var colors = linearGradientBrush.GetColors();
                                            var points = linearGradientBrush.GetStopPoints();
                                            float[] v2f, color4f;
                                            GLGradientColorProvider.CalculateLinearGradientVxs2(
                                                 points[0].X, points[0].Y,
                                                 points[1].X, points[1].Y,
                                                 colors[0],
                                                 colors[1], out v2f, out color4f);
                                            rectFillShader.OrthoView = orthoView;
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
        public void FillRenderVx(Drawing.Brush brush, Drawing.RenderVx renderVx)
        {
            GLRenderVx glRenderVx = (GLRenderVx)renderVx;
            FillGfxPath(brush, glRenderVx.gxpth);
        }

        public void DrawRect(float x, float y, float w, float h)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.Smooth:
                    {
                        smoothLineShader.StrokeColor = this.strokeColor;
                        smoothLineShader.StrokeWidth = (float)this.StrokeWidth;
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
    }
}