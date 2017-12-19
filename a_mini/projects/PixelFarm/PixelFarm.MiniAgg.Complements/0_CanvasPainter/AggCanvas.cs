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
    public class AggCanvas : IDrawBoard
    {

        public AggCanvas()
        {

        }
        public override SmoothingMode SmoothingMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float StrokeWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Color StrokeColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Brush CurrentBrush { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Rectangle InvalidateArea => throw new NotImplementedException();

        public override int Top => throw new NotImplementedException();

        public override int Left => throw new NotImplementedException();

        public override int Width => throw new NotImplementedException();

        public override int Height => throw new NotImplementedException();

        public override int Bottom => throw new NotImplementedException();

        public override int Right => throw new NotImplementedException();

        public override Rectangle Rect => throw new NotImplementedException();

        public override int CanvasOriginX => throw new NotImplementedException();

        public override int CanvasOriginY => throw new NotImplementedException();

        public override Rectangle CurrentClipRect => throw new NotImplementedException();

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