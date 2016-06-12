using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg.Image;
using System.Text;
namespace Mini.WinForms
{
    //for test only

    abstract class PixelToolController
    {
        public abstract void Draw(Graphics g);
        protected bool IsMouseDown { get; private set; }
        internal void InvokeMouseDown(int x, int y)
        {
            IsMouseDown = true;
            OnMouseDown(x, y);
        }
        internal void InvokeMouseUp(int x, int y)
        {
            OnMouseUp(x, y);
            IsMouseDown = false;
        }
        internal void InvokeMouseMove(int x, int y)
        {
            OnMouseMove(x, y);
        }
        protected virtual void OnMouseDown(int x, int y) { }
        protected virtual void OnMouseMove(int x, int y) { }
        protected virtual void OnMouseUp(int x, int y) { }
    }

    class MyDrawingBrushController : PixelToolController
    {
        PixelFarm.Agg.Image.Point _latestMousePoint;
        PixelFarm.Agg.Samples.MyBrushPath _myBrushPath;
        GraphicsPath _latestBrushPathCache = null;
        List<System.Drawing.Point> _points = new List<System.Drawing.Point>();
        public MyDrawingBrushController()
        {
        }
        public override void Draw(Graphics g)
        {
            DrawLines(g);
            DrawBrushPath(g);
        }
        void DrawLines(Graphics g)
        {
            if (_points.Count > 1)
            {
                g.DrawLines(Pens.Black, _points.ToArray());
            }
        }
        void DrawBrushPath(Graphics g)
        {
            if (_latestBrushPathCache != null)
            {
                g.FillPath(Brushes.Red, _latestBrushPathCache);
                return;
            }

            if (_myBrushPath.vxs != null)
            {
                //create new path 
                PixelFarm.Agg.VertexStore vxs = _myBrushPath.vxs;
                //render vertice in store
                int vcount = vxs.Count;
                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;
                //for (int i = 0; i < vcount; ++i)
                //{
                //    double x, y;
                //    PixelFarm.Agg.VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                //    switch (cmd)
                //    {
                //        case PixelFarm.Agg.VertexCmd.MoveTo:
                //            prevMoveToX = prevX = x;
                //            prevMoveToY = prevY = y;
                //            break;
                //        case PixelFarm.Agg.VertexCmd.LineTo:
                //            g.DrawLine(Pens.Red, (float)prevX, (float)prevY, (float)x, (float)y);
                //            prevX = x;
                //            prevY = y;
                //            break;
                //        case PixelFarm.Agg.VertexCmd.EndAndCloseFigure:
                //            g.DrawLine(Pens.Red, (float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                //            prevMoveToX = prevX = x;
                //            prevMoveToY = prevY = y;
                //            break;
                //        case PixelFarm.Agg.VertexCmd.EndFigure:
                //            break;
                //        case PixelFarm.Agg.VertexCmd.HasMore:
                //            break;
                //        case PixelFarm.Agg.VertexCmd.Stop:
                //            i = vcount + 1;//exit from loop
                //            break;
                //        default:
                //            break;
                //    }
                //}
                var brush_path = new GraphicsPath(FillMode.Winding);//*** winding for overlapped path
                for (int i = 0; i < vcount; ++i)
                {
                    double x, y;
                    PixelFarm.Agg.VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                    switch (cmd)
                    {
                        case PixelFarm.Agg.VertexCmd.MoveTo:
                            prevMoveToX = prevX = x;
                            prevMoveToY = prevY = y;
                            break;
                        case PixelFarm.Agg.VertexCmd.LineTo:
                            brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                            prevX = x;
                            prevY = y;
                            break;
                        case PixelFarm.Agg.VertexCmd.CloseAndEndFigure:
                            brush_path.AddLine((float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                            prevMoveToX = prevX = x;
                            prevMoveToY = prevY = y;
                            break;
                        case PixelFarm.Agg.VertexCmd.EndFigure:
                            break;
                        case PixelFarm.Agg.VertexCmd.HasMore:
                            break;
                        case PixelFarm.Agg.VertexCmd.Stop:
                            i = vcount + 1;//exit from loop
                            break;
                        default:
                            break;
                    }
                }

                _latestBrushPathCache = brush_path;
                g.FillPath(Brushes.Red, _latestBrushPathCache);
            }
            else
            {
                var contPoints = _myBrushPath.contPoints;
                int pcount = contPoints.Count;
                for (int i = 1; i < pcount; ++i)
                {
                    var p0 = contPoints[i - 1];
                    var p1 = contPoints[i];
                    g.DrawLine(Pens.Red, (float)p0.x, (float)p0.y, (float)p1.x, (float)p1.y);
                }
            }
        }

        protected override void OnMouseDown(int x, int y)
        {
            _latestBrushPathCache = null;
            _latestMousePoint = new PixelFarm.Agg.Image.Point(x, y);
            _points.Clear();
            _myBrushPath = new PixelFarm.Agg.Samples.MyBrushPath();
            _points.Add(new System.Drawing.Point(x, y));
            _myBrushPath.AddPointAtFirst(x, y);
        }
        protected override void OnMouseMove(int x, int y)
        {
            _points.Add(new System.Drawing.Point(x, y));
            //dragging
            //---------
            //find diff 

            Vector newPoint = new Vector(x, y);
            //find distance
            Vector oldPoint = new Vector(_latestMousePoint.x, _latestMousePoint.y);
            Vector delta = (newPoint - oldPoint) / 2; // 2,4 etc 
                                                      //midpoint
            Vector midPoint = (newPoint + oldPoint) / 2;
            delta = delta.NewLength(5);
            delta.Rotate(90);
            Vector newTopPoint = midPoint + delta;
            Vector newBottomPoint = midPoint - delta;
            //bottom point
            _myBrushPath.AddPointAtFirst((int)newBottomPoint.X, (int)newBottomPoint.Y);
            _myBrushPath.AddPointAtLast((int)newTopPoint.X, (int)newTopPoint.Y);
            _latestMousePoint = new PixelFarm.Agg.Image.Point(x, y);
        }
        protected override void OnMouseUp(int x, int y)
        {
            _points.Add(new System.Drawing.Point(x, y));
            _myBrushPath.AddPointAtLast(x, y);
            _myBrushPath.MakeSmoothPath();
        }
    }
}
