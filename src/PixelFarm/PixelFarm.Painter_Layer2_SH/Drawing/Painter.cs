﻿//BSD, 2014-present, WinterDev

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



using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing
{


    public enum TargetBuffer
    {
        ColorBuffer,
        MaskBuffer
    }
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

        public abstract float OriginX { get; }
        public abstract float OriginY { get; }
        public abstract void SetOrigin(float ox, float oy);
        public abstract PixelFarm.CpuBlit.VertexProcessing.ICoordTransformer CoordTransformer { get; set; }

        public abstract RenderQuality RenderQuality { get; set; }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract Rectangle ClipBox { get; set; }
        public abstract void SetClipBox(int x1, int y1, int x2, int y2);
        /// <summary>
        /// we DO NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public abstract void SetClipRgn(VertexStore vxs);
        public abstract TargetBuffer TargetBuffer { get; set; }
        public abstract FillingRule FillingRule { get; set; }

        public abstract bool EnableMask { get; set; }
        //
        public abstract double StrokeWidth { get; set; }
        public abstract SmoothingMode SmoothingMode { get; set; }
        public abstract bool UseLcdEffectSubPixelRendering { get; set; }
        public abstract float FillOpacity { get; set; }
        public abstract Color FillColor { get; set; }
        public abstract Color StrokeColor { get; set; }

        public abstract LineJoin LineJoin { get; set; }
        public abstract LineCap LineCap { get; set; }
        public abstract IDashGenerator LineDashGen { get; set; }
        //

        public abstract Brush CurrentBrush { get; set; }
        public abstract Pen CurrentPen { get; set; }

        //
        public abstract void Clear(Color color);
        public abstract RenderSurfaceOriginKind Orientation { get; set; }
        public abstract void DrawLine(double x0, double y0, double x1, double y1);
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
        /// <param name="img"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public abstract void DrawImage(Image img, double left, double top);
        public abstract void DrawImage(Image img, double left, double top, int srcLeft, int srcTop, int srcW, int srcH);
        public abstract void DrawImage(Image img);
        public abstract void DrawImage(Image img, in AffineMat mat);
        public abstract void DrawImage(Image img, double left, double top, CpuBlit.VertexProcessing.ICoordTransformer coordTx);

        public abstract void ApplyFilter(IImageFilter imgFilter);
        ////////////////////////////////////////////////////////////////////////////
        //vertext store/snap/rendervx
        public abstract void Fill(VertexStore vxs);
        public abstract void Draw(VertexStore vxs);
        //---------------------------------------
        public abstract void FillRegion(Region rgn);
        public abstract void FillRegion(VertexStore vxs);
        public abstract void DrawRegion(Region rgn);
        public abstract void DrawRegion(VertexStore vxs);
        //---------------------------------------


        public abstract RenderVx CreateRenderVx(VertexStore vxs);
        public abstract void FillRenderVx(Brush brush, RenderVx renderVx);
        public abstract void FillRenderVx(RenderVx renderVx);
        public abstract void DrawRenderVx(RenderVx renderVx);
        public abstract void Render(RenderVx renderVx);

        //////////////////////////////////////////////////////////////////////////////
        
        public abstract RenderVxFormattedString CreateRenderVx(IFormattedGlyphPlanList formattedGlyphPlans);
        public abstract RenderVxFormattedString CreateRenderVx(string textspan);
        public abstract RenderVxFormattedString CreateRenderVx(char[] textspanBuff, int startAt, int len);

        //text,string
        //TODO: review text drawing funcs 

        public abstract RequestFont CurrentFont { get; set; }

        public abstract void DrawString(
           string text,
           double x,
           double y);
        public abstract void DrawString(RenderVxFormattedString renderVx, double x, double y);
    }


   

    public interface IDashGenerator
    {

    }
    public enum LineCap
    {
        Butt,
        Square,
        Round
    }

    public enum LineJoin
    {
        Miter,
        MiterRevert,
        Round,
        Bevel,
        MiterRound

        //TODO: implement svg arg join
    }

    public enum InnerJoin
    {
        Bevel,
        Miter,
        Jag,
        Round
    }
}