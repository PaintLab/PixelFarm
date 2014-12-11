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
        CanvasGL2d canvasGL2d;
        public MyCanvasGL(GraphicsPlatform platform, int hPageNum, int vPageNum, int left, int top, int width, int height)
            : base(platform, hPageNum, vPageNum, left, top, width, height)
        {
            canvasGL2d = new CanvasGL2d();
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
            canvasGL2d.FillColor = color;
            canvasGL2d.FillRect(left, top, width, height);
        }
        public override void FillPolygon(PointF[] points)
        {
            int j = points.Length;
            float[] polygonPoints = new float[j];

            int n = 0;
            for (int i = 0; i < j; )
            {
                polygonPoints[n] = points[i].X;
                polygonPoints[n + 1] = points[i].Y;
                n += 2;
            }
            canvasGL2d.FillPolygon(polygonPoints);

        }
        public override void FillPath(GraphicsPath gfxPath)
        {
            //convert graphics path to vxs ?
        }
        public override void FillPath(GraphicsPath path, Brush brush)
        {

        }
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            //not implement
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
        public override void DrawPath(GraphicsPath gfxPath)
        {
            throw new NotImplementedException();
        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            throw new NotImplementedException();

        }
        //---------------------------------------------------
        public override void DrawText(char[] buffer, int x, int y)
        {
            //base.DrawText(buffer, x, y);
            throw new NotImplementedException();
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            //base.DrawText(buffer, logicalTextBox, textAlignment);
            throw new NotImplementedException();
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //base.DrawText(str, startAt, len, logicalTextBox, textAlignment);
            throw new NotImplementedException();
        }
        //---------------------------------------------------


    }
}