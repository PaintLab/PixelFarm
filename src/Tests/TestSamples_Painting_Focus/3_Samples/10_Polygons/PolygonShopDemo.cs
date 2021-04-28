//MIT, 2019-present, WinterDev
using System;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using Mini;

namespace PixelFarm
{



    [Info(OrderCode = "09")]
    [Info(DemoCategory.Vector)]
    public class PolygonShopDemo : DemoBase
    {
        VertexStore _arrowSolidHead;
        VertexStore _arrowLineHead;
        VertexStore _roundRectSolid;
        VertexStore _roundRectOutline;
        VertexStore _roundCornerPolygon;
        VertexStore _roundCornerPolygon2;

        VertexStore _outsidePartOfLines;
        VertexStore _insidePartOfLines;
        VertexStore _generalLines; //general line stroke


        VertexStore _catmullRomSpline1;
        VertexStore _cardinalSpline2;
        //--------------


        public enum PolygonKind
        {
            //
            ArrowSolidHead,
            ArrowLineHead,
            //

            RoundCornerRect, RoundCornerRect_Outline,
            RoundCornerPolygon, RoundCornerPolygon2,


            CatmullRom1,
            CardinalSpline2,

            GeneralLines,
            InsidePartOfLines,
            OutsidePartOfLines,

            CatRom2,
            Hermite1,
            UbSpline1,
        }
        public PolygonShopDemo()
        {
            _arrowSolidHead = BuildArrow(true);
            _arrowLineHead = BuildArrow(false);
            _roundRectSolid = BuildRoundedRect(false);
            _roundRectOutline = BuildRoundedRect(true);
            //
            _roundCornerPolygon = BuildRoundCornerPolygon();
            _roundCornerPolygon2 = BuildRoundCornerPolygon2();

            _insidePartOfLines = BuildInsidePartOfLines();
            _outsidePartOfLines = BuildOutsidePartOfLines();
            _generalLines = BuildGeneralLines();

            _catmullRomSpline1 = BuildCatmullRomSpline1();
            _cardinalSpline2 = BuildCardinalSpline();
        }

        public static VertexStore BuildCardinalSpline()
        {
            using (Tools.BorrowVxs(out var v1, out var v3))
            using (Tools.BorrowCurveFlattener(out var flatten))
            using (Tools.BorrowPathWriter(v1, out var w))
            {
                w.MoveTo(10, 10);
                w.DrawCurve(new float[]
                {
                    10, 10,
                    100,50,
                    50,200,
                    120,250,
                    30,240,
                    10,100,
                    10,10
                }, 0.25f);

                //w.CatmullRomSegmentToCurve4(
                //       10, 10,
                //       25, 10,//p1
                //       25, 25,//p2
                //       10, 25);
                //w.CatmullRomSegmentToCurve4(
                //      25, 10,
                //      25, 25,//p1
                //      10, 25,//p2
                //      10, 10);

                w.CloseFigure();
                //v1.ScaleToNewVxs(3, v2);

                return flatten.MakeVxs(v1, v3).CreateTrim();
            }
        }

        public static VertexStore BuildCatmullRomSpline1()
        {
            double[] xyCoords = new double[]
            {
                10,100,
                40,50,
                70,100,
                100,50,
                130,100,
                160,50,
                190,100,
                200,50,
                230,100
            };


            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowPathWriter(v1, out var pw))
            using (Tools.BorrowCurveFlattener(out var flatten))
            {

                pw.MoveTo(xyCoords[2], xyCoords[3]);//***
                for (int i = 0; i < xyCoords.Length - (4 * 2);)
                {
                    pw.CatmullRomSegmentToCurve4(
                        xyCoords[i], xyCoords[i + 1],
                        xyCoords[i + 2], xyCoords[i + 3],
                        xyCoords[i + 4], xyCoords[i + 5],
                        xyCoords[i + 6], xyCoords[i + 7]
                        );
                    i += 2;
                }
                pw.CloseFigure();
                return flatten.MakeVxs(v1, v2).CreateTrim();
            }
        }
        public static VertexStore BuildRoundCornerPolygon2()
        {

            using (Tools.BorrowVxs(out var v1, out var v2, out var v3))
            using (Tools.BorrowStroke(out var stroke))
            using (Tools.BorrowCurveFlattener(out var flattener))
            using (Tools.BorrowArc(out var arc))
            {
                arc.Init(50, 50, 10, 20, Math.PI, 0);
                arc.SetStartEndLimit(40, 50, 60, 50);

               
                arc.MakeVxs(v1);
                //--------------------------
                v1.ScaleToNewVxs(3, v2);
                flattener.MakeVxs(v2, v3);
                return v3.CreateTrim();
            }
        }
        public static VertexStore BuildRoundCornerPolygon()
        {
            using (Tools.BorrowShapeBuilder(out var b))
            using (Tools.BorrowStroke(out var stroke))
            {

                b.MoveTo(5, 20);
                b.LineTo(10, 10);
                b.LineTo(15, 20);
                b.CloseFigure();

                stroke.StrokeSideForClosedShape = StrokeSideForClosedShape.Outside;
                stroke.Width = 5;
                stroke.LineJoin = LineJoin.Round;

                b.Scale(3);
                b.Stroke(stroke);
                return b.CreateTrim();
            }
        }
        public static VertexStore BuildGeneralLines()
        {
            //use this example with BuildOutsidePartOfLines() and  BuildInsidePartOfLines()
            using (Tools.BorrowShapeBuilder(out var b))
            using (Tools.BorrowStroke(out var stroke))
            {

                stroke.Width = 5;
                stroke.LineJoin = LineJoin.Round;

                b.MoveTo(5, 20);
                b.LineTo(10, 10);
                b.LineTo(15, 20);
                b.Scale(3);
                b.Stroke(stroke);

                return b.CreateTrim();
            }
        }
        public static VertexStore BuildOutsidePartOfLines()
        {
            using (Tools.BorrowShapeBuilder(out var b))
            using (Tools.BorrowStroke(out var stroke))
            {
                stroke.StrokeSideForOpenShape = StrokeSideForOpenShape.Outside;
                stroke.Width = 5;
                stroke.LineJoin = LineJoin.Round;

                b.MoveTo(5, 20);
                b.LineTo(10, 10);
                b.LineTo(15, 20);
                b.Scale(3);
                b.Stroke(stroke);
                return b.CreateTrim();
            }
        }
        public static VertexStore BuildInsidePartOfLines()
        {
            using (Tools.BorrowShapeBuilder(out var b))
            using (Tools.BorrowStroke(out Stroke stroke))
            {
                stroke.StrokeSideForOpenShape = StrokeSideForOpenShape.Inside;
                stroke.Width = 5;
                stroke.LineJoin = LineJoin.Round;

                b.MoveTo(5, 20);
                b.LineTo(10, 10);
                b.LineTo(15, 20);
                b.Scale(3);
                b.Stroke(stroke);

                return b.CreateTrim();
            }
        }
        public static VertexStore BuildArrow(bool solidHead)
        {
            VertexStore arrow;
            VertexStore stem;
            using (Tools.BorrowStroke(out var stroke))
            using (Tools.BorrowVxs(out var v1, out var v3, out var v4))
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
            using (Tools.BorrowVxs(out var v1))
            using (Tools.BorrowStroke(out var stroke))
            {
                stroke.Width = 3;
                v1.AddMoveTo(x0, y0);
                v1.AddLineTo(x1, y1);
                stroke.MakeVxs(v1, output);
            }
        }

        public static VertexStore BuildRoundedRect(bool outline)
        {
            using (Tools.BorrowRoundedRect(out var roundedRect))
            {
                if (outline)
                {
                    roundedRect.SetRadius(5, 5, 0, 0, 5, 5, 0, 0);
                    roundedRect.SetRect(10, 10, 30, 30);
                    using (Tools.BorrowVxs(out var v1))
                    using (Tools.BorrowStroke(out var stroke))
                    {
                        stroke.LineJoin = LineJoin.Bevel;
                        stroke.Width = 3;
                        roundedRect.MakeVxs(v1);
                        return stroke.CreateTrim(v1);
                    }
                }
                else
                {
                    roundedRect.SetRadius(5, 5, 0, 0, 5, 5, 0, 0);
                    roundedRect.SetRect(10, 10, 30, 30);
                    return roundedRect.CreateTrim();
                }
            }
        }

        [DemoConfig]
        public bool ShowRotatingPolygons { get; set; }


        void DrawLine3(Painter p)
        {
            double[] xyCoords = new double[]
            {
                10,100,
                40,50,
                70,100,
                100,50,
                130,100,
                160,50,
                190,100,
                200,50,
                230,100
            };

            if (xyCoords.Length > 4)
            {

                using (Tools.BorrowVxs(out var v1, out var v2))
                using (Tools.BorrowPathWriter(v1, out var pw))
                using (Tools.BorrowCurveFlattener(out var flattener))
                {

                    //for Catrom,
                    switch (ReqPolygonKind)
                    {
                        case PolygonKind.CatRom2:
                            pw.CatmulRom(xyCoords);
                            break;
                        case PolygonKind.UbSpline1:

                            pw.UbSpline(xyCoords);
                            break;
                        case PolygonKind.Hermite1:

                            pw.Hermite(xyCoords);

                            break;
                    }

                    flattener.MakeVxs(v1, v2);
                    p.FillStroke(v2, 2, Color.Red);
                }
            }

        }


        public override void Draw(Painter p)
        {
            p.Clear(Color.White);


            VertexStore selectedVxs = null;
            switch (ReqPolygonKind)
            {
                case PolygonKind.ArrowLineHead:
                    selectedVxs = _arrowLineHead;
                    break;
                case PolygonKind.ArrowSolidHead:
                    selectedVxs = _arrowSolidHead;
                    break;
                case PolygonKind.RoundCornerRect:
                    selectedVxs = _roundRectSolid;
                    break;
                case PolygonKind.RoundCornerRect_Outline:
                    selectedVxs = _roundRectOutline;
                    break;
                case PolygonKind.RoundCornerPolygon:
                    selectedVxs = _roundCornerPolygon;
                    break;
                case PolygonKind.RoundCornerPolygon2:
                    selectedVxs = _roundCornerPolygon2;
                    break;
                case PolygonKind.CatmullRom1:
                    selectedVxs = _catmullRomSpline1;
                    break;


                case PolygonKind.CardinalSpline2:
                    selectedVxs = _cardinalSpline2;
                    break;
                case PolygonKind.CatRom2:
                case PolygonKind.Hermite1:
                case PolygonKind.UbSpline1:
                    DrawLine3(p);
                    return;


                //-----------------
                case PolygonKind.GeneralLines:
                    selectedVxs = _generalLines;
                    break;
                case PolygonKind.InsidePartOfLines:
                    selectedVxs = _insidePartOfLines;
                    break;
                case PolygonKind.OutsidePartOfLines:
                    selectedVxs = _outsidePartOfLines;
                    break;
                    //-----------------
            }

            if (selectedVxs == null) return;

            float ox = p.OriginX;
            float oy = p.OriginY;

            p.SetOrigin(200, 200);
            p.FillColor = Color.Red;
            p.FillRect(0.5, 0.5, 2, 2);


            p.FillColor = Color.Black;
            p.Fill(selectedVxs);

            //test transform the shape
            using (Tools.BorrowVxs(out var v1))
            {

                for (int i = 0; i < 1; i++)
                {

                    selectedVxs.RotateDegToNewVxs(i * (360 / 8), v1);
                    p.Fill(v1);
                    v1.Clear();
                }

            }
            p.SetOrigin(ox, oy);
            base.Draw(p);
        }
        [DemoConfig]
        public PolygonKind ReqPolygonKind { get; set; }

    }
}