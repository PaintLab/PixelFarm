//BSD, 2014-present, WinterDev
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


namespace PixelFarm.CpuBlit.VertexProcessing
{
    public interface ICoordTransformer
    {
        void Transform(ref double x, ref double y);
        ICoordTransformer MultiplyWith(ICoordTransformer another);
        ICoordTransformer CreateInvert();
        CoordTransformerKind Kind { get; }
    }


    public enum CoordTransformerKind
    {
        Unknown,
        Affine,
        Perspective,
        Bilinear,
        TransformChain,
    }


    //Manual Serializer/ Deserializer
    //---------------------------
    //TODO: move this class to another file
    /// <summary>
    /// ICoordTransform Serializer/ Deseralizer
    /// </summary>
    public static class ICoordTransformRW
    {
        /// <summary>
        /// serialize coord-transform-chain to specific stream
        /// </summary>
        /// <param name="coordTx"></param>
        /// <param name="writer"></param>
        public static void Write(ICoordTransformer coordTx, System.IO.BinaryWriter writer)
        {
            //write transformation matrix to binary stream
            CoordTransformerKind txKind = coordTx.Kind;
            switch (txKind)
            {
                case CoordTransformerKind.Unknown:
                default:
                    throw new System.NotSupportedException();
                case CoordTransformerKind.Affine:
                    {
                        Affine aff = (Affine)coordTx;

                    }
                    break;
                case CoordTransformerKind.Bilinear:
                    {
                        Bilinear binTx = (Bilinear)coordTx;

                    }
                    break;
                case CoordTransformerKind.Perspective:
                    {
                        Perspective perTx = (Perspective)coordTx;

                    }
                    break;
                case CoordTransformerKind.TransformChain:
                    {
                        CoordTransformationChain chainTx = (CoordTransformationChain)coordTx;
                    }

                    break;
            }
        }



        /// <summary>
        /// read back, read  coord-transform-chain  back from specific stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static CoordTransformationChain ReadCoordTransfromChain(System.IO.BinaryReader reader)
        {

            return null;
        }
    }
}