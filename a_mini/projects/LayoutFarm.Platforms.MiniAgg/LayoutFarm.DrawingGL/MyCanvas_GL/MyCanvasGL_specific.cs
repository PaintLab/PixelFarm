//2014 BSD, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.DrawingGL;
using DrawingBridge;

namespace LayoutFarm.Drawing.DrawingGL
{

    partial class MyCanvasGL : LayoutFarm.Drawing.WinGdi.MyCanvas
    {
        CanvasGL2d canvasGL2d = new CanvasGL2d();
        public MyCanvasGL(GraphicsPlatform platform, int hPageNum, int vPageNum, int left, int top, int width, int height)
            : base(platform, hPageNum, vPageNum, left, top, width, height)
        {

        }
        //-------------------------------------------
        public override void SetCanvasOrigin(int x, int y)
        {
            canvasGL2d.SetCanvasOrigin(x, y);
        }
        public override int CanvasOriginX
        {
            get
            {
                return canvasGL2d.CanvasOriginX;
            }
        }
        public override int CanvasOriginY
        {
            get
            {
                return canvasGL2d.CanvasOriginY;
            }
        }
        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            canvasGL2d.EnableClipRect();
            //--------------------------
            canvasGL2d.SetClipRect(
                 rect.X,
                 rect.Y,
                rect.Width,
                 rect.Height);
            //--------------------------
        }
        //-------------------------------------------
        public override void FillRectangle(Color color, float left, float top, float width, float height)
        {
            canvasGL2d.CurrentBrushKind = BrushKind.Solid;
            canvasGL2d.FillColor = color;
            canvasGL2d.FillRect(left, top, width, height);
        }
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        var solidBrush = brush as SolidBrush;
                        var currentKind = canvasGL2d.CurrentBrushKind;
                        canvasGL2d.CurrentBrushKind = BrushKind.Solid;
                        canvasGL2d.FillColor = solidBrush.Color;
                        canvasGL2d.FillRect(left, top, width, height);
                        canvasGL2d.CurrentBrushKind = currentKind;
                    } break;
                case BrushKind.LinearGradient:
                    {
                        var linearGradientBrush = brush as LinearGradientBrush;
                        //create bg gradient first 
                        //fill linear gradient in spefic area
                        var currentKind = canvasGL2d.CurrentBrushKind;
                        canvasGL2d.CurrentBrushKind = BrushKind.LinearGradient;
                        canvasGL2d.Brush = linearGradientBrush;
                        canvasGL2d.FillRect(left, top, width, height);
                        canvasGL2d.CurrentBrushKind = currentKind;
                    } break;
                case BrushKind.CirculatGraident:
                    {

                    } break;
                default:
                    {

                    } break;
            }
        }
        public override void FillPolygon(PointF[] points)
        {

            int j = points.Length;
            float[] polygonPoints = new float[j * 2];
            int n = 0;
            for (int i = 0; i < j; ++i)
            {
                polygonPoints[n] = points[i].X;
                polygonPoints[n + 1] = points[i].Y;
                n += 2;
            }
            canvasGL2d.Note1 = this.Note1;
            canvasGL2d.FillPolygon(polygonPoints);


        }
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            canvasGL2d.Note1 = this.Note1;
            switch (brush.BrushKind)
            {
                case BrushKind.LinearGradient:
                    {
                        //linear grdient brush for polygon 

                        int j = points.Length;
                        float[] polygonPoints = new float[j * 2];
                        int n = 0;
                        for (int i = 0; i < j; ++i)
                        {
                            polygonPoints[n] = points[i].X;
                            polygonPoints[n + 1] = points[i].Y;
                            n += 2;
                        }

                        //use stencil buffer
                        var currentBrushKind = canvasGL2d.CurrentBrushKind;
                        canvasGL2d.CurrentBrushKind = BrushKind.LinearGradient;
                        canvasGL2d.FillPolygon(polygonPoints);
                        canvasGL2d.CurrentBrushKind = currentBrushKind;

                    } break;
                case BrushKind.Texture:
                    {

                        int j = points.Length;
                        float[] polygonPoints = new float[j * 2];
                        int n = 0;
                        for (int i = 0; i < j; ++i)
                        {
                            polygonPoints[n] = points[i].X;
                            polygonPoints[n + 1] = points[i].Y;
                            n += 2;
                        }

                        //use stencil buffer
                        var currentBrushKind = canvasGL2d.CurrentBrushKind;
                        canvasGL2d.CurrentBrushKind = BrushKind.Texture;
                        //prepare texture brush 

                        var tbrush = (TextureBrush)brush;
                        GLBitmapTexture bmpTexture = null;
                        if (tbrush.InnerImage2 == null)
                        {
                            //create gl image
                            var textureImage = tbrush.TextureImage; 
                            tbrush.InnerImage2 = bmpTexture = GLBitmapTextureHelper.CreateBitmapTexture((System.Drawing.Bitmap)textureImage.InnerImage);
                            canvasGL2d.Brush = tbrush; //set current brush**
                        }
                        canvasGL2d.FillPolygon(polygonPoints);
                        canvasGL2d.CurrentBrushKind = currentBrushKind;

                        //delete gl
                        bmpTexture.Dispose();
                        tbrush.InnerImage2 = null;

                    } break;
                default:
                    {
                        FillPolygon(points);
                    } break;
            }

        }
        public override Brush CurrentBrush
        {
            get
            {
                return base.CurrentBrush;
            }
            set
            {

                base.CurrentBrush = value;
                if (value != null)
                {
                    this.canvasGL2d.CurrentBrushKind = value.BrushKind;
                }
                this.canvasGL2d.Brush = value;
            }
        }
        public override Color FillColor
        {
            get
            {

                return this.canvasGL2d.FillColor;
            }
            set
            {
                this.canvasGL2d.FillColor = value;
                base.FillColor = value;
            }
        }
        public override void ClearSurface(Color c)
        {
            canvasGL2d.Clear(c);
        }
        //-------------------------------------------
        public override void DrawImage(Image image, RectangleF destRect)
        {
            GLBitmapTexture glBitmapTexture = image.InnerImage as GLBitmapTexture;
            if (glBitmapTexture != null)
            {
                canvasGL2d.DrawImage(glBitmapTexture, destRect.X, destRect.Y, destRect.Width, destRect.Height);
            }
            else
            {
                var currentInnerImage = image.InnerImage as System.Drawing.Bitmap;
                if (currentInnerImage != null)
                {
                    //create  and replace ?
                    //TODO: add to another field
                    image.InnerImage = glBitmapTexture = GLBitmapTextureHelper.CreateBitmapTexture(currentInnerImage);
                    canvasGL2d.DrawImage(glBitmapTexture, destRect.X, destRect.Y, destRect.Width, destRect.Height);
                }
            }
        }
        public override void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {
            //copy from src to dest

            GLBitmapTexture glBitmapTexture = image.InnerImage as GLBitmapTexture;
            if (glBitmapTexture != null)
            {
                canvasGL2d.DrawImage(glBitmapTexture, srcRect,
                    destRect.X, destRect.Y, destRect.Width, destRect.Height, ImageFillStyle.Stretch);
            }
            else
            {
                var currentInnerImage = image.InnerImage as System.Drawing.Bitmap;
                if (currentInnerImage != null)
                {
                    //create  and replace ?
                    //TODO: add to another field
                    image.InnerImage = glBitmapTexture = GLBitmapTextureHelper.CreateBitmapTexture(currentInnerImage);
                    canvasGL2d.DrawImage(glBitmapTexture,
                        srcRect, destRect.X, destRect.Y, destRect.Width, destRect.Height, ImageFillStyle.Stretch);
                }
            }
        }
        public override Color StrokeColor
        {
            get
            {
                return base.StrokeColor;
            }
            set
            {
                base.StrokeColor = value;
                canvasGL2d.FillColor = value;
            }
        }
        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            canvasGL2d.DrawLine(x1, y1, x2, y2);
        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            //stroke color
            canvasGL2d.FillColor = color;
            canvasGL2d.DrawRect(left, top, width, height);

        }
        //---------------------------------------------------

        public override Font CurrentFont
        {
            get
            {
                return base.CurrentFont;
            }
            set
            {
                //sample only *** 
                canvasGL2d.CurrentFont = PixelFarm.Agg.Fonts.NativeFontStore.LoadFont("c:\\Windows\\Fonts\\Tahoma.ttf", 64);
                base.CurrentFont = value;
            }
        }
        public override void DrawText(char[] buffer, int x, int y)
        {
            canvasGL2d.DrawString(buffer, x, y);

        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            canvasGL2d.DrawString(buffer, logicalTextBox.X, logicalTextBox.Y);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            canvasGL2d.DrawString(str, logicalTextBox.X, logicalTextBox.Y);
        }
        //--------------------------------------------------- 
        public override void FillPath(GraphicsPath gfxPath)
        {
            var innerPath2 = gfxPath.InnerPath2;
            if (innerPath2 == null)
            {
                System.Drawing.Drawing2D.PathData pathData = gfxPath.GetPathData() as System.Drawing.Drawing2D.PathData;
                PixelFarm.Agg.VertexStore vxs = new PixelFarm.Agg.VertexStore();
                PixelFarm.Agg.GdiPathConverter.ConvertToVxs(pathData, vxs);
                PixelFarm.Agg.VertexSource.CurveFlattener flattener = new PixelFarm.Agg.VertexSource.CurveFlattener();
                vxs = flattener.MakeVxs2(vxs);
                gfxPath.InnerPath2 = vxs;

                this.canvasGL2d.FillVxs(vxs);
            }
            else
            {
                PixelFarm.Agg.VertexStore vxs = innerPath2 as PixelFarm.Agg.VertexStore;
                if (vxs != null)
                {
                    this.canvasGL2d.FillVxs(vxs);
                }
            }
        }
        public override void FillPath(GraphicsPath path, Brush brush)
        {
            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //solid brush
                        var solidBrush = (SolidBrush)brush;
                        this.FillColor = solidBrush.Color;
                        this.FillPath(path);
                    } break;
            }
        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            var innerPath2 = gfxPath.InnerPath2;
            if (innerPath2 == null)
            {
                System.Drawing.Drawing2D.PathData pathData = gfxPath.GetPathData() as System.Drawing.Drawing2D.PathData;
                PixelFarm.Agg.VertexStore vxs = new PixelFarm.Agg.VertexStore();
                PixelFarm.Agg.GdiPathConverter.ConvertToVxs(pathData, vxs);
                PixelFarm.Agg.VertexSource.CurveFlattener flattener = new PixelFarm.Agg.VertexSource.CurveFlattener();
                vxs = flattener.MakeVxs2(vxs);
                gfxPath.InnerPath2 = vxs;

                this.canvasGL2d.DrawVxs(vxs);

            }
            else
            {
                PixelFarm.Agg.VertexStore vxs = innerPath2 as PixelFarm.Agg.VertexStore;
                if (vxs != null)
                {
                    this.canvasGL2d.DrawVxs(vxs);
                }
            }
        }

        //---------------------------------------------------

    }
}