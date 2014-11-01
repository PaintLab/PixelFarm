//2014 BSD,WinterDev  
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
using System.Collections.Generic;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Transform;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg
{

    public abstract class Graphics2D
    {

        protected IImageReaderWriter destImageByte;
        protected ScanlineRasterizer rasterizer;


        Affine currentTxMatrix;
        public Graphics2D(IImageReaderWriter destImage, ScanlineRasterizer rasterizer)
        {
            currentTxMatrix = Affine.IdentityMatrix;
            destImageByte = destImage;
            this.rasterizer = rasterizer;
        }

        //------------------------------------------------------------------------
        public abstract void Clear(ColorRGBA color);
        public abstract void SetClippingRect(RectangleDouble rect_d);
        public abstract RectangleDouble GetClippingRect();
        public abstract RectangleInt GetClippingRectInt();
        //------------------------------------------------------------------------

        public abstract void Render(VertexStoreSnap vertexSource, ColorRGBA colorBytes);
        //------------------------------------------------------------------------
        public abstract void Render(IImageReaderWriter imageSource,
            double x, double y,
            double angleRadians,
            double scaleX, double ScaleY);
        public abstract void Render(IImageReaderWriter imageSource, double x, double y);
        public void Render(IImageReaderWriter imageSource, int x, int y)
        {
            this.Render(imageSource, (double)x, (double)y);
        }
        public abstract void Render(ActualImage actualImage, int x, int y);

        //------------------------------------------------------------------------


        public void Render(VertexStore vxStorage, ColorRGBA c)
        {
            Render(new VertexStoreSnap(vxStorage), c);
        }
        public void Render(VertexStoreSnap vertexSource, double x, double y, ColorRGBA color)
        {
            var inputVxs = vertexSource.GetInternalVxs();
            var vxs = Affine.NewTranslation(x, y).TransformToVertexSnap(inputVxs);
            Render(vxs, color);
        }



        public Affine CurrentTransformMatrix
        {
            get { return this.currentTxMatrix; }
            set
            {

                this.currentTxMatrix = value;
            }
        }
 
        public ScanlineRasterizer Rasterizer
        {
            get { return rasterizer; }
        }


        public IImageReaderWriter DestImage
        {
            get
            {
                return destImageByte;
            }
        }
        //================
        public static Graphics2D CreateFromImage(ActualImage actualImage)
        {
            var img = new MyImageReaderWriter(actualImage);
            var childImage = new ChildImage(img, img.GetRecieveBlender());
            var scanlineRaster = new ScanlineRasterizer();
            var scanlineCachedPacked8 = new ScanlinePacked8();

            ImageGraphics2D imageRenderer = new ImageGraphics2D(childImage, scanlineRaster, scanlineCachedPacked8);
            imageRenderer.Rasterizer.SetVectorClipBox(0, 0, img.Width, img.Height);
            return imageRenderer;
        }
        public static Graphics2D CreateFromImage(IImageReaderWriter img)
        {

            var childImage = new ChildImage(img, img.GetRecieveBlender());
            var scanlineRaster = new ScanlineRasterizer();
            var scanlineCachedPacked8 = new ScanlinePacked8();

            ImageGraphics2D imageRenderer = new ImageGraphics2D(childImage, scanlineRaster, scanlineCachedPacked8);
            imageRenderer.Rasterizer.SetVectorClipBox(0, 0, img.Width, img.Height);
            return imageRenderer;
        }

#if DEBUG
        public void dbugLine(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            PathStorage m_LinesToDraw = new PathStorage();
            m_LinesToDraw.Clear();
            m_LinesToDraw.MoveTo(x1, y1);
            m_LinesToDraw.LineTo(x2, y2);

            Render(new Stroke(1).MakeVxs(m_LinesToDraw.Vxs), color);
        }
#endif



    }
}
