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
// Bilinear 2D transformations
//
//----------------------------------------------------------------------------
using System;

namespace MatterHackers.Agg.Transform
{

    //==========================================================trans_bilinear
    public sealed partial class Bilinear : ITransform
    {

        double[,] m_mtx = new double[4, 2];
        bool m_valid;

        //--------------------------------------------------------------------
        public Bilinear()
        {
            m_valid = false;
        }

        //--------------------------------------------------------------------
        // Arbitrary quadrangle transformations
        public Bilinear(double[] src, double[] dst)
        {
            quad_to_quad(src, dst);
        }
        //--------------------------------------------------------------------
        // Direct transformations 
        public Bilinear(double x1, double y1, double x2, double y2, double[] quad)
        {
            rect_to_quad(x1, y1, x2, y2, quad);
        }
        //--------------------------------------------------------------------
        // Reverse transformations 
        public Bilinear(double[] quad,
                       double x1, double y1,
                       double x2, double y2)
        {
            quad_to_rect(quad, x1, y1, x2, y2);
        }
        //--------------------------------------------------------------------
        // Set the transformations using two arbitrary quadrangles.
        public void quad_to_quad(double[] src, double[] dst)
        {
            double[,] left = new double[4, 4];
            double[,] right = new double[4, 2];
            uint i;
            for (i = 0; i < 4; i++)
            {
                uint ix = i * 2;
                uint iy = ix + 1;
                left[i, 0] = 1.0;
                left[i, 1] = src[ix] * src[iy];
                left[i, 2] = src[ix];
                left[i, 3] = src[iy];

                right[i, 0] = dst[ix];
                right[i, 1] = dst[iy];
            }
            m_valid = SimulEq.Solve(left, right, m_mtx);
        }


        //--------------------------------------------------------------------
        // Set the direct transformations, i.e., rectangle -> quadrangle
        public void rect_to_quad(double x1, double y1, double x2, double y2,
                          double[] quad)
        {
            double[] src = new double[8];
            src[0] = src[6] = x1;
            src[2] = src[4] = x2;
            src[1] = src[3] = y1;
            src[5] = src[7] = y2;
            quad_to_quad(src, quad);
        }


        //--------------------------------------------------------------------
        // Set the reverse transformations, i.e., quadrangle -> rectangle
        public void quad_to_rect(double[] quad,
                          double x1, double y1, double x2, double y2)
        {
            double[] dst = new double[8];
            dst[0] = dst[6] = x1;
            dst[2] = dst[4] = x2;
            dst[1] = dst[3] = y1;
            dst[5] = dst[7] = y2;
            quad_to_quad(quad, dst);
        }

        //--------------------------------------------------------------------
        // Check if the equations were solved successfully
        public bool IsValid() { return m_valid; }

        //--------------------------------------------------------------------
        // Transform a point (x, y)
        public void Transform(ref double x, ref double y)
        {
            double tx = x;
            double ty = y;
            double xy = tx * ty;
            x = m_mtx[0, 0] + m_mtx[1, 0] * xy + m_mtx[2, 0] * tx + m_mtx[3, 0] * ty;
            y = m_mtx[0, 1] + m_mtx[1, 1] * xy + m_mtx[2, 1] * tx + m_mtx[3, 1] * ty;
        }
    }


    //============================================================matrix_pivot
    //template<uint Rows, uint Cols>
    partial class Bilinear
    {

        //===============================================================simul_eq
        //template<uint Size, uint RightCols>
        static class SimulEq
        {

            public static bool Solve(double[,] left,
                              double[,] right,
                              double[,] result)
            {
                //please check before use !

                //if (left.GetLength(0) != 4
                //    || right.GetLength(0) != 4
                //    || left.GetLength(1) != 4
                //    || result.GetLength(0) != 4
                //    || right.GetLength(1) != 2
                //    || result.GetLength(1) != 2)
                //{
                //    throw new System.FormatException("left right and result must all be the same size.");
                //}

                double a1;
                int size = right.GetLength(0);
                int rightCols = right.GetLength(1);

                double[,] tmp = new double[size, size + rightCols];

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        tmp[i, j] = left[i, j];
                    }
                    for (int j = 0; j < rightCols; j++)
                    {
                        tmp[i, size + j] = right[i, j];
                    }
                }

                for (int k = 0; k < size; k++)
                {
                    if (DoMatrixPivot(tmp, (uint)k) < 0)
                    {
                        return false; // Singularity....
                    }

                    a1 = tmp[k, k];

                    for (int j = k; j < size + rightCols; j++)
                    {
                        tmp[k, j] /= a1;
                    }

                    for (int i = k + 1; i < size; i++)
                    {
                        a1 = tmp[i, k];
                        for (int j = k; j < size + rightCols; j++)
                        {
                            tmp[i, j] -= a1 * tmp[k, j];
                        }
                    }
                }


                for (int k = 0; k < rightCols; k++)
                {
                    int m;
                    for (m = (int)(size - 1); m >= 0; m--)
                    {
                        result[m, k] = tmp[m, size + k];
                        for (int j = m + 1; j < size; j++)
                        {
                            result[m, k] -= tmp[m, j] * result[j, k];
                        }
                    }
                }
                return true;
            }


            static void SwapArraysIndex1(double[,] a1, uint a1Index0, double[,] a2, uint a2Index0)
            {
                int cols = a1.GetLength(1);
                if (a2.GetLength(1) != cols)
                {
                    throw new System.FormatException("a1 and a2 must have the same second dimension.");
                }

                for (int i = 0; i < cols; i++)
                {
                    double tmp = a1[a1Index0, i];
                    a1[a1Index0, i] = a2[a2Index0, i];
                    a2[a2Index0, i] = tmp;
                }
            }

            static int DoMatrixPivot(double[,] m, uint row)
            {
                int k = (int)(row);
                double max_val, tmp;

                max_val = -1.0;
                int i;
                int rows = m.GetLength(0);
                for (i = (int)row; i < rows; i++)
                {
                    if ((tmp = Math.Abs(m[i, row])) > max_val && tmp != 0.0)
                    {
                        max_val = tmp;
                        k = i;
                    }
                }

                if (m[k, row] == 0.0)
                {
                    return -1;
                }

                if (k != (int)(row))
                {
                    SwapArraysIndex1(m, (uint)k, m, row);
                    return k;
                }
                return 0;
            }
        }
    }
}