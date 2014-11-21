//2014 BSD,WinterDev   

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using PixelFarm.VectorMath;

using FlagsAndCommand = PixelFarm.Agg.ShapePath.CmdAndFlags;

namespace PixelFarm.Agg.VertexSource
{

    /// <summary>
    /// bezire curve generator
    /// </summary>
    public static class BezierCurve
    {
        public static void CreateBezierVxs4(Agg.VertexStore vxs, Vector2 start, Vector2 end,
            Vector2 control1, Vector2 control2)
        {
            var curve = new VectorMath.BezierCurveCubic(
                start, end,
                control1, control2);

            vxs.AddMoveTo(start.x, start.y);
            float eachstep = (float)1 / 20;
            for (int i = 1; i < 20; ++i)
            {
                var vector2 = curve.CalculatePoint((float)(i * eachstep));
                vxs.AddLineTo(vector2.x, vector2.y);
            }
            vxs.AddLineTo(end.x, end.y);
            vxs.AddStop();
        }

        public static void CreateBezierVxs3(Agg.VertexStore vxs, Vector2 start, Vector2 end,
           Vector2 control1)
        {
            var curve = new VectorMath.BezierCurveQuadric(
                start, end,
                control1);
            vxs.AddLineTo(start.x, start.y);
            float eachstep = (float)1 / 20;
            for (int i = 1; i < 20; ++i)
            {
                var vector2 = curve.CalculatePoint((float)(i * eachstep));
                vxs.AddLineTo(vector2.x, vector2.y);
            }
            vxs.AddLineTo(end.x, end.y);

        }
    }
}