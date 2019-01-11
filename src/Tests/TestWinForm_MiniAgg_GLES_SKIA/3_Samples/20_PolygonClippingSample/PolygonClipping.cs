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
        CurveFlattener _curveFlattener = new CurveFlattener();
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
                            //
                            ps2.MoveTo(100 + 32, 100 + 77);
                            ps2.LineTo(100 + 473, 100 + 263);
                            ps2.LineTo(100 + 351, 100 + 290);
                            ps2.LineTo(100 + 354, 100 + 374);
                            ps2.CloseFigure();
                            p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                            p.Fill(v1);
                            p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                            p.Fill(v2);
                            CreateAndRenderCombined(p, v1, v2);
                        }


                    }
                    break;
                case PolygonExampleSet.CloseStroke:
                    {
                        //------------------------------------
                        // Closed stroke
                        //

                        using (VxsTemp.Borrow(out var v1, out var v2))
                        using (VxsTemp.Borrow(out var v3))
                        using (VectorToolBox.Borrow(v1, out PathWriter ps1))
                        using (VectorToolBox.Borrow(v2, out PathWriter ps2))
                        {

                            Stroke stroke = new Stroke(1);
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


                            ps2.MoveTo(100 + 32, 100 + 77);
                            ps2.LineTo(100 + 473, 100 + 263);
                            ps2.LineTo(100 + 351, 100 + 290);
                            ps2.LineTo(100 + 354, 100 + 374);
                            ps2.CloseFigure();
                            p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                            p.Fill(v1);
                            //graphics2D.Render(ps1.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f)); 
                            //graphics2D.Render(stroke.MakeVxs(vxs), ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));
                            p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                            p.Fill(stroke.MakeVxs(v2, v3));
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
                        using (VectorToolBox.Borrow(v1_gb_poly, out PathWriter gb_poly))
                        using (VectorToolBox.Borrow(v2_arrows, out PathWriter arrows))
                        {
                            PixelFarm.CpuBlit.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
                            make_arrows(arrows);
                            //Affine mtx1 = Affine.NewIdentity();                        
                            //mtx1 *= Affine.NewTranslation(-1150, -1150);
                            //mtx1 *= Affine.NewScaling(2.0);
                            Affine mtx1 = Affine.NewMatix(
                                    AffinePlan.Translate(-1150, -1150),
                                    AffinePlan.Scale(2)
                                 );
                            //Affine.NewIdentity();
                            //mtx2 = mtx1;
                            //mtx2 *= Affine.NewTranslation(m_x - Width / 2, m_y - Height / 2);
                            Affine mtx2 = mtx1 * Affine.NewTranslation(_x - Width / 2, _y - Height / 2);
                            //VertexSourceApplyTransform trans_gb_poly = new VertexSourceApplyTransform(gb_poly, mtx1);
                            //VertexSourceApplyTransform trans_arrows = new VertexSourceApplyTransform(arrows, mtx2);

                            var trans_gb_poly = new VertexStore();
                            mtx1.TransformToVxs(v1_gb_poly, trans_gb_poly);

                            var trans_arrows = new VertexStore();
                            mtx2.TransformToVxs(v2_arrows, trans_arrows);


                            p.FillColor = ColorEx.Make(0.5f, 0.5f, 0f, 0.1f);
                            p.Fill(trans_gb_poly);
                            //graphics2D.Render(trans_gb_poly, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));
                            //stroke_gb_poly.Width = 0.1;
                            p.FillColor = ColorEx.Make(0, 0, 0);
                            //
                            //
                            p.Fill(new Stroke(0.1).MakeVxs(trans_gb_poly, v3));
                            //
                            //
                            //graphics2D.Render(new Stroke(0.1).MakeVxs(trans_gb_poly), ColorRGBAf.MakeColorRGBA(0, 0, 0));
                            //graphics2D.Render(trans_arrows, ColorRGBAf.MakeColorRGBA(0f, 0.5f, 0.5f, 0.1f));
                            p.FillColor = ColorEx.Make(0f, 0.5f, 0.5f, 0.1f);
                            p.Fill(trans_arrows);
                            CreateAndRenderCombined(p, trans_gb_poly, trans_arrows);

                        }
                    }
                    break;
                case PolygonExampleSet.GBAndSpiral:
                    {
                        //------------------------------------
                        // Great Britain and a Spiral
                        // 
                        using (VxsTemp.Borrow(out var v1_gb_poly))
                        using (VectorToolBox.Borrow(v1_gb_poly, out PathWriter gb_poly))
                        using (VxsTemp.Borrow(out var s1, out var v1))
                        using (VxsTemp.Borrow(out var v2, out var v3))
                        {

                            spiral sp = new spiral(_x, _y, 10, 150, 30, 0.0);

                            PixelFarm.CpuBlit.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
                            Affine mtx = Affine.NewMatix(
                                    AffinePlan.Translate(-1150, -1150),
                                    AffinePlan.Scale(2));


                            mtx.TransformToVxs(v1_gb_poly, s1);
                            p.FillColor = ColorEx.Make(0.5f, 0.5f, 0f, 0.1f);
                            p.Fill(s1);
                            //graphics2D.Render(s1, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));

                            //graphics2D.Render(new Stroke(0.1).MakeVxs(s1), ColorRGBA.Black);
                            p.FillColor = Color.Black;


                            p.Fill(new Stroke(0.1).MakeVxs(s1, v1));
                            var stroke_vxs = new Stroke(15).MakeVxs(sp.MakeVxs(v2), v3);
                            p.FillColor = ColorEx.Make(0.0f, 0.5f, 0.5f, 0.1f);// XUolorRXBAf.MakeColorRGBA(0.0f, 0.5f, 0.5f, 0.1f);
                            p.Fill(stroke_vxs);
                            //graphics2D.Render(stroke_vxs, ColorRGBAf.MakeColorRGBA(0.0f, 0.5f, 0.5f, 0.1f));
                            CreateAndRenderCombined(p, s1, stroke_vxs);
                        }

                    }
                    break;
                case PolygonExampleSet.SprialAndGlyph:
                    {
                        //------------------------------------
                        // Spiral and glyph
                        using (VxsTemp.Borrow(out var t_glyph, out var curveVxs))
                        using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
                        using (VxsTemp.Borrow(out var glyph_vxs))
                        using (VectorToolBox.Borrow(glyph_vxs, out PathWriter glyph))
                        {
                            spiral sp = new spiral(_x, _y, 10, 150, 30, 0.0);
                            Stroke stroke = new Stroke(15);
                            glyph.MoveTo(28.47, 6.45);
                            glyph.Curve3(21.58, 1.12, 19.82, 0.29);
                            glyph.Curve3(17.19, -0.93, 14.21, -0.93);
                            glyph.Curve3(9.57, -0.93, 6.57, 2.25);
                            glyph.Curve3(3.56, 5.42, 3.56, 10.60);
                            glyph.Curve3(3.56, 13.87, 5.03, 16.26);
                            glyph.Curve3(7.03, 19.58, 11.99, 22.51);
                            glyph.Curve3(16.94, 25.44, 28.47, 29.64);
                            glyph.LineTo(28.47, 31.40);
                            glyph.Curve3(28.47, 38.09, 26.34, 40.58);
                            glyph.Curve3(24.22, 43.07, 20.17, 43.07);
                            glyph.Curve3(17.09, 43.07, 15.28, 41.41);
                            glyph.Curve3(13.43, 39.75, 13.43, 37.60);
                            glyph.LineTo(13.53, 34.77);
                            glyph.Curve3(13.53, 32.52, 12.38, 31.30);
                            glyph.Curve3(11.23, 30.08, 9.38, 30.08);
                            glyph.Curve3(7.57, 30.08, 6.42, 31.35);
                            glyph.Curve3(5.27, 32.62, 5.27, 34.81);
                            glyph.Curve3(5.27, 39.01, 9.57, 42.53);
                            glyph.Curve3(13.87, 46.04, 21.63, 46.04);
                            glyph.Curve3(27.59, 46.04, 31.40, 44.04);
                            glyph.Curve3(34.28, 42.53, 35.64, 39.31);
                            glyph.Curve3(36.52, 37.21, 36.52, 30.71);
                            glyph.LineTo(36.52, 15.53);
                            glyph.Curve3(36.52, 9.13, 36.77, 7.69);
                            glyph.Curve3(37.01, 6.25, 37.57, 5.76);
                            glyph.Curve3(38.13, 5.27, 38.87, 5.27);
                            glyph.Curve3(39.65, 5.27, 40.23, 5.62);
                            glyph.Curve3(41.26, 6.25, 44.19, 9.18);
                            glyph.LineTo(44.19, 6.45);
                            glyph.Curve3(38.72, -0.88, 33.74, -0.88);
                            glyph.Curve3(31.35, -0.88, 29.93, 0.78);
                            glyph.Curve3(28.52, 2.44, 28.47, 6.45);
                            glyph.CloseFigure();
                            glyph.MoveTo(28.47, 9.62);
                            glyph.LineTo(28.47, 26.66);
                            glyph.Curve3(21.09, 23.73, 18.95, 22.51);
                            glyph.Curve3(15.09, 20.36, 13.43, 18.02);
                            glyph.Curve3(11.77, 15.67, 11.77, 12.89);
                            glyph.Curve3(11.77, 9.38, 13.87, 7.06);
                            glyph.Curve3(15.97, 4.74, 18.70, 4.74);
                            glyph.Curve3(22.41, 4.74, 28.47, 9.62);
                            glyph.CloseFigure();
                            //Affine mtx = Affine.NewIdentity();
                            //mtx *= Affine.NewScaling(4.0);
                            //mtx *= Affine.NewTranslation(220, 200);
                            Affine mtx = Affine.NewMatix(
                                AffinePlan.Scale(4),
                                AffinePlan.Translate(220, 200));


                            mtx.TransformToVertexSnap(glyph_vxs, t_glyph);

                            //-----------------------------------------
                            //
                            VertexStore sp1 = stroke.MakeVxs(sp.MakeVxs(v1), v2);


                            _curveFlattener.MakeVxs(t_glyph, curveVxs);

                            CreateAndRenderCombined(p, sp1, curveVxs);
                            p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                            p.Fill(stroke.MakeVxs(sp1, v3));
                            //graphics2D.Render(stroke.MakeVxs(sp1), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));

                            p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                            p.Fill(curveVxs);
                            //graphics2D.Render(curveVxs, ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f)); 
                        }

                    }
                    break;
            }
        }


        void CreateAndRenderCombined(Painter p, VertexStore vxsSnap1, VertexStore vxsSnap2)
        {
            //TODO: review here again. 

            List<VertexStore> combined = new List<VertexStore>();
            switch (this.OpOption)
            {
                default: throw new NotSupportedException();
                case OperationOption.None:
                    return;
                case OperationOption.OR:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.Union, false, combined);
                    break;
                case OperationOption.AND:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.InterSect, false, combined);
                    break;
                case OperationOption.XOR:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.Xor, false, combined);
                    break;
                case OperationOption.A_B:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.Difference, false, combined);
                    break;
                case OperationOption.B_A:
                    VxsClipper.CombinePaths(vxsSnap2, vxsSnap1, VxsClipperType.Difference, false, combined);
                    break;
            }

            if (combined != null)
            {
                p.FillColor = ColorEx.Make(0.5f, 0.0f, 0f, 0.5f);
                //TODO=?
                p.Fill(combined[0]);
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



    public class spiral
    {
        double _x;
        double _y;
        double _r1;
        double _r2;
        double _step;
        double _start_angle;
        double _angle;
        double _curr_r;
        double _da;
        double _dr;
        bool _start;
        public spiral(double x, double y, double r1, double r2, double step, double start_angle = 0)
        {
            _x = x;
            _y = y;
            _r1 = r1;
            _r2 = r2;
            _step = step;
            _start_angle = start_angle;
            _angle = start_angle;
            _da = AggMath.deg2rad(4.0);
            _dr = _step / 90.0;
        }

        public IEnumerable<VertexData> GetVertexIter()
        {
            //--------------
            //rewind
            _angle = _start_angle;
            _curr_r = _r1;
            _start = true;
            //--------------

            VertexCmd cmd;
            double x, y;
            for (; ; )
            {
                cmd = GetNextVertex(out x, out y);
                switch (cmd)
                {
                    case VertexCmd.NoMore:
                        {
                            yield return new VertexData(cmd, x, y);
                            yield break;
                        }
                    default:
                        {
                            yield return new VertexData(cmd, x, y);
                        }
                        break;
                }
            }
        }
        public VertexStore MakeVxs(VertexStore vxs)
        {

            foreach (VertexData v in this.GetVertexIter())
            {
                vxs.AddVertex(v.x, v.y, v.command);
            }
            return vxs;
        }



        public VertexCmd GetNextVertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            if (_curr_r > _r2)
            {
                return VertexCmd.NoMore;
            }

            x = _x + Math.Cos(_angle) * _curr_r;
            y = _y + Math.Sin(_angle) * _curr_r;
            _curr_r += _dr;
            _angle += _da;
            if (_start)
            {
                _start = false;
                return VertexCmd.MoveTo;
            }
            return VertexCmd.LineTo;
        }
    }

    class conv_poly_counter
    {
        int _contours;
        int _points;

        conv_poly_counter(VertexStore src)
        {
            _contours = 0;
            _points = 0;

            VertexCmd cmd;
            double x, y;
            int index = 0;
            while ((cmd = src.GetVertex(index++, out x, out y)) != VertexCmd.NoMore)
            {
                if (VertexHelper.IsVertextCommand(cmd))
                {
                    ++_points;
                }

                if (VertexHelper.IsMoveTo(cmd))
                {
                    ++_contours;
                }
            } while (cmd != VertexCmd.NoMore) ;
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


                ////Affine mtx1 = Affine.NewIdentity();                        
                ////mtx1 *= Affine.NewTranslation(-1150, -1150);
                ////mtx1 *= Affine.NewScaling(2.0);
                //Affine mtx1 = Affine.NewMatix(
                //        AffinePlan.Translate(-1150, -1150),
                //        AffinePlan.Scale(2)
                //     );
                ////Affine.NewIdentity();
                ////mtx2 = mtx1;
                ////mtx2 *= Affine.NewTranslation(m_x - Width / 2, m_y - Height / 2);
                //Affine mtx2 = mtx1 * Affine.NewTranslation(_x - Width / 2, _y - Height / 2);
                ////VertexSourceApplyTransform trans_gb_poly = new VertexSourceApplyTransform(gb_poly, mtx1);
                ////VertexSourceApplyTransform trans_arrows = new VertexSourceApplyTransform(arrows, mtx2);

                //var trans_gb_poly = new VertexStore();
                //mtx1.TransformToVxs(v1_gb_poly, trans_gb_poly);

                //var trans_arrows = new VertexStore();
                //mtx2.TransformToVxs(v2_arrows, trans_arrows);


                //p.FillColor = ColorEx.Make(0.5f, 0.5f, 0f, 0.1f);
                //p.Fill(trans_gb_poly);
                ////graphics2D.Render(trans_gb_poly, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));
                ////stroke_gb_poly.Width = 0.1;
                //p.FillColor = ColorEx.Make(0, 0, 0);
                ////
                ////
                //p.Fill(new Stroke(0.1).MakeVxs(trans_gb_poly, v3));
                ////
                ////
                ////graphics2D.Render(new Stroke(0.1).MakeVxs(trans_gb_poly), ColorRGBAf.MakeColorRGBA(0, 0, 0));
                ////graphics2D.Render(trans_arrows, ColorRGBAf.MakeColorRGBA(0f, 0.5f, 0.5f, 0.1f));
                //p.FillColor = ColorEx.Make(0f, 0.5f, 0.5f, 0.1f);
                //p.Fill(trans_arrows);
                //CreateAndRenderCombined(p, trans_gb_poly, trans_arrows);

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
            {
                spiral sp = new spiral(_x, _y, 10, 150, 30, 0.0);
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

                //p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                //p.Fill(_rgnA);
                //p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                //p.Fill(_rgnB);

                p.FillColor = ColorEx.Make(0.5f, 0.0f, 0f, 0.5f);
                p.Fill(_rgnC);
            }
            else
            {
                using (VxsTemp.Borrow(out var v1))
                {
                    //translate _simplepath2
                    Affine.NewTranslation(_x, _y).TransformToVxs(a, v1);

                    p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                    p.Fill(v1);
                    p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                    p.Fill(b);

                    CreateAndRenderCombined(p, v1, b);
                }
            }
        }


        //void RenderPolygon2(Painter p)
        //{
        //    switch (this.PolygonSet)
        //    {
        //        case PolygonExampleSet.TwoSimplePaths:
        //            {
        //                //------------------------------------
        //                // Two simple paths


        //                //using (VxsTemp.Borrow(out var v1, out var v2))
        //                //using (VectorToolBox.Borrow(v1, out PathWriter ps1))
        //                //using (VectorToolBox.Borrow(v2, out PathWriter ps2))
        //                //{
        //                //    double x = _x - Width / 2 + 100;
        //                //    double y = _y - Height / 2 + 100;
        //                //    ps1.MoveTo(x + 140, y + 145);
        //                //    ps1.LineTo(x + 225, y + 44);
        //                //    ps1.LineTo(x + 296, y + 219);
        //                //    ps1.CloseFigure();
        //                //    // 
        //                //    ps1.LineTo(x + 226, y + 289);
        //                //    ps1.LineTo(x + 82, y + 292);
        //                //    //
        //                //    ps1.MoveTo(x + 220, y + 222);
        //                //    ps1.LineTo(x + 363, y + 249);
        //                //    ps1.LineTo(x + 265, y + 331);
        //                //    ps1.MoveTo(x + 242, y + 243);
        //                //    ps1.LineTo(x + 268, y + 309);
        //                //    ps1.LineTo(x + 325, y + 261);
        //                //    ps1.MoveTo(x + 259, y + 259);
        //                //    ps1.LineTo(x + 273, y + 288);
        //                //    ps1.LineTo(x + 298, y + 266);
        //                //    ps1.CloseFigure();

        //                //    //
        //                //    ps2.MoveTo(100 + 32, 100 + 77);
        //                //    ps2.LineTo(100 + 473, 100 + 263);
        //                //    ps2.LineTo(100 + 351, 100 + 290);
        //                //    ps2.LineTo(100 + 354, 100 + 374);
        //                //    ps2.CloseFigure();
        //                //    p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
        //                //    p.Fill(v1);
        //                //    p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
        //                //    p.Fill(v2);
        //                //    CreateAndRenderCombined(p, v1, v2);
        //                //}


        //            }
        //            break;
        //        case PolygonExampleSet.CloseStroke:
        //            {
        //                ////------------------------------------
        //                //// Closed stroke
        //                ////

        //                //using (VxsTemp.Borrow(out var v1, out var v2))
        //                //using (VxsTemp.Borrow(out var v3))
        //                //using (VectorToolBox.Borrow(v1, out PathWriter ps1))
        //                //using (VectorToolBox.Borrow(v2, out PathWriter ps2))
        //                //{

        //                //    Stroke stroke = new Stroke(1);
        //                //    stroke.Width = 10;
        //                //    double x = _x - Width / 2 + 100;
        //                //    double y = _y - Height / 2 + 100;
        //                //    //-----------------------------------------
        //                //    ps1.MoveTo(x + 140, y + 145);
        //                //    ps1.LineTo(x + 225, y + 44);
        //                //    ps1.LineTo(x + 296, y + 219);
        //                //    ps1.CloseFigure();
        //                //    ps1.LineTo(x + 226, y + 289);
        //                //    ps1.LineTo(x + 82, y + 292);
        //                //    ps1.MoveTo(x + 220 - 50, y + 222);
        //                //    ps1.LineTo(x + 265 - 50, y + 331);
        //                //    ps1.LineTo(x + 363 - 50, y + 249);
        //                //    ps1.CloseFigureCCW();
        //                //    //-----------------------------------------


        //                //    ps2.MoveTo(100 + 32, 100 + 77);
        //                //    ps2.LineTo(100 + 473, 100 + 263);
        //                //    ps2.LineTo(100 + 351, 100 + 290);
        //                //    ps2.LineTo(100 + 354, 100 + 374);
        //                //    ps2.CloseFigure();
        //                //    p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
        //                //    p.Fill(v1);
        //                //    //graphics2D.Render(ps1.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f)); 
        //                //    //graphics2D.Render(stroke.MakeVxs(vxs), ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));
        //                //    p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
        //                //    p.Fill(stroke.MakeVxs(v2, v3));
        //                //    CreateAndRenderCombined(p, v1, v2);
        //                //}

        //            }
        //            break;
        //        case PolygonExampleSet.GBAndArrow:
        //            {
        //                ////------------------------------------
        //                //// Great Britain and Arrows
        //                //using (VxsTemp.Borrow(out var v1_gb_poly, out var v2_arrows))
        //                //using (VxsTemp.Borrow(out var v3))
        //                //using (VectorToolBox.Borrow(v1_gb_poly, out PathWriter gb_poly))
        //                //using (VectorToolBox.Borrow(v2_arrows, out PathWriter arrows))
        //                //{
        //                //    PixelFarm.CpuBlit.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
        //                //    make_arrows(arrows);
        //                //    //Affine mtx1 = Affine.NewIdentity();                        
        //                //    //mtx1 *= Affine.NewTranslation(-1150, -1150);
        //                //    //mtx1 *= Affine.NewScaling(2.0);
        //                //    Affine mtx1 = Affine.NewMatix(
        //                //            AffinePlan.Translate(-1150, -1150),
        //                //            AffinePlan.Scale(2)
        //                //         );
        //                //    //Affine.NewIdentity();
        //                //    //mtx2 = mtx1;
        //                //    //mtx2 *= Affine.NewTranslation(m_x - Width / 2, m_y - Height / 2);
        //                //    Affine mtx2 = mtx1 * Affine.NewTranslation(_x - Width / 2, _y - Height / 2);
        //                //    //VertexSourceApplyTransform trans_gb_poly = new VertexSourceApplyTransform(gb_poly, mtx1);
        //                //    //VertexSourceApplyTransform trans_arrows = new VertexSourceApplyTransform(arrows, mtx2);

        //                //    var trans_gb_poly = new VertexStore();
        //                //    mtx1.TransformToVxs(v1_gb_poly, trans_gb_poly);

        //                //    var trans_arrows = new VertexStore();
        //                //    mtx2.TransformToVxs(v2_arrows, trans_arrows);


        //                //    p.FillColor = ColorEx.Make(0.5f, 0.5f, 0f, 0.1f);
        //                //    p.Fill(trans_gb_poly);
        //                //    //graphics2D.Render(trans_gb_poly, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));
        //                //    //stroke_gb_poly.Width = 0.1;
        //                //    p.FillColor = ColorEx.Make(0, 0, 0);
        //                //    //
        //                //    //
        //                //    p.Fill(new Stroke(0.1).MakeVxs(trans_gb_poly, v3));
        //                //    //
        //                //    //
        //                //    //graphics2D.Render(new Stroke(0.1).MakeVxs(trans_gb_poly), ColorRGBAf.MakeColorRGBA(0, 0, 0));
        //                //    //graphics2D.Render(trans_arrows, ColorRGBAf.MakeColorRGBA(0f, 0.5f, 0.5f, 0.1f));
        //                //    p.FillColor = ColorEx.Make(0f, 0.5f, 0.5f, 0.1f);
        //                //    p.Fill(trans_arrows);
        //                //    CreateAndRenderCombined(p, trans_gb_poly, trans_arrows);

        //                //}
        //            }
        //            break;
        //        case PolygonExampleSet.GBAndSpiral:
        //            {
        //                //    //------------------------------------
        //                //    // Great Britain and a Spiral
        //                //    // 
        //                //    using (VxsTemp.Borrow(out var v1_gb_poly))
        //                //    using (VectorToolBox.Borrow(v1_gb_poly, out PathWriter gb_poly))
        //                //    using (VxsTemp.Borrow(out var s1, out var v1))
        //                //    using (VxsTemp.Borrow(out var v2, out var v3))
        //                //    {

        //                //        spiral sp = new spiral(_x, _y, 10, 150, 30, 0.0);

        //                //        PixelFarm.CpuBlit.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
        //                //        Affine mtx = Affine.NewMatix(
        //                //                AffinePlan.Translate(-1150, -1150),
        //                //                AffinePlan.Scale(2));


        //                //        mtx.TransformToVxs(v1_gb_poly, s1);
        //                //        p.FillColor = ColorEx.Make(0.5f, 0.5f, 0f, 0.1f);
        //                //        p.Fill(s1);
        //                //        //graphics2D.Render(s1, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));

        //                //        //graphics2D.Render(new Stroke(0.1).MakeVxs(s1), ColorRGBA.Black);
        //                //        p.FillColor = Color.Black;


        //                //        p.Fill(new Stroke(0.1).MakeVxs(s1, v1));
        //                //        var stroke_vxs = new Stroke(15).MakeVxs(sp.MakeVxs(v2), v3);
        //                //        p.FillColor = ColorEx.Make(0.0f, 0.5f, 0.5f, 0.1f);// XUolorRXBAf.MakeColorRGBA(0.0f, 0.5f, 0.5f, 0.1f);
        //                //        p.Fill(stroke_vxs);
        //                //        //graphics2D.Render(stroke_vxs, ColorRGBAf.MakeColorRGBA(0.0f, 0.5f, 0.5f, 0.1f));
        //                //        CreateAndRenderCombined(p, s1, stroke_vxs);
        //                //    }

        //            }
        //            break;
        //        case PolygonExampleSet.SprialAndGlyph:
        //            {
        //                ////------------------------------------
        //                //// Spiral and glyph
        //                //using (VectorToolBox.Borrow(out Stroke stroke))
        //                //using (VxsTemp.Borrow(out var t_glyph, out var curveVxs))
        //                //using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
        //                //using (VxsTemp.Borrow(out var glyph_vxs))
        //                //using (VectorToolBox.Borrow(glyph_vxs, out PathWriter glyph))
        //                //{


        //                //    glyph.MoveTo(28.47, 6.45);
        //                //    glyph.Curve3(21.58, 1.12, 19.82, 0.29);
        //                //    glyph.Curve3(17.19, -0.93, 14.21, -0.93);
        //                //    glyph.Curve3(9.57, -0.93, 6.57, 2.25);
        //                //    glyph.Curve3(3.56, 5.42, 3.56, 10.60);
        //                //    glyph.Curve3(3.56, 13.87, 5.03, 16.26);
        //                //    glyph.Curve3(7.03, 19.58, 11.99, 22.51);
        //                //    glyph.Curve3(16.94, 25.44, 28.47, 29.64);
        //                //    glyph.LineTo(28.47, 31.40);
        //                //    glyph.Curve3(28.47, 38.09, 26.34, 40.58);
        //                //    glyph.Curve3(24.22, 43.07, 20.17, 43.07);
        //                //    glyph.Curve3(17.09, 43.07, 15.28, 41.41);
        //                //    glyph.Curve3(13.43, 39.75, 13.43, 37.60);
        //                //    glyph.LineTo(13.53, 34.77);
        //                //    glyph.Curve3(13.53, 32.52, 12.38, 31.30);
        //                //    glyph.Curve3(11.23, 30.08, 9.38, 30.08);
        //                //    glyph.Curve3(7.57, 30.08, 6.42, 31.35);
        //                //    glyph.Curve3(5.27, 32.62, 5.27, 34.81);
        //                //    glyph.Curve3(5.27, 39.01, 9.57, 42.53);
        //                //    glyph.Curve3(13.87, 46.04, 21.63, 46.04);
        //                //    glyph.Curve3(27.59, 46.04, 31.40, 44.04);
        //                //    glyph.Curve3(34.28, 42.53, 35.64, 39.31);
        //                //    glyph.Curve3(36.52, 37.21, 36.52, 30.71);
        //                //    glyph.LineTo(36.52, 15.53);
        //                //    glyph.Curve3(36.52, 9.13, 36.77, 7.69);
        //                //    glyph.Curve3(37.01, 6.25, 37.57, 5.76);
        //                //    glyph.Curve3(38.13, 5.27, 38.87, 5.27);
        //                //    glyph.Curve3(39.65, 5.27, 40.23, 5.62);
        //                //    glyph.Curve3(41.26, 6.25, 44.19, 9.18);
        //                //    glyph.LineTo(44.19, 6.45);
        //                //    glyph.Curve3(38.72, -0.88, 33.74, -0.88);
        //                //    glyph.Curve3(31.35, -0.88, 29.93, 0.78);
        //                //    glyph.Curve3(28.52, 2.44, 28.47, 6.45);
        //                //    glyph.CloseFigure();
        //                //    glyph.MoveTo(28.47, 9.62);
        //                //    glyph.LineTo(28.47, 26.66);
        //                //    glyph.Curve3(21.09, 23.73, 18.95, 22.51);
        //                //    glyph.Curve3(15.09, 20.36, 13.43, 18.02);
        //                //    glyph.Curve3(11.77, 15.67, 11.77, 12.89);
        //                //    glyph.Curve3(11.77, 9.38, 13.87, 7.06);
        //                //    glyph.Curve3(15.97, 4.74, 18.70, 4.74);
        //                //    glyph.Curve3(22.41, 4.74, 28.47, 9.62);
        //                //    glyph.CloseFigure();
        //                //    //Affine mtx = Affine.NewIdentity();
        //                //    //mtx *= Affine.NewScaling(4.0);
        //                //    //mtx *= Affine.NewTranslation(220, 200);
        //                //    Affine mtx = Affine.NewMatix(
        //                //        AffinePlan.Scale(4),
        //                //        AffinePlan.Translate(220, 200));


        //                //    mtx.TransformToVertexSnap(glyph_vxs, t_glyph);

        //                //    //-----------------------------------------

        //                //    spiral sp = new spiral(_x, _y, 10, 150, 30, 0.0);
        //                //    stroke.Width = 15;
        //                //    VertexStore sp1 = stroke.MakeVxs(sp.MakeVxs(v1), v2);


        //                //    _curveFlattener.MakeVxs(t_glyph, curveVxs);

        //                //    CreateAndRenderCombined(p, sp1, curveVxs);
        //                //    p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
        //                //    p.Fill(stroke.MakeVxs(sp1, v3));
        //                //    //graphics2D.Render(stroke.MakeVxs(sp1), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));

        //                //    p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
        //                //    p.Fill(curveVxs);
        //                //    //graphics2D.Render(curveVxs, ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f)); 
        //                //}

        //            }
        //            break;
        //    }
        //}


        void CreateAndRenderCombined(Painter p, VertexStore vxsSnap1, VertexStore vxsSnap2)
        {
            //TODO: review here again. 

            List<VertexStore> combined = new List<VertexStore>();
            switch (this.OpOption)
            {
                default: throw new NotSupportedException();
                case OperationOption.None:
                    return;
                case OperationOption.OR:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.Union, false, combined);
                    break;
                case OperationOption.AND:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.InterSect, false, combined);
                    break;
                case OperationOption.XOR:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.Xor, false, combined);
                    break;
                case OperationOption.A_B:
                    VxsClipper.CombinePaths(vxsSnap1, vxsSnap2, VxsClipperType.Difference, false, combined);
                    break;
                case OperationOption.B_A:
                    VxsClipper.CombinePaths(vxsSnap2, vxsSnap1, VxsClipperType.Difference, false, combined);
                    break;
            }

            if (combined != null)
            {
                p.FillColor = ColorEx.Make(0.5f, 0.0f, 0f, 0.5f);
                //TODO=?
                p.Fill(combined[0]);
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
