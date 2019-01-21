//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using Mini;

namespace PixelFarm
{
    //simple cut, copy , paste example (simplified version of flood fill demo)
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

        public enum PolygonKind
        {
            //
            ArrowSolidHead,
            ArrowLineHead,
            //

            RoundCornerRect, RoundCornerRect_Outline,
            RoundCornerPolygon, RoundCornerPolygon2,

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

        }
        static VertexStore BuildRoundCornerPolygon2()
        {
            VertexStore polygon;
            using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VectorToolBox.Borrow(out CurveFlattener flattenr))
            using (VectorToolBox.Borrow(v1, out PathWriter pw))
            {
                pw.MoveTo(5, 20);
                pw.Curve3(10, 10, 15, 20);
                pw.CloseFigure();

                //--------------------------
                v1.ScaleToNewVxs(3, v2);
                flattenr.MakeVxs(v2, v3);
                polygon = v3.CreateTrim();
                return polygon;
            }
        }
        static VertexStore BuildRoundCornerPolygon()
        {
            VertexStore polygon;
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
            {
                v1.AddMoveTo(5, 20);
                v1.AddLineTo(10, 10);
                v1.AddLineTo(15, 20);
                v1.AddCloseFigure();

                stroke.GenerateOnlyOuterBorderForClosedShape = true;
                stroke.Width = 5;
                stroke.LineJoin = LineJoin.Round;

                v1.ScaleToNewVxs(3, v2);
                polygon = stroke.MakeVxs(v2, v3).CreateTrim();
                return polygon;
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

        static VertexStore BuildRoundedRect(bool outline)
        {
            using (VectorToolBox.Borrow(out RoundedRect roundedRect))
            {
                if (outline)
                {
                    roundedRect.SetRadius(5, 5, 0, 0, 5, 5, 0, 0);
                    roundedRect.SetRect(10, 10, 30, 30);
                    using (VxsTemp.Borrow(out var v1))
                    using (VectorToolBox.Borrow(out Stroke stroke))
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
            using (VxsTemp.Borrow(out var v1))
            {

                for (int i = 0; i < 8; i++)
                {

                    selectedVxs.RotateToNewVxs(i * (360 / 8), v1);
                    p.Fill(v1);

                    v1.Clear();
                }

            }
            p.SetOrigin(ox, oy);
            base.Draw(p);
        }
        [DemoConfig]
        public PolygonKind ReqPolygonKind
        {
            get;
            set;
        }

    }
}