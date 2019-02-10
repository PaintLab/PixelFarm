//MIT, 2016, Viktor Chlumsky, Multi-channel signed distance field generator, from https://github.com/Chlumsky/msdfgen
//MIT, 2017-present, WinterDev (C# port)

using System;

namespace ExtMsdfgen
{


    public class BmpEdgeLut
    {
        int _w;
        int _h;
        int[] _buffer;
        public BmpEdgeLut(int w, int h, int[] buffer)
        {
            _w = w;
            _h = h;
            _buffer = buffer;
        }
        public int GetPixel(int x, int y)
        {
            return _buffer[y * _w + x];
        }

    }
    public class ShapeCornerArms
    {
        public int CornerNo;


        public int LeftIndex;
        public int MiddleIndex;
        public int RightIndex;

        public PixelFarm.Drawing.PointF leftPoint;
        public PixelFarm.Drawing.PointF middlePoint;
        public PixelFarm.Drawing.PointF rightPoint;



        //-----------
        /// <summary>
        /// extended point of left->middle line
        /// </summary>
        public PixelFarm.Drawing.PointF leftExtendedPoint_Outer;
        public PixelFarm.Drawing.PointF leftExtendedPoint_OuterGap;
        public PixelFarm.Drawing.PointF leftExtendedPoint_Inner;
        /// <summary>
        /// extended point of right->middle line
        /// </summary>
        public PixelFarm.Drawing.PointF rightExtendedPoint_Outer;
        public PixelFarm.Drawing.PointF rightExtendedPoint_OuterGap;
        public PixelFarm.Drawing.PointF rightExtendedPoint_Inner;



        public PixelFarm.Drawing.PointF leftExtendedPointDest_Inner;
        /// <summary>
        /// destination point of left-extened point (to right extened point of other)
        /// </summary>
        public PixelFarm.Drawing.PointF leftExtendedPointDest_Outer;

        /// <summary>
        /// destination point right-extended point
        /// </summary>
        public PixelFarm.Drawing.PointF rightExtendedPointDest_Outer;
        public PixelFarm.Drawing.PointF rightExtendedPointDest_Inner;
        //-----------


        public ShapeCornerArms()
        {

        }
        public PixelFarm.Drawing.Color OuterColor
        {
            get
            {
                float color = (CornerNo * 2) + 50;
                return new PixelFarm.Drawing.Color((byte)color, (byte)color, 0);
            }
        }
        public PixelFarm.Drawing.Color InnerColor
        {
            get
            {
                float color = (CornerNo * 2) + 50;
                return new PixelFarm.Drawing.Color((byte)color, 0, (byte)color);
            }
        }
        public void Offset(float dx, float dy)
        {
            //
            leftPoint.Offset(dx, dy);
            middlePoint.Offset(dx, dy);
            rightPoint.Offset(dx, dy);

            leftExtendedPoint_Outer.Offset(dx, dy);
            rightExtendedPoint_Outer.Offset(dx, dy);
            leftExtendedPointDest_Outer.Offset(dx, dy);
            rightExtendedPointDest_Outer.Offset(dx, dy);
            //

            leftExtendedPoint_Inner.Offset(dx, dy);
            rightExtendedPoint_Inner.Offset(dx, dy);
            leftExtendedPointDest_Inner.Offset(dx, dy);
            rightExtendedPointDest_Inner.Offset(dx, dy);
            //
            leftExtendedPoint_OuterGap.Offset(dx, dy);
            rightExtendedPoint_OuterGap.Offset(dx, dy);
        }
        static double CurrentLen(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
        {
            float dx = p1.X - p0.X;
            float dy = p1.Y - p0.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        public void CreateExtendedEdges()
        {
            //
            rightExtendedPoint_Outer = CreateExtendedOuterEdges(rightPoint, middlePoint);
            rightExtendedPoint_OuterGap = CreateExtendedOuterGapEdges(rightPoint, middlePoint);
            //
            leftExtendedPoint_Outer = CreateExtendedOuterEdges(leftPoint, middlePoint);
            leftExtendedPoint_OuterGap = CreateExtendedOuterGapEdges(leftPoint, middlePoint);
            //
            rightExtendedPoint_Inner = CreateExtendedInnerEdges(rightPoint, middlePoint);
            leftExtendedPoint_Inner = CreateExtendedInnerEdges(leftPoint, middlePoint);
            // 
        }
        PixelFarm.Drawing.PointF CreateExtendedOuterEdges(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
        {

            double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
            double currentLen = CurrentLen(p0, p1);
            double newLen = currentLen + 3;

            double new_dx = Math.Cos(rad) * newLen;
            double new_dy = Math.Sin(rad) * newLen;


            return new PixelFarm.Drawing.PointF((float)(p0.X + new_dx), (float)(p0.Y + new_dy));
        }
        PixelFarm.Drawing.PointF CreateExtendedOuterGapEdges(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
        {

            double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
            double currentLen = CurrentLen(p0, p1);
            double newLen = currentLen + 2;

            double new_dx = Math.Cos(rad) * newLen;
            double new_dy = Math.Sin(rad) * newLen;

            return new PixelFarm.Drawing.PointF((float)(p0.X + new_dx), (float)(p0.Y + new_dy));
        }
        PixelFarm.Drawing.PointF CreateExtendedInnerEdges(PixelFarm.Drawing.PointF p0, PixelFarm.Drawing.PointF p1)
        {

            double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
            double currentLen = CurrentLen(p0, p1);
            if (currentLen - 3 < 0)
            {
                return p0;//***
            }

            double newLen = currentLen - 3;
            double new_dx = Math.Cos(rad) * newLen;
            double new_dy = Math.Sin(rad) * newLen;
            return new PixelFarm.Drawing.PointF((float)(p0.X + new_dx), (float)(p0.Y + new_dy));
        }
        public override string ToString()
        {
            return LeftIndex + "," + MiddleIndex + "," + RightIndex;
        }
    }
}