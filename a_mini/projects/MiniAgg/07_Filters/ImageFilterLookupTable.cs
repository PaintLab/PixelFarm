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
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
#define USE_UNSAFE_CODE

using System;

using MatterHackers.Agg.Image;
using MatterHackers.VectorMath;

using image_subpixel_scale_e = MatterHackers.Agg.ImageFilterLookUpTable.image_subpixel_scale_e;
using image_filter_scale_e = MatterHackers.Agg.ImageFilterLookUpTable.image_filter_scale_e;


namespace MatterHackers.Agg
{
    public interface IImageFilterFunction
    {
        double radius();
        double calc_weight(double x);
    }

    //-----------------------------------------------------ImageFilterLookUpTable
    public class ImageFilterLookUpTable
    {
        double m_radius;
        int m_diameter;
        int m_start;
        ArrayList<int> m_weight_array;

        public enum image_filter_scale_e
        {
            image_filter_shift = 14,                      //----image_filter_shift
            image_filter_scale = 1 << image_filter_shift, //----image_filter_scale 
            image_filter_mask = image_filter_scale - 1   //----image_filter_mask 
        }

        public enum image_subpixel_scale_e
        {
            image_subpixel_shift = 8,                         //----image_subpixel_shift
            image_subpixel_scale = 1 << image_subpixel_shift, //----image_subpixel_scale 
            image_subpixel_mask = image_subpixel_scale - 1   //----image_subpixel_mask 
        }

        public void calculate(IImageFilterFunction filter)
        {
            calculate(filter, true);
        }

        public void calculate(IImageFilterFunction filter, bool normalization)
        {
            double r = filter.radius();
            realloc_lut(r);
            int i;
            int pivot = diameter() << ((int)image_subpixel_scale_e.image_subpixel_shift - 1);
            for (i = 0; i < pivot; i++)
            {
                double x = (double)i / (double)image_subpixel_scale_e.image_subpixel_scale;
                double y = filter.calc_weight(x);
                m_weight_array.Array[pivot + i] =
                m_weight_array.Array[pivot - i] = AggBasics.iround(y * (int)image_filter_scale_e.image_filter_scale);
            }
            int end = (diameter() << (int)image_subpixel_scale_e.image_subpixel_shift) - 1;
            m_weight_array.Array[0] = m_weight_array.Array[end];
            if (normalization)
            {
                normalize();
            }
        }

        public ImageFilterLookUpTable()
        {
            m_weight_array = new ArrayList<int>(256);
            m_radius = (0);
            m_diameter = (0);
            m_start = (0);
        }

        public ImageFilterLookUpTable(IImageFilterFunction filter)
            : this(filter, true)
        {

        }
        public ImageFilterLookUpTable(IImageFilterFunction filter, bool normalization)
        {
            m_weight_array = new ArrayList<int>(256);
            calculate(filter, normalization);
        }

        public double radius() { return m_radius; }
        public int diameter() { return m_diameter; }
        public int start() { return m_start; }
        public int[] weight_array() { return m_weight_array.Array; }

        //--------------------------------------------------------------------
        // This function normalizes integer values and corrects the rounding 
        // errors. It doesn't do anything with the source floating point values
        // (m_weight_array_dbl), it corrects only integers according to the rule 
        // of 1.0 which means that any sum of pixel weights must be equal to 1.0.
        // So, the filter function must produce a graph of the proper shape.
        //--------------------------------------------------------------------
        public void normalize()
        {
            int i;
            int flip = 1;

            for (i = 0; i < (int)image_subpixel_scale_e.image_subpixel_scale; i++)
            {
                for (; ; )
                {
                    int sum = 0;
                    int j;
                    for (j = 0; j < m_diameter; j++)
                    {
                        sum += m_weight_array.Array[j * (int)image_subpixel_scale_e.image_subpixel_scale + i];
                    }

                    if (sum == (int)image_filter_scale_e.image_filter_scale) break;

                    double k = (double)((int)image_filter_scale_e.image_filter_scale) / (double)(sum);
                    sum = 0;
                    for (j = 0; j < m_diameter; j++)
                    {
                        sum += m_weight_array.Array[j * (int)image_subpixel_scale_e.image_subpixel_scale + i] =
                            (int)AggBasics.iround(m_weight_array.Array[j * (int)image_subpixel_scale_e.image_subpixel_scale + i] * k);
                    }

                    sum -= (int)image_filter_scale_e.image_filter_scale;
                    int inc = (sum > 0) ? -1 : 1;

                    for (j = 0; j < m_diameter && sum != 0; j++)
                    {
                        flip ^= 1;
                        int idx = flip != 0 ? m_diameter / 2 + j / 2 : m_diameter / 2 - j / 2;
                        int v = m_weight_array.Array[idx * (int)image_subpixel_scale_e.image_subpixel_scale + i];
                        if (v < (int)image_filter_scale_e.image_filter_scale)
                        {
                            m_weight_array.Array[idx * (int)image_subpixel_scale_e.image_subpixel_scale + i] += (int)inc;
                            sum += inc;
                        }
                    }
                }
            }

            int pivot = m_diameter << ((int)image_subpixel_scale_e.image_subpixel_shift - 1);

            for (i = 0; i < pivot; i++)
            {
                m_weight_array.Array[pivot + i] = m_weight_array.Array[pivot - i];
            }
            int end = (diameter() << (int)image_subpixel_scale_e.image_subpixel_shift) - 1;
            m_weight_array.Array[0] = m_weight_array.Array[end];
        }

        private void realloc_lut(double radius)
        {
            m_radius = radius;
            m_diameter = AggBasics.uceil(radius) * 2;
            m_start = -(int)(m_diameter / 2 - 1);
            int size = (int)m_diameter << (int)image_subpixel_scale_e.image_subpixel_shift;
            if (size > m_weight_array.Count)
            {
                m_weight_array.Resize(size);
            }
        }
    }
}