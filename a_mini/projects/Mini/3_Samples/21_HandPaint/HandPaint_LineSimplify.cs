//BSD 2015, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.Agg.Image;
using PixelFarm.VectorMath;
using Mini;
using burningmime.curves; //for curve fit
namespace PixelFarm.Agg.Samples
{
    [Info(OrderCode = "21")]
    [Info("hand paint with line simplification")]
    public class HandPaintWithLineSimplifyExample : DemoBase
    {
        Point latestMousePoint;
        List<List<Point>> pointSets = new List<List<Point>>();
        List<List<Point>> simplifiedPointSets = new List<List<Point>>();
        CanvasPainter p;
        List<Point> currentPointSet;// = new List<Point>();//current point list
        List<CubicBezier> cubicCurves = new List<CubicBezier>();
        public override void Init()
        {
            List<Point> testPoints = new List<Point>();
            //new VECTOR[]{
            //      new VECTOR(50,300),
            //      new VECTOR(52,50),
            //      new VECTOR(100,45),
            //      new VECTOR(150,150),
            //  }

            testPoints.AddRange(
             new Point[] { new Point(305, 397), new Point(305, 394),
                 new Point(302, 387), new Point(301, 364),
                 new Point(283, 335), new Point(270, 296),
                 new Point(268, 215), new Point(206, 174),
                 new Point(201, 163), new Point(200, 157),
                 new Point(198, 154), new Point(198, 154),
                 new Point(197, 154), new Point(195, 157),
                 new Point(194, 163), new Point(190, 175),
                 new Point(166, 252), new Point(225, 319),
                 new Point(253, 354), new Point(274, 383),
                 new Point(296, 392), new Point(301, 396),
                 new Point(304, 398) }
             );
            //new VECTOR(0,0),
            // new VECTOR(50,50),
            // new VECTOR(90,100),
            // new VECTOR(150,150)

            pointSets.Add(testPoints);
            currentPointSet = testPoints;
            CreateFitCurves();
        }
        public override void Draw(CanvasPainter p)
        {
            p.Clear(ColorRGBA.White);
            var plistCount = pointSets.Count;
            p.StrokeColor = ColorRGBA.Black;
            for (int n = 0; n < plistCount; ++n)
            {
                var contPoints = pointSets[n];
                DrawLineSet(p, contPoints);
            }

            plistCount = simplifiedPointSets.Count;
            p.StrokeColor = ColorRGBA.Red;
            for (int n = 0; n < plistCount; ++n)
            {
                var contPoints = simplifiedPointSets[n];
                DrawLineSet(p, contPoints);
            }

            //p.StrokeColor = ColorRGBA.Blue;
            //p.FillColor = ColorRGBA.Black;
            //int ccount = cubicCurves.Count;
            //for (int i = 0; i < ccount; ++i)
            //{
            //    var cc = cubicCurves[i];
            //    FillPoint(cc.p0, p);
            //    FillPoint(cc.p1, p);
            //    FillPoint(cc.p2, p);
            //    FillPoint(cc.p3, p);
            //    p.DrawBezierCurve(
            //       (float)cc.p0.x, (float)cc.p0.y,
            //       (float)cc.p3.x, (float)cc.p3.y,
            //       (float)cc.p1.x, (float)cc.p1.y,
            //       (float)cc.p2.x, (float)cc.p2.y);
            //}
        }

        static void FillPoint(Vector2 v, CanvasPainter p)
        {
            p.FillRectangle(
                  v.x, v.y,
                  v.x + 3, v.y + 3);
        }
        static void DrawLineSet(CanvasPainter p, List<Point> contPoints)
        {
            int pcount = contPoints.Count;
            for (int i = 1; i < pcount; ++i)
            {
                var p0 = contPoints[i - 1];
                var p1 = contPoints[i];
                p.Line(p0.x, p0.y, p1.x, p1.y);
            }
        }
        public override void MouseDrag(int x, int y)
        {
            //add data to draw             
            currentPointSet.Add(new Point(x, y));
        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            currentPointSet = new List<Point>();
            this.pointSets.Add(currentPointSet);
            latestMousePoint = new Point(x, y);
            base.MouseDown(x, y, isRightButton);
        }
        public override void MouseUp(int x, int y)
        {
            //finish the current set
            //create a simplified point set
            //var newSimplfiedSet = LineSimplifiedUtility.DouglasPeuckerReduction(currentPointSet, 15); 
            //this.simplifiedPointSets.Add(newSimplfiedSet);

            CreateFitCurves();
            base.MouseUp(x, y);
        }
#if DEBUG
        static string dbugDumpPointsToString(List<Point> points)
        {
            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            int j = points.Count;
            stbuilder.Append("new VECTOR[]{");
            for (int i = 0; i < j; ++i)
            {
                Point pp = points[i];
                stbuilder.Append("new VECTOR(" + pp.x.ToString() + "," + pp.y.ToString() + ")");
                if (i < j - 1)
                {
                    stbuilder.Append(',');
                }
            }
            stbuilder.Append("}");
            return stbuilder.ToString();
        }
        static string dbugDumpPointsToString2(List<Point> points)
        {
            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            int j = points.Count;
            stbuilder.Append("new Point[]{");
            for (int i = 0; i < j; ++i)
            {
                Point pp = points[i];
                stbuilder.Append("new Point(" + pp.x.ToString() + "," + pp.y.ToString() + ")");
                if (i < j - 1)
                {
                    stbuilder.Append(',');
                }
            }
            stbuilder.Append("}");
            return stbuilder.ToString();
        }
#endif
        void CreateFitCurves()
        {
            //---------------------------------------
            //convert point to vector  
            int j = currentPointSet.Count;
            List<Vector2> data = new List<Vector2>(j);
            for (int i = 0; i < j; ++i)
            {
                Point pp = currentPointSet[i];
                data.Add(new Vector2(pp.x, pp.y));
            }

            //CurvePreprocess.Linearize(data, 8);
            //List<Vector2> reduced = CurvePreprocess.RdpReduce(data, 2);

            //string code = DumpPointsToString();
            //string code2 = DumpPointsToString2(); 
            //var data2 = data;
            var data2 = CurvePreprocess.RdpReduce(data, 2);
            j = data2.Count;
            List<Point> simplifiedPoints = new List<Point>();
            this.simplifiedPointSets.Add(simplifiedPoints);
            for (int i = 0; i < j; ++i)
            {
                var pp = data2[i];
                simplifiedPoints.Add(new Point((int)pp.x, (int)pp.y));
            }


            CubicBezier[] cubicBzs = CurveFit.Fit(data2, 8);
            cubicCurves.AddRange(cubicBzs);
        }
    }
}

