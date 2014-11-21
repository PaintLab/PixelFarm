//2014 BSD, WinterDev
//MatterHackers

using System;
using System.Collections.Generic;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;

using PixelFarm.VectorMath;
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
        PathStore CombinePaths(VertexStoreSnap a, VertexStoreSnap b, ClipType clipType)
        {
            List<List<IntPoint>> aPolys = CreatePolygons(a);
            List<List<IntPoint>> bPolys = CreatePolygons(b);

            Clipper clipper = new Clipper();

            clipper.AddPaths(aPolys, PolyType.ptSubject, true);
            clipper.AddPaths(bPolys, PolyType.ptClip, true);

            List<List<IntPoint>> intersectedPolys = new List<List<IntPoint>>();
            clipper.Execute(clipType, intersectedPolys);

            PathStore output = new PathStore();

            foreach (List<IntPoint> polygon in intersectedPolys)
            {
                bool first = true;
                int j = polygon.Count;

                if (j > 0)
                {
                    //first one
                    IntPoint point = polygon[0];

                    output.MoveTo(point.X / 1000.0, point.Y / 1000.0);

                    //next ...
                    if (j > 1)
                    {
                        for (int i = 1; i < j; ++i)
                        {
                            point = polygon[i];
                            output.LineTo(point.X / 1000.0, point.Y / 1000.0);
                        }
                    }
                }
                //foreach (IntPoint point in polygon)
                //{
                //    if (first)
                //    {
                //        output.AddVertex(point.X / 1000.0, point.Y / 1000.0, ShapePath.FlagsAndCommand.CommandMoveTo);
                //        first = false;
                //    }
                //    else
                //    {
                //        output.AddVertex(point.X / 1000.0, point.Y / 1000.0, ShapePath.FlagsAndCommand.CommandLineTo);
                //    }
                //}

                output.ClosePolygon();
            }


            output.Stop();
            return output;
        }

        private static List<List<IntPoint>> CreatePolygons(VertexStoreSnap a)
        {
            List<List<IntPoint>> allPolys = new List<List<IntPoint>>();
            List<IntPoint> currentPoly = null;
            VertexData last = new VertexData();
            VertexData first = new VertexData();
            bool addedFirst = false;

            var snapIter = a.GetVertexSnapIter();
            ShapePath.CmdAndFlags cmd;
            double x, y;
            cmd = snapIter.GetNextVertex(out x, out y);
            do
            {
                if (cmd == ShapePath.CmdAndFlags.LineTo)
                {
                    if (!addedFirst)
                    {
                        currentPoly.Add(new IntPoint((long)(last.x * 1000), (long)(last.y * 1000)));
                        addedFirst = true;
                        first = last;
                    }
                    currentPoly.Add(new IntPoint((long)(x * 1000), (long)(y * 1000)));
                    last = new VertexData(cmd, x, y);
                }
                else
                {
                    addedFirst = false;
                    currentPoly = new List<IntPoint>();
                    allPolys.Add(currentPoly);
                    if (cmd == ShapePath.CmdAndFlags.MoveTo)
                    {
                        last = new VertexData(cmd, x, y);
                    }
                    else
                    {
                        last = first;
                    }
                }
            } while (cmd != ShapePath.CmdAndFlags.Empty);

            return allPolys;
        }

        double m_x;
        double m_y;
        ColorRGBA BackgroundColor;


        public PolygonClippingDemo()
        {
            BackgroundColor = ColorRGBA.White;
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

        public override void Draw(Graphics2D g)
        {
            if (BackgroundColor.Alpha0To255 > 0)
            {
                g.FillRectangle(new RectD(0, 0, this.Width, Height), BackgroundColor);
            }
            render_gpc(g);
        }

        void render_gpc(Graphics2D graphics2D)
        {

            switch (this.PolygonSet)
            {
                case PolygonExampleSet.TwoSimplePaths:
                    {
                        //------------------------------------
                        // Two simple paths
                        //
                        PathStore ps1 = new PathStore();
                        PathStore ps2 = new PathStore();

                        double x = m_x - Width / 2 + 100;
                        double y = m_y - Height / 2 + 100;
                        ps1.MoveTo(x + 140, y + 145);
                        ps1.LineTo(x + 225, y + 44);
                        ps1.LineTo(x + 296, y + 219);
                        ps1.ClosePolygon();

                        ps1.LineTo(x + 226, y + 289);
                        ps1.LineTo(x + 82, y + 292);

                        ps1.MoveTo(x + 220, y + 222);
                        ps1.LineTo(x + 363, y + 249);
                        ps1.LineTo(x + 265, y + 331);

                        ps1.MoveTo(x + 242, y + 243);
                        ps1.LineTo(x + 268, y + 309);
                        ps1.LineTo(x + 325, y + 261);

                        ps1.MoveTo(x + 259, y + 259);
                        ps1.LineTo(x + 273, y + 288);
                        ps1.LineTo(x + 298, y + 266);

                        ps2.MoveTo(100 + 32, 100 + 77);
                        ps2.LineTo(100 + 473, 100 + 263);
                        ps2.LineTo(100 + 351, 100 + 290);
                        ps2.LineTo(100 + 354, 100 + 374);

                        graphics2D.Render(ps1.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));
                        graphics2D.Render(ps2.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));

                        CreateAndRenderCombined(graphics2D, ps1.MakeVertexSnap(), ps2.MakeVertexSnap());
                    }
                    break;

                case PolygonExampleSet.CloseStroke:
                    {
                        //------------------------------------
                        // Closed stroke
                        //
                        PathStore ps1 = new PathStore();
                        PathStore ps2 = new PathStore();
                        Stroke stroke = new Stroke(1);

                        stroke.Width = 10;
                        double x = m_x - Width / 2 + 100;
                        double y = m_y - Height / 2 + 100;

                        //-----------------------------------------
                        ps1.MoveTo(x + 140, y + 145);
                        ps1.LineTo(x + 225, y + 44);
                        ps1.LineTo(x + 296, y + 219);
                        ps1.ClosePolygon();

                        ps1.LineTo(x + 226, y + 289);
                        ps1.LineTo(x + 82, y + 292);

                        ps1.MoveTo(x + 220 - 50, y + 222);
                        ps1.LineTo(x + 265 - 50, y + 331);
                        ps1.LineTo(x + 363 - 50, y + 249);
                        ps1.ClosePolygonCCW();
                        //-----------------------------------------


                        ps2.MoveTo(100 + 32, 100 + 77);
                        ps2.LineTo(100 + 473, 100 + 263);
                        ps2.LineTo(100 + 351, 100 + 290);
                        ps2.LineTo(100 + 354, 100 + 374);
                        ps2.ClosePolygon();

                        graphics2D.Render(ps1.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));


                        var vxs = ps2.Vxs;
                        graphics2D.Render(stroke.MakeVxs(vxs), ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));
                        CreateAndRenderCombined(graphics2D, ps1.MakeVertexSnap(), new VertexStoreSnap(vxs));
                    }
                    break;


                case PolygonExampleSet.GBAndArrow:
                    {
                        //------------------------------------
                        // Great Britain and Arrows
                        //
                        PathStore gb_poly = new PathStore();
                        PathStore arrows = new PathStore();
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
                        var trans_gb_poly = mtx1.TransformToVxs(gb_poly.Vxs);
                        var trans_arrows = mtx2.TransformToVxs(arrows.Vxs);

                        graphics2D.Render(trans_gb_poly, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));

                        //stroke_gb_poly.Width = 0.1;
                        graphics2D.Render(new Stroke(0.1).MakeVxs(trans_gb_poly), ColorRGBAf.MakeColorRGBA(0, 0, 0));
                        graphics2D.Render(trans_arrows, ColorRGBAf.MakeColorRGBA(0f, 0.5f, 0.5f, 0.1f));

                        CreateAndRenderCombined(graphics2D, new VertexStoreSnap(trans_gb_poly), new VertexStoreSnap(trans_arrows));
                    }
                    break;

                case PolygonExampleSet.GBAndSpiral:
                    {
                        //------------------------------------
                        // Great Britain and a Spiral
                        //
                        spiral sp = new spiral(m_x, m_y, 10, 150, 30, 0.0);


                        PathStore gb_poly = new PathStore();
                        PixelFarm.Agg.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);

                        Affine mtx = Affine.NewMatix(
                                AffinePlan.Translate(-1150, -1150),
                                AffinePlan.Scale(2));


                        VertexStore s1 = mtx.TransformToVxs(gb_poly.Vxs);
                        graphics2D.Render(s1, ColorRGBAf.MakeColorRGBA(0.5f, 0.5f, 0f, 0.1f));
                        graphics2D.Render(new Stroke(0.1).MakeVxs(s1), ColorRGBA.Black);
                        var stroke_vxs = new Stroke(15).MakeVxs(sp.MakeVxs());
                        graphics2D.Render(stroke_vxs, ColorRGBAf.MakeColorRGBA(0.0f, 0.5f, 0.5f, 0.1f));

                        CreateAndRenderCombined(graphics2D, new VertexStoreSnap(s1), new VertexStoreSnap(stroke_vxs));
                    }
                    break;

                case PolygonExampleSet.SprialAndGlyph:
                    {
                        //------------------------------------
                        // Spiral and glyph
                        //
                        spiral sp = new spiral(m_x, m_y, 10, 150, 30, 0.0);
                        Stroke stroke = new Stroke(15);


                        PathStore glyph = new PathStore();
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
                        glyph.ClosePolygon();

                        glyph.MoveTo(28.47, 9.62);
                        glyph.LineTo(28.47, 26.66);
                        glyph.Curve3(21.09, 23.73, 18.95, 22.51);
                        glyph.Curve3(15.09, 20.36, 13.43, 18.02);
                        glyph.Curve3(11.77, 15.67, 11.77, 12.89);
                        glyph.Curve3(11.77, 9.38, 13.87, 7.06);
                        glyph.Curve3(15.97, 4.74, 18.70, 4.74);
                        glyph.Curve3(22.41, 4.74, 28.47, 9.62);
                        glyph.ClosePolygon();

                        //Affine mtx = Affine.NewIdentity();
                        //mtx *= Affine.NewScaling(4.0);
                        //mtx *= Affine.NewTranslation(220, 200);
                        Affine mtx = Affine.NewMatix(
                            AffinePlan.Scale(4),
                            AffinePlan.Translate(220, 200));

                        var t_glyph = mtx.TransformToVertexSnap(glyph.Vxs);

                        CurveFlattener curveFlattener = new CurveFlattener();

                        var sp1 = stroke.MakeVxs(sp.MakeVxs());

                        var curveVxs = curveFlattener.MakeVxs(t_glyph);

                        CreateAndRenderCombined(graphics2D, new VertexStoreSnap(sp1), new VertexStoreSnap(curveVxs));

                        graphics2D.Render(stroke.MakeVxs(sp1), ColorRGBAf.MakeColorRGBA(0f, 0f, 0f, 0.1f));

                        graphics2D.Render(curveVxs, ColorRGBAf.MakeColorRGBA(0f, 0.6f, 0f, 0.1f));
                    }
                    break;
            }
        }


        void CreateAndRenderCombined(Graphics2D graphics2D, VertexStoreSnap ps1, VertexStoreSnap ps2)
        {
            PathStore combined = null;

            switch (this.OpOption)
            {
                case OperationOption.OR:
                    combined = CombinePaths(ps1, ps2, ClipType.ctUnion);
                    break;
                case OperationOption.AND:
                    combined = CombinePaths(ps1, ps2, ClipType.ctIntersection);
                    break;
                case OperationOption.XOR:
                    combined = CombinePaths(ps1, ps2, ClipType.ctXor);
                    break;
                case OperationOption.A_B:
                    combined = CombinePaths(ps1, ps2, ClipType.ctDifference);
                    break;
                case OperationOption.B_A:
                    combined = CombinePaths(ps2, ps1, ClipType.ctDifference);
                    break;
            }

            if (combined != null)
            {
                graphics2D.Render(combined.MakeVertexSnap(), ColorRGBAf.MakeColorRGBA(0.5f, 0.0f, 0f, 0.5f));
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
        void make_arrows(PathStore ps)
        {
            ps.Clear();
            ps.MoveTo(1330.599999999999909, 1282.399999999999864);
            ps.LineTo(1377.400000000000091, 1282.399999999999864);
            ps.LineTo(1361.799999999999955, 1298.000000000000000);
            ps.LineTo(1393.000000000000000, 1313.599999999999909);
            ps.LineTo(1361.799999999999955, 1344.799999999999955);
            ps.LineTo(1346.200000000000045, 1313.599999999999909);
            ps.LineTo(1330.599999999999909, 1329.200000000000045);
            ps.ClosePolygon();

            ps.MoveTo(1330.599999999999909, 1266.799999999999955);
            ps.LineTo(1377.400000000000091, 1266.799999999999955);
            ps.LineTo(1361.799999999999955, 1251.200000000000045);
            ps.LineTo(1393.000000000000000, 1235.599999999999909);
            ps.LineTo(1361.799999999999955, 1204.399999999999864);
            ps.LineTo(1346.200000000000045, 1235.599999999999909);
            ps.LineTo(1330.599999999999909, 1220.000000000000000);
            ps.ClosePolygon();

            ps.MoveTo(1315.000000000000000, 1282.399999999999864);
            ps.LineTo(1315.000000000000000, 1329.200000000000045);
            ps.LineTo(1299.400000000000091, 1313.599999999999909);
            ps.LineTo(1283.799999999999955, 1344.799999999999955);
            ps.LineTo(1252.599999999999909, 1313.599999999999909);
            ps.LineTo(1283.799999999999955, 1298.000000000000000);
            ps.LineTo(1268.200000000000045, 1282.399999999999864);
            ps.ClosePolygon();

            ps.MoveTo(1268.200000000000045, 1266.799999999999955);
            ps.LineTo(1315.000000000000000, 1266.799999999999955);
            ps.LineTo(1315.000000000000000, 1220.000000000000000);
            ps.LineTo(1299.400000000000091, 1235.599999999999909);
            ps.LineTo(1283.799999999999955, 1204.399999999999864);
            ps.LineTo(1252.599999999999909, 1235.599999999999909);
            ps.LineTo(1283.799999999999955, 1251.200000000000045);
            ps.ClosePolygon();
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
            m_da = AggBasics.deg2rad(4.0);
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

            ShapePath.CmdAndFlags cmd;
            double x, y;
            for (; ; )
            {
                cmd = GetNextVertex(out x, out y);
                switch (cmd)
                {
                    case ShapePath.CmdAndFlags.Empty:
                        {
                            yield return new VertexData(cmd, x, y);
                            yield break;
                        }
                    default:
                        {
                            yield return new VertexData(cmd, x, y);
                        } break;
                }
            } 
        }
        public VertexStore MakeVxs()
        {
            VertexStore vxs = new VertexStore(2);
            foreach (VertexData v in this.GetVertexIter())
            {
                vxs.AddVertex(v.x, v.y, v.command);
            }
            return vxs; 
        }
        public VertexStoreSnap MakeVertexSnap()
        {
            return new VertexStoreSnap(this.MakeVxs());
        }


        public ShapePath.CmdAndFlags GetNextVertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            if (m_curr_r > m_r2)
            {
                return ShapePath.CmdAndFlags.Empty;
            }

            x = m_x + Math.Cos(m_angle) * m_curr_r;
            y = m_y + Math.Sin(m_angle) * m_curr_r;
            m_curr_r += m_dr;
            m_angle += m_da;
            if (m_start)
            {
                m_start = false;
                return ShapePath.CmdAndFlags.MoveTo;
            }
            return ShapePath.CmdAndFlags.LineTo;
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

            ShapePath.CmdAndFlags cmd;
            double x, y;

            do
            {

                cmd = snapIter.GetNextVertex(out x, out y);
                if (ShapePath.IsVertextCommand(cmd))
                {
                    ++m_points;
                }

                if (ShapePath.IsMoveTo(cmd))
                {
                    ++m_contours;
                }

            } while (cmd != ShapePath.CmdAndFlags.Empty);

            //foreach (VertexData vertexData in src.GetVertexIter())
            //{
            //    if (ShapePath.IsVertextCommand(vertexData.command))
            //    {
            //        ++m_points;
            //    }

            //    if (ShapePath.IsMoveTo(vertexData.command))
            //    {
            //        ++m_contours;
            //    }
            //}
        }
    }
}
