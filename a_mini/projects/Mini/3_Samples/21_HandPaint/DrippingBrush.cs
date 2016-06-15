//BSD 2014,2015 WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;
using PixelFarm.Agg.Image;
using Mini;
namespace PixelFarm.Agg.Samples
{
    [Info(OrderCode = "22")]
    [Info("DrippingBrush")]
    public class DrippingBrushExample : DemoBase
    {
        Point latestMousePoint;
        List<MyBrushPath> myBrushPathList = new List<MyBrushPath>();
        CanvasPainter p;
        MyBrushPath currentBrushPath;
        public override void Init()
        {
        }
        public override void Draw(CanvasPainter p)
        {
            p.Clear(ColorRGBA.White);
            p.FillColor = ColorRGBA.Black;
            foreach (var brushPath in this.myBrushPathList)
            {
                if (brushPath.Vxs != null)
                {
                    p.Fill(brushPath.Vxs);
                }
                else
                {
                    var contPoints = brushPath.contPoints;
                    int pcount = contPoints.Count;
                    for (int i = 1; i < pcount; ++i)
                    {
                        var p0 = contPoints[i - 1];
                        var p1 = contPoints[i];
                        p.Line(p0.x, p0.y, p1.x, p1.y);
                    }
                }
            }
        }

        public override void MouseUp(int x, int y)
        {
            if (currentBrushPath != null)
            {
                currentBrushPath.Close();
                currentBrushPath = null;
            }
            base.MouseUp(x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            //find diff 
            Vector newPoint = new Vector(x, y);
            //find distance
            Vector oldPoint = new Vector(latestMousePoint.x, latestMousePoint.y);
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
            currentBrushPath.AddPointAtFirst((int)newBottomPoint.X, (int)newBottomPoint.Y);
            currentBrushPath.AddPointAtLast((int)newTopPoint.X, (int)newTopPoint.Y);
            latestMousePoint = new Point(x, y);
        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            latestMousePoint = new Point(x, y);
            currentBrushPath = new MyBrushPath();
            this.myBrushPathList.Add(currentBrushPath);
            currentBrushPath.AddPointAtFirst(x, y);
            base.MouseDown(x, y, isRightButton);
        }
    }
}

