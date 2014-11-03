//BSD 2014, WinterDev

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Agg;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Font;


namespace PixelFarm.Agg
{
    public class CanvasPainter
    {
        Graphics2D gx;
        Stroke stroke;

        ColorRGBA fillColor;
        ColorRGBA strokeColor;

        ScanlinePacked8 scline;
        ScanlineRasterizer sclineRas;
        ScanlineRasToDestBitmapRenderer sclineRasToBmp;

        FilterMan filterMan = new FilterMan();

        //-------------
        //tools
        //-------------
        SimpleRect simpleRect = new SimpleRect();
        Ellipse ellipse = new Ellipse();
        PathStorage lines = new PathStorage();
        RoundedRect roundRect = null;
        MyTypeFacePrinter stringPrinter = new MyTypeFacePrinter();

        //-------------
        public CanvasPainter(Graphics2D graphic2d)
        {
            this.gx = graphic2d;
            this.sclineRas = gx.ScanlineRasterizer;
            this.stroke = new Stroke(1);//default

            this.scline = graphic2d.ScanlinePacked8;
            this.sclineRasToBmp = graphic2d.ScanlineRasToDestBitmap;

        }
        public void Clear(ColorRGBA color)
        {
            gx.Clear(color);
        }
        public RectInt ClipBox
        {
            get { return this.gx.GetClippingRect(); }
            set { this.gx.SetClippingRect(value); }
        }
        public void SetClipBox(int x1, int y1, int x2, int y2)
        {
            this.gx.SetClippingRect(new RectInt(x1, y1, x2, y2));
        }
        public Graphics2D Graphics
        {
            get { return this.gx; }
        }

        /// <summary>
        /// draw circle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void FillCircle(double x, double y, double radius, ColorRGBA color)
        {
            ellipse.Reset(x, y, radius, radius);
            gx.Render(ellipse.MakeVxs(), color);
        }
        public void FillCircle(double x, double y, double radius)
        {
            ellipse.Reset(x, y, radius, radius);
            gx.Render(ellipse.MakeVxs(), this.fillColor);
        }
        public void FillEllipse(double left, double bottom, double right, double top, int nsteps)
        {
            ellipse.Reset((left + right) * 0.5,
                          (bottom + top) * 0.5,
                          (right - left) * 0.5,
                          (top - bottom) * 0.5,
                           nsteps);
            gx.Render(ellipse.MakeVxs(), this.fillColor);
            //VertexStoreSnap trans_ell = txBilinear.TransformToVertexSnap(vxs);
        }
        public void DrawEllipse()
        {

        }
        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void Line(double x1, double y1, double x2, double y2, ColorRGBA color)
        {

            lines.Clear();
            lines.MoveTo(x1, y1);
            lines.LineTo(x2, y2);
            gx.Render(stroke.MakeVxs(lines.Vxs), color);
        }
        /// <summary>
        /// draw line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="color"></param>
        public void Line(double x1, double y1, double x2, double y2)
        {
            lines.Clear();
            lines.MoveTo(x1, y1);
            lines.LineTo(x2, y2);
            gx.Render(stroke.MakeVxs(lines.Vxs), this.strokeColor);
        }
        public double StrokeWidth
        {
            get { return this.stroke.Width; }
            set { this.stroke.Width = value; }
        }
        public void Draw(VertexStore vxs)
        {
            gx.Render(stroke.MakeVxs(vxs), this.strokeColor);
        }

        /// <summary>
        /// draw rectangle
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="color"></param>
        /// <param name="strokeWidth"></param>
        public void Rectangle(double left, double bottom, double right, double top, ColorRGBA color)
        {
            simpleRect.SetRect(left + .5, bottom + .5, right - .5, top - .5);
            gx.Render(stroke.MakeVxs(simpleRect.MakeVxs()), color);
        }
        public void Rectangle(double left, double bottom, double right, double top)
        {
            simpleRect.SetRect(left + .5, bottom + .5, right - .5, top - .5);
            gx.Render(stroke.MakeVxs(simpleRect.MakeVxs()), this.fillColor);
        }
        public void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            simpleRect.SetRect(left, bottom, right, top);
            gx.Render(simpleRect.MakeVertexSnap(), fillColor);
        }
        public void FillRectangle(double left, double bottom, double right, double top)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            simpleRect.SetRect(left, bottom, right, top);
            gx.Render(simpleRect.MakeVertexSnap(), this.fillColor);
        }
        public void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            this.Fill(roundRect.MakeVxs());
        }
        public void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            this.Draw(roundRect.MakeVxs());
        }
        //public void DrawString(
        //    string text,
        //    double x,
        //    double y,
        //    double pointSize = 12,
        //    Justification justification = Justification.Left,
        //    Baseline baseline = Baseline.Text,
        //    ColorRGBA color = new ColorRGBA(),
        //    bool drawFromHintedCache = false,
        //    ColorRGBA backgroundColor = new ColorRGBA())
        //{

        //    TypeFacePrinter stringPrinter = new TypeFacePrinter(text, pointSize, new Vector2(x, y), justification, baseline);
        //    if (color.Alpha0To255 == 0)
        //    {
        //        color = ColorRGBA.Black;
        //    }

        //    if (backgroundColor.Alpha0To255 != 0)
        //    {
        //        gx.FillRectangle(stringPrinter.LocalBounds, backgroundColor);
        //    }

        //    stringPrinter.DrawFromHintedCache = drawFromHintedCache;
        //    stringPrinter.Render(gx, color);
        //}
        public void DrawString(
           string text,
           double x,
           double y)
        {

            //1. parse text              
            stringPrinter.DrawFromHintedCache = false; 
            stringPrinter.LoadText(text);
            var vxs = stringPrinter.MakeVxs();
            vxs = Affine.NewTranslation(x, y).TransformToVxs(vxs);
            this.gx.Render(vxs, this.fillColor); 
        }

        /// <summary>
        /// fill vertex store
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public void Fill(VertexStoreSnap snap)
        {
            sclineRas.AddPath(snap);
            sclineRasToBmp.RenderScanlineSolidAA(this.gx.DestImage, sclineRas, scline, fillColor);
        }
        public void Fill(VertexStore vxs)
        {
            sclineRas.AddPath(vxs);
            sclineRasToBmp.RenderScanlineSolidAA(this.gx.DestImage, sclineRas, scline, fillColor);
        }

        public ColorRGBA FillColor
        {
            get { return fillColor; }
            set { this.fillColor = value; }
        }
        public ColorRGBA StrokeColor
        {
            get { return strokeColor; }
            set { this.strokeColor = value; }
        }
        public void PaintSeries(VertexStore vxs, ColorRGBA[] colors, int[] pathIndexs, int numPath)
        {
            sclineRasToBmp.RenderSolidAllPaths(this.gx.DestImage,
                this.sclineRas,
                this.scline,
                vxs,
                colors,
                pathIndexs,
                numPath);

        }

        //----------------------
        /// <summary>
        /// do filter at specific area
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="area"></param>
        public void DoFilterBlurStack(RectInt area, int r)
        {
            ChildImage img = new ChildImage(this.gx.DestImage, gx.PixelBlender,
                area.Left, area.Bottom, area.Right, area.Top);
            filterMan.DoStackBlur(img, r);
        }
        public void DoFilterBlurRecursive(RectInt area, int r)
        {
            ChildImage img = new ChildImage(this.gx.DestImage, gx.PixelBlender,
                area.Left, area.Bottom, area.Right, area.Top);
            filterMan.DoRecursiveBlur(img, r);
        }
        //----------------




    }

}