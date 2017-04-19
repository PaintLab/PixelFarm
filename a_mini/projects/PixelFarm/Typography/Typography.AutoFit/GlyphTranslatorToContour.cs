//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{

    //this is PixelFarm version ***
    //render with MiniAgg 

    public class GlyphContourBuilder : OpenFont.IGlyphTranslator
    {

        List<GlyphContour> contours;
        float curX;
        float curY;
        float latestMoveToX;
        float latestMoveToY;
        GlyphContour currentCnt;
        GlyphPart _latestPart;
        public GlyphContourBuilder()
        {

        }
        public List<GlyphContour> GetContours()
        {
            return contours;
        }
        public void MoveTo(float x0, float y0)
        {
            this.latestMoveToX = this.curX = x0;
            this.latestMoveToY = this.curY = y0;
            _latestPart = null;
            //----------------------------

        }
        public void LineTo(float x1, float y1)
        {
            if (_latestPart != null)
            {
                currentCnt.AddPart(_latestPart = new GlyphLine(_latestPart, x1, y1));
            }
            else
            {
                currentCnt.AddPart(_latestPart = new GlyphLine(curX, curY, x1, y1));
            }
            this.curX = x1;
            this.curY = y1;
        }


        public void Curve3(float x1, float y1, float x2, float y2)
        {
            if (_latestPart != null)
            {
                currentCnt.AddPart(_latestPart = new GlyphCurve3(
                 _latestPart,
                  x1, y1,
                  x2, y2));
            }
            else
            {
                currentCnt.AddPart(new GlyphCurve3(
                    curX, curY,
                    x1, y1,
                    x2, y2));
            }

            this.curX = x2;
            this.curY = y2;
        }
        public void Curve4(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            if (_latestPart != null)
            {
                currentCnt.AddPart(_latestPart = new GlyphCurve4(
                   _latestPart,
                    x1, y1,
                    x2, y2,
                    x3, y3));
            }
            else
            {
                currentCnt.AddPart(_latestPart = new GlyphCurve4(
                   curX, curY,
                   x1, y1,
                   x2, y2,
                   x3, y3));
            }
            this.curX = x3;
            this.curY = y3;
        }

        public void CloseContour()
        {
            if (curX == latestMoveToX && curY == latestMoveToY)
            {
                //we not need to close 
            }
            else
            {
                if (_latestPart != null)
                {
                    currentCnt.AddPart(_latestPart = new GlyphLine(_latestPart, latestMoveToX, latestMoveToY));
                }
                else
                {
                    currentCnt.AddPart(_latestPart = new GlyphLine(curX, curY, latestMoveToX, latestMoveToY));
                }
            }

            this.curX = latestMoveToX;
            this.curY = latestMoveToY;

            if (currentCnt != null)
            {
                this.contours.Add(currentCnt);
                currentCnt = null;
            }
            //
            currentCnt = new GlyphContour();
        }
        public void BeginRead(int contourCount)
        {
            //reset all
            contours = new List<GlyphContour>();
            _latestPart = null;
            this.latestMoveToX = this.curX = this.latestMoveToY = this.curY = 0;
            //
            currentCnt = new GlyphContour();
            //new contour, but not add
        }
        public void EndRead()
        {

        }

    }

    public class GlyphContour
    {

        public List<GlyphPart> parts = new List<GlyphPart>();
        internal List<GlyphPoint> flattenPoints;

        bool analyzed;
        bool analyzedClockDirection;
        bool isClockwise;
        public GlyphContour()
        {
        }

        public void AddPart(GlyphPart part)
        {
            parts.Add(part);
        }

        internal void ClearAllAdjustValues()
        {
            for (int i = flattenPoints.Count - 1; i >= 0; --i)
            {
                flattenPoints[i].ClearAdjustValues();
            }
        }
        internal void Flatten(GlyphPartFlattener flattener)
        {
            //flatten once
            if (analyzed) return;
            //flatten each part ...
            //-------------------------------
            int j = parts.Count;
            //---------------
            List<GlyphPoint> prevResult = flattener.Result;
            flattenPoints = flattener.Result = new List<GlyphPoint>();
            //start ...
            for (int i = 0; i < j; ++i)
            {
                //flatten each part
                parts[i].Flatten(flattener);
            }

            flattener.Result = prevResult;
            analyzed = true;
        }
        public bool IsClosewise()
        {
            //after flatten
            if (analyzedClockDirection)
            {
                return isClockwise;
            }

            List<GlyphPoint> f_points = this.flattenPoints;
            if (f_points == null)
            {
                throw new NotSupportedException();
            }
            analyzedClockDirection = true;



            //---------------
            //http://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
            //check if hole or not
            //clockwise or counter-clockwise
            {
                //Some of the suggested methods will fail in the case of a non-convex polygon, such as a crescent. 
                //Here's a simple one that will work with non-convex polygons (it'll even work with a self-intersecting polygon like a figure-eight, telling you whether it's mostly clockwise).

                //Sum over the edges, (x2 − x1)(y2 + y1). 
                //If the result is positive the curve is clockwise,
                //if it's negative the curve is counter-clockwise. (The result is twice the enclosed area, with a +/- convention.)
                int j = flattenPoints.Count;
                double total = 0;
                for (int i = 1; i < j; ++i)
                {
                    GlyphPoint p0 = f_points[i - 1];
                    GlyphPoint p1 = f_points[i];

                    double x0 = p0.x;
                    double y0 = p0.y;
                    double x1 = p1.x;
                    double y1 = p1.y;

                    total += (x1 - x0) * (y1 + y0);
                    i += 2;
                }
                //the last one
                {
                    GlyphPoint p0 = f_points[j - 1];
                    GlyphPoint p1 = f_points[0];

                    double x0 = p0.x;
                    double y0 = p0.y;
                    double x1 = p1.x;
                    double y1 = p1.y;
                    total += (x1 - x0) * (y1 + y0);
                }
                isClockwise = total >= 0;
            }
            return isClockwise;
        }
    }

    public enum GlyphPartKind
    {
        Unknown,
        Line,
        Curve3,
        Curve4
    }

    public class GlyphPartFlattener
    {
        List<GlyphPoint> points;

        public GlyphPartFlattener()
        {
            this.NSteps = 2;//default
        }
        public List<GlyphPoint> Result
        {
            get { return points; }
            set { points = value; }
        }
        public int NSteps { get; set; }

        void AddPoint(float x, float y, PointKind kind)
        {
            var p = new GlyphPoint(x, y, kind);
#if DEBUG
            p.dbugOwnerPart = dbug_ownerPart;
#endif
            points.Add(p);
        }

        public void GeneratePointsFromLine(
           Vector2 start, Vector2 end)
        {
            if (points.Count == 0)
            {
                AddPoint(start.X, start.Y, PointKind.LineStart);
            }
            AddPoint(end.X, end.Y, PointKind.LineStop);
        }

        public void GeneratePointsFromCurve4(
            int nsteps,
            Vector2 start, Vector2 end,
            Vector2 control1, Vector2 control2)
        {
            var curve = new BezierCurveCubic( //Cubic curve -> curve4
                start, end,
                control1, control2);
            if (points.Count == 0)
            {
                AddPoint(start.X, start.Y, PointKind.C4Start);
            }
            float eachstep = (float)1 / nsteps;
            float stepSum = eachstep;//start
            for (int i = 1; i < nsteps; ++i)
            {
                //start at i=1, this will not include the last step that stepSum=1
                Vector2 vector2 = curve.CalculatePoint(stepSum);
                AddPoint(vector2.X, vector2.Y, PointKind.CurveInbetween);
                stepSum += eachstep;
            }

            AddPoint(end.X, end.Y, PointKind.C4End);
        }
        public void GeneratePointsFromCurve3(
            int nsteps,
            Vector2 start, Vector2 end,
            Vector2 control1)
        {
            var curve = new BezierCurveQuadric( //Quadric curve-> curve3
                start, end,
                control1);
            if (points.Count == 0)
            {
                AddPoint(start.X, start.Y, PointKind.C3Start);
            }
            float eachstep = (float)1 / nsteps;
            float stepSum = eachstep;//start
            for (int i = 1; i < nsteps; ++i)
            {
                //start at i=1, this will not include the last step that stepSum=1
                Vector2 vector2 = curve.CalculatePoint(stepSum);
                AddPoint(vector2.X, vector2.Y, PointKind.CurveInbetween);
                stepSum += eachstep;
            }
            AddPoint(end.X, end.Y, PointKind.C3End);
        }




#if DEBUG
        GlyphPart dbug_ownerPart;
        public void dbugSetCurrentOwnerPart(GlyphPart dbug_ownerPart)
        {
            this.dbug_ownerPart = dbug_ownerPart;
        }
#endif
    }
    public abstract class GlyphPart
    {
        float _x0, _y0;
        public Vector2 FirstPoint
        {
            get
            {
                if (PrevPart != null)
                {
                    return PrevPart.GetLastPoint();
                }
                else
                {
                    return new Vector2(_x0, _y0);
                }
            }
            protected set
            {
                this._x0 = value.X;
                this._y0 = value.Y;
            }
        }
        public abstract GlyphPartKind Kind { get; }
        public GlyphPart NextPart { get; set; }
        public GlyphPart PrevPart { get; set; }
        public abstract void Flatten(GlyphPartFlattener flattener);

        public abstract Vector2 GetLastPoint();


#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
        public GlyphPart()
        {
            //if (this.dbugId == 16)
            //{
            //}
        }
#endif
    }

    public enum PointKind : byte
    {
        LineStart,
        LineStop,
        //
        C3Start,
        C3Control1,
        C3End,
        //
        C4Start,
        C4Control1,
        C4Control2,
        C4End,

        CurveInbetween,
    }

    public class GlyphPoint
    {
        public readonly float x;
        public readonly float y;
        public readonly PointKind kind;

        // 
        float _adjX;
        float _adjY;
        //
        public bool isPartOfHorizontalEdge;
        public bool isUpperSide;
        public EdgeLine horizontalEdge;


        //TODO: remove this
        List<EdgeLine> _edges;
        internal List<GlyphBoneJoint> _assocJoints; //associatedJoints


#if DEBUG
        //for debug only
        public GlyphPart dbugOwnerPart;  //link back to owner part
        public Poly2Tri.TriangulationPoint dbugTriangulationPoint;
        Dictionary<GlyphBoneJoint, bool> dbug_jointDic;
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public GlyphPoint(float x, float y, PointKind kind)
        {
            this.x = x;
            this.y = y;
            this.kind = kind;
        }
        public bool IsEqualValues(GlyphPoint another)
        {
            return x == another.x && y == another.y;
        }
        public float AdjustedY
        {
            get { return _adjY; }
            internal set
            {
                _adjY = value;
            }
        }
        public float AdjustedX
        {
            get { return _adjX; }
            internal set
            {
                _adjX = value;
            }
        }
        internal void ClearAdjustValues()
        {
            _adjX = _adjY = 0;
        }
        internal void AddVerticalEdge(EdgeLine v_edge)
        {
            //associated 
            if (!this.IsPartOfVerticalEdge)
            {
                this.IsPartOfVerticalEdge = true;
            }
            if (!this.IsLeftSide)
            {
                this.IsLeftSide = v_edge.IsLeftSide;
            }

            if (_edges == null)
            {
                _edges = new List<EdgeLine>();
            }
            _edges.Add(v_edge);
        }
        internal EdgeLine GetMatchingVerticalEdge()
        {
            if (_edges == null)
            {
                return null;
            }
            if (_edges.Count == 1)
            {
                return _edges[0].GetMatchingOutsideEdge();
            }
            else
            {
                return null;
            }
        }

        internal void AddAssociatedBoneJoint(GlyphBoneJoint joint)
        {
            if (_assocJoints == null)
            {
                _assocJoints = new List<GlyphBoneJoint>();
#if DEBUG
                dbug_jointDic = new Dictionary<GlyphBoneJoint, bool>();
#endif
            }
            //
            //if (_assocJoints.Count > 0)
            //{

            //}
#if DEBUG
            if (dbug_jointDic.ContainsKey(joint))
            {
                return;
            }
            dbug_jointDic.Add(joint, true);
#endif

            _assocJoints.Add(joint);

        }

        public bool IsLeftSide { get; private set; }
        public bool IsPartOfVerticalEdge { get; private set; }
#if DEBUG
        public override string ToString()
        {
            return dbugId + " :" + ((AdjustedY != 0) ? "***" : "") +
                    (x + "," + y + " " + kind.ToString());
        }
#endif 
    }




    public class GlyphLine : GlyphPart
    {

        public float x1;
        public float y1;

        public GlyphLine(float x0, float y0, float x1, float y1)
        {
            this.FirstPoint = new Vector2(x0, y0);
            this.x1 = x1;
            this.y1 = y1;
        }
        public GlyphLine(GlyphPart prevPart, float x1, float y1)
        {
            //this.x0 = x0;
            //this.y0 = y0;
            this.PrevPart = prevPart;
            this.x1 = x1;
            this.y1 = y1;
        }
        public override Vector2 GetLastPoint()
        {
            return new Vector2(x1, y1);
        }
        public override void Flatten(GlyphPartFlattener flattener)
        {
#if DEBUG
            flattener.dbugSetCurrentOwnerPart(this);
#endif
            flattener.GeneratePointsFromLine(
                this.FirstPoint,
                new Vector2(x1, y1));
        }

        public override GlyphPartKind Kind { get { return GlyphPartKind.Line; } }

#if DEBUG
        public override string ToString()
        {
            return "L(" + this.FirstPoint + "), (" + x1 + "," + y1 + ")";
        }
#endif
    }
    public class GlyphCurve3 : GlyphPart
    {
        public float x1, y1, x2, y2;
        public GlyphCurve3(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            this.FirstPoint = new Vector2(x0, y0);
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        public GlyphCurve3(GlyphPart prevPart, float x1, float y1, float x2, float y2)
        {
            this.PrevPart = prevPart;
            //this.x0 = x0;
            //this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        public override Vector2 GetLastPoint()
        {
            return new Vector2(x2, y2);
        }
        public override void Flatten(GlyphPartFlattener flattener)
        {
#if DEBUG
            flattener.dbugSetCurrentOwnerPart(this);
#endif
            flattener.GeneratePointsFromCurve3(
                flattener.NSteps,
                this.FirstPoint, //first
                new Vector2(x2, y2), //end
                new Vector2(x1, y1)); //control1
        }

        public override GlyphPartKind Kind { get { return GlyphPartKind.Curve3; } }
#if DEBUG
        public override string ToString()
        {
            return "C3(" + this.FirstPoint + "), (" + x1 + "," + y1 + "),(" + x2 + "," + y2 + ")";
        }
#endif
    }
    public class GlyphCurve4 : GlyphPart
    {
        public float x1, y1, x2, y2, x3, y3;

        public GlyphCurve4(float x0, float y0, float x1, float y1,
            float x2, float y2,
            float x3, float y3)
        {
            this.FirstPoint = new Vector2(x0, y0);
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.x3 = x3;
            this.y3 = y3;
        }
        public GlyphCurve4(GlyphPart prevPart, float x1, float y1,
         float x2, float y2,
         float x3, float y3)
        {
            //this.x0 = x0;
            //this.y0 = y0;
            this.PrevPart = prevPart;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.x3 = x3;
            this.y3 = y3;
        }
        public override Vector2 GetLastPoint()
        {
            return new Vector2(x3, y3);
        }
        public override void Flatten(GlyphPartFlattener flattener)
        {
#if DEBUG
            flattener.dbugSetCurrentOwnerPart(this);
#endif
            flattener.GeneratePointsFromCurve4(
                flattener.NSteps,
                this.FirstPoint,    //first
                new Vector2(x3, y3), //end
                new Vector2(x1, y1), //control1
                new Vector2(x2, y2) //control2
                );
        }

        public override GlyphPartKind Kind { get { return GlyphPartKind.Curve4; } }
#if DEBUG
        public override string ToString()
        {
            return "C4(" + this.FirstPoint + "), (" + x1 + "," + y1 + "),(" + x2 + "," + y2 + "), (" + x3 + "," + y3 + ")";
        }
#endif

    }


}