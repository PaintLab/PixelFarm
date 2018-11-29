//BSD, 2014-present, WinterDev
//ArthurHub, Jose Manuel Menendez Poo

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

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.Drawing.GLES2
{

    partial class MyGLDrawBoard
    {
        class MyGLCanvasException : Exception { }

        float _strokeWidth = 1f;
        Color _fillSolidColor = Color.Transparent;
        Color _strokeColor = Color.Black;

        //==========================================================
        public override Color StrokeColor
        {
            get
            {
                return this._strokeColor;
            }
            set
            {
                _gpuPainter.StrokeColor = this._strokeColor = value;
            }
        }
        public override float StrokeWidth
        {
            get
            {
                return this._strokeWidth;
            }
            set
            {
                _gpuPainter.StrokeWidth = this._strokeWidth = value;
            }
        }

        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {

            //throw new MyGLCanvasException();
            //IntPtr gxdc = gx.GetHdc();
            //MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //MyWin32.BitBlt(destHdc, destArea.X, destArea.Y,
            //destArea.Width, destArea.Height, gxdc, sourceX, sourceY, MyWin32.SRCCOPY);
            //MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            //gx.ReleaseHdc();
        }
        public override void Clear(PixelFarm.Drawing.Color c)
        {
            _gpuPainter.Clear(c);
        }

        static void ResolveGraphicsPath(GraphicsPath path, VertexStore outputVxs)
        {
            //convert from graphics path to internal presentation
            VertexStore innerPath = path.InnerPath as VertexStore;
            if (innerPath != null)
            {
                return;
                //return innerPath;
            }
            //-------- 

            path.InnerPath = outputVxs;
            using (VectorToolBox.Borrow(out PathWriter writer))
            {
                List<float> points;
                List<PathCommand> cmds;

                writer.AttachExternalVxs(outputVxs);
                GraphicsPath.GetPathData(path, out points, out cmds);
                int j = cmds.Count;
                int p_index = 0;


                for (int i = 0; i < j; ++i)
                {
                    PathCommand cmd = cmds[i];
                    switch (cmd)
                    {
                        default:
                            throw new NotSupportedException();
                        case PathCommand.Arc:
                            {
                                //TODO: review arc
                                //convert to curve?
                            }
                            //innerPath.AddArc(
                            //    points[p_index],
                            //    points[p_index + 1],
                            //    points[p_index + 2],
                            //    points[p_index + 3],
                            //    points[p_index + 4],
                            //    points[p_index + 5]);
                            p_index += 6;
                            break;
                        case PathCommand.Bezier:

                            writer.MoveTo(points[p_index],
                                points[p_index + 1]);
                            writer.Curve4(
                                points[p_index + 2],
                                points[p_index + 3],
                                points[p_index + 4],
                                points[p_index + 5],
                                points[p_index + 6],
                                points[p_index + 7]);

                            p_index += 8;
                            break;
                        case PathCommand.CloseFigure:
                            writer.CloseFigure();
                            //innerPath.CloseFigure();
                            break;
                        case PathCommand.Ellipse:
                            using (VectorToolBox.Borrow(out CpuBlit.VertexProcessing.Ellipse ellipse))
                            {
                                ellipse.SetFromLTWH(
                                    points[p_index],
                                    points[p_index + 1],
                                    points[p_index + 2],
                                    points[p_index + 3]);
                                ellipse.MakeVxs(writer);
                            }

                            p_index += 4;
                            break;
                        case PathCommand.Line:
                            {
                                writer.MoveTo(points[p_index],
                                              points[p_index + 1]);
                                writer.LineTo(points[p_index + 2],
                                              points[p_index + 3]);
                            }
                            p_index += 4;
                            break;
                        case PathCommand.Rect:
                            using (VectorToolBox.Borrow(out CpuBlit.VertexProcessing.SimpleRect simpleRect))
                            {
                                simpleRect.SetRectFromLTWH(
                                    points[p_index],
                                    points[p_index + 1],
                                    points[p_index + 2],
                                    points[p_index + 3]
                                    );
                                simpleRect.MakeVxs(writer);
                            }

                            p_index += 4;
                            break;
                        case PathCommand.StartFigure:
                            break;
                    }
                }

                writer.DetachExternalVxs();
            }
        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            //convert path to vxs


            //throw new MyGLCanvasException();
            //gx.DrawPath(internalPen, gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {

            switch (brush.BrushKind)
            {
                case BrushKind.Solid:
                    {
                        //use default solid brush
                        SolidBrush solidBrush = (SolidBrush)brush;
                        _gpuPainter.FillRect(
                            left, top,
                            width, height,
                            solidBrush.Color);
                    }
                    break;
                case BrushKind.LinearGradient:
                    {


                        // throw new MyGLCanvasException();
                    }
                    break;
                case BrushKind.GeometryGradient:
                    {
                    }
                    break;
                case BrushKind.CircularGraident:
                    {
                    }
                    break;
                case BrushKind.Texture:
                    {
                    }
                    break;
            }
        }
        public override void FillRectangle(Color color, float left, float top, float width, float height)
        {
            _gpuPainter.FillRect(left, top, width, height, color);
        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            _gpuPainter.DrawRect(left, top, width, height);
        }
        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            _gpuPainter.DrawLine(x1, y1, x2, y2);
        }



        /// <summary>
        /// Gets or sets the rendering quality for this <see cref="T:System.Drawing.Graphics"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Drawing.Drawing2D.SmoothingMode"/> values.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return _gpuPainter.SmoothingMode;
            }
            set
            {
                _gpuPainter.SmoothingMode = value;
            }
        }

        /// <summary>
        /// Draws the specified portion of the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param>
        /// <param name="destRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle. </param>
        /// <param name="srcRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the portion of the <paramref name="image"/> object to draw. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception>
        public override void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {
            DrawingGL.GLBitmap glbmp = ResolveForGLBitmap(image);
            if (glbmp != null)
            {
                glbmp.NotifyUsage();
                _gpuPainter.Canvas.DrawSubImage(glbmp, destRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, destRect.Left, destRect.Top);
            }
        }
        public override void DrawImage(Image image, int x, int y)
        {
            DrawingGL.GLBitmap glbmp = ResolveForGLBitmap(image);
            if (glbmp != null)
            {
                glbmp.NotifyUsage();
                _gpuPainter.Canvas.DrawSubImage(glbmp, 0, 0, glbmp.Width, glbmp.Height, x, y);
            }
        }
        public override void DrawImages(Image image, RectangleF[] destAndSrcPairs)
        {
            //... 
            //throw new MyGLCanvasException();
            //int j = destAndSrcPairs.Length;
            //if (j > 1)
            //{
            //    if ((j % 2) != 0)
            //    {
            //        //make it even number
            //        j -= 1;
            //    }
            //    //loop draw
            //    var inner = image.InnerImage as System.Drawing.Image;
            //    for (int i = 0; i < j;)
            //    {
            //        gx.DrawImage(inner,
            //            destAndSrcPairs[i].ToRectF(),
            //            destAndSrcPairs[i + 1].ToRectF(),
            //            System.Drawing.GraphicsUnit.Pixel);
            //        i += 2;
            //    }
            //}
        }
        static DrawingGL.GLBitmap ResolveForGLBitmap(Image image)
        {
            //1.
            DrawingGL.GLBitmap glBmp = image as DrawingGL.GLBitmap;
            if (glBmp != null) return glBmp;
            //
            var cacheBmp = Image.GetCacheInnerImage(image) as DrawingGL.GLBitmap;
            if (cacheBmp != null)
            {
                return cacheBmp;
            }

            var binder = image as BitmapBufferProvider;
            if (binder != null)
            {
                glBmp = new DrawingGL.GLBitmap(binder);
                Image.SetCacheInnerImage(image, glBmp);
                return glBmp;
            }


            //TODO: review here
            //we should create 'borrow' method ? => send direct exact ptr to img buffer
            //for now, create a new one -- after we copy we, don't use it
            MemBitmap bmp = image as MemBitmap;
            if (bmp != null)
            {
                glBmp = new DrawingGL.GLBitmap(new LazyMemBitmapBufferProvider(bmp, false));
                Image.SetCacheInnerImage(image, glBmp);
                return glBmp;
            }
            else
            {
                return null;
                //var req = new Image.ImgBufferRequestArgs(32, Image.RequestType.Copy);
                //image.RequestInternalBuffer(ref req);
                ////**
                //glBmp = new DrawingGL.GLBitmap(image.Width, image.Height, req.OutputBuffer32, req.IsInvertedImage);
                //Image.SetCacheInnerImage(image, glBmp);
                //return glBmp;
            }


        }
        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void DrawImage(Image image, RectangleF destRect)
        {
            //1. image from outside
            //resolve to internal presentation 
            DrawingGL.GLBitmap glbmp = ResolveForGLBitmap(image);
            if (glbmp != null)
            {
                glbmp.NotifyUsage();
                _gpuPainter.Canvas.DrawImage(glbmp, destRect.Left, destRect.Top, destRect.Width, destRect.Height);
            }

        }
#if DEBUG


#endif
        public override void FillPath(Color color, GraphicsPath path)
        {
            using (VxsTemp.Borrow(out VertexStore vxs))
            {
                ResolveGraphicsPath(path, vxs);
                Color prevFill = _gpuPainter.FillColor;
                _gpuPainter.FillColor = color;
                _gpuPainter.Fill(vxs);
                _gpuPainter.FillColor = prevFill;
            }
        }
        /// <summary>
        /// Fills the interior of a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> that represents the path to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void FillPath(Brush brush, GraphicsPath path)
        {
            using (VxsTemp.Borrow(out VertexStore vxs))
            {
                ResolveGraphicsPath(path, vxs);
                switch (brush.BrushKind)
                {
                    case BrushKind.Solid:
                        {
                            SolidBrush solidBrush = (SolidBrush)brush;
                            Color prevFill = _gpuPainter.FillColor;
                            _gpuPainter.FillColor = solidBrush.Color;
                            _gpuPainter.Fill(vxs);
                            _gpuPainter.FillColor = prevFill;
                        }
                        break;
                        // case BrushKind.LinearGradient:
                        //TODO: implement this 
                }
            }

        }

        public override void FillPolygon(Brush brush, PointF[] points)
        {
            //throw new MyGLCanvasException();
            //var pps = ConvPointFArray(points);
            ////use internal solid color            
            //gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
        }
        public override void FillPolygon(Color color, PointF[] points)
        {
            //throw new MyGLCanvasException();
            //var pps = ConvPointFArray(points);
            //internalSolidBrush.Color = ConvColor(color);
            //gx.FillPolygon(this.internalSolidBrush, pps);
        }

    }
}