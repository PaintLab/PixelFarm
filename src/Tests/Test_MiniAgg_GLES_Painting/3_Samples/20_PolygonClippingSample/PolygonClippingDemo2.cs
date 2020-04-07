//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_PolygonClipping
{
    public enum RegionKind
    {
        VxsRegion,
        BitmapBasedRegion,
    }

    [Info(OrderCode = "20")]
    public class PolygonClippingDemo2 : DemoBase
    {
        double _x;
        double _y;
        bool _needUpdate;

        Color _backgroundColor;
        CurveFlattener _curveFlattener = new CurveFlattener();
        OperationOption _opOption;
        RegionKind _rgnKind;

        public PolygonClippingDemo2()
        {
            _backgroundColor = Color.White;
            this.Width = 800;
            this.Height = 600;


            CreateTwoSimplePath();
            CreateCloseStroke();
            CreateGBAndArrow();
            CreateSpiral();

            _needUpdate = true;

        }
        [DemoConfig]
        public PolygonExampleSet PolygonSet
        {
            get;
            set;
        }
        [DemoConfig]
        public OperationOption OpOption
        {
            get => _opOption;
            set
            {
                _opOption = value;
                _needUpdate = true;
            }
        }

        [DemoConfig]
        public bool ConvertToRegion
        {
            get;
            set;
        }
        [DemoConfig]
        public RegionKind RegionKind
        {
            get => _rgnKind;
            set
            {
                _rgnKind = value;
                _needUpdate = true;
            }
        }

        public override void Draw(Painter p)
        {
            p.Clear(Color.White);
            if (_backgroundColor.A > 0)
            {
                p.FillColor = _backgroundColor;
                p.FillRect(0, 0, this.Width, Height);
            }

            RenderPolygon(p);
        }

        VertexStore _simplePath1;
        VertexStore _simplePath2;
        void CreateTwoSimplePath()
        {
            using (Tools.BorrowVxs(out var v1, out var v2))
            {
                double x = _x - Width / 2 + 100;
                double y = _y - Height / 2 + 100;
                PolygonClippingDemoHelper.WritePath1(v1, x, y);
                PolygonClippingDemoHelper.WritePath1(v2, 0, 0);
                _simplePath1 = v1.CreateTrim();
                _simplePath2 = v2.CreateTrim();
            }
        }

        VertexStore _closeStroke1;
        VertexStore _closeStroke2;
        void CreateCloseStroke()
        {

            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowVxs(out var v3, out var v4))
            using (Tools.BorrowStroke(out var stroke))
            {

                double x = _x - Width / 2 + 100;
                double y = _y - Height / 2 + 100;
                PolygonClippingDemoHelper.WritePath1(v1, x, y);
                stroke.Width = 10;
                stroke.MakeVxs(v1, v3);
                _closeStroke1 = v3.CreateTrim();


                PolygonClippingDemoHelper.WritePath2(v2, x, y);
                stroke.MakeVxs(v2, v4);
                _closeStroke2 = v4.CreateTrim();
            }
        }
        VertexStore _gb;
        VertexStore _arrow;
        VertexStore _spiral;
        void CreateGBAndArrow()
        {
            //------------------------------------
            // Great Britain and Arrows
            using (Tools.BorrowVxs(out var v1, out var v2))
            {
                PolygonClippingDemoHelper.WriteGBObject(v1, 0, 0);
                _gb = v1.CreateTrim();
                //
                PolygonClippingDemoHelper.WriteArrow(v2, 0, 0);
                _arrow = v2.CreateTrim();
            }
        }
        void CreateSpiral()
        {
            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowStroke(out var stroke))
            {

                PolygonClippingDemoHelper.WriteSpiral(v1, _x, _y);

                stroke.Width = 15;
                stroke.MakeVxs(v1, v2);
                _spiral = v2.CreateTrim();
            }
        }
        Region _rgnA;
        Region _rgnB;
        Region _rgnC;

        static MemBitmap CreateMaskBitmapFromVxs(VertexStore vxs)
        {

            Q1RectD bounds = vxs.GetBoundingRect();
            using (Tools.BorrowVxs(out var v1))
            {
                vxs.TranslateToNewVxs(-bounds.Left, -bounds.Bottom, v1);
                bounds = v1.GetBoundingRect();

                int width = (int)Math.Round(bounds.Width);
                int height = (int)Math.Round(bounds.Height);

                //
                MemBitmap newbmp = new MemBitmap(width, height);
                using (Tools.BorrowAggPainter(newbmp, out var _reusablePainter))
                {
                    _reusablePainter.Clear(Color.Black);
                    _reusablePainter.Fill(v1, Color.White);
                }
                return newbmp;
            }

        }
        void RenderPolygon(Painter p)
        {
            VertexStore a = null;
            VertexStore b = null;

            switch (PolygonSet)
            {
                case PolygonExampleSet.TwoSimplePaths:
                    a = _simplePath1;
                    b = _simplePath2;
                    break;
                case PolygonExampleSet.GBAndArrow:
                    a = _gb;
                    b = _arrow;
                    break;
                case PolygonExampleSet.GBAndSpiral:
                    a = _gb;
                    b = _spiral;
                    break;
                case PolygonExampleSet.CloseStroke:
                    a = _closeStroke1;
                    b = _closeStroke2;
                    break;
            }



            if (a == null || b == null)
            {
                return;
            }

            if (ConvertToRegion)
            {
                if (_needUpdate)
                {
                    //create rgn
                    _rgnA?.Dispose();
                    _rgnB?.Dispose();
                    _rgnC?.Dispose();

                    switch (_rgnKind)
                    {
                        case RegionKind.VxsRegion:
                            {
                                using (Tools.BorrowVxs(out var v1))
                                {
                                    Affine.NewTranslation(_x, _y).TransformToVxs(a, v1);
                                    //CreateAndRenderCombined(p, v1, b); 
                                    _rgnA = new PathReconstruction.VxsRegion(v1.CreateTrim());
                                    _rgnB = new PathReconstruction.VxsRegion(b.CreateTrim());
                                }
                            }
                            break;
                        case RegionKind.BitmapBasedRegion:
                            {
                                //this case, we create bitmap rgn from a and b
                                //
                                using (Tools.BorrowVxs(out var v1))
                                {
                                    Affine.NewTranslation(_x, _y).TransformToVxs(a, v1);
                                    _rgnA = new PathReconstruction.BitmapBasedRegion(CreateMaskBitmapFromVxs(v1));
                                    _rgnB = new PathReconstruction.BitmapBasedRegion(CreateMaskBitmapFromVxs(b));
                                }
                            }
                            break;
                    }

                    //
                    switch (this.OpOption)
                    {
                        case OperationOption.OR: //union
                            _rgnC = _rgnA.CreateUnion(_rgnB);
                            break;
                        case OperationOption.AND: //intersect
                            _rgnC = _rgnA.CreateIntersect(_rgnB);
                            break;
                        case OperationOption.XOR:
                            _rgnC = _rgnA.CreateXor(_rgnB);
                            break;
                        case OperationOption.A_B:
                            _rgnC = _rgnA.CreateExclude(_rgnB);
                            break;
                        case OperationOption.B_A:
                            _rgnC = _rgnB.CreateExclude(_rgnA);
                            break;
                    }
                }
                p.Fill(_rgnC, ColorEx.Make(0.5f, 0.0f, 0f, 0.5f));
            }
            else
            {
                using (Tools.BorrowVxs(out var v1))
                {
                    //translate _simplepath2
                    Affine.NewTranslation(_x, _y).TransformToVxs(a, v1);
                    p.Fill(v1, ColorEx.Make(0f, 0f, 0f, 0.1f));
                    p.Fill(b, ColorEx.Make(0f, 0.6f, 0f, 0.1f));
                    CreateAndRenderCombined(p, v1, b);
                }
            }
        }

        void CreateAndRenderCombined(Painter p, VertexStore vxsSnap1, VertexStore vxsSnap2)
        {
            //TODO: review here again. 

            List<VertexStore> resultPolygons = new List<VertexStore>();
            PolygonClippingDemoHelper.CreateAndRenderCombined(vxsSnap1, vxsSnap2, OpOption, false, resultPolygons);

            if (resultPolygons.Count > 0)
            {
                p.FillColor = ColorEx.Make(0.5f, 0.0f, 0f, 0.5f);
                //TODO=?
                p.Fill(resultPolygons[0]);
            }
        }
        public override void MouseDrag(int x, int y)
        {
            _x = x;
            _y = y;
            _needUpdate = true;
        }
        public override void MouseDown(int x, int y, bool isRightoy)
        {
            _x = x;
            _y = y;
            _needUpdate = true;
        }
        public override void MouseUp(int x, int y)
        {
            _x = x;
            _y = y;
            _needUpdate = true;
        }
    }
}