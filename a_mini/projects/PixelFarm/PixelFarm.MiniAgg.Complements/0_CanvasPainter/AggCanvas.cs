//MIT, 2014-2017, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg.Imaging;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Agg
{
    public class AggDrawBoard : DrawBoard
    {
        //this class wrap agg painter functionality


        ActualImage _rawAggImage;
        AggRenderSurface _renderSurface;
        AggPainter _aggPainter;
        public AggDrawBoard(int width, int height)
        {
            //1. 
            _rawAggImage = new ActualImage(width, height, PixelFormat.ARGB32);
            //2. 
            _renderSurface = new AggRenderSurface(_rawAggImage);
            //3. 
            _aggPainter = new AggPainter(_renderSurface);
        }
        public override SmoothingMode SmoothingMode
        {
            get { return _aggPainter.SmoothingMode; }
            set
            {
                _aggPainter.SmoothingMode = value;
            }
        }
        public override float StrokeWidth
        {
            get { return (float)_aggPainter.StrokeWidth; }
            set
            {
                _aggPainter.StrokeWidth = value;
            }
        }
        public override Color StrokeColor
        {
            get
            {
                return _aggPainter.StrokeColor;
            }
            set
            {
                _aggPainter.StrokeColor = value;
            }
        }
        public override Brush CurrentBrush
        {

            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }

        }

        public override Rectangle InvalidateArea
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public override int Top
        {
            get { return 0; }
        }
        public override int Left
        {
            get { return 0; }
        }

        public override int Width { get { return _rawAggImage.Width; } }
        public override int Height { get { return _rawAggImage.Height; } }
        public override int Bottom { get { return _rawAggImage.Height; } }
        public override int Right { get { return _rawAggImage.Width; } }

        public override Rectangle Rect
        {
            get
            {
                return new Rectangle(0, 0, Width, Height);
            }
        }

        public override int OriginX
        {
            get
            {
                //this loss the data
                return (int)_aggPainter.OriginX;
            }
        }


        public override int OriginY
        {
            get
            {
                //this loss the data
                return (int)_aggPainter.OriginY;
            }
        }

        public override Rectangle CurrentClipRect
        {
            get
            {
                throw new NotImplementedException();
            }

        }

        public override RequestFont CurrentFont { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Color CurrentTextColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Clear(Color c)
        {
            throw new NotImplementedException();
        }

        public override void CloseCanvas()
        {
            throw new NotImplementedException();
        }

        public override void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public override void dbug_DrawRuler(int x)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(Image image, RectangleF dest, RectangleF src)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(Image image, RectangleF dest)
        {
            throw new NotImplementedException();
        }

        public override void DrawImages(Image image, RectangleF[] destAndSrcPairs)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            throw new NotImplementedException();
        }

        public override void DrawPath(GraphicsPath gfxPath)
        {
            throw new NotImplementedException();
        }

        public override void DrawRectangle(float left, float top, float width, float height)
        {
            throw new NotImplementedException();
        }

        public override void DrawText(char[] buffer, int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            throw new NotImplementedException();
        }

        public override void DrawText(char[] buffer, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override void FillPath(GraphicsPath gfxPath)
        {
            throw new NotImplementedException();
        }

        public override void FillPolygon(PointF[] points)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle(float left, float top, float width, float height)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Invalidate(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public override void PopClipAreaRect()
        {
            throw new NotImplementedException();
        }

        public override bool PushClipAreaRect(int width, int height, ref Rectangle updateArea)
        {
            throw new NotImplementedException();
        }

        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {
            throw new NotImplementedException();
        }

        public override void ResetInvalidateArea()
        {
            throw new NotImplementedException();
        }

        public override void SetCanvasOrigin(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void SetClipRect(Rectangle clip, CombineMode combineMode = CombineMode.Replace)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}