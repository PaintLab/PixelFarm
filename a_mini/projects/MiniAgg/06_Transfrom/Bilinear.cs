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
        //readonly double[,] m_mtx = new double[4, 2];//row x column
        //4 row, 2 columns

        double rc00, rc01,
               rc10, rc11,
               rc20, rc21,
               rc30, rc31;

        

        bool m_valid;

        //--------------------------------------------------------------------
        //public Bilinear()
        //{
        //    m_valid = false;
        //}

        //--------------------------------------------------------------------
        // Arbitrary quadrangle transformations
        //public Bilinear(double[] src, double[] dst)
        //{
        //    quad_to_quad(src, dst);
        //}
        //--------------------------------------------------------------------
        // Direct transformations 
        public Bilinear(double x1, double y1, double x2, double y2, double[] quad)
        {
            double[] src = new double[8];
            src[0] = src[6] = x1;
            src[2] = src[4] = x2;
            src[1] = src[3] = y1;
            src[5] = src[7] = y2;

            double[,] result = new double[4, 2];
            if (this.m_valid = GenerateMatrixQuadToQuad(src, quad, result))
            {
                this.rc00 = result[0, 0];
                this.rc10 = result[1, 0];
                this.rc20 = result[2, 0];
                this.rc30 = result[3, 0];

                this.rc01 = result[0, 1];
                this.rc11 = result[1, 1];
                this.rc21 = result[2, 1];
                this.rc31 = result[3, 1];
            }
        }
        //--------------------------------------------------------------------
        // Reverse transformations 
        public Bilinear(double[] quad,
                       double x1, double y1,
                       double x2, double y2)
        {
            //--------------------------------------------------------------------
            // Set the reverse transformations, i.e., quadrangle -> rectangle

            double[] dst = new double[8];
            dst[0] = dst[6] = x1;
            dst[2] = dst[4] = x2;
            dst[1] = dst[3] = y1;
            dst[5] = dst[7] = y2;

            double[,] result = new double[4, 2];

            if (this.m_valid = GenerateMatrixQuadToQuad(quad, dst, result))
            {
                this.rc00 = result[0, 0];
                this.rc10 = result[1, 0];
                this.rc20 = result[2, 0];
                this.rc30 = result[3, 0];

                this.rc01 = result[0, 1];
                this.rc11 = result[1, 1];
                this.rc21 = result[2, 1];
                this.rc31 = result[3, 1];
            }

        }
        //--------------------------------------------------------------------
        // Set the transformations using two arbitrary quadrangles.

        static bool GenerateMatrixQuadToQuad(double[] src, double[] dst, double[,] result)
        {
            double[,] left = new double[4, 4];
            double[,] right = new double[4, 2];

            for (int i = 0; i < 4; i++)
            {
                int ix = i << 1;
                int iy = ix + 1;

                left[i, 0] = 1.0;
                left[i, 1] = src[ix] * src[iy];
                left[i, 2] = src[ix];
                left[i, 3] = src[iy];

                right[i, 0] = dst[ix];
                right[i, 1] = dst[iy];
            }
            //create result  
            return SimulEqGeneral.Solve(left, right, result);
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
            x = rc00 + rc10 * xy + rc20 * tx + rc30 * ty;
            y = rc01 + rc11 * xy + rc21 * tx + rc31 * ty;
        }
    }




}