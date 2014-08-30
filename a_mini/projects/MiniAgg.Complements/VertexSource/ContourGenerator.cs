using System;
using System.Collections.Generic;

using System.Text;

using MatterHackers.VectorMath;

namespace MatterHackers.Agg.VertexSource
{
    class ContourGenerator : IGenerator
    {
        StrokeMath m_stroker;
        double m_width;
        VertexSequence m_src_vertices;
        Vector2Container m_out_vertices;
        StrokeMath.Status m_status;
        int m_src_vertex;
        int m_out_vertex;
        bool m_closed;
        ShapePath.FlagsAndCommand m_orientation;
        bool m_auto_detect;
        double m_shorten;

        public ContourGenerator()
        {
            m_stroker = new StrokeMath();
            m_width = 1;
            m_src_vertices = new VertexSequence();
            m_out_vertices = new Vector2Container();
            m_status = StrokeMath.Status.Init;
            m_src_vertex = 0;
            m_closed = false;
            m_orientation = 0;
            m_auto_detect = false;
        }

        public void line_cap(LineCap lc) { m_stroker.line_cap(lc); }
        public void line_join(LineJoin lj) { m_stroker.line_join(lj); }
        public void inner_join(InnerJoin ij) { m_stroker.inner_join(ij); }

        public LineCap line_cap() { return m_stroker.line_cap(); }
        public LineJoin line_join() { return m_stroker.line_join(); }
        public InnerJoin inner_join() { return m_stroker.inner_join(); }

        public void width(double w) { m_stroker.width(w); }
        public void miter_limit(double ml) { m_stroker.miter_limit(ml); }
        public void miter_limit_theta(double t) { m_stroker.miter_limit_theta(t); }
        public void inner_miter_limit(double ml) { m_stroker.inner_miter_limit(ml); }
        public void approximation_scale(double approx_scale) { m_stroker.approximation_scale(approx_scale); }

        public double width() { return m_stroker.width(); }
        public double miter_limit() { return m_stroker.miter_limit(); }
        public double inner_miter_limit() { return m_stroker.inner_miter_limit(); }
        public double approximation_scale() { return m_stroker.approximation_scale(); }

        public void shorten(double s) { m_shorten = s; }
        public double shorten() { return m_shorten; }

        public void auto_detect_orientation(bool v) { m_auto_detect = v; }
        public bool auto_detect_orientation() { return m_auto_detect; }

        // Generator interface
        public void RemoveAll()
        {
            m_src_vertices.Clear();
            m_closed = false;
            m_status = StrokeMath.Status.Init;
        }

        public void AddVertex(double x, double y, ShapePath.FlagsAndCommand cmd)
        {
            m_status = StrokeMath.Status.Init;
            if (ShapePath.is_move_to(cmd))
            {
                m_src_vertices.modify_last(new VertexDistance(x, y));
            }
            else
            {
                if (ShapePath.IsVertextCommand(cmd))
                {
                    m_src_vertices.AddVertex(new VertexDistance(x, y));
                }
                else
                {
                    if (ShapePath.is_end_poly(cmd))
                    {
                        m_closed = (ShapePath.get_close_flag(cmd) == ShapePath.FlagsAndCommand.FlagClose);
                        if (m_orientation == ShapePath.FlagsAndCommand.FlagNone)
                        {
                            m_orientation = ShapePath.get_orientation(cmd);
                        }
                    }
                }
            }
        }

        // Vertex Source Interface
        public void Rewind(int idx)
        {
            if (m_status == StrokeMath.Status.Init)
            {
                m_src_vertices.close(true);
                if (m_auto_detect)
                {
                    if (!ShapePath.is_oriented(m_orientation))
                    {
                        m_orientation = (AggMath.calc_polygon_area(m_src_vertices) > 0.0) ?
                                        ShapePath.FlagsAndCommand.FlagCCW :
                                        ShapePath.FlagsAndCommand.FlagCW;
                    }
                }
                if (ShapePath.is_oriented(m_orientation))
                {
                    m_stroker.width(ShapePath.is_ccw(m_orientation) ? m_width : -m_width);
                }
            }
            m_status = StrokeMath.Status.Ready;
            m_src_vertex = 0;
        }

        public ShapePath.FlagsAndCommand Vertex(ref double x, ref double y)
        {
            ShapePath.FlagsAndCommand cmd = ShapePath.FlagsAndCommand.CommandLineTo;
            while (!ShapePath.is_stop(cmd))
            {
                switch (m_status)
                {
                    case StrokeMath.Status.Init:
                        Rewind(0);
                        goto case StrokeMath.Status.Ready;

                    case StrokeMath.Status.Ready:
                        if (m_src_vertices.Count < 2 + (m_closed ? 1 : 0))
                        {
                            cmd = ShapePath.FlagsAndCommand.CommandStop;
                            break;
                        }
                        m_status = StrokeMath.Status.Outline1;
                        cmd = ShapePath.FlagsAndCommand.CommandMoveTo;
                        m_src_vertex = 0;
                        m_out_vertex = 0;
                        goto case StrokeMath.Status.Outline1;

                    case StrokeMath.Status.Outline1:
                        if (m_src_vertex >= m_src_vertices.Count)
                        {
                            m_status = StrokeMath.Status.EndPoly1;
                            break;
                        }
                        m_stroker.calc_join(m_out_vertices,
                                            m_src_vertices.prev(m_src_vertex),
                                            m_src_vertices.curr(m_src_vertex),
                                            m_src_vertices.next(m_src_vertex),
                                            m_src_vertices.prev(m_src_vertex).dist,
                                            m_src_vertices.curr(m_src_vertex).dist);
                        ++m_src_vertex;
                        m_status = StrokeMath.Status.OutVertices;
                        m_out_vertex = 0;
                        goto case StrokeMath.Status.OutVertices;

                    case StrokeMath.Status.OutVertices:
                        if (m_out_vertex >= m_out_vertices.Count)
                        {
                            m_status = StrokeMath.Status.Outline1;
                        }
                        else
                        {
                            Vector2 c = m_out_vertices[m_out_vertex++];
                            x = c.x;
                            y = c.y;
                            return cmd;
                        }
                        break;

                    case StrokeMath.Status.EndPoly1:
                        if (!m_closed) return ShapePath.FlagsAndCommand.CommandStop;
                        m_status = StrokeMath.Status.Stop;
                        return ShapePath.FlagsAndCommand.CommandEndPoly | ShapePath.FlagsAndCommand.FlagClose | ShapePath.FlagsAndCommand.FlagCCW;

                    case StrokeMath.Status.Stop:
                        return ShapePath.FlagsAndCommand.CommandStop;
                }
            }
            return cmd;
        }
    }
}
