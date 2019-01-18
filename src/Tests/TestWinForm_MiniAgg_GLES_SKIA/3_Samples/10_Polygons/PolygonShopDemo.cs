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
        VertexStore _tmpArrow;
        VertexStore _stem;

        public enum PolygonKind
        {
            Arrow,
        }
        public PolygonShopDemo()
        {
            //create simple arrow head 
            //using (VxsTemp.Borrow(out var v1, out var v2))
            //using (VxsTemp.Borrow(out var v3))
            //{
            //    BuildLine(5, 20, 10, 10, v1);
            //    BuildLine(10, 10, 15, 20, v2);
            //    //v1.AddMoveTo(5, 20);
            //    //v1.AddLineTo(10, 10);
            //    //v1.AddLineTo(20, 20);

            //    BuildLine(10, 10, 10, 50, v3);
            //    _tmpArrow = VxsClipper.CombinePaths(v1, v2, VxsClipperType.Union, false, null);
            //    _tmpArrow = VxsClipper.CombinePaths(_tmpArrow, v3, VxsClipperType.Union, false, null);
            //}
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VxsTemp.Borrow(out var v3, out var v4))
            {

                v1.AddMoveTo(5, 20);
                v1.AddLineTo(10, 10);
                v1.AddLineTo(15, 20);
                stroke.LineCap = LineCap.Round;
                stroke.Width = 3;
                stroke.MakeVxs(v1, v2);
                _tmpArrow = v2.CreateTrim();

                BuildLine(10, 10, 10, 50, v3);
                _stem = v3.CreateTrim();

                _tmpArrow = VxsClipper.CombinePaths(_tmpArrow, _stem, VxsClipperType.Union, false, null);

                Affine.NewScaling(3).TransformToVxs(_tmpArrow, v4);
                _tmpArrow = v4.CreateTrim();
            }
        }
        void BuildLine(float x0, float y0, float x1, float y1, VertexStore output)
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

        public override void Draw(Painter p)
        {
            p.Clear(Color.White);

            switch (ReqPolygonKind)
            {
                case PolygonKind.Arrow:
                    break;
            }

            p.FillColor = Color.Black;
            p.Fill(_tmpArrow);
            //p.Fill(_stem);
            base.Draw(p);
        }

        public PolygonKind ReqPolygonKind
        {
            get;
            set;
        }

    }
}