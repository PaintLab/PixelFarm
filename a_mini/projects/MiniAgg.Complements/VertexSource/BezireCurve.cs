//2014 BSD,WinterDev   

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using PixelFarm.VectorMath;

using FlagsAndCommand = PixelFarm.Agg.ShapePath.FlagsAndCommand;

namespace PixelFarm.Agg.VertexSource
{

    /// <summary>
    /// bezire curve generator
    /// </summary>
    public static class BezierCurve
    {
        public static void CreateBezierVxs(Agg.VertexStore vxs, Vector2 start, Vector2 end,
            Vector2 control1, Vector2 control2)
        {
            var curve = new VectorMath.BezierCurveCubic(
                start, end, control1, control2);

            vxs.AddVertex(new VertexData(FlagsAndCommand.CommandMoveTo, start));

            float eachstep = (float)1 / 20;
            for (int i = 1; i < 20; ++i)
            {
                vxs.AddVertex(new VertexData(FlagsAndCommand.CommandLineTo, curve.CalculatePoint((float)(i * eachstep))));
            }
            vxs.AddVertex(new VertexData(FlagsAndCommand.CommandLineTo, end));
            vxs.AddVertex(new VertexData(FlagsAndCommand.CommandStop));
        }



    }
}