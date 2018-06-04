//BSD, 2014-2018, WinterDev
//MatterHackers

using System;
using PixelFarm.Drawing;
using System.Collections.Generic;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Transform;
using Mini;
using ClipperLib;
namespace PixelFarm.Agg.Sample_PolygonClipping
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
        double m_x;
        double m_y;
        Color BackgroundColor;
        CurveFlattener curveFlattener = new CurveFlattener();
        public PolygonClippingDemo()
        {
            BackgroundColor = Color.White;
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
            if (BackgroundColor.Alpha0To255 > 0)
            {
                p.FillColor = BackgroundColor;
                p.FillRect(0, 0, this.Width, Height);
            }
            render_gpc(p);
        }


        void render_gpc(Painter p)
        {
            switch (this.PolygonSet)
            {
                case PolygonExampleSet.TwoSimplePaths:
                    {
                        //------------------------------------
                        // Two simple paths
                        //
                        PathWriter ps1 = new PathWriter();
                        PathWriter ps2 = new PathWriter();
                        double x = m_x - Width / 2 + 100;
                        double y = m_y - Height / 2 + 100;
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
                        p.Fill(ps1.MakeVertexSnap());
                        p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                        p.Fill(ps2.MakeVertexSnap());
                        CreateAndRenderCombined(p, ps1.MakeVertexSnap(), ps2.MakeVertexSnap());
                    }
                    break;
                case PolygonExampleSet.CloseStroke:
                    {
                        //------------------------------------
                        // Closed stroke
                        //
                        PathWriter ps1 = new PathWriter();
                        PathWriter ps2 = new PathWriter();
                        Stroke stroke = new Stroke(1);
                        stroke.Width = 10;
                        double x = m_x - Width / 2 + 100;
                        double y = m_y - Height / 2 + 100;
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
                        p.Fill(ps1.MakeVertexSnap());
                        //graphics2D.Render(ps1.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));
                        var vxs = ps2.Vxs;
                        //graphics2D.Render(stroke.MakeVxs(vxs), ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));
                        p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);

                        VectorToolBox.GetFreeVxs(out var v1);
                        p.Fill(stroke.MakeVxs(vxs, v1));
                        VectorToolBox.ReleaseVxs(ref v1);
                        CreateAndRenderCombined(p, ps1.MakeVertexSnap(), new VertexStoreSnap(vxs));
                    }
                    break;
                case PolygonExampleSet.GBAndArrow:
                    {
                        //------------------------------------
                        // Great Britain and Arrows
                        //
                        PathWriter gb_poly = new PathWriter();
                        PathWriter arrows = new PathWriter();
                        PixelFarm.Agg.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
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
                        Affine mtx2 = mtx1 * Affine.NewTranslation(m_x - Width / 2, m_y - Height / 2);
                        //VertexSourceApplyTransform trans_gb_poly = new VertexSourceApplyTransform(gb_poly, mtx1);
                        //VertexSourceApplyTransform trans_arrows = new VertexSourceApplyTransform(arrows, mtx2);

                        var trans_gb_poly = new VertexStore();
                        mtx1.TransformToVxs(gb_poly.Vxs, trans_gb_poly);

                        var trans_arrows = new VertexStore();
                        mtx2.TransformToVxs(arrows.Vxs, trans_arrows);


                        p.FillColor = ColorEx.Make(0.5f, 0.5f, 0f, 0.1f);
                        p.Fill(trans_gb_poly);
                        //graphics2D.Render(trans_gb_poly, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));
                        //stroke_gb_poly.Width = 0.1;
                        p.FillColor = ColorEx.Make(0, 0, 0);

                        VectorToolBox.GetFreeVxs(out var v1);
                        p.Fill(new Stroke(0.1).MakeVxs(trans_gb_poly, v1));
                        VectorToolBox.ReleaseVxs(ref v1);
                        //graphics2D.Render(new Stroke(0.1).MakeVxs(trans_gb_poly), ColorRGBAf.MakeColorRGBA(0, 0, 0));
                        //graphics2D.Render(trans_arrows, ColorRGBAf.MakeColorRGBA(0f, 0.5f, 0.5f, 0.1f));
                        p.FillColor = ColorEx.Make(0f, 0.5f, 0.5f, 0.1f);
                        p.Fill(trans_arrows);
                        CreateAndRenderCombined(p, new VertexStoreSnap(trans_gb_poly), new VertexStoreSnap(trans_arrows));
                    }
                    break;
                case PolygonExampleSet.GBAndSpiral:
                    {
                        //------------------------------------
                        // Great Britain and a Spiral
                        //
                        spiral sp = new spiral(m_x, m_y, 10, 150, 30, 0.0);
                        PathWriter gb_poly = new PathWriter();
                        PixelFarm.Agg.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);
                        Affine mtx = Affine.NewMatix(
                                AffinePlan.Translate(-1150, -1150),
                                AffinePlan.Scale(2));
                        //

                        VectorToolBox.GetFreeVxs(out var s1, out var v1);
                        VectorToolBox.GetFreeVxs(out var v2, out var v3);

                        mtx.TransformToVxs(gb_poly.Vxs, s1);
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
                        CreateAndRenderCombined(p, new VertexStoreSnap(s1), new VertexStoreSnap(stroke_vxs));

                        VectorToolBox.ReleaseVxs(ref s1, ref v1);
                        VectorToolBox.ReleaseVxs(ref v2, ref v3);
                    }
                    break;
                case PolygonExampleSet.SprialAndGlyph:
                    {
                        //------------------------------------
                        // Spiral and glyph
                        //
                        spiral sp = new spiral(m_x, m_y, 10, 150, 30, 0.0);
                        Stroke stroke = new Stroke(15);
                        PathWriter glyph = new PathWriter();
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
                        var t_glyph = new VertexStore();

                        mtx.TransformToVertexSnap(glyph.Vxs, t_glyph);


                        VectorToolBox.GetFreeVxs(out var v1, out var v2, out var v3);

                        var sp1 = stroke.MakeVxs(sp.MakeVxs(v1), v2);

                        var curveVxs = new VertexStore();
                        curveFlattener.MakeVxs(t_glyph, curveVxs);
                        CreateAndRenderCombined(p, new VertexStoreSnap(sp1), new VertexStoreSnap(curveVxs));
                        p.FillColor = ColorEx.Make(0f, 0f, 0f, 0.1f);
                        p.Fill(stroke.MakeVxs(sp1, v3));
                        //graphics2D.Render(stroke.MakeVxs(sp1), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));

                        p.FillColor = ColorEx.Make(0f, 0.6f, 0f, 0.1f);
                        p.Fill(curveVxs);
                        //graphics2D.Render(curveVxs, ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));


                        VectorToolBox.ReleaseVxs(ref v1, ref v2, ref v3);
                    }
                    break;
            }
        }


        void CreateAndRenderCombined(Painter p, VertexStoreSnap ps1, VertexStoreSnap ps2)
        {
            List<VertexStore> combined = null;
            switch (this.OpOption)
            {
                default: throw new NotSupportedException();
                case OperationOption.None:
                    return;
                case OperationOption.OR:
                    combined = VxsClipper.CombinePaths(ps1, ps2, VxsClipperType.Union, false);
                    break;
                case OperationOption.AND:
                    combined = VxsClipper.CombinePaths(ps1, ps2, VxsClipperType.InterSect, false);
                    break;
                case OperationOption.XOR:
                    combined = VxsClipper.CombinePaths(ps1, ps2, VxsClipperType.Xor, false);
                    break;
                case OperationOption.A_B:
                    combined = VxsClipper.CombinePaths(ps1, ps2, VxsClipperType.Difference, false);
                    break;
                case OperationOption.B_A:
                    combined = VxsClipper.CombinePaths(ps2, ps1, VxsClipperType.Difference, false);
                    break;
            }

            if (combined != null)
            {
                p.FillColor = ColorEx.Make(0.5f, 0.0f, 0f, 0.5f);
                p.Fill(new VertexStoreSnap(combined[0]));
                //graphics2D.Render(new VertexStoreSnap(combined[0]), ColorRGBAf.MakeColorRGBA(0.5f, 0.0f, 0f, 0.5f));
            }
        }
        public override void MouseDrag(int x, int y)
        {
            m_x = x;
            m_y = y;
        }
        public override void MouseDown(int x, int y, bool isRightoy)
        {
            m_x = x;
            m_y = y;
        }
        public override void MouseUp(int x, int y)
        {
            m_x = x;
            m_y = y;
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
        double m_x;
        double m_y;
        double m_r1;
        double m_r2;
        double m_step;
        double m_start_angle;
        double m_angle;
        double m_curr_r;
        double m_da;
        double m_dr;
        bool m_start;
        public spiral(double x, double y, double r1, double r2, double step, double start_angle = 0)
        {
            m_x = x;
            m_y = y;
            m_r1 = r1;
            m_r2 = r2;
            m_step = step;
            m_start_angle = start_angle;
            m_angle = start_angle;
            m_da = AggMath.deg2rad(4.0);
            m_dr = m_step / 90.0;
        }

        public IEnumerable<VertexData> GetVertexIter()
        {
            //--------------
            //rewind
            m_angle = m_start_angle;
            m_curr_r = m_r1;
            m_start = true;
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
        public VertexStoreSnap MakeVertexSnap(VertexStore vxs)
        {
            return new VertexStoreSnap(this.MakeVxs(vxs));
        }


        public VertexCmd GetNextVertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            if (m_curr_r > m_r2)
            {
                return VertexCmd.NoMore;
            }

            x = m_x + Math.Cos(m_angle) * m_curr_r;
            y = m_y + Math.Sin(m_angle) * m_curr_r;
            m_curr_r += m_dr;
            m_angle += m_da;
            if (m_start)
            {
                m_start = false;
                return VertexCmd.MoveTo;
            }
            return VertexCmd.LineTo;
        }
    }

    class conv_poly_counter
    {
        int m_contours;
        int m_points;
        conv_poly_counter(VertexStoreSnap src)
        {
            m_contours = 0;
            m_points = 0;
            var snapIter = src.GetVertexSnapIter();
            VertexCmd cmd;
            double x, y;
            do
            {
                cmd = snapIter.GetNextVertex(out x, out y);
                if (VertexHelper.IsVertextCommand(cmd))
                {
                    ++m_points;
                }

                if (VertexHelper.IsMoveTo(cmd))
                {
                    ++m_contours;
                }
            } while (cmd != VertexCmd.NoMore);
        }
    }
}
