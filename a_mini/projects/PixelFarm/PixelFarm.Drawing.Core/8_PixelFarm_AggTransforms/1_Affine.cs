//BSD, 2014-2017, WinterDev
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
// Affine transformation classes.
//
//----------------------------------------------------------------------------
//#ifndef AGG_TRANS_AFFINE_INCLUDED
//#define AGG_TRANS_AFFINE_INCLUDED

//#include <math.h>
//#include "agg_basics.h"

using System;
using PixelFarm.VectorMath;
namespace PixelFarm.Agg.Transform
{


    public static class AffineExtensions
    {
        public static void TransformToVertexSnap(this Affine affine, VertexStore src, VertexStore output)
        {
            affine.TransformToVxs(src, output);
        }
        /// <summary>
        /// we do NOT store vxs, return original outputVxs
        /// </summary>
        /// <param name="src"></param>
        /// <param name="outputVxs"></param>
        public static VertexStore TransformToVxs(this Affine aff, VertexStore src, VertexStore outputVxs)
        {
            int count = src.Count;
            VertexCmd cmd;
            double x, y;
            for (int i = 0; i < count; ++i)
            {
                cmd = src.GetVertex(i, out x, out y);
                aff.Transform(ref x, ref y);
                outputVxs.AddVertex(x, y, cmd);
            }

            //outputVxs.HasMoreThanOnePart = src.HasMoreThanOnePart;
            return outputVxs;
        }
        /// <summary>
        /// we do NOT store vxs, return original outputVxs
        /// </summary>
        /// <param name="src"></param>
        /// <param name="outputVxs"></param>
        /// <returns></returns>
        public static VertexStore TransformToVxs(this Affine aff, VertexStoreSnap src, VertexStore outputVxs)
        {
            var snapIter = src.GetVertexSnapIter();
            VertexCmd cmd;
            double x, y;
            while ((cmd = snapIter.GetNextVertex(out x, out y)) != VertexCmd.NoMore)
            {
                aff.Transform(ref x, ref y);
                outputVxs.AddVertex(x, y, cmd);
            }
            return outputVxs;
        }
       
    }




}