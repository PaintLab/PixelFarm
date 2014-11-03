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
        ScanlineRasToDestBitmapRenderer sclineRasToBmp;
        ColorRGBA fillColor;
        ScanlineRasterizer m_ras;       
        ScanlinePacked8 m_sl;

        FilterMan filterMan = new FilterMan();
        IPixelBlender pixBlender;

        public CanvasPainter(Graphics2D graphic2d)
        {
            this.gx = graphic2d;
            this.m_ras = gx.ScanlineRasterizer;
            this.stroke = new Stroke(1);//default

            this.m_sl = graphic2d.ScanlinePacked8;
            this.pixBlender = graphic2d.PixelBlender;
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
        public void Circle(double x, double y, double radius, ColorRGBA color)
        {
            Ellipse elipse = new Ellipse(x, y, radius, radius);
            gx.Render(elipse.MakeVxs(), color);
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
            PathStorage m_LinesToDraw = new PathStorage();
            m_LinesToDraw.Clear();
            m_LinesToDraw.MoveTo(x1, y1);
            m_LinesToDraw.LineTo(x2, y2);
            gx.Render(stroke.MakeVxs(m_LinesToDraw.Vxs), color);
        }
        public double StrokeWidth
        {
            get { return this.stroke.Width; }
            set { this.stroke.Width = value; }
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
            SimpleRect simpleRect = new SimpleRect(left + .5, bottom + .5, right - .5, top - .5);
            gx.Render(stroke.MakeVxs(simpleRect.MakeVxs()), color);
        }

        public void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            SimpleRect rect = new SimpleRect(left, bottom, right, top);
            gx.Render(rect.MakeVertexSnap(), fillColor);
        }

        public void DrawString(
            string text,
            double x,
            double y,
            double pointSize = 12,
            Justification justification = Justification.Left,
            Baseline baseline = Baseline.Text,
            ColorRGBA color = new ColorRGBA(),
            bool drawFromHintedCache = false,
            ColorRGBA backgroundColor = new ColorRGBA())
        {

            TypeFacePrinter stringPrinter = new TypeFacePrinter(text, pointSize, new Vector2(x, y), justification, baseline);
            if (color.Alpha0To255 == 0)
            {
                color = ColorRGBA.Black;
            }

            if (backgroundColor.Alpha0To255 != 0)
            {
                gx.FillRectangle(stringPrinter.LocalBounds, backgroundColor);
            }

            stringPrinter.DrawFromHintedCache = drawFromHintedCache;
            stringPrinter.Render(gx, color);
        }
        public void DrawString2(
           string text,
           double x,
           double y,
           double pointSize = 12,
           Justification justification = Justification.Left,
           Baseline baseline = Baseline.Text,
           ColorRGBA color = new ColorRGBA(),
           bool drawFromHintedCache = false,
           ColorRGBA backgroundColor = new ColorRGBA())
        {

            //1. parse text  
            var stringPrinter = new PixelFarm.Agg.Font.TypeFacePrinter2(
                text,
                pointSize,
                new Vector2(x, y), justification, baseline);

            if (color.Alpha0To255 == 0)
            {
                color = ColorRGBA.Black;
            }

            if (backgroundColor.Alpha0To255 != 0)
            {
                gx.FillRectangle(stringPrinter.LocalBounds, backgroundColor);
            }

            stringPrinter.DrawFromHintedCache = drawFromHintedCache;
            stringPrinter.Render(gx, color);
        }
        /// <summary>
        /// fill vertex store
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public void Fill(VertexStoreSnap snap)
        {   
            m_ras.AddPath(snap);
            sclineRasToBmp.RenderScanlineSolidAA(this.gx.DestImage, m_ras, m_sl, fillColor);
        }
        public void Fill(VertexStore vxs)
        {
            m_ras.AddPath(vxs);
            sclineRasToBmp.RenderScanlineSolidAA(this.gx.DestImage, m_ras, m_sl, fillColor);
        }
        //public void RenderImage(IImageReaderWriter img)
        //{
        //    //sclineRasToBmp.RenderScanlineSolidAA(clippingProxy, m_ras, m_sl,
        //    //    ColorRGBAf.MakeColorRGBA(0.6, 0.9, 0.7, 0.8));
        //}
        public ColorRGBA FillColor
        {
            get { return fillColor; }
            set { this.fillColor = value; }
        }

        /// <summary>
        /// do filter at specific area
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="area"></param>
        public void DoFilterBlurStack(RectInt area, int r)
        {
            ChildImage img = new ChildImage(this.gx.DestImage, this.pixBlender,
                area.Left, area.Bottom, area.Right, area.Top);
            filterMan.DoStackBlur(img, r);
        }
        public void DoFilterBlurRecursive(RectInt area, int r)
        {   
            ChildImage img = new ChildImage(this.gx.DestImage, this.pixBlender,
                area.Left, area.Bottom, area.Right, area.Top);
            filterMan.DoRecursiveBlur(img, r);
        }

    }
    public enum BlurMethod
    {
        StackBlur,
        RecursiveBlur,
        ChannelBlur
    }

    class FilterMan
    {
        StackBlur stackBlur;
        RecursiveBlur m_recursive_blur;

        public void DoStackBlur(ImageReaderWriterBase readerWriter, int radius)
        {
            if (stackBlur == null)
            {
                stackBlur = new StackBlur();
            }
            stackBlur.Blur(readerWriter, radius, radius);
        }
        public void DoRecursiveBlur(ImageReaderWriterBase readerWriter, int radius)
        {   
            if (m_recursive_blur == null)
            {
                m_recursive_blur = new RecursiveBlur(new RecursiveBlurCalcRGB());
            }
            m_recursive_blur.Blur(readerWriter, radius);
        }

    }
}