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

using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;
using MatterHackers.Agg.Transform;
using MatterHackers.VectorMath;

namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute
    {
    }
}

namespace MatterHackers.Agg
{

    public abstract class Graphics2D
    {
        const int COVER_FULL = 255;

        protected IImage destImageByte;
        protected ScanlineRasterizer rasterizer;

        Stack<Affine> affineTransformStack = new Stack<Affine>(); 

        public Graphics2D(IImage destImage, ScanlineRasterizer rasterizer)
        {
            affineTransformStack.Push(Affine.IdentityMatrix);
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

        public abstract void Render(IImage imageSource,
            double x, double y,
            double angleRadians,
            double scaleX, double ScaleY);
        public abstract void Render(IImage imageSource, double x, double y);

        public void Render(IImage imageSource, int x, int y)
        {
            this.Render(imageSource, (double)x, (double)y);
        }
         
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

        public int TransformStackCount
        {
            get { return affineTransformStack.Count; }
        }

        public Affine PopTransform()
        {
            if (affineTransformStack.Count == 1)
            {
                throw new System.Exception("You cannot remove the last transform from the stack.");
            }

            return affineTransformStack.Pop();
        }

        public void PushTransform()
        {
            if (affineTransformStack.Count > 1000)
            {
                throw new System.Exception("You seem to be leaking transforms.  You should be poping some of them at some point.");
            }

            affineTransformStack.Push(affineTransformStack.Peek());
        }

        public Affine GetTransform()
        {
            return affineTransformStack.Peek();
        }

        public void SetTransform(Affine value)
        {
            affineTransformStack.Pop();
            affineTransformStack.Push(value);
        }

        public ScanlineRasterizer Rasterizer
        {
            get { return rasterizer; }
        }


        public IImage DestImage
        {
            get
            {
                return destImageByte;
            }
        }
        //================
        public static Graphics2D CreateFromImage(IImage img)
        {

            var imgProxy = new ChildImage(img, img.GetRecieveBlender());
            var scanlineRaster = new ScanlineRasterizer();
            var scanlineCachedPacked8 = new ScanlinePacked8();
            ImageGraphics2D imageRenderer = new ImageGraphics2D(imgProxy, scanlineRaster, scanlineCachedPacked8);
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
