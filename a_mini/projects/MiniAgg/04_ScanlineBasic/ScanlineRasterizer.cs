//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// The author gratefully acknowleges the support of David Turner, 
// Robert Wilhelm, and Werner Lemberg - the authors of the FreeType 
// libray - in producing this work. See http://www.freetype.org for details.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Adaptation for 32-bit screen coordinates has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
using System;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;
using filling_rule_e = MatterHackers.Agg.FillingRule;
using status = MatterHackers.Agg.ScanlineRasterizer.Status;
using poly_subpixel_scale_e = MatterHackers.Agg.AggBasics.PolySubPixelScale;

namespace MatterHackers.Agg
{
    //==================================================rasterizer_scanline_aa
    // Polygon rasterizer that is used to render filled polygons with 
    // high-quality Anti-Aliasing. Internally, by default, the class uses 
    // integer coordinates in format 24.8, i.e. 24 bits for integer part 
    // and 8 bits for fractional - see poly_subpixel_shift. This class can be 
    // used in the following  way:
    //
    // 1. filling_rule(filling_rule_e ft) - optional.
    //
    // 2. gamma() - optional.
    //
    // 3. reset()
    //
    // 4. move_to(x, y) / line_to(x, y) - make the polygon. One can create 
    //    more than one contour, but each contour must consist of at least 3
    //    vertices, i.e. move_to(x1, y1); line_to(x2, y2); line_to(x3, y3);
    //    is the absolute minimum of vertices that define a triangle.
    //    The algorithm does not check either the number of vertices nor
    //    coincidence of their coordinates, but in the worst case it just 
    //    won't draw anything.
    //    The order of the vertices (clockwise or counterclockwise) 
    //    is important when using the non-zero filling rule (fill_non_zero).
    //    In this case the vertex order of all the contours must be the same
    //    if you want your intersecting polygons to be without "holes".
    //    You actually can use different vertices order. If the contours do not 
    //    intersect each other the order is not important anyway. If they do, 
    //    contours with the same vertex order will be rendered without "holes" 
    //    while the intersecting contours with different orders will have "holes".
    //
    // filling_rule() and gamma() can be called anytime before "sweeping".
    //------------------------------------------------------------------------
    public interface IRasterizer
    {

        int MinX { get; }
        int MinY { get; }
        int MaxX { get; }
        int MaxY { get; }

        void ResetGamma(IGammaFunction gamma_function);

        bool SweepScanline(IScanline sl);
        void Reset(); 
        void AddPath(SinglePath spath);
        bool RewindScanlines();
    }

    public sealed class ScanlineRasterizer : IRasterizer
    {
        CellAARasterizer m_outline;
        VectorClipper m_VectorClipper;

        int[] m_gammaLut = new int[AA_SCALE];

        FillingRule m_filling_rule;
        bool m_auto_close;
        int m_start_x;
        int m_start_y;
        Status m_status;
        int m_scan_y;


        //---------------------------
        const int AA_SHIFT = 8;
        const int AA_SCALE = 1 << AA_SHIFT;
        const int AA_MASK = AA_SCALE - 1;
        const int AA_SCALE2 = AA_SCALE * 2;
        const int AA_MASK2 = AA_SCALE2 - 1;

        //---------------------------

        public enum Status
        {
            Initial,
            MoveTo,
            LineTo,
            Closed
        }


        public ScanlineRasterizer()
            : this(new VectorClipper())
        {
        }
        //--------------------------------------------------------------------
        ScanlineRasterizer(VectorClipper rasterizer_sl_clip)
        {
            m_outline = new CellAARasterizer();
            m_VectorClipper = rasterizer_sl_clip;
            m_filling_rule = FillingRule.NonZero;
            m_auto_close = true;
            m_start_x = 0;
            m_start_y = 0;
            m_status = Status.Initial;

            for (int i = AA_SCALE - 1; i >= 0; --i)
            {
                m_gammaLut[i] = i;
            }
        }

        //--------------------------------------------------------------------
        public void Reset()
        {
            m_outline.Reset();
            m_status = Status.Initial;
        }

        public void ResetClipping()
        {
            Reset();
            m_VectorClipper.ResetClipping();
        }

        public RectangleDouble GetVectorClipBox()
        {
            return m_VectorClipper.GetVectorClipBox();
        }

        public void SetVectorClipBox(RectangleDouble clippingRect)
        {
            SetVectorClipBox(clippingRect.Left, clippingRect.Bottom, clippingRect.Right, clippingRect.Top);
        }

        public void SetVectorClipBox(double x1, double y1, double x2, double y2)
        {
            Reset();
            m_VectorClipper.ClipBox(
                                upscale(x1), upscale(y1),
                                upscale(x2), upscale(y2));
        }
        //---------------------------------
        //from vector clipper
        static int upscale(double v)
        {
            //m_VectorClipper.upscale
            return AggBasics.iround(v * (int)poly_subpixel_scale_e.SCALE);
        }
        //from vector clipper
        static int downscale(int v)
        {
            //  m_VectorClipper.downscale
            return v / (int)poly_subpixel_scale_e.SCALE;
        }
        //---------------------------------
        FillingRule FillingRule
        {
            get { return this.m_filling_rule; }
            set { this.m_filling_rule = value; }
        }
        bool AutoClose
        {
            get { return m_auto_close; }
            set { this.m_auto_close = value; }
        }
        //--------------------------------------------------------------------
        public void ResetGamma(IGammaFunction gamma_function)
        {
            for (int i = AA_SCALE - 1; i >= 0; --i)
            {
                m_gammaLut[i] = (int)AggBasics.uround(
                    gamma_function.GetGamma((double)(i) / AA_MASK) * AA_MASK);
            }
        }
        //--------------------------------------------------------------------
        void MoveTo(int x, int y)
        {
            if (m_outline.Sorted) Reset();
            if (m_auto_close) ClosePolygon();
            m_VectorClipper.MoveTo(m_start_x = downscale(x),
                              m_start_y = downscale(y));
            m_status = Status.MoveTo;
        }

        //------------------------------------------------------------------------
        void LineTo(int x, int y)
        {
            m_VectorClipper.LineTo(m_outline,
                             downscale(x),
                             downscale(y));
            m_status = Status.LineTo;
        }

        //------------------------------------------------------------------------
        public void MoveTo(double x, double y)
        {
            if (m_outline.Sorted) Reset();
            if (m_auto_close) ClosePolygon();
            m_VectorClipper.MoveTo(m_start_x = upscale(x),
                              m_start_y = upscale(y));
            m_status = Status.MoveTo;
        }

        //------------------------------------------------------------------------
        public void LineTo(double x, double y)
        {
            m_VectorClipper.LineTo(m_outline,
                              upscale(x),
                              upscale(y));
            m_status = Status.LineTo;
        }

        void ClosePolygon()
        {
            if (m_status == Status.LineTo)
            {
                m_VectorClipper.LineTo(m_outline, m_start_x, m_start_y);
                m_status = Status.Closed;
            }
        }

        void AddVertex(ShapePath.FlagsAndCommand cmd, double x, double y)
        {
            switch (cmd)
            {
                case ShapePath.FlagsAndCommand.CommandMoveTo:
                    {
                        MoveTo(x, y);
                    } break;
                case ShapePath.FlagsAndCommand.CommandLineTo:
                case ShapePath.FlagsAndCommand.CommandCurve3:
                case ShapePath.FlagsAndCommand.CommandCurve4:
                    {
                        LineTo(x, y);
                    } break;
                default:
                    {
                        if (ShapePath.IsClose(cmd))
                        {
                            ClosePolygon();
                        }
                    } break;
            }

        }
        //------------------------------------------------------------------------
        void EdgeInt32(int x1, int y1, int x2, int y2)
        {
            if (m_outline.Sorted) Reset();
            m_VectorClipper.MoveTo(downscale(x1), downscale(y1));
            m_VectorClipper.LineTo(m_outline,
                              downscale(x2),
                               downscale(y2));
            m_status = Status.MoveTo;
        }

        //------------------------------------------------------------------------
        void Edge(double x1, double y1, double x2, double y2)
        {
            if (m_outline.Sorted) Reset();
            m_VectorClipper.MoveTo(upscale(x1), upscale(y1));
            m_VectorClipper.LineTo(m_outline,
                              upscale(x2),
                              upscale(y2));
            m_status = Status.MoveTo;
        }
        public void AddPath(IVertexSource vs)
        {
            this.AddPath(vs.MakeSinglePath());
        }


        //public void AddPath(IVertexSource vs)
        //{
        //    double x = 0;
        //    double y = 0;

        //    ShapePath.FlagsAndCommand cmd;
        //    vs.RewindZero();
        //    if (m_outline.Sorted)
        //    {
        //        Reset();
        //    }
        //    while ((cmd = vs.GetNextVertex(out x, out y)) != ShapePath.FlagsAndCommand.CommandStop)
        //    {
        //        AddVertex(cmd, x, y);
        //    }
        //}
        //-------------------------------------------------------------------
        public void AddPath(VertexStorage vxs)
        {
            this.AddPath(new SinglePath(vxs));
        }
        public void AddPath(SinglePath spath)
        {

            double x = 0;
            double y = 0;

            ShapePath.FlagsAndCommand cmd;
            spath.RewindZero();
            if (m_outline.Sorted)
            {
                Reset();
            }
            if (spath.VxsHasMoreThanOnePart)
            {
                var vxs = spath.MakeVxs();
                int j = vxs.Count;

                for (int i = 0; i < j; ++i)
                {
                    var cmd2 = vxs.GetVertex(i, out x, out y);
                    if (cmd2 != ShapePath.FlagsAndCommand.CommandStop)
                    {
                        AddVertex(cmd2, x, y);
                    } 
                }
            }
            else
            {
                while ((cmd = spath.GetNextVertex(out x, out y)) != ShapePath.FlagsAndCommand.CommandStop)
                {

                    AddVertex(cmd, x, y);
                }
            }
        }

        public int MinX { get { return m_outline.MinX; } }
        public int MinY { get { return m_outline.MinY; } }
        public int MaxX { get { return m_outline.MaxX; } }
        public int MaxY { get { return m_outline.MaxY; } }


        //--------------------------------------------------------------------
        void Sort()
        {
            if (m_auto_close) { ClosePolygon(); }

            m_outline.SortCells();
        }

        //------------------------------------------------------------------------
        public bool RewindScanlines()
        {
            if (m_auto_close) ClosePolygon();

            m_outline.SortCells();

            if (m_outline.TotalCells == 0) return false;

            m_scan_y = m_outline.MinY;
            return true;
        }

        //------------------------------------------------------------------------
        bool NavigateScanline(int y)
        {
            if (m_auto_close) ClosePolygon();

            m_outline.SortCells();

            if (m_outline.TotalCells == 0 ||
               y < m_outline.MinY ||
               y > m_outline.MinY)
            {
                return false;
            }
            m_scan_y = y;
            return true;
        }

        //--------------------------------------------------------------------
        int CalculateAlpha(int area)
        {
            int cover = area >> (poly_subpixel_scale_e.SHIFT * 2 + 1 - AA_SHIFT);

            if (cover < 0)
            {
                cover = -cover;
            }

            if (m_filling_rule == FillingRule.EvenOdd)
            {
                cover &= AA_SCALE2;
                if (cover > AA_SCALE)
                {
                    cover = AA_SCALE2 - cover;
                }
            }

            if (cover > AA_MASK)
            {
                cover = AA_MASK;
            }

            return m_gammaLut[cover];
        }

        //--------------------------------------------------------------------
        public bool SweepScanline(IScanline scline)
        {
            for (; ; )
            {
                if (m_scan_y > m_outline.MaxY)
                {
                    return false;
                }

                scline.ResetSpans();

                CellAA[] cells;
                int offset;
                int num_cells;

                m_outline.GetCells(m_scan_y, out cells, out offset, out num_cells);

                int cover = 0;

                while (num_cells != 0)
                {
                    CellAA cur_cell = cells[offset];
                    int x = cur_cell.x;
                    int area = cur_cell.area;
                    int alpha;

                    cover += cur_cell.cover;

                    //accumulate all cells with the same X
                    while (--num_cells != 0)
                    {
                        offset++;
                        cur_cell = cells[offset];
                        if (cur_cell.x != x)
                        {
                            break;
                        }

                        area += cur_cell.area;
                        cover += cur_cell.cover;
                    }

                    if (area != 0)
                    {
                        alpha = CalculateAlpha((cover << (poly_subpixel_scale_e.SHIFT + 1)) - area);
                        if (alpha != 0)
                        {
                            scline.AddCell(x, alpha);
                        }
                        x++;
                    }

                    if ((num_cells != 0) && (cur_cell.x > x))
                    {
                        alpha = CalculateAlpha(cover << (poly_subpixel_scale_e.SHIFT + 1));
                        if (alpha != 0)
                        {
                            scline.AddSpan(x, (cur_cell.x - x), alpha);
                        }
                    }
                }

                if (scline.SpanCount != 0) break;

                ++m_scan_y;
            }

            scline.CloseLine(m_scan_y);
            ++m_scan_y;
            return true;
        }

        //--------------------------------------------------------------------
        bool HitTest(int tx, int ty)
        {
            if (!NavigateScanline(ty)) return false;
            //scanline_hit_test sl(tx);
            //sweep_scanline(sl);
            //return sl.hit();
            return true;
        }
    };
}

