//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using Mini;

namespace PixelFarm
{

    public enum Poly2TriDemoExample
    {
        SimpleRect,
        SimpleGrid,
        Vxs,
    }


    [Info(OrderCode = "09")]
    [Info(DemoCategory.Vector)]
    public class Poly2TriDemo : DemoBase
    {
        [DemoConfig]
        public Poly2TriDemoExample DemoExample { get; set; }

        public override void Draw(Painter p)
        {
            p.Clear(Color.White);

            switch (DemoExample)
            {
                default:
                case Poly2TriDemoExample.SimpleRect:
                    DrawSimpleRectExample(p);
                    break;
                case Poly2TriDemoExample.SimpleGrid:
                    DrawSimpleGridExample(p);
                    break;
                case Poly2TriDemoExample.Vxs:
                    DrawVxsArrowExample(p);
                    break;
            }
        }
        static VertexStore BuildArrow(bool solidHead)
        {
            VertexStore arrow;
            VertexStore stem;
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VxsTemp.Borrow(out var v1))
            using (VxsTemp.Borrow(out var v3, out var v4))
            {
                if (solidHead)
                {
                    v1.AddMoveTo(5, 20);
                    v1.AddLineTo(10, 10);
                    v1.AddLineTo(15, 20);
                    v1.AddCloseFigure();
                    arrow = v1.CreateTrim();

                    BuildLine(10, 20, 10, 50, v3);
                    stem = v3.CreateTrim();
                }
                else
                {
                    v1.AddMoveTo(5, 20);
                    v1.AddLineTo(10, 10);
                    v1.AddLineTo(15, 20);
                    stroke.LineJoin = LineJoin.Round;
                    stroke.LineCap = LineCap.Round;
                    stroke.Width = 3;
                    arrow = stroke.CreateTrim(v1);

                    BuildLine(10, 10, 10, 50, v3);
                    stem = v3.CreateTrim();
                }

                arrow = VxsClipper.CombinePaths(arrow, stem, VxsClipperType.Union, false, null);
                return arrow.ScaleToNewVxs(3, v4).CreateTrim();
            }
        }
        static void BuildLine(float x0, float y0, float x1, float y1, VertexStore output)
        {
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(out Stroke stroke))
            {
                stroke.Width = 3;
                v1.AddMoveTo(x0, y0);
                v1.AddLineTo(x1, y1);
                stroke.MakeVxs(v1, output);
            }
        }
        void DrawSimpleRectExample(Painter painter)
        {

            Poly2Tri.TriangulationPoint[] points = new Poly2Tri.TriangulationPoint[]
            {
                  new Poly2Tri.TriangulationPoint(10,10),
                  new Poly2Tri.TriangulationPoint(40,10),
                  new Poly2Tri.TriangulationPoint(40,40),
                  new Poly2Tri.TriangulationPoint(10,40)
            };

            Poly2Tri.Polygon polygon = new Poly2Tri.Polygon(points);
            Poly2Tri.P2T.Triangulate(polygon);


            painter.StrokeColor = Color.Black;

            DrawPoly2TriPolygon(painter, new List<Poly2Tri.Polygon>() { polygon });
        }
        void DrawSimpleGridExample(Painter painter)
        {

           
        }

        void DrawVxsArrowExample(Painter painter)
        {
            VertexStore vxs = BuildArrow(true);
            using (Poly2TriTool.Borrow(out var p23tool))
            {
                p23tool.YAxisPointDown = true; //since our vxs is create from Y axis point down world

                List<Poly2Tri.Polygon> polygons = new List<Poly2Tri.Polygon>();
                p23tool.Triangulate(vxs, polygons);

                painter.StrokeColor = Color.Black;
                DrawPoly2TriPolygon(painter, polygons);
            }
        }

        static void DrawPoly2TriPolygon(Painter painter, List<Poly2Tri.Polygon> polygons)
        {

            foreach (Poly2Tri.Polygon polygon in polygons)
            {
                foreach (Poly2Tri.DelaunayTriangle tri in polygon.Triangles)
                {
                    Poly2Tri.TriangulationPoint p0 = tri.P0;
                    Poly2Tri.TriangulationPoint p1 = tri.P1;
                    Poly2Tri.TriangulationPoint p2 = tri.P2;

                    painter.DrawLine(p0.X, p0.Y, p1.X, p1.Y);
                    painter.DrawLine(p1.X, p1.Y, p2.X, p2.Y);
                    painter.DrawLine(p2.X, p2.Y, p0.X, p0.Y);
                }
            }
        }
    }
}