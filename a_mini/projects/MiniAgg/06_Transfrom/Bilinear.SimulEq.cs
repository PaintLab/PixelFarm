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


    partial class Bilinear : ITransform
    {
        //============================================================matrix_pivot
        //template<uint Rows, uint Cols> 
        //===============================================================simul_eq
        //template<uint Size, uint RightCols>
        static class SimulEqGeneral
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
                int rowCountRt = right.GetLength(0);
                int colCountRt = right.GetLength(1);


                double[,] tmp = new double[rowCountRt, rowCountRt + colCountRt];
                //--------------------------------------
                //merge left and right matrix to tmp
                for (int i = 0; i < rowCountRt; i++)
                {

                    for (int j = 0; j < rowCountRt; j++)
                    {
                        tmp[i, j] = left[i, j];
                    }

                    for (int j = 0; j < colCountRt; j++)
                    {
                        tmp[i, rowCountRt + j] = right[i, j];
                    }
                }
                //--------------------------------------

                for (int k = 0; k < rowCountRt; k++)
                {
                    if (DoMatrixPivot(tmp, rowCountRt, k) < 0)
                    {
                        return false; // Singularity....
                    }

                    double a1 = tmp[k, k];

                    for (int j = k; j < rowCountRt + colCountRt; j++)
                    {
                        tmp[k, j] /= a1;
                    }

                    for (int i = k + 1; i < rowCountRt; i++)
                    {
                        a1 = tmp[i, k];
                        for (int j = k; j < rowCountRt + colCountRt; j++)
                        {
                            tmp[i, j] -= a1 * tmp[k, j];
                        }
                    }
                }


                for (int k = 0; k < colCountRt; k++)
                {

                    for (int m = rowCountRt - 1; m >= 0; m--)
                    {
                        ///2

                        result[m, k] = tmp[m, rowCountRt + k];
                        for (int j = m + 1; j < rowCountRt; j++)
                        {
                            result[m, k] -= tmp[m, j] * result[j, k];
                        }
                    }
                }


                return true;
            }


            static void SwapArraysIndex1(double[,] a1, uint a1Index0, double[,] a2, int a2Index0)
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

            static int DoMatrixPivot(double[,] m, int rowCount, int row)
            {
                int k = (int)(row);
                double max_val, tmp;

                max_val = -1.0;


                for (int i = (int)row; i < rowCount; i++)
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