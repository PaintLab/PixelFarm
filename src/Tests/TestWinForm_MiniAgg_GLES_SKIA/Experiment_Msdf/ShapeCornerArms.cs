//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
namespace ExtMsdfgen
{

    public enum AreaKind
    {
        Outide,
        Inside,
        //Gap
    }
    public struct EdgeStructure
    {
        readonly ShapeCornerArms _shapeCornerArms;
        readonly AreaKind _areaKind;
        readonly bool _isEmpty;
        public EdgeStructure(ShapeCornerArms shapeCornerArms, AreaKind areaKind)
        {
            _isEmpty = false;
            _shapeCornerArms = shapeCornerArms;
            _areaKind = areaKind;
        }
        public AreaKind AreaKind => _areaKind;
        public bool IsEmpty => _isEmpty;
        public static readonly EdgeStructure Empty = new EdgeStructure();
    }
    public class BmpEdgeLut
    {
        int _w;
        int _h;
        int[] _buffer;
        List<ShapeCornerArms> _cornerArms;
        public BmpEdgeLut(int w, int h, int[] buffer, List<ShapeCornerArms> cornerArms)
        {
            _w = w;
            _h = h;
            _buffer = buffer;
            _cornerArms = cornerArms;
        }
        public int GetPixel(int x, int y) => _buffer[y * _w + x];

        const int WHITE = (255 << 24) | (255 << 16) | (255 << 8) | 255;
        public EdgeStructure GetCornerArm(int x, int y)
        {
            int pixel = _buffer[y * _w + x];
            if (pixel == 0)
            {
                return EdgeStructure.Empty;
            }
            else if (pixel == WHITE)
            {
                return EdgeStructure.Empty;
            }
            else
            {
                //G
                int g = (pixel >> 8) & 0xFF;
                //find index
                int r = pixel & 0xFF;
                int index = (r - 50) / 2;//just our encoding (see ShapeCornerArms.OuterColor, ShapeCornerArms.InnerColor)

                ShapeCornerArms cornerArm = _cornerArms[index];
                if (g == 50)
                {
                    //outside
                    return new EdgeStructure(cornerArm, AreaKind.Outide);
                }
                else
                {
                    //inside
                    return new EdgeStructure(cornerArm, AreaKind.Inside);
                }
            }
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
                return new PixelFarm.Drawing.Color((byte)color, 50, (byte)color);
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