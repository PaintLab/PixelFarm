//MIT, 2019-present, WinterDev

using System;
using System.Collections.Generic;
namespace ExtMsdfGen
{

    public enum AreaKind : byte
    {
        Outside,
        Inside,
        OuterGap
    }
    public struct EdgeStructure
    {
        readonly ContourCorner _corner;
        readonly AreaKind _areaKind;
        readonly bool _isEmpty;
        readonly ExtMsdfGen.EdgeSegment _edgeSegment;
        public EdgeStructure(ContourCorner contourCorner, AreaKind areaKind, ExtMsdfGen.EdgeSegment edgeSegment)
        {
            _isEmpty = false;
            _corner = contourCorner;
            _areaKind = areaKind;
            _edgeSegment = edgeSegment;
        }
        public ExtMsdfGen.EdgeSegment Segment => _edgeSegment;
        public AreaKind AreaKind => _areaKind;
        public bool IsEmpty => _isEmpty;
        public static readonly EdgeStructure Empty = new EdgeStructure();
    }

    /// <summary>
    /// edge bitmap lookup table
    /// </summary>
    public class EdgeBmpLut
    {
        int _w;
        int _h;
        int[] _buffer;
        List<ContourCorner> _corners;
        List<EdgeSegment> _flattenEdges;
        public EdgeBmpLut(List<ContourCorner> corners, List<ExtMsdfGen.EdgeSegment> flattenEdges, List<int> segOfNextContours, List<int> cornerOfNextContours)
        {
            //move first to last 
            int startAt = 0;
            for (int i = 0; i < segOfNextContours.Count; ++i)
            {
                int nextStartAt = segOfNextContours[i];
                //
                ExtMsdfGen.EdgeSegment firstSegment = flattenEdges[startAt];

                flattenEdges.RemoveAt(startAt);
                if (i == segOfNextContours.Count - 1)
                {
                    flattenEdges.Add(firstSegment);
                }
                else
                {
                    flattenEdges.Insert(nextStartAt - 1, firstSegment);
                }
                startAt = nextStartAt;
            }

            _corners = corners;
            _flattenEdges = flattenEdges;
            EdgeOfNextContours = segOfNextContours;
            CornerOfNextContours = cornerOfNextContours;

            ConnectExtendedPoints(corners, cornerOfNextContours); //after arrange 
        }
        static void ConnectExtendedPoints(List<ExtMsdfGen.ContourCorner> corners, List<int> cornerOfNextContours)
        {
            //test 2 if each edge has unique color 
            int startAt = 0;
            for (int i = 0; i < cornerOfNextContours.Count; ++i)
            {
                int nextStartAt = cornerOfNextContours[i];
                for (int n = startAt + 1; n < nextStartAt; ++n)
                {
                    ExtMsdfGen.ContourCorner c_prev = corners[n - 1];
                    ExtMsdfGen.ContourCorner c_current = corners[n];
                    c_prev.ExtPoint_LeftOuterDest = c_current.ExtPoint_RightOuter;
                    c_prev.ExtPoint_LeftInnerDest = c_current.ExtPoint_RightInner;
                    //
                    c_current.ExtPoint_RightOuterDest = c_prev.ExtPoint_LeftOuter;
                    c_current.ExtPoint_RightInnerDest = c_prev.ExtPoint_LeftInner;
                }

                //last 
                {
                    //the last one
                    ExtMsdfGen.ContourCorner c_prev = corners[nextStartAt - 1];
                    ExtMsdfGen.ContourCorner c_current = corners[startAt];
                    c_prev.ExtPoint_LeftOuterDest = c_current.ExtPoint_RightOuter;
                    c_prev.ExtPoint_LeftInnerDest = c_current.ExtPoint_RightInner;
                    //
                    c_current.ExtPoint_RightOuterDest = c_prev.ExtPoint_LeftOuter;
                    c_current.ExtPoint_RightInnerDest = c_prev.ExtPoint_LeftInner;
                }

                startAt = nextStartAt;//***
            }
        }
        //
        public List<int> EdgeOfNextContours { get; private set; }
        public List<int> CornerOfNextContours { get; private set; }

        //
        public void SetBmpBuffer(int w, int h, int[] buffer)
        {
            _w = w;
            _h = h;
            _buffer = buffer;
        }
        public List<ContourCorner> Corners => _corners;

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

                ContourCorner cornerArm = _corners[index];
                //EdgeSegment segment = _flattenEdges[index];
                if (g == 50)
                {
                    //outside
                    return new EdgeStructure(cornerArm, AreaKind.Outside, cornerArm.CenterSegment);
                }
                else if (g == 25)
                {
                    return new EdgeStructure(cornerArm, AreaKind.OuterGap, cornerArm.CenterSegment);
                }
                else
                {
                    //inside
                    return new EdgeStructure(cornerArm, AreaKind.Inside, cornerArm.CenterSegment);
                }
            }
        }
    }
    public class Vec2Info
    {
        public double x, y;
        public Vec2PointKind Kind;
        public EdgeSegment owner;
        public Vec2Info(EdgeSegment owner)
        {
            this.owner = owner;
        }
    }
    public enum Vec2PointKind
    {
        Touch1,//on curve point
        C2, //quadratic curve control point (off-curve)
        C3, //cubic curve control point (off-curve)
        Touch2, //on curve point
    }

    public class ContourCorner
    {

        /// <summary>
        /// corner number in flatten list
        /// </summary>
        public int CornerNo;


#if DEBUG
        public int dbugLeftIndex;
        public int dbugMiddleIndex;
        public int dbugRightIndex;
#endif

        PixelFarm.Drawing.PointD _pLeft;
        PixelFarm.Drawing.PointD _pCenter;
        PixelFarm.Drawing.PointD _pRight;

        //to other point
        public PixelFarm.Drawing.PointD ExtPoint_LeftInnerDest;
        public PixelFarm.Drawing.PointD ExtPoint_LeftOuterDest;

        public PixelFarm.Drawing.PointD ExtPoint_RightOuterDest;
        public PixelFarm.Drawing.PointD ExtPoint_RightInnerDest;
        //-----------
        Vec2Info _left; //left 
        Vec2Info _center;
        Vec2Info _right;
        //-----------


        public ContourCorner(Vec2Info left, Vec2Info center, Vec2Info right)
        {
            _left = left;
            _center = center;
            _right = right;

            _pLeft = new PixelFarm.Drawing.PointD(left.x, left.y);
            _pCenter = new PixelFarm.Drawing.PointD(center.x, center.y);
            _pRight = new PixelFarm.Drawing.PointD(right.x, right.y);

        }

        public PixelFarm.Drawing.PointD LeftPoint => _pLeft;
        public PixelFarm.Drawing.PointD middlePoint => _pCenter;
        public PixelFarm.Drawing.PointD RightPoint => _pRight;

        public EdgeSegment LeftSegment => _left.owner;
        public EdgeSegment CenterSegment => _center.owner;
        public EdgeSegment RightSegment => _right.owner;

        public Vec2PointKind LeftPointKind => _left.Kind;
        public Vec2PointKind MiddlePointKind => _center.Kind;
        public Vec2PointKind RightPointKind => _right.Kind;


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
        public void Offset(double dx, double dy)
        {
            //
            _pLeft.Offset(dx, dy);
            _pCenter.Offset(dx, dy);
            _pRight.Offset(dx, dy);

            ExtPoint_LeftOuterDest.Offset(dx, dy);
            ExtPoint_RightOuterDest.Offset(dx, dy);

            ExtPoint_LeftInnerDest.Offset(dx, dy);
            ExtPoint_RightInnerDest.Offset(dx, dy);
        }


        public bool MiddlePointKindIsTouchPoint => MiddlePointKind == Vec2PointKind.Touch1 || MiddlePointKind == Vec2PointKind.Touch2;
        public bool LeftPointKindIsTouchPoint => LeftPointKind == Vec2PointKind.Touch1 || LeftPointKind == Vec2PointKind.Touch2;
        public bool RightPointKindIsTouchPoint => RightPointKind == Vec2PointKind.Touch1 || RightPointKind == Vec2PointKind.Touch2;
        static double CurrentLen(PixelFarm.Drawing.PointD p0, PixelFarm.Drawing.PointD p1)
        {
            double dx = p1.X - p0.X;
            double dy = p1.Y - p0.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        //-----------
        /// <summary>
        /// extended point of left->middle line
        /// </summary>
        public PixelFarm.Drawing.PointD ExtPoint_LeftOuter => CreateExtendedOuterEdges(LeftPoint, middlePoint);
        public PixelFarm.Drawing.PointD ExtPoint_LeftInner => CreateExtendedInnerEdges(LeftPoint, middlePoint);
        /// <summary>
        /// extended point of right->middle line
        /// </summary>
        public PixelFarm.Drawing.PointD ExtPoint_RightOuter => CreateExtendedOuterEdges(RightPoint, middlePoint);
        public PixelFarm.Drawing.PointD ExtPoint_RightOuter2 => CreateExtendedOuterEdges(RightPoint, middlePoint, 2);
        public PixelFarm.Drawing.PointD ExtPoint_RightInner => CreateExtendedInnerEdges(RightPoint, middlePoint);


        static PixelFarm.Drawing.PointD CreateExtendedOuterEdges(PixelFarm.Drawing.PointD p0, PixelFarm.Drawing.PointD p1, double dlen = 3)
        {

            double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
            double currentLen = CurrentLen(p0, p1);
            double newLen = currentLen + dlen;

            //double new_dx = Math.Cos(rad) * newLen;
            //double new_dy = Math.Sin(rad) * newLen;
            return new PixelFarm.Drawing.PointD(p0.X + (Math.Cos(rad) * newLen), p0.Y + (Math.Sin(rad) * newLen));
        }

        static PixelFarm.Drawing.PointD CreateExtendedInnerEdges(PixelFarm.Drawing.PointD p0, PixelFarm.Drawing.PointD p1)
        {

            double rad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
            double currentLen = CurrentLen(p0, p1);
            if (currentLen - 3 < 0)
            {
                return p0;//***
            }
            double newLen = currentLen - 3;
            //double new_dx = Math.Cos(rad) * newLen;
            //double new_dy = Math.Sin(rad) * newLen;
            return new PixelFarm.Drawing.PointD(p0.X + (Math.Cos(rad) * newLen), p0.Y + (Math.Sin(rad) * newLen));
        }
#if DEBUG
        public override string ToString()
        {
            return dbugLeftIndex + "," + dbugMiddleIndex + "(" + middlePoint + ")," + dbugRightIndex;
        }
#endif
    }
}