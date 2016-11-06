//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// classes polygon_ctrl_impl, polygon_ctrl
//
//----------------------------------------------------------------------------

using PixelFarm.Drawing;
using System;
using System.Collections.Generic;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
namespace PixelFarm.Agg.UI
{
    class SimplePolygonVertexSource
    {
        double[] m_polygon;
        int m_num_points;
        int m_vertex;
        bool m_roundoff;
        bool m_close;
        public SimplePolygonVertexSource(double[] polygon, int np)
            : this(polygon, np, false, true)
        {
        }

        public SimplePolygonVertexSource(double[] polygon, int np,
                                     bool roundoff)
            : this(polygon, np, roundoff, true)
        {
        }

        public SimplePolygonVertexSource(double[] polygon, int np, bool roundoff, bool close)
        {
            m_polygon = (polygon);
            m_num_points = (np);
            m_vertex = (0);
            m_roundoff = (roundoff);
            m_close = (close);
        }

        public void Close(bool f) { m_close = f; }
        public bool Close() { return m_close; }


        VertexCmd GetNextVertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            if (m_vertex > m_num_points)
            {
                return VertexCmd.Stop;
            }
            if (m_vertex == m_num_points)
            {
                ++m_vertex;
                return m_close ? VertexCmd.CloseAndEndFigure : VertexCmd.EndFigure;
            }
            x = m_polygon[m_vertex * 2];
            y = m_polygon[m_vertex * 2 + 1];
            if (m_roundoff)
            {
                x = Math.Floor(x) + 0.5;
                y = Math.Floor(y) + 0.5;
            }
            ++m_vertex;
            return (m_vertex == 1) ? VertexCmd.MoveTo : VertexCmd.LineTo;
        }


        public VertexStore MakeVxs(VertexStore vxs)
        {
            m_vertex = 0;
            for (; ; )
            {
                double x, y;
                var cmd = this.GetNextVertex(out x, out y);
                vxs.AddVertex(x, y, cmd);
                if (cmd == VertexCmd.Stop)
                {
                    break;
                }
            }
            return vxs;
        }
        public VertexStoreSnap MakeVertexSnap(VertexStore vxs)
        {
            return new VertexStoreSnap(this.MakeVxs(vxs));
        }
    }

    public class PolygonControl : SimpleVertexSourceWidget
    {
        double[] m_polygon;
        int m_num_points;
        int m_node;
        int m_edge;
        SimplePolygonVertexSource m_vs;
        Stroke m_stroke;
        VertexSource.Ellipse m_ellipse;
        double m_point_radius;
        int m_status;
        double m_dx;
        double m_dy;
        bool m_in_polygon_check;
        bool needToRecalculateBounds = true;
        public delegate void ChangedHandler(object sender, EventArgs e);
        public event ChangedHandler Changed;
        public PolygonControl(int np, double point_radius)
            : base(new Vector2())
        {
            m_ellipse = new PixelFarm.Agg.VertexSource.Ellipse();
            m_polygon = new double[np * 2];
            m_num_points = (np);
            m_node = (-1);
            m_edge = (-1);
            m_vs = new SimplePolygonVertexSource(m_polygon, m_num_points, false);
            m_stroke = new Stroke(1);
            m_point_radius = (point_radius);
            m_status = (0);
            m_dx = (0.0);
            m_dy = (0.0);
            m_in_polygon_check = (true);
            m_stroke.Width = 1;
        }


        public double GetXN(int n) { return m_polygon[n << 1]; }
        public double GetYN(int n) { return m_polygon[(n << 1) + 1]; }

        public void SetXN(int n, double newXN) { needToRecalculateBounds = true; m_polygon[n * 2] = newXN; }
        public void AddXN(int n, double newXN) { needToRecalculateBounds = true; m_polygon[n * 2] += newXN; }


        public void SetYN(int n, double newYN) { needToRecalculateBounds = true; m_polygon[n * 2 + 1] = newYN; }
        public void AddYN(int n, double newYN) { needToRecalculateBounds = true; m_polygon[n * 2 + 1] += newYN; }

        public double[] GetInnerCoords() { return m_polygon; }


        public double LineWidth
        {
            get { return this.m_stroke.Width; }
            set { this.m_stroke.Width = value; }
        }

        public override void RewindZero()
        {
            if (needToRecalculateBounds)
            {
                RecalculateBounds();
            }
            m_status = 0;
        }

        void RecalculateBounds()
        {
            needToRecalculateBounds = false;
            return;
#if false
            double extraForControlPoints = m_point_radius * 1.3;
            RectangleDouble newBounds = new RectangleDouble(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);
            for (int i = 0; i < m_num_points; i++)
            {
                newBounds.Left = Math.Min(GetXN(i) - extraForControlPoints, newBounds.Left);
                newBounds.Right = Math.Max(GetXN(i) + extraForControlPoints, newBounds.Right);
                newBounds.Bottom = Math.Min(GetYN(i) - extraForControlPoints, newBounds.Bottom);
                newBounds.Top = Math.Max(GetYN(i) + extraForControlPoints, newBounds.Top);
            }

            Invalidate();
            LocalBounds = newBounds;
            Invalidate();
#endif
        }

        public override VertexStore MakeVxs(VertexStore vxs)
        {

            this.RewindZero();
            //this polygon control has  2 subcontrol
            //stroke and ellipse 
            var v1 = GetFreeVxs();
            var v2 = GetFreeVxs();
            VertexStore s_vxs = this.m_stroke.MakeVxs(this.m_vs.MakeVxs(v1), v2);
            int j = s_vxs.Count;
            double x, y;
            for (int i = 0; i < j; ++i)
            {
                var cmd = s_vxs.GetVertex(i, out x, out y);
                if (cmd == VertexCmd.Stop)
                {
                    break;
                }
                else
                {
                    vxs.AddVertex(x, y, cmd);
                }
            }
            ReleaseVxs(ref v1);
            ReleaseVxs(ref v2);
            //------------------------------------------------------------
            //draw each polygon point
            double r = m_point_radius;
            if (m_node >= 0 && m_node == (int)(m_status)) { r *= 1.2; }

            int n_count = m_polygon.Length / 2;
            var v3 = GetFreeVxs();
            for (int m = 0; m < n_count; ++m)
            {
                m_ellipse.Reset(GetXN(m), GetYN(m), r, r, 32);

                var ellipseVxs = m_ellipse.MakeVxs(v3);
                j = ellipseVxs.Count;
                for (int i = 0; i < j; ++i)
                {
                    var cmd = ellipseVxs.GetVertex(i, out x, out y);
                    if (cmd == VertexCmd.Stop)
                    {
                        break;
                    }
                    vxs.AddVertex(x, y, cmd);
                }
                m_status++;

                //reuse
                v3.Clear();
            }
            ReleaseVxs(ref v3);
            //------------------------------------------------------------

            //close with stop
            vxs.AddVertex(0, 0, VertexCmd.Stop);
            return vxs;
        }
        protected override RectD CalculateLocalBounds()
        {
            RectD localBounds = new RectD(double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity);
            this.RewindZero();

            var v1 = GetFreeVxs();
            this.MakeVxs(v1);
            int j = v1.Count;
            for (int i = 0; i < j; ++i)
            {
                double x, y;
                v1.GetVertexXY(i, out x, out y);
                localBounds.ExpandToInclude(x, y);
            }
            ReleaseVxs(ref v1);
            return localBounds;
            throw new NotImplementedException();
        }


        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            bool ret = false;
            m_node = -1;
            m_edge = -1;
            double x = mouseEvent.X;
            double y = mouseEvent.Y;
            ParentToChildTransform.InverseTransform(ref x, ref y);
            for (int i = 0; i < m_num_points; i++)
            {
                //check if the testpoint is in the area of a control point.
                //if our control point is circle ... the computation is expansive than square shape control point***
                if (Math.Sqrt((x - GetXN(i)) * (x - GetXN(i)) + (y - GetYN(i)) * (y - GetYN(i))) < m_point_radius)
                {
                    m_dx = x - GetXN(i);
                    m_dy = y - GetYN(i);
                    m_node = (int)(i);
                    ret = true;
                    break;
                }
            }

            //check if on edge 
            if (!ret)
            {
                for (int i = 0; i < m_num_points; i++)
                {
                    if (CheckEdge(i, x, y))
                    {
                        m_dx = x;
                        m_dy = y;
                        m_edge = (int)(i);
                        ret = true;
                        break;
                    }
                }
            }

            if (!ret)
            {
                if (IsPointInPolygon(x, y))
                {
                    m_dx = x;
                    m_dy = y;
                    m_node = (int)(m_num_points);
                    ret = true;
                }
            }

            Invalidate();
            base.OnMouseDown(mouseEvent);
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            bool ret = (m_node >= 0) || (m_edge >= 0);
            m_node = -1;
            m_edge = -1;
            Invalidate();
            base.OnMouseUp(mouseEvent);
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            bool handled = false;
            double dx;
            double dy;
            double x = mouseEvent.X;
            double y = mouseEvent.Y;
            ParentToChildTransform.InverseTransform(ref x, ref y);
            if (m_node == (int)(m_num_points))
            {
                dx = x - m_dx;
                dy = y - m_dy;
                for (int i = 0; i < m_num_points; i++)
                {
                    SetXN(i, GetXN(i) + dx);
                    SetYN(i, GetYN(i) + dy);
                }
                m_dx = x;
                m_dy = y;
                handled = true;
            }
            else
            {
                if (m_edge >= 0)
                {
                    int n1 = (int)m_edge;
                    int n2 = (n1 + m_num_points - 1) % m_num_points;
                    dx = x - m_dx;
                    dy = y - m_dy;
                    SetXN(n1, GetXN(n1) + dx);
                    SetYN(n1, GetYN(n1) + dy);
                    SetXN(n2, GetXN(n2) + dx);
                    SetYN(n2, GetYN(n2) + dy);
                    m_dx = x;
                    m_dy = y;
                    handled = true;
                }
                else
                {
                    if (m_node >= 0)
                    {
                        SetXN((int)m_node, x - m_dx);
                        SetYN((int)m_node, y - m_dy);
                        handled = true;
                    }
                }
            }

            // TODO: set bounds correctly and invalidate
            if (handled)
            {
                if (Changed != null)
                {
                    Changed(this, null);
                }
                RecalculateBounds();
            }

            base.OnMouseMove(mouseEvent);
        }



        bool CheckEdge(int i, double x, double y)
        {
            bool ret = false;
            int n1 = i;
            int n2 = (i + m_num_points - 1) % m_num_points;
            double x1 = GetXN(n1);
            double y1 = GetYN(n1);
            double x2 = GetXN(n2);
            double y2 = GetYN(n2);
            double dx = x2 - x1;
            double dy = y2 - y1;
            if (Math.Sqrt(dx * dx + dy * dy) > 0.0000001)
            {
                double x3 = x;
                double y3 = y;
                double x4 = x3 - dy;
                double y4 = y3 + dx;
                double den = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
                double u1 = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / den;
                double xi = x1 + u1 * (x2 - x1);
                double yi = y1 + u1 * (y2 - y1);
                dx = xi - x;
                dy = yi - y;
                if (u1 > 0.0 && u1 < 1.0 && Math.Sqrt(dx * dx + dy * dy) <= m_point_radius)
                {
                    ret = true;
                }
            }
            return ret;
        }

        //======= Crossings Multiply algorithm of InsideTest ======================== 
        //
        // By Eric Haines, 3D/Eye Inc, erich@eye.com
        //
        // This version is usually somewhat faster than the original published in
        // Graphics Gems IV; by turning the division for testing the X axis crossing
        // into a tricky multiplication test this part of the test became faster,
        // which had the additional effect of making the test for "both to left or
        // both to right" a bit slower for triangles than simply computing the
        // intersection each time.  The main increase is in triangle testing speed,
        // which was about 15% faster; all other polygon complexities were pretty much
        // the same as before.  On machines where division is very expensive (not the
        // case on the HP 9000 series on which I tested) this test should be much
        // faster overall than the old code.  Your mileage may (in fact, will) vary,
        // depending on the machine and the test data, but in general I believe this
        // code is both shorter and faster.  This test was inspired by unpublished
        // Graphics Gems submitted by Joseph Samosky and Mark Haigh-Hutchinson.
        // Related work by Samosky is in:
        //
        // Samosky, Joseph, "SectionView: A system for interactively specifying and
        // visualizing sections through three-dimensional medical image data",
        // M.S. Thesis, Department of Electrical Engineering and Computer Science,
        // Massachusetts Institute of Technology, 1993.
        //
        // Shoot a test ray along +X axis.  The strategy is to compare vertex Y values
        // to the testing point's Y and quickly discard edges which are entirely to one
        // side of the test ray.  Note that CONVEX and WINDING code can be added as
        // for the CrossingsTest() code; it is left out here for clarity.
        //
        // Input 2D polygon _pgon_ with _numverts_ number of vertices and test point
        // _point_, returns 1 if inside, 0 if outside.
        bool IsPointInPolygon(double tx, double ty)
        {
            if (m_num_points < 3) return false;
            if (!m_in_polygon_check) return false;
            int j;
            bool yflag0, yflag1, inside_flag;
            double vtx0, vty0, vtx1, vty1;
            vtx0 = GetXN(m_num_points - 1);
            vty0 = GetYN(m_num_points - 1);
            // get test bit for above/below X axis
            yflag0 = (vty0 >= ty);
            vtx1 = GetXN(0);
            vty1 = GetYN(0);
            inside_flag = false;
            for (j = 1; j <= m_num_points; ++j)
            {
                yflag1 = (vty1 >= ty);
                // Check if endpoints straddle (are on opposite sides) of X axis
                // (i.e. the Y's differ); if so, +X ray could intersect this edge.
                // The old test also checked whether the endpoints are both to the
                // right or to the left of the test point.  However, given the faster
                // intersection point computation used below, this test was found to
                // be a break-even proposition for most polygons and a loser for
                // triangles (where 50% or more of the edges which survive this test
                // will cross quadrants and so have to have the X intersection computed
                // anyway).  I credit Joseph Samosky with inspiring me to try dropping
                // the "both left or both right" part of my code.
                if (yflag0 != yflag1)
                {
                    // Check intersection of pgon segment with +X ray.
                    // Note if >= point's X; if so, the ray hits it.
                    // The division operation is avoided for the ">=" test by checking
                    // the sign of the first vertex wrto the test point; idea inspired
                    // by Joseph Samosky's and Mark Haigh-Hutchinson's different
                    // polygon inclusion tests.
                    if (((vty1 - ty) * (vtx0 - vtx1) >=
                          (vtx1 - tx) * (vty0 - vty1)) == yflag1)
                    {
                        inside_flag = !inside_flag;
                    }
                }

                // Move to the next pair of vertices, retaining info as possible.
                yflag0 = yflag1;
                vtx0 = vtx1;
                vty0 = vty1;
                int k = (j >= m_num_points) ? j - m_num_points : j;
                vtx1 = GetXN(k);
                vty1 = GetYN(k);
            }
            return inside_flag;
        }
    };
    //----------------------------------------------------------polygon_ctrl
    //template<class ColorT> 
    public class PolygonEditWidget : PolygonControl
    {
        Color m_color;
        public PolygonEditWidget(int np) : this(np, 5) { }

        public PolygonEditWidget(int np, double point_radius)
            : base(np, point_radius)
        {
            //m_color = new ColorRGBAf(0.0, 0.0, 0.0);
            m_color = Color.Black;
        }
        public Color LineColor
        {
            get { return m_color; }
            set { this.m_color = value; }
        }

        public override void OnDraw(CanvasPainter p)
        {
            p.FillColor = LineColor;
            var v1 = GetFreeVxs();
            p.Draw(new VertexStoreSnap(this.MakeVxs(v1)));
            ReleaseVxs(ref v1);
        }
    }
}
