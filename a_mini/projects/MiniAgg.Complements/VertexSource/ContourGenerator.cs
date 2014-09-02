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


        public LineCap LineCap
        {
            get { return this.m_stroker.LineCap; }
            set { this.m_stroker.LineCap = value; }
        }
        public LineJoin LineJoin
        {
            get { return this.m_stroker.LineJoin; }
            set { this.m_stroker.LineJoin = value; }
        }
        public InnerJoin InnerJoin
        {
            get { return this.m_stroker.InnerJoin; }
            set { this.m_stroker.InnerJoin = value; }
        }
        public double MiterLimit
        {
            get { return this.m_stroker.MiterLimit; }
            set { this.m_stroker.MiterLimit = value; }
        }
        public double InnerMiterLimit
        {
            get { return this.m_stroker.InnerMiterLimit; }
            set { this.m_stroker.InnerMiterLimit = value; }
        }


        public void SetMiterLimitTheta(double t) { m_stroker.SetMiterLimitTheta(t); }



        public double Width { get { return m_stroker.Width; } set { this.m_stroker.Width = value; } }

        public double ApproximateScale
        {
            get { return this.m_stroker.ApproximateScale; }
            set { this.m_stroker.ApproximateScale = value; }
        }
        public double Shorten
        {
            get { return this.m_shorten; }
            set { this.m_shorten = value; }
        }
        public bool AutoDetectOrientation
        {
            get { return m_auto_detect; }
            set { this.m_auto_detect = value; }
        }
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
            switch ((ShapePath.FlagsAndCommand.CommandsMask) & cmd)
            {
                case ShapePath.FlagsAndCommand.CommandMoveTo:
                    m_src_vertices.ReplaceLast(new VertexDistance(x, y));
                    break;
                case ShapePath.FlagsAndCommand.CommandEndPoly:
                    m_closed = (ShapePath.GetCloseFlags(cmd) == ShapePath.FlagsAndCommand.FlagClose);
                    if (m_orientation == ShapePath.FlagsAndCommand.FlagNone)
                    {
                        m_orientation = ShapePath.GetOrientation(cmd);
                    }
                    break;
                default:

                    m_src_vertices.AddVertex(new VertexDistance(x, y));
                    break;
            }
            
        }

        // Vertex Source Interface
        public void RewindZero()
        {
            if (m_status == StrokeMath.Status.Init)
            {
                m_src_vertices.Close(true);
                if (m_auto_detect)
                {
                    if (!ShapePath.HasOrientationInfo(m_orientation))
                    {
                        m_orientation = (AggMath.calc_polygon_area(m_src_vertices) > 0.0) ?
                                        ShapePath.FlagsAndCommand.FlagCCW :
                                        ShapePath.FlagsAndCommand.FlagCW;
                    }
                }

                if (ShapePath.HasOrientationInfo(m_orientation))
                {
                    m_stroker.Width = ShapePath.IsCcw(m_orientation) ? m_width : -m_width;
                }
            }
            m_status = StrokeMath.Status.Ready;
            m_src_vertex = 0;
        }

        public ShapePath.FlagsAndCommand GetNextVertex(ref double x, ref double y)
        {
            ShapePath.FlagsAndCommand cmd = ShapePath.FlagsAndCommand.CommandLineTo;
            while (!ShapePath.IsStop(cmd))
            {
                switch (m_status)
                {
                    case StrokeMath.Status.Init:
                        this.RewindZero();
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
                        m_stroker.CreateJoin(m_out_vertices,
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
