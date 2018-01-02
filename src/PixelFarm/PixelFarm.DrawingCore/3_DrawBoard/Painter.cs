//BSD, 2014-2018, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Class StringPrinter.cs
// 
// Class to output the vertex source of a string as a run of glyphs.
//----------------------------------------------------------------------------

using PixelFarm.Agg;
namespace PixelFarm.Drawing
{


    /// <summary>
    /// this class provides drawing method on specific drawboard,
    /// (0,0) is on left-lower corner for every implementaion
    /// </summary>
    public abstract class Painter
    {
        //who implement this class
        //1. AggPainter 
        //2. GdiPlusPainter 
        //3. GLPainter 
        //4. SkiaPainter
        //5. PdfPainter


        public abstract float OriginX { get; }
        public abstract float OriginY { get; }
        public abstract void SetOrigin(float ox, float oy);
        public abstract RenderQualtity RenderQuality { get; set; }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract RectInt ClipBox { get; set; }
        public abstract void SetClipBox(int x1, int y1, int x2, int y2);
        //
        public abstract double StrokeWidth { get; set; }
        public abstract SmoothingMode SmoothingMode { get; set; }
        public abstract bool UseSubPixelLcdEffect { get; set; }
        public abstract Color FillColor { get; set; }
        public abstract Color StrokeColor { get; set; }
        //
        public abstract void Clear(Color color);
        public abstract DrawBoardOrientation Orientation { get; set; }
        public abstract void DrawLine(double x1, double y1, double x2, double y2);
        // 
        public abstract void DrawRect(double left, double top, double width, double height);
        public abstract void FillRect(double left, double top, double width, double height);
        //
        public abstract void FillEllipse(double left, double top, double width, double height);
        public abstract void DrawEllipse(double left, double top, double width, double height);
        //
 
        /// <summary>
        /// draw image, not scale
        /// </summary>
        /// <param name="actualImage"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public abstract void DrawImage(Image actualImage, double left, double top);
        public abstract void DrawImage(Image actualImage, params Agg.Transform.AffinePlan[] affinePlans);

        public abstract void ApplyFilter(ImageFilter imgFilter);

      
        ////////////////////////////////////////////////////////////////////////////
        //vertext store/snap/rendervx
        public abstract void Fill(VertexStoreSnap snap);
        public abstract void Fill(VertexStore vxs);

        //TODO: remove paint series, 
        public abstract void PaintSeries(VertexStore vxs, Color[] colors, int[] pathIndexs, int numPath);
        public abstract void Draw(VertexStore vxs);
        public abstract void Draw(VertexStoreSnap vxs);
        public abstract RenderVx CreateRenderVx(VertexStoreSnap snap);
        public abstract RenderVxFormattedString CreateRenderVx(string textspan);
        public abstract void FillRenderVx(Brush brush, RenderVx renderVx);
        public abstract void FillRenderVx(RenderVx renderVx);
        public abstract void DrawRenderVx(RenderVx renderVx);
        //////////////////////////////////////////////////////////////////////////////
        //text,string
        //TODO: review text drawing funcs 

        public abstract RequestFont CurrentFont { get; set; }
        public abstract void DrawString(
           string text,
           double x,
           double y);
        public abstract void DrawString(RenderVxFormattedString renderVx, double x, double y);
    }




}