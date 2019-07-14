//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Collections.Generic;


using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_PolygonClipping
{

    public enum OperationOption
    {
        None,
        OR,
        AND,
        XOR,
        [Note("A-B")]
        A_B,
        [Note("B-A")]
        B_A,
    }

    public enum PolygonExampleSet
    {
        [Note("Two Simple Paths")]
        TwoSimplePaths,
        [Note("Closed Stroke")]
        CloseStroke,
        [Note("Great Britain and Arrows")]
        GBAndArrow,
        [Note("Great Britain and Spiral")]
        GBAndSpiral,
        [Note("Spiral and Glyph")]
        SprialAndGlyph
    }

    [Info(OrderCode = "20")]
    public class PolygonClippingDemo : DemoBase
    {
        double _x;
        double _y;
        Color _backgroundColor;

        public PolygonClippingDemo()
        {
            _backgroundColor = Color.White;
            this.Width = 800;
            this.Height = 600;
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
            get;
            set;
        }
        public override void Draw(Painter p)
        {
            p.Clear(Color.White);
            if (_backgroundColor.Alpha0To255 > 0)
            {
                p.FillColor = _backgroundColor;
                p.FillRect(0, 0, this.Width, Height);
            }
            RenderPolygon(p);
        }


        void RenderPolygon(Painter p)
        {
            switch (this.PolygonSet)
            {
                case PolygonExampleSet.TwoSimplePaths:
                    {
                        //------------------------------------
                        // Two simple paths
                        using (VxsTemp.Borrow(out var v1, out var v2))
                        {
                            double x = _x - Width / 2 + 100;
                            double y = _y - Height / 2 + 100;

                            PolygonClippingDemoHelper.WritePath1(v1, x, y);
                            PolygonClippingDemoHelper.WritePath2(v2, x, y);

                            p.Fill(v1, ColorEx.Make(0f, 0f, 0f, 0.1f));
                            p.Fill(v2, ColorEx.Make(0f, 0.6f, 0f, 0.1f));

                            CreateAndRenderCombined(p, v1, v2);
                        }
                    }
                    break;
                case PolygonExampleSet.CloseStroke:
                    {
                        //------------------------------------
                        // Closed stroke 
                        using (VxsTemp.Borrow(out var v1, out var v2))
                        {
                            double x = _x - Width / 2 + 100;
                            double y = _y - Height / 2 + 100;

                            PolygonClippingDemoHelper.WriteCloseStrokeObj1(v1, x, y);
                            PolygonClippingDemoHelper.WriteCloseStrokeObj2(v2, x, y);

                            p.Fill(v1, ColorEx.Make(0f, 0f, 0f, 0.1f));
                            p.FillStroke(v2, 10, ColorEx.Make(0f, 0.6f, 0f, 0.1f));

                            CreateAndRenderCombined(p, v1, v2);
                        }
                    }
                    break;
                case PolygonExampleSet.GBAndArrow:
                    {
                        //------------------------------------
                        // Great Britain and Arrows
                        using (VxsTemp.Borrow(out var v1_gb_poly, out var v2_arrows))
                        using (VxsTemp.Borrow(out var v3))
                        {
                            Affine mtx1 = Affine.NewMatix(
                                    AffinePlan.Translate(-1150, -1150),
                                    AffinePlan.Scale(2)
                                 );
                            PolygonClippingDemoHelper.WriteGBObject(v1_gb_poly, 0, 0, mtx1);

                            p.Fill(v1_gb_poly, ColorEx.Make(0.5f, 0.5f, 0f, 0.1f));
                            p.FillStroke(v1_gb_poly, 0.1f, ColorEx.Make(0, 0, 0));

                            //
                            Affine mtx2 = mtx1 * Affine.NewTranslation(_x - Width / 2, _y - Height / 2);
                            PolygonClippingDemoHelper.WriteArrow(v2_arrows, 0, 0, mtx2);
                            p.Fill(v2_arrows, ColorEx.Make(0f, 0.5f, 0.5f, 0.1f));

                            CreateAndRenderCombined(p, v1_gb_poly, v2_arrows);
                        }
                    }
                    break;
                case PolygonExampleSet.GBAndSpiral:
                    {
                        //------------------------------------
                        // Great Britain and a Spiral
                        // 
                        using (VxsTemp.Borrow(out var v1_gb_poly))
                        using (VxsTemp.Borrow(out var v2_spiral, out var v2_spiral_outline))
                        using (VectorToolBox.Borrow(out Stroke stroke))
                        {
                            Affine mtx = Affine.NewMatix(
                                    AffinePlan.Translate(-1150, -1150),
                                    AffinePlan.Scale(2));

                            PolygonClippingDemoHelper.WriteGBObject(v1_gb_poly, 0, 0, mtx);
                            PolygonClippingDemoHelper.WriteSpiral(v2_spiral, _x, _y);

                            p.Fill(v1_gb_poly, ColorEx.Make(0.5f, 0.5f, 0f, 0.1f));
                            p.FillStroke(v1_gb_poly, 0.1f, Color.Black);

                            stroke.Width = 15;
                            p.Fill(stroke.MakeVxs(v2_spiral, v2_spiral_outline), ColorEx.Make(0.0f, 0.5f, 0.5f, 0.1f));

                            CreateAndRenderCombined(p, v1_gb_poly, v2_spiral_outline);
                        }
                    }
                    break;
                case PolygonExampleSet.SprialAndGlyph:
                    {
                        //------------------------------------
                        // Spiral and glyph                         
                        using (VxsTemp.Borrow(out var v1_spiral, out var v1_spiralOutline, out var v3))
                        using (VxsTemp.Borrow(out var glyph_vxs))
                        using (VectorToolBox.Borrow(out Stroke stroke))
                        {

                            Affine mtx = Affine.NewMatix(
                               AffinePlan.Scale(4),
                               AffinePlan.Translate(220, 200));

                            PolygonClippingDemoHelper.WriteSpiral(v1_spiral, _x, _y);
                            PolygonClippingDemoHelper.WriteGlyphObj(glyph_vxs, 0, 0, mtx);

                            //-----------------------------------------      
                            stroke.Width = 1;
                            stroke.MakeVxs(v1_spiral, v1_spiralOutline);

                            CreateAndRenderCombined(p, v1_spiralOutline, glyph_vxs);

                            p.Fill(v1_spiralOutline, ColorEx.Make(0f, 0f, 0f, 0.1f));
                            p.Fill(glyph_vxs, ColorEx.Make(0f, 0.6f, 0f, 0.1f));
                        }

                    }
                    break;
            }
        }


        void CreateAndRenderCombined(Painter p, VertexStore vxsSnap1, VertexStore vxsSnap2)
        {
            //TODO: review here again. 

            List<VertexStore> resultPolygons = new List<VertexStore>();
            PolygonClippingDemoHelper.CreateAndRenderCombined(vxsSnap1, vxsSnap2, OpOption, false, resultPolygons);
            if (resultPolygons.Count > 0)
            {
                //draw combine result
                p.Fill(resultPolygons[0], ColorEx.Make(0.5f, 0.0f, 0f, 0.5f));
            }
        }
        public override void MouseDrag(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public override void MouseDown(int x, int y, bool isRightoy)
        {
            _x = x;
            _y = y;
        }
        public override void MouseUp(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }

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
        public override void Draw(Painter p)
        {
            p.Clear(Color.White);
            if (_backgroundColor.Alpha0To255 > 0)
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

            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(v1, out PathWriter ps1))
            using (VectorToolBox.Borrow(v2, out PathWriter ps2))
            {
                double x = _x - Width / 2 + 100;
                double y = _y - Height / 2 + 100;
                ps1.MoveTo(x + 140, y + 145);
                ps1.LineTo(x + 225, y + 44);
                ps1.LineTo(x + 296, y + 219);
                ps1.CloseFigure();
                // 
                ps1.LineTo(x + 226, y + 289);
                ps1.LineTo(x + 82, y + 292);
                //
                ps1.MoveTo(x + 220, y + 222);
                ps1.LineTo(x + 363, y + 249);
                ps1.LineTo(x + 265, y + 331);
                ps1.MoveTo(x + 242, y + 243);
                ps1.LineTo(x + 268, y + 309);
                ps1.LineTo(x + 325, y + 261);
                ps1.MoveTo(x + 259, y + 259);
                ps1.LineTo(x + 273, y + 288);
                ps1.LineTo(x + 298, y + 266);
                ps1.CloseFigure();

                _simplePath1 = v1.CreateTrim();
                //
                ps2.MoveTo(100 + 32, 100 + 77);
                ps2.LineTo(100 + 473, 100 + 263);
                ps2.LineTo(100 + 351, 100 + 290);
                ps2.LineTo(100 + 354, 100 + 374);
                ps2.CloseFigure();

                _simplePath2 = v2.CreateTrim();
            }

        }

        VertexStore _closeStroke1;
        VertexStore _closeStroke2;
        void CreateCloseStroke()
        {
            //

            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VxsTemp.Borrow(out var v3))
            using (VectorToolBox.Borrow(v1, out PathWriter ps1))
            using (VectorToolBox.Borrow(v2, out PathWriter ps2))
            using (VectorToolBox.Borrow(out Stroke stroke))
            {
                stroke.Width = 10;
                double x = _x - Width / 2 + 100;
                double y = _y - Height / 2 + 100;
                //-----------------------------------------
                ps1.MoveTo(x + 140, y + 145);
                ps1.LineTo(x + 225, y + 44);
                ps1.LineTo(x + 296, y + 219);
                ps1.CloseFigure();
                ps1.LineTo(x + 226, y + 289);
                ps1.LineTo(x + 82, y + 292);
                ps1.MoveTo(x + 220 - 50, y + 222);
                ps1.LineTo(x + 265 - 50, y + 331);
                ps1.LineTo(x + 363 - 50, y + 249);
                ps1.CloseFigureCCW();
                //-----------------------------------------

                stroke.MakeVxs(v1, v3);
                _closeStroke1 = v3.CreateTrim();

                ps2.MoveTo(100 + 32, 100 + 77);
                ps2.LineTo(100 + 473, 100 + 263);
                ps2.LineTo(100 + 351, 100 + 290);
                ps2.LineTo(100 + 354, 100 + 374);
                ps2.CloseFigure();

                v3.Clear();
                stroke.MakeVxs(v2, v3);
                _closeStroke2 = v3.CreateTrim();

                //p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                //p.Fill(v1);
                ////graphics2D.Render(ps1.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f)); 
                ////graphics2D.Render(stroke.MakeVxs(vxs), ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));
                //p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                //p.Fill(stroke.MakeVxs(v2, v3));
                //CreateAndRenderCombined(p, v1, v2);
            }
        }


        VertexStore _gb;
        VertexStore _arrow;

        void CreateGBAndArrow()
        {
            //------------------------------------
            // Great Britain and Arrows
            using (VxsTemp.Borrow(out var v1_gb_poly, out var v2_arrows))
            using (VxsTemp.Borrow(out var v3))
            using (VectorToolBox.Borrow(v1_gb_poly, out PathWriter gb_poly))
            using (VectorToolBox.Borrow(v2_arrows, out PathWriter arrows))
            {
                PixelFarm.CpuBlit.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
                make_arrows(arrows);
                _gb = v1_gb_poly.CreateTrim();
                _arrow = v2_arrows.CreateTrim();
            }
        }

        VertexStore _spiral;

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
        void CreateSpiral()
        {
            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(out Stroke stroke))
            using (VectorToolBox.Borrow(out Spiral sp))
            {
                sp.SetParameters(_x, _y, 10, 150, 30, 0.0);
                stroke.Width = 15;
                stroke.MakeVxs(sp.MakeVxs(v1), v2);
                _spiral = v2.CreateTrim();
            }
        }
        Region _rgnA;
        Region _rgnB;
        Region _rgnC;

        MemBitmap CreateMaskBitmapFromVxs(VertexStore vxs)
        {

            RectD bounds = vxs.GetBoundingRect();
            using (VxsTemp.Borrow(out var v1))
            {
                vxs.TranslateToNewVxs(-bounds.Left, -bounds.Bottom, v1);
                bounds = v1.GetBoundingRect();

                int width = (int)Math.Round(bounds.Width);
                int height = (int)Math.Round(bounds.Height);

                //
                MemBitmap newbmp = new MemBitmap(width, height);
                using (AggPainterPool.Borrow(newbmp, out var _reusablePainter))
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
                                using (VxsTemp.Borrow(out var v1))
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
                                using (VxsTemp.Borrow(out var v1))
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
                using (VxsTemp.Borrow(out var v1))
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
        void make_arrows(PathWriter ps)
        {
            ps.Clear();
            ps.MoveTo(1330.599999999999909, 1282.399999999999864);
            ps.LineTo(1377.400000000000091, 1282.399999999999864);
            ps.LineTo(1361.799999999999955, 1298.000000000000000);
            ps.LineTo(1393.000000000000000, 1313.599999999999909);
            ps.LineTo(1361.799999999999955, 1344.799999999999955);
            ps.LineTo(1346.200000000000045, 1313.599999999999909);
            ps.LineTo(1330.599999999999909, 1329.200000000000045);
            ps.CloseFigure();
            ps.MoveTo(1330.599999999999909, 1266.799999999999955);
            ps.LineTo(1377.400000000000091, 1266.799999999999955);
            ps.LineTo(1361.799999999999955, 1251.200000000000045);
            ps.LineTo(1393.000000000000000, 1235.599999999999909);
            ps.LineTo(1361.799999999999955, 1204.399999999999864);
            ps.LineTo(1346.200000000000045, 1235.599999999999909);
            ps.LineTo(1330.599999999999909, 1220.000000000000000);
            ps.CloseFigure();
            ps.MoveTo(1315.000000000000000, 1282.399999999999864);
            ps.LineTo(1315.000000000000000, 1329.200000000000045);
            ps.LineTo(1299.400000000000091, 1313.599999999999909);
            ps.LineTo(1283.799999999999955, 1344.799999999999955);
            ps.LineTo(1252.599999999999909, 1313.599999999999909);
            ps.LineTo(1283.799999999999955, 1298.000000000000000);
            ps.LineTo(1268.200000000000045, 1282.399999999999864);
            ps.CloseFigure();
            ps.MoveTo(1268.200000000000045, 1266.799999999999955);
            ps.LineTo(1315.000000000000000, 1266.799999999999955);
            ps.LineTo(1315.000000000000000, 1220.000000000000000);
            ps.LineTo(1299.400000000000091, 1235.599999999999909);
            ps.LineTo(1283.799999999999955, 1204.399999999999864);
            ps.LineTo(1252.599999999999909, 1235.599999999999909);
            ps.LineTo(1283.799999999999955, 1251.200000000000045);
            ps.CloseFigure();
        }
    }






}
