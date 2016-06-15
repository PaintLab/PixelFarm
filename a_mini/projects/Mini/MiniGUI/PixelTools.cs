using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
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
        public virtual void Offset(int dx, int dy) { }
        internal virtual void SetPreviousPixelControllerObjects(List<PixelToolController> prevPixTools) { }
        internal abstract VertexStore GetVxs();
        internal abstract void SetVxs(VertexStore vxs);
        public virtual bool HitTest(int x, int y) { return false; }
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
                ColorRGBA brushColor = _myBrushPath.FillColor;
                using (SolidBrush br = new SolidBrush(Color.FromArgb(brushColor.alpha, brushColor.red, brushColor.green, brushColor.blue)))
                {
                    g.FillPath(br, _latestBrushPathCache);
                }
                return;
            }

            if (_myBrushPath.Vxs != null)
            {
                //create new path  
                _latestBrushPathCache = PixelFarm.Drawing.WinGdi.VxsHelper.CreateGraphicsPath(_myBrushPath.Vxs);
                ColorRGBA brushColor = _myBrushPath.FillColor;
                using (SolidBrush br = new SolidBrush(Color.FromArgb(brushColor.alpha, brushColor.red, brushColor.green, brushColor.blue)))
                {
                    g.FillPath(br, _latestBrushPathCache);
                }
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
        public override bool HitTest(int x, int y)
        {
            return _myBrushPath.HitTest(x, y);
        }
        protected override void OnMouseDown(int x, int y)
        {
            _latestBrushPathCache = null;
            _latestMousePoint = new PixelFarm.Agg.Image.Point(x, y);
            _points.Clear();
            _myBrushPath = new PixelFarm.Agg.Samples.MyBrushPath();
            _myBrushPath.FillColor = ColorRGBA.Red;
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
                return _myBrushPath.Vxs;
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
                _myBrushPath.SetVxs(vxs);
            }
            else
            {
                _myBrushPath = new PixelFarm.Agg.Samples.MyBrushPath();
                _myBrushPath.SetVxs(vxs);
            }
            _latestBrushPathCache = null;
        }
        public Color PathFillColor
        {
            get
            {
                ColorRGBA color = _myBrushPath.FillColor;
                return Color.FromArgb(color.alpha, color.red, color.green, color.blue);
            }
            set
            {
                _myBrushPath.FillColor = new ColorRGBA(
                    value.R,
                    value.G,
                    value.B,
                    value.A
                    );
            }
        }
        public override void Offset(int dx, int dy)
        {
            for (int i = _points.Count - 1; i >= 0; --i)
            {
                System.Drawing.Point p = _points[i];
                p.Offset(dx, dy);
                _points[i] = p;
            }
            _myBrushPath.MoveBy(dx, dy);
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
                    List<VertexStore> resultList = PixelFarm.Agg.VertexSource.VxsClipper.CombinePaths(
                         new VertexStoreSnap(prevPixTool.GetVxs()),
                         new VertexStoreSnap(this.GetVxs()),
                         PixelFarm.Agg.VertexSource.VxsClipperType.Difference,
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
                                var brushPath = prevPixTool as MyDrawingBrushController;
                                if (brushPath != null)
                                {
                                    Color fillColor = brushPath.PathFillColor;
                                    prevPixTools.RemoveAt(n);
                                    for (int i = 0; i < count; ++i)
                                    {
                                        var subBrush = new MyDrawingBrushController();
                                        subBrush.SetVxs(resultList[i]);
                                        subBrush.PathFillColor = fillColor;
                                        prevPixTools.Insert(n, subBrush);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    class MyShapePickupTool : PixelToolController
    {
        List<PixelToolController> prevPixTools;
        PixelToolController _lastestSelectedController;
        Color _latestSelectControllerFillColor;
        int _latest_mouseX, _latest_mouseY;
        public override bool IsDrawingTool { get { return false; } }
        public override void Draw(Graphics g)
        {
        }
        internal override void SetPreviousPixelControllerObjects(List<PixelToolController> prevPixTools)
        {
            this.prevPixTools = prevPixTools;
        }
        protected override void OnMouseUp(int x, int y)
        {
            base.OnMouseUp(x, y);
        }
        protected override void OnMouseMove(int x, int y)
        {
            if (this.IsMouseDown)
            {
                //drag ...
                if (_lastestSelectedController != null)
                {
                    _lastestSelectedController.Offset(x - _latest_mouseX, y - _latest_mouseY);
                }
                _latest_mouseX = x;
                _latest_mouseY = y;
            }
            base.OnMouseMove(x, y);
        }
        protected override void OnMouseDown(int x, int y)
        {
            _latest_mouseX = x;
            _latest_mouseY = y;
            if (_lastestSelectedController != null)
            {
                if (_lastestSelectedController is MyDrawingBrushController)
                {
                    ((MyDrawingBrushController)_lastestSelectedController).PathFillColor = _latestSelectControllerFillColor;
                }
            }

            int j = this.prevPixTools.Count;
            PixelToolController selectedShape = null;
            for (int i = this.prevPixTools.Count - 1; i >= 0; --i)
            {
                PixelToolController p = prevPixTools[i];
                if (p.HitTest(x, y))
                {
                    //found 
                    //then check fill color
                    _lastestSelectedController = p;
                    if (_lastestSelectedController is MyDrawingBrushController)
                    {
                        var b = (MyDrawingBrushController)_lastestSelectedController;
                        _latestSelectControllerFillColor = b.PathFillColor;
                        b.PathFillColor = Color.Blue;
                    }

                    selectedShape = p;
                    break;
                }
            }

            base.OnMouseDown(x, y);
        }
        internal override VertexStore GetVxs()
        {
            return null;
        }
        internal override void SetVxs(VertexStore vxs)
        {
        }
    }


    abstract class PixelToolControllerFactory
    {
        public abstract PixelToolController CreateNewTool();
    }
    class PixelToolControllerFactory<T> : PixelToolControllerFactory
        where T : PixelToolController, new()
    {
        T _lastestControl;
        public override PixelToolController CreateNewTool()
        {
            if (CreateOnce)
            {
                if (_lastestControl == default(T))
                {
                    _lastestControl = new T();
                }
                return _lastestControl;
            }
            else
            {
                return new T();
            }
        }
        public PixelToolControllerFactory(string name)
        {
            this.Name = name;
        }
        public bool CreateOnce { get; set; }
        public string Name { get; private set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

