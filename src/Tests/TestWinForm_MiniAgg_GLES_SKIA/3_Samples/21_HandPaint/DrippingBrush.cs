//BSD, 2014-present, WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;

using Mini;
namespace PixelFarm.CpuBlit.Samples
{
    [Info(OrderCode = "22")]
    [Info("DrippingBrush")]
    public class DrippingBrushExample : DemoBase
    {
        Point _latestMousePoint;
        List<MyBrushPath> _myBrushPathList = new List<MyBrushPath>();
        
        MyBrushPath _currentBrushPath;
        public override void Init()
        {
        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.FillColor = Drawing.Color.Black;
            p.DrawRect(10, 10, 30, 30);

            foreach (var brushPath in _myBrushPathList)
            {
                brushPath.PaintLatest(p);

                //if (brushPath.Vxs != null)
                //{
                //    p.Fill(brushPath.Vxs);
                //}
                //else
                //{
                //    var contPoints = brushPath.contPoints;
                //    int pcount = contPoints.Count;
                //    for (int i = 1; i < pcount; ++i)
                //    {
                //        var p0 = contPoints[i - 1];
                //        var p1 = contPoints[i];
                //        p.DrawLine(p0.x, p0.y, p1.x, p1.y);
                //    }
                //}
            }
        }

        public override void MouseUp(int x, int y)
        {
            if (_currentBrushPath != null)
            {
                _currentBrushPath.Close();
                _currentBrushPath = null;
            }
            base.MouseUp(x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            //find diff 
            Vector newPoint = new Vector(x, y);
            //find distance
            Vector oldPoint = new Vector(_latestMousePoint.x, _latestMousePoint.y);
            var delta = (newPoint - oldPoint) / 2; // 2,4 etc 
            //midpoint
            var midPoint = (newPoint + oldPoint) / 2;
            //find angle
            var topPoint = delta;//create top point
            var bottomPoint = delta; //bottom point            
            topPoint.Rotate(90);
            bottomPoint.Rotate(-90);
            var newTopPoint = midPoint + topPoint;
            var newBottomPoint = midPoint + bottomPoint;
            //bottom point
            _currentBrushPath.AddPointAtFirst((int)newBottomPoint.X, (int)newBottomPoint.Y);
            _currentBrushPath.AddPointAtLast((int)newTopPoint.X, (int)newTopPoint.Y);
            _latestMousePoint = new Point(x, y);
        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            _latestMousePoint = new Point(x, y);
            _currentBrushPath = new MyBrushPath();
            _myBrushPathList.Add(_currentBrushPath);
            _currentBrushPath.AddPointAtFirst(x, y);
            base.MouseDown(x, y, isRightButton);
        }
    }
}

