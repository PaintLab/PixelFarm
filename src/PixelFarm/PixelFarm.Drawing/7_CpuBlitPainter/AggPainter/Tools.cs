//MIT, 2016-present, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.CpuBlit
{
    public sealed class Tools
    {
        /// <summary>
        /// instance for extension methods
        /// </summary>
        public static readonly Tools More = new Tools();
        private Tools() { }

        public static TempContext<AggPainter> BorrowAggPainter(MemBitmap bmp, out AggPainter painter)
        {

            if (!Temp<AggPainter>.IsInit())
            {
                Temp<AggPainter>.SetNewHandler(
                    () => new AggPainter(new AggRenderSurface()),
                    p =>
                    {
                        p.RenderSurface.DetachDstBitmap();
                        p.Reset();
                    }
                    );
            }

            var tmpPainter = Temp<AggPainter>.Borrow(out painter);
            painter.RenderSurface.AttachDstBitmap(bmp);
            return tmpPainter;
        }
        public static TempContext<ShapeBuilder> BorrowShapeBuilder(out ShapeBuilder shapeBuilder)
        {
            if (!Temp<ShapeBuilder>.IsInit())
            {
                Temp<ShapeBuilder>.SetNewHandler(
                    () => new ShapeBuilder(),
                    f => f.Reset());
            }

            TempContext<ShapeBuilder> context = Temp<ShapeBuilder>.Borrow(out shapeBuilder);
            shapeBuilder.InitVxs();//make it ready-to-use
            return context;
        }


        //TODO: add agressive inlining...

        public static TempContext<Stroke> BorrowStroke(out Stroke stroke) => VectorToolBox.Borrow(out stroke);        
        public static VxsContext1 BorrowVxs(out VertexStore vxs) => new VxsContext1(out vxs);
        public static VxsContext2 BorrowVxs(out VertexStore vxs1, out VertexStore vxs2) => new VxsContext2(out vxs1, out vxs2);
        public static VxsContext3 BorrowVxs(out VertexStore vxs1, out VertexStore vxs2, out VertexStore vxs3) => new VxsContext3(out vxs1, out vxs2, out vxs3);
    }

    public class ShapeBuilder
    {
        VertexStore _vxs;
        public void Reset()
        {
            if (_vxs != null)
            {
                VxsTemp.ReleaseVxs(_vxs);
                _vxs = null;
            }
        }
        public ShapeBuilder InitVxs()
        {
            Reset();
            VxsTemp.Borrow(out _vxs);
            return this;
        }
        public ShapeBuilder InitVxs(VertexStore src)
        {
            Reset();
            VxsTemp.Borrow(out _vxs);
            _vxs.AppendVertexStore(src);
            return this;
        }
        public ShapeBuilder MoveTo(double x0, double y0)
        {
            _vxs.AddMoveTo(x0, y0);
            return this;
        }
        public ShapeBuilder LineTo(double x1, double y1)
        {
            _vxs.AddLineTo(x1, y1);
            return this;
        }
        public ShapeBuilder CloseFigure()
        {
            _vxs.AddCloseFigure();
            return this;
        }
        public ShapeBuilder Scale(float s)
        {
            VxsTemp.Borrow(out VertexStore v2);
            Affine aff = Affine.NewScaling(s, s);
            aff.TransformToVxs(_vxs, v2);

            //release _vxs
            VxsTemp.ReleaseVxs(_vxs);
            _vxs = v2;
            return this;
        }
        public ShapeBuilder Stroke(Stroke stroke)
        {
            VxsTemp.Borrow(out VertexStore v2);
            stroke.MakeVxs(_vxs, v2);
            VxsTemp.ReleaseVxs(_vxs);
            _vxs = v2;
            return this;
        }
        public ShapeBuilder Stroke(float width)
        {
            VxsTemp.Borrow(out VertexStore v2);
            using (VectorToolBox.Borrow(out Stroke stroke))
            {
                stroke.Width = width;
                stroke.MakeVxs(_vxs, v2);
            }
            VxsTemp.ReleaseVxs(_vxs);
            _vxs = v2;
            return this;
        }
        public ShapeBuilder Curve4To(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3)
        {
            _vxs.AddVertex(x1, y1, VertexCmd.C4);
            _vxs.AddVertex(x2, y2, VertexCmd.C4);
            _vxs.AddVertex(x3, y3, VertexCmd.LineTo);
            return this;
        }
        public ShapeBuilder Curve3To(
           double x1, double y1,
           double x2, double y2)
        {
            _vxs.AddVertex(x1, y1, VertexCmd.C3);
            _vxs.AddVertex(x2, y2, VertexCmd.LineTo);
            return this;
        }
        public ShapeBuilder NoMore()
        {
            _vxs.AddNoMore();
            return this;
        }
        public VertexStore CreateTrim()
        {
            return _vxs.CreateTrim();
        }

        public ShapeBuilder TranslateToNewVxs(double dx, double dy)
        {
            VxsTemp.Borrow(out VertexStore v2);
            int count = _vxs.Count;
            VertexCmd cmd;
            for (int i = 0; i < count; ++i)
            {
                cmd = _vxs.GetVertex(i, out double x, out double y);
                x += dx;
                y += dy;
                v2.AddVertex(x, y, cmd);
            }
            VxsTemp.ReleaseVxs(_vxs);
            _vxs = v2;
            return this;
        }
        public ShapeBuilder Flatten(CurveFlattener flattener)
        {
            VxsTemp.Borrow(out VertexStore v2);
            flattener.MakeVxs(_vxs, v2);
            VxsTemp.ReleaseVxs(_vxs);
            _vxs = v2;
            return this;
        }
        /// <summary>
        /// flatten with default setting
        /// </summary>
        /// <returns></returns>
        public ShapeBuilder Flatten()
        {
            using (VectorToolBox.Borrow(out CurveFlattener flattener))
            {
                return Flatten(flattener);
            }
        }
        public VertexStore CurrentSharedVxs => _vxs;
    }


}

