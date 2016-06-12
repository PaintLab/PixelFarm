using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using System.Text;
using burningmime.curves; //for curve fit
using ClipperLib;

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
        public abstract bool IsDrawingTool { get; }

        internal virtual void SetPreviousPixelControllerObjects(List<PixelToolController> prevPixTools) { }
        internal abstract VertexStore GetVxs();
        internal abstract void SetVxs(VertexStore vxs);
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
        public override bool IsDrawingTool { get { return true; } }
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
                            brush_path.StartFigure();
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
                            brush_path.CloseFigure();
                            break;
                        case PixelFarm.Agg.VertexCmd.EndFigure:
                            break;
                        case PixelFarm.Agg.VertexCmd.HasMore:
                            break;
                        case PixelFarm.Agg.VertexCmd.Stop:
                            i = vcount + 1;//exit from loop
                            break;

                        default:
                            throw new NotSupportedException();
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

        internal override VertexStore GetVxs()
        {
            if (_myBrushPath != null)
            {
                return _myBrushPath.vxs;
            }
            else
            {
                return null;
            }
        }
        internal override void SetVxs(VertexStore vxs)
        {
            if (_myBrushPath != null)
            {
                //replace
                _myBrushPath.vxs = vxs;
            }
            else
            {
                _myBrushPath = new PixelFarm.Agg.Samples.MyBrushPath();
                _myBrushPath.vxs = vxs;
            }
            _latestBrushPathCache = null;
        }


    }


    class MyCuttingBrushController : MyDrawingBrushController
    {
        List<PixelToolController> prevPixTools;

        public override bool IsDrawingTool { get { return false; } }
        internal override void SetPreviousPixelControllerObjects(List<PixelToolController> prevPixTools)
        {
            this.prevPixTools = prevPixTools;
        }
        internal override VertexStore GetVxs()
        {
            return base.GetVxs();
        }
        protected override void OnMouseUp(int x, int y)
        {
            base.OnMouseUp(x, y);

            if (prevPixTools.Count > 0)
            {

                int prevPixToolCount = prevPixTools.Count;
                for (int n = prevPixToolCount - 1; n >= 0; --n)
                {
                    PixelToolController prevPixTool = prevPixTools[n];
                    //do path clip***

                    List<VertexStore> resultList = CombinePaths(
                         new VertexStoreSnap(prevPixTool.GetVxs()),
                         new VertexStoreSnap(this.GetVxs()),
                         ClipType.ctDifference,
                         true);

                    int count;
                    switch (count = resultList.Count)
                    {
                        case 1:
                            {   //replace the last one with newBrushPath   
                                prevPixTool.SetVxs(resultList[0]);
                            }
                            break;
                        case 0:
                            break;
                        default:
                            {
                                //we will replace all with new set***
                                prevPixTools.RemoveAt(n);
                                for (int i = 0; i < count; ++i)
                                {
                                    var subBrush = new MyDrawingBrushController();
                                    subBrush.SetVxs(resultList[i]);
                                    prevPixTools.Insert(n, subBrush);
                                }
                            }
                            break;
                    }
                } 
            }
        }
        //for clipping ...

        static List<VertexStore> CombinePaths(VertexStoreSnap a, VertexStoreSnap b, ClipType clipType, bool separateIntoSmallSubPaths)
        {

            List<List<IntPoint>> aPolys = CreatePolygons(a);
            List<List<IntPoint>> bPolys = CreatePolygons(b);
            Clipper clipper = new Clipper();
            clipper.AddPaths(aPolys, PolyType.ptSubject, true);
            clipper.AddPaths(bPolys, PolyType.ptClip, true);
            List<List<IntPoint>> intersectedPolys = new List<List<IntPoint>>();
            clipper.Execute(clipType, intersectedPolys);

            List<VertexStore> resultList = new List<VertexStore>();
            PathWriter output = new PathWriter();

            if (separateIntoSmallSubPaths)
            {
                foreach (List<IntPoint> polygon in intersectedPolys)
                {
                    int j = polygon.Count;
                    if (j > 0)
                    {
                        //first one
                        IntPoint point = polygon[0];
                        output.MoveTo(point.X / 1000.0, point.Y / 1000.0);
                        //next others ...
                        if (j > 1)
                        {
                            for (int i = 1; i < j; ++i)
                            {
                                point = polygon[i];
                                output.LineTo(point.X / 1000.0, point.Y / 1000.0);
                            }
                        }

                        output.CloseFigure();
                        resultList.Add(output.Vxs);
                        //---
                        //clear 
                        output.ClearAndStartNewVxs();
                    }
                }
            }
            else
            {
                foreach (List<IntPoint> polygon in intersectedPolys)
                {
                    int j = polygon.Count;
                    if (j > 0)
                    {
                        //first one
                        IntPoint point = polygon[0];
                        output.MoveTo(point.X / 1000.0, point.Y / 1000.0);
                        //next others ...
                        if (j > 1)
                        {
                            for (int i = 1; i < j; ++i)
                            {
                                point = polygon[i];
                                output.LineTo(point.X / 1000.0, point.Y / 1000.0);
                            }
                        }
                        output.CloseFigure();
                    }

                    //bool first = true;
                    //foreach (IntPoint point in polygon)
                    //{
                    //    if (first)
                    //    {
                    //        output.AddVertex(point.X / 1000.0, point.Y / 1000.0, ShapePath.FlagsAndCommand.CommandMoveTo);
                    //        first = false;
                    //    }
                    //    else
                    //    {
                    //        output.AddVertex(point.X / 1000.0, point.Y / 1000.0, ShapePath.FlagsAndCommand.CommandLineTo);
                    //    }
                    //} 
                }

                output.Stop();
                resultList.Add(output.Vxs);
            }

            return resultList;
        }
        static List<List<IntPoint>> CreatePolygons(VertexStoreSnap a)
        {
            List<List<IntPoint>> allPolys = new List<List<IntPoint>>();
            List<IntPoint> currentPoly = null;
            VertexData last = new VertexData();
            VertexData first = new VertexData();
            bool addedFirst = false;
            var snapIter = a.GetVertexSnapIter();
            double x, y;
            VertexCmd cmd = snapIter.GetNextVertex(out x, out y);
            do
            {
                if (cmd == VertexCmd.LineTo)
                {
                    if (!addedFirst)
                    {
                        currentPoly.Add(new IntPoint((long)(last.x * 1000), (long)(last.y * 1000)));
                        addedFirst = true;
                        first = last;
                    }
                    currentPoly.Add(new IntPoint((long)(x * 1000), (long)(y * 1000)));
                    last = new VertexData(cmd, x, y);
                }
                else
                {
                    addedFirst = false;
                    currentPoly = new List<IntPoint>();
                    allPolys.Add(currentPoly);
                    if (cmd == VertexCmd.MoveTo)
                    {
                        last = new VertexData(cmd, x, y);
                    }
                    else
                    {
                        last = first;
                    }
                }
                cmd = snapIter.GetNextVertex(out x, out y);
            } while (cmd != VertexCmd.Stop);
            return allPolys;
        }
    }


    abstract class PixelToolControllerFactory
    {
        public abstract PixelToolController CreateNewTool();

    }
    class PixelToolControllerFactory<T> : PixelToolControllerFactory
        where T : PixelToolController, new()
    {
        public override PixelToolController CreateNewTool()
        {
            return new T();
        }
        public PixelToolControllerFactory(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

