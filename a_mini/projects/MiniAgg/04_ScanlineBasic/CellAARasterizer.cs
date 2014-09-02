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
using poly_subpixel_scale_e = MatterHackers.Agg.AggBasics.PolySubPixelScale;

namespace MatterHackers.Agg
{
    //-----------------------------------------------------------------cell_aa
    // A pixel cell. There're no constructors defined and it was done ***
    // intentionally in order to avoid extra overhead when allocating an ****
    // array of cells. ***
    struct CellAA
    {
        public int x;
        public int y;
        public int cover;
        public int area;
        public int left;
        public int right;

        public void initial()
        {
            x = 0x7FFFFFFF;
            y = 0x7FFFFFFF;
            cover = 0;
            area = 0;
            left = -1;
            right = -1;
        }

        public void CopyAllFrom(CellAA cellB)
        {
            x = cellB.x;
            y = cellB.y;
            cover = cellB.cover;
            area = cellB.area;
            left = cellB.left;
            right = cellB.right;
        }

        //public void CopyLeftRightFrom(CellAA cellB)
        //{
        //    left = cellB.left;
        //    right = cellB.right;
        //}

        //public bool NotEqual(int ex, int ey, CellAA cell)
        //{
        //    unchecked
        //    {
        //        return ((ex - x) | (ey - y) | (left - cell.left) | (right - cell.right)) != 0;
        //    }
        //}
#if DEBUG
        public override string ToString()
        {
            return "x:" + x + ",y:" + y + ",cover:" + cover + ",area:" + area + ",left:" + left + ",right:" + right;
        }
#endif

    }


    //-----------------------------------------------------rasterizer_cells_aa
    // An internal class that implements the main rasterization algorithm.
    // Used in the rasterizer. Should not be used directly.
    sealed class CellAARasterizer
    {
        int m_num_used_cells;
        ArrayList<CellAA> m_cells;
        ArrayList<CellAA> m_sorted_cells;
        ArrayList<SortedY> m_sorted_y;

        //current cell 
        CellAA cCell;


        int m_min_x;
        int m_min_y;
        int m_max_x;
        int m_max_y;
        bool m_sorted;


        const int BLOCK_SHIFT = 12;
        const int BLOCK_SIZE = 1 << BLOCK_SHIFT;
        const int BLOCK_MASK = BLOCK_SIZE - 1;
        const int BLOCK_POOL = 256;
        const int BLOCK_LIMIT = BLOCK_SIZE * 1024;


        struct SortedY
        {
            internal int start;
            internal int num;
        }

        public CellAARasterizer()
        {

            m_sorted_cells = new ArrayList<CellAA>();
            m_sorted_y = new ArrayList<SortedY>();
            m_min_x = (0x7FFFFFFF);
            m_min_y = (0x7FFFFFFF);
            m_max_x = (-0x7FFFFFFF);
            m_max_y = (-0x7FFFFFFF);
            m_sorted = (false);

            //m_style_cell.initial();
            cCell.initial();
        }

        public void Reset()
        {
            m_num_used_cells = 0;

            cCell.initial();
            //m_style_cell.initial();
            m_sorted = false;
            m_min_x = 0x7FFFFFFF;
            m_min_y = 0x7FFFFFFF;
            m_max_x = -0x7FFFFFFF;
            m_max_y = -0x7FFFFFFF;
        }

        //public void SetStyleCell(CellAA style_cell)
        //{
        //    m_style_cell.CopyLeftRightFrom(style_cell);
        //}


        const int DX_LIMIT = (16384 << AggBasics.PolySubPixelScale.SHIFT);
        const int POLY_SUBPIXEL_SHIFT = AggBasics.PolySubPixelScale.SHIFT;
        const int POLY_SUBPIXEL_MASK = AggBasics.PolySubPixelScale.MASK;
        const int POLY_SUBPIXEL_SCALE = AggBasics.PolySubPixelScale.SCALE;

        public void DrawLine(int x1, int y1, int x2, int y2)
        {

            int dx = x2 - x1;

            if (dx >= DX_LIMIT || dx <= -DX_LIMIT)
            {
                int cx = (x1 + x2) >> 1;
                int cy = (y1 + y2) >> 1;
                DrawLine(x1, y1, cx, cy);
                DrawLine(cx, cy, x2, y2);
            }

            int dy = y2 - y1;
            int ex1 = x1 >> POLY_SUBPIXEL_SHIFT;
            int ex2 = x2 >> POLY_SUBPIXEL_SHIFT;
            int ey1 = y1 >> POLY_SUBPIXEL_SHIFT;
            int ey2 = y2 >> POLY_SUBPIXEL_SHIFT;
            int fy1 = y1 & POLY_SUBPIXEL_MASK;
            int fy2 = y2 & POLY_SUBPIXEL_MASK;

            int x_from, x_to;
            int p, rem, mod, lift, delta, first, incr;

            if (ex1 < m_min_x) m_min_x = ex1;
            if (ex1 > m_max_x) m_max_x = ex1;
            if (ey1 < m_min_y) m_min_y = ey1;
            if (ey1 > m_max_y) m_max_y = ey1;
            if (ex2 < m_min_x) m_min_x = ex2;
            if (ex2 > m_max_x) m_max_x = ex2;
            if (ey2 < m_min_y) m_min_y = ey2;
            if (ey2 > m_max_y) m_max_y = ey2;

            AddNewCell(ex1, ey1);

            //everything is on a single horizontal line
            if (ey1 == ey2)
            {
                RenderHLine(ey1, x1, fy1, x2, fy2);
                return;
            }

            //Vertical line - we have to calculate start and end cells,
            //and then - the common values of the area and coverage for
            //all cells of the line. We know exactly there's only one 
            //cell, so, we don't have to call render_hline().
            incr = 1;
            if (dx == 0)
            {
                int ex = x1 >> POLY_SUBPIXEL_SHIFT;
                int two_fx = (x1 - (ex << POLY_SUBPIXEL_SHIFT)) << 1;
                int area;

                first = POLY_SUBPIXEL_SCALE;
                if (dy < 0)
                {
                    first = 0;
                    incr = -1;
                }

                x_from = x1;

                delta = first - fy1;
                cCell.cover += delta;
                cCell.area += two_fx * delta;

                ey1 += incr;
                AddNewCell(ex, ey1);

                delta = first + first - POLY_SUBPIXEL_SCALE;
                area = two_fx * delta;
                while (ey1 != ey2)
                {
                    cCell.cover = delta;
                    cCell.area = area;
                    ey1 += incr;
                    AddNewCell(ex, ey1);
                }
                delta = fy2 - POLY_SUBPIXEL_SCALE + first;
                cCell.cover += delta;
                cCell.area += two_fx * delta;
                return;
            }

            //ok, we have to render several hlines
            p = (POLY_SUBPIXEL_SCALE - fy1) * dx;
            first = POLY_SUBPIXEL_SCALE;

            if (dy < 0)
            {
                p = fy1 * dx;
                first = 0;
                incr = -1;
                dy = -dy;
            }

            delta = p / dy;
            mod = p % dy;

            if (mod < 0)
            {
                delta--;
                mod += dy;
            }

            x_from = x1 + delta;
            RenderHLine(ey1, x1, fy1, x_from, first);

            ey1 += incr;
            AddNewCell(x_from >> POLY_SUBPIXEL_SHIFT, ey1);

            if (ey1 != ey2)
            {
                p = POLY_SUBPIXEL_SCALE * dx;
                lift = p / dy;
                rem = p % dy;

                if (rem < 0)
                {
                    lift--;
                    rem += dy;
                }
                mod -= dy;

                while (ey1 != ey2)
                {
                    delta = lift;
                    mod += rem;
                    if (mod >= 0)
                    {
                        mod -= dy;
                        delta++;
                    }

                    x_to = x_from + delta;
                    RenderHLine(ey1, x_from, POLY_SUBPIXEL_SCALE - first, x_to, first);
                    x_from = x_to;

                    ey1 += incr;
                    AddNewCell(x_from >> POLY_SUBPIXEL_SHIFT, ey1);
                }
            }
            RenderHLine(ey1, x_from, POLY_SUBPIXEL_SCALE - first, x2, fy2);
        }



        public int MinX { get { return m_min_x; } }
        public int MinY { get { return m_min_y; } }
        public int MaxX { get { return m_max_x; } }
        public int MaxY { get { return m_max_y; } }

        public void SortCells()
        {
            if (m_sorted) return; //Perform sort only the first time.

            WriteCurrentCell();
            //----------------------------------
            //reset current cell 
            cCell.x = 0x7FFFFFFF;
            cCell.y = 0x7FFFFFFF;
            cCell.cover = 0;
            cCell.area = 0;
            //----------------------------------

            if (m_num_used_cells == 0) return;

            // Allocate the array of cell pointers 
            m_sorted_cells.Allocate(m_num_used_cells);

            // Allocate and zero the Y array
            m_sorted_y.Allocate((int)(m_max_y - m_min_y + 1));
            m_sorted_y.Zero();

            CellAA[] cells = m_cells.Array;
            SortedY[] sortedYData = m_sorted_y.Array;
            CellAA[] sortedCellsData = m_sorted_cells.Array;

            // Create the Y-histogram (count the numbers of cells for each Y)
            for (int i = 0; i < m_num_used_cells; ++i)
            {
                int index = cells[i].y - m_min_y;
                sortedYData[index].start++;
            }

            // Convert the Y-histogram into the array of starting indexes
            int start = 0;
            int sortedYSize = m_sorted_y.Count;
            for (int i = 0; i < sortedYSize; i++)
            {
                int v = sortedYData[i].start;
                sortedYData[i].start = start;
                start += v;
            }

            // Fill the cell pointer array sorted by Y
            for (int i = 0; i < m_num_used_cells; ++i)
            {
                int sortedIndex = cells[i].y - m_min_y;
                int curr_y_start = sortedYData[sortedIndex].start;
                int curr_y_num = sortedYData[sortedIndex].num;
                sortedCellsData[curr_y_start + curr_y_num] = cells[i];
                sortedYData[sortedIndex].num++;
            }

            // Finally arrange the X-arrays
            for (int i = 0; i < sortedYSize; i++)
            {
                var yData = sortedYData[i];
                if (yData.num != 0)
                {
                    QuickSort.Sort(sortedCellsData,
                        yData.start,
                        yData.start + yData.num - 1);
                }
            }
            m_sorted = true;
        }

        public int TotalCells
        {
            get { return this.m_num_used_cells; }
        }


        public void GetCells(int y, out CellAA[] cellData, out int offset, out int num)
        {
            cellData = m_sorted_cells.GetArray();
            var d = m_sorted_y[y - m_min_y];
            offset = d.start;
            num = d.num;
        }


        public bool Sorted
        {
            get
            {
                return this.m_sorted;
            }
        }

        void AddNewCell(int x, int y)
        {
            WriteCurrentCell();

            cCell.x = x;
            cCell.y = y;
            cCell.cover = 0;
            cCell.area = 0;

            // if (m_curr_cell.NotEqual(x, y, m_style_cell))
            //{
            //    AddCurrCell();
            //    m_curr_cell.CopyLeftRightFrom(m_style_cell);
            //    m_curr_cell.x = x;
            //    m_curr_cell.y = y;
            //    m_curr_cell.cover = 0;
            //    m_curr_cell.area = 0;
            //}
        }

        void WriteCurrentCell()
        {
            if ((cCell.area | cCell.cover) != 0)
            {
                //check cell limit
                if (m_num_used_cells >= BLOCK_LIMIT)
                {
                    return;
                }

                //------------------------------------------
                //alloc if required
                if (m_cells == null || (m_num_used_cells + 1) >= m_cells.AllocatedSize)
                {

                    int new_num_allocated_cells = m_num_used_cells + BLOCK_SIZE;
                    ArrayList<CellAA> new_cells = new ArrayList<CellAA>(new_num_allocated_cells);
                    if (m_cells != null)
                    {
                        new_cells.CopyFrom(m_cells);
                    }
                    m_cells = new_cells;
                }
                //------------------------------------------  
                m_cells.GetArray()[m_num_used_cells].CopyAllFrom(cCell);
                m_num_used_cells++;
            }
        }

        void RenderHLine(int ey, int x1, int y1, int x2, int y2)
        {
            int ex1 = x1 >> poly_subpixel_scale_e.SHIFT;
            int ex2 = x2 >> poly_subpixel_scale_e.SHIFT;


            //trivial case. Happens often
            if (y1 == y2)
            {
                AddNewCell(ex2, ey);
                return;
            }

            int fx1 = x1 & (int)poly_subpixel_scale_e.MASK;
            int fx2 = x2 & (int)poly_subpixel_scale_e.MASK;

            int delta, p, first, dx;
            int incr, lift, mod, rem;


            //everything is located in a single cell.  That is easy!
            if (ex1 == ex2)
            {
                delta = y2 - y1;
                cCell.cover += delta;
                cCell.area += (fx1 + fx2) * delta;
                return;
            }

            //ok, we'll have to render a run of adjacent cells on the same hline...
            p = ((int)poly_subpixel_scale_e.SCALE - fx1) * (y2 - y1);
            first = (int)poly_subpixel_scale_e.SCALE;
            incr = 1;

            dx = x2 - x1;

            if (dx < 0)
            {
                p = fx1 * (y2 - y1);
                first = 0;
                incr = -1;
                dx = -dx;
            }

            delta = p / dx;
            mod = p % dx;

            if (mod < 0)
            {
                delta--;
                mod += dx;
            }

            cCell.cover += delta;
            cCell.area += (fx1 + first) * delta;

            ex1 += incr;
            AddNewCell(ex1, ey);
            y1 += delta;

            if (ex1 != ex2)
            {
                p = (int)poly_subpixel_scale_e.SCALE * (y2 - y1 + delta);
                lift = p / dx;
                rem = p % dx;

                if (rem < 0)
                {
                    lift--;
                    rem += dx;
                }

                mod -= dx;

                while (ex1 != ex2)
                {
                    delta = lift;
                    mod += rem;
                    if (mod >= 0)
                    {
                        mod -= dx;
                        delta++;
                    }

                    cCell.cover += delta;
                    cCell.area += (int)poly_subpixel_scale_e.SCALE * delta;
                    y1 += delta;
                    ex1 += incr;
                    AddNewCell(ex1, ey);
                }
            }
            delta = y2 - y1;
            cCell.cover += delta;
            cCell.area += (fx2 + (int)poly_subpixel_scale_e.SCALE - first) * delta;
        }





        //------------
        static class QuickSort
        {

            public static void Sort(CellAA[] dataToSort)
            {
                Sort(dataToSort, 0, dataToSort.Length - 1);
            }

            public static void Sort(CellAA[] dataToSort, int beg, int end)
            {
                if (end == beg)
                {
                    return;
                }
                else
                {
                    int pivot = GetPivotPoint(dataToSort, beg, end);
                    if (pivot > beg)
                    {
                        Sort(dataToSort, beg, pivot - 1);
                    }

                    if (pivot < end)
                    {
                        Sort(dataToSort, pivot + 1, end);
                    }
                }
            }

            static int GetPivotPoint(CellAA[] dataToSort, int begPoint, int endPoint)
            {
                int pivot = begPoint;
                int m = begPoint + 1;
                int n = endPoint;

                var x_at_PivotPoint = dataToSort[pivot].x;

                while ((m < endPoint)
                    && x_at_PivotPoint >= dataToSort[m].x)
                {
                    m++;
                }

                while ((n > begPoint) && (x_at_PivotPoint <= dataToSort[n].x))
                {
                    n--;
                }

                while (m < n)
                {
                    //swap data between m and n
                    CellAA temp = dataToSort[m];
                    dataToSort[m] = dataToSort[n];
                    dataToSort[n] = temp;

                    while ((m < endPoint) && (x_at_PivotPoint >= dataToSort[m].x))
                    {
                        m++;
                    }

                    while ((n > begPoint) && (x_at_PivotPoint <= dataToSort[n].x))
                    {
                        n--;
                    }
                }

                if (pivot != n)
                {
                    CellAA temp2 = dataToSort[n];
                    dataToSort[n] = dataToSort[pivot];
                    dataToSort[pivot] = temp2;

                }
                return n;
            }
        }
    }


}