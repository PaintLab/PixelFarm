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

        public enum PolygonKind
        {
            //
            ArrowSolidHead,
            ArrowLineHead,
            //
            RoundCornerTriangle,
            RoundCornerRect, RoundCornerRect_Outline,
            RoundCornerPolygon
        }
        public PolygonShopDemo()
        {
            _arrowSolidHead = BuildArrow(true);
            _arrowLineHead = BuildArrow(false);
            _roundRectSolid = BuildRoundedRect(false);
            _roundRectOutline = BuildRoundedRect(true);
            //
            _roundCornerPolygon = BuildRoundCornerPolygon();
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

                Affine.NewScaling(3).TransformToVxs(v1, v2);
                polygon = stroke.MakeVxs(v2, v3).CreateTrim();
                return polygon;
            }
        }
        static VertexStore BuildArrow(bool solidHead)
        {
            VertexStore arrow;
            VertexStore stem;
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VxsTemp.Borrow(out var v1, out var v2))
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
                    stroke.MakeVxs(v1, v2);
                    arrow = v2.CreateTrim();

                    BuildLine(10, 10, 10, 50, v3);
                    stem = v3.CreateTrim();
                }
                arrow = VxsClipper.CombinePaths(arrow, stem, VxsClipperType.Union, false, null);

                Affine.NewScaling(3).TransformToVxs(arrow, v4);
                arrow = v4.CreateTrim();

                return arrow;
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

            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(out RoundedRect roundedRect))
            {
                if (outline)
                {
                    roundedRect.SetRadius(5, 5, 0, 0, 5, 5, 0, 0);
                    roundedRect.SetRect(10, 10, 30, 30);
                    using (VxsTemp.Borrow(out var v2))
                    using (VectorToolBox.Borrow(out Stroke stroke))
                    {
                        stroke.LineJoin = LineJoin.Bevel;
                        stroke.Width = 3;
                        roundedRect.MakeVxs(v1);
                        return stroke.MakeVxs(v1, v2).CreateTrim();
                    }
                }
                else
                {
                    roundedRect.SetRadius(5, 5, 0, 0, 5, 5, 0, 0);
                    roundedRect.SetRect(10, 10, 30, 30);
                    return roundedRect.MakeVxs(v1).CreateTrim();
                }
            }
        }

        public override void Draw(Painter p)
        {
            p.Clear(Color.White);
            p.FillColor = Color.Black;

            switch (ReqPolygonKind)
            {
                case PolygonKind.ArrowLineHead:
                    p.Fill(_arrowLineHead);
                    break;
                case PolygonKind.ArrowSolidHead:
                    p.Fill(_arrowSolidHead);
                    break;
                case PolygonKind.RoundCornerRect:
                    p.Fill(_roundRectSolid);
                    break;
                case PolygonKind.RoundCornerRect_Outline:
                    p.Fill(_roundRectOutline);
                    break;
                case PolygonKind.RoundCornerPolygon:
                    p.Fill(_roundCornerPolygon);
                    break;
            }

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