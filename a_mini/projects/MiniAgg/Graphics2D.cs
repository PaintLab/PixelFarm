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
//using MatterHackers.Agg.Font;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg
{
    public interface IStyleHandler
    {
        bool is_solid(int style);
        RGBA_Bytes color(int style);
        void generate_span(RGBA_Bytes[] span, int spanIndex, int x, int y, int len, int style);
    };

    public abstract class Graphics2D
    {
        const int cover_full = 255;
        protected IImageByte destImageByte;
        protected IImageFloat destImageFloat;
        protected Stroke StrockedText;
        protected Stack<Affine> affineTransformStack = new Stack<Affine>();
        protected ScanlineRasterizer rasterizer;

        public Graphics2D()
        {
            affineTransformStack.Push(Affine.NewIdentity());
        }

        public Graphics2D(IImageByte destImage, ScanlineRasterizer rasterizer)
            : this()
        {
            Initialize(destImage, rasterizer);
        }

        public void Initialize(IImageByte destImage, ScanlineRasterizer rasterizer)
        {
            destImageByte = destImage;
            destImageFloat = null;
            this.rasterizer = rasterizer;
        }

        public void Initialize(IImageFloat destImage, ScanlineRasterizer rasterizer)
        {
            destImageByte = null;
            destImageFloat = destImage;
            this.rasterizer = rasterizer;
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

        public abstract IScanlineCache ScanlineCache
        {
            get;
            set;
        }

        public IImageByte DestImage
        {
            get
            {
                return destImageByte;
            }
        }

        public IImageFloat DestImageFloat
        {
            get
            {
                return destImageFloat;
            }
        }

        public abstract void Render(IVertexSource vertexSource, int pathIndexToRender, RGBA_Bytes colorBytes);

        public void Render(IImageByte imageSource, Point2D position)
        {
            Render(imageSource, position.x, position.y);
        }

        public void Render(IImageByte imageSource, Vector2 position)
        {
            Render(imageSource, position.x, position.y);
        }

        public void Render(IImageByte imageSource, double x, double y)
        {
            Render(imageSource, x, y, 0, 1, 1);
        }

        public abstract void Render(IImageByte imageSource,
            double x, double y,
            double angleRadians,
            double scaleX, double ScaleY);

        public abstract void Render(IImageFloat imageSource,
            double x, double y,
            double angleRadians,
            double scaleX, double ScaleY);

        public void Render(IVertexSource vertexSource, RGBA_Bytes[] colorArray, int[] pathIdArray, int numPaths)
        {
            for (int i = 0; i < numPaths; i++)
            {
                Render(vertexSource, pathIdArray[i], colorArray[i]);
            }
        }

        public void Render(IVertexSource vertexSource, RGBA_Bytes color)
        {
            Render(vertexSource, 0, color);
        }

        public void Render(IVertexSource vertexSource, double x, double y, RGBA_Bytes color)
        {
            Render(new VertexSourceApplyTransform(vertexSource, Affine.NewTranslation(x, y)), 0, color);
        }

        public void Render(IVertexSource vertexSource, Vector2 position, RGBA_Bytes color)
        {
            Render(new VertexSourceApplyTransform(vertexSource, Affine.NewTranslation(position.x, position.y)), 0, color);
        }

        public abstract void Clear(IColorType color);

        
     
     

        public void Line(Vector2 start, Vector2 end, RGBA_Bytes color)
        {
            Line(start.x, start.y, end.x, end.y, color);
        }

        public void Line(double x1, double y1, double x2, double y2, RGBA_Bytes color)
        {
            PathStorage m_LinesToDraw = new PathStorage();
            m_LinesToDraw.remove_all();
            m_LinesToDraw.MoveTo(x1, y1);
            m_LinesToDraw.LineTo(x2, y2);
            Stroke StrockedLineToDraw = new Stroke(m_LinesToDraw);
            Render(StrockedLineToDraw, color);
        }

        public abstract void SetClippingRect(RectangleDouble rect_d);
        public abstract RectangleDouble GetClippingRect();

        

        public static void AssertDebugNotDefined()
        {
#if DEBUG
            throw new Exception("DEBUG is defined and should not be!");
#endif
        }








    }
}
