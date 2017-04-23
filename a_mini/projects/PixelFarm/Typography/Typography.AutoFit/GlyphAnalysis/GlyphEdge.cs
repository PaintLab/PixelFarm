//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Typography.Rendering
{
    public class GlyphEdge
    {
        internal readonly EdgeLine _edgeLine;
        public readonly GlyphPoint _P;
        public readonly GlyphPoint _Q;

        float _relativeDistance = 1;
        Vector2 _o_edgeVector; //original edge vector 
        Vector2 _bone_midPoint;
        double _originalDistanceToBone;
        //-----------

        Vector2 _bone_to_edgeVector;//perpendicular line
        Vector2 _newEdgeCutPoint;



        internal GlyphEdge(GlyphPoint p0, GlyphPoint p1, EdgeLine edgeLine)
        {
            this._P = p0;
            this._Q = p1;
            this._edgeLine = edgeLine;
            //----------- 
            _o_edgeVector = new Vector2((float)(p1.x - p0.x), (float)(p1.y - p0.y));
#if DEBUG
            edgeLine.dbugGlyphEdge = this;
#endif
            //if (edgeLine.PerpendicularBone == null)
            //{
            //    return;
            //} 
            //_bone_midPoint = edgeLine.PerpendicularBone.GetMidPoint(); 
            //_bone_to_edgeVector = _edgeLine.PerpendicularBone.cutPoint_onEdge - _bone_midPoint;
            //_originalDistanceToBone = _bone_to_edgeVector.Length(); 
            //ApplyNewEdgeDistance(1); 
        }
        internal void FindPerpendicularBone(List<GlyphBone> newBones)
        {
            GlyphTriangle ownerTri = this._edgeLine.OwnerTriangle;

            //GlyphCentroidLine ownerCentroidLine = this._edgeLine.OwnerTriangle.OwnerCentroidLine;



            //we try to find a perpedicular bone from 
            //from edgeLine
            //int j = newBones.Count;
            //Vector2 midEdge = this._edgeLine.GetMidPoint();
            //Vector2 midEdge = new Vector2(this._P.x, this._P.y);

            //we find cut bone from owner Centroid Line
            //(not cross centroid line)



            //List<BoneAndCutPoint> foundCutPoints = new List<BoneAndCutPoint>();
            //for (int i = 0; i < j; ++i)
            //{
            //    //find perpendicular cutpoint from midEdge to the bone
            //    Vector2 cutPoint;
            //    GlyphBone bone = newBones[i];
            //    if (MyMath.FindPerpendicularCutPoint(bone, midEdge, out cutPoint))
            //    {
            //        BoneAndCutPoint found = new BoneAndCutPoint();
            //        found.bone = bone;
            //        found.cutPoint = cutPoint;
            //        foundCutPoints.Add(found);
            //    }
            //}
            //if (foundCutPoints.Count > 1)
            //{
            //    //find min
            //    double min = Double.MaxValue;
            //    int minAt = -1;
            //    int n = foundCutPoints.Count;
            //    for (int i = 0; i < n; ++i)
            //    {
            //        BoneAndCutPoint cut = foundCutPoints[i];
            //        double sqLen = MyMath.SquareDistance(cut.cutPoint, midEdge);
            //        if (sqLen < min)
            //        {
            //            minAt = i;
            //            min = sqLen;
            //        }
            //    }

            //    BoneAndCutPoint found = foundCutPoints[minAt];
            //    RelatedBone = found.bone;
            //    RelatedBoneCutPoint = found.cutPoint;

            //}
            //else if (foundCutPoints.Count == 1)
            //{
            //    BoneAndCutPoint found = foundCutPoints[0];
            //    RelatedBone = found.bone;
            //    RelatedBoneCutPoint = found.cutPoint;
            //}
            //else
            //{
            //    //not found
            //    //any perpedicular bone
            //    this._edgeLine.dbugNoPerpendicularBone = true;
            //}
        }
        public GlyphBone RelatedBone { get; set; }
        public Vector2 RelatedBoneCutPoint { get; set; }

        internal void ApplyNewEdgeDistance(float newRelativeDistance)
        {
            _relativeDistance = newRelativeDistance;
            //find new edge end point 
            Vector2 newBoneToEdgeVector = _bone_to_edgeVector.NewLength(_originalDistanceToBone * newRelativeDistance);
            _bone_to_edgeVector = newBoneToEdgeVector;
            _newEdgeCutPoint = _bone_midPoint + _bone_to_edgeVector;
        }
        internal static void FindCutPoint(GlyphEdge e0, GlyphEdge e1)
        {
            //find cutpoint from e0.q to e1.p 
            //new sample
            Vector2 tmp_e0_q = e0._newEdgeCutPoint + e0._o_edgeVector;
            Vector2 tmp_e1_p = e1._newEdgeCutPoint - e1._o_edgeVector;
            Vector2 cutpoint = FindCutPoint(e0._newEdgeCutPoint, tmp_e0_q, e1._newEdgeCutPoint, tmp_e1_p);

            e0._Q.newX = e1._P.newX = cutpoint.X;
            e0._Q.newY = e1._P.newY = cutpoint.Y;
        }
        static Vector2 FindCutPoint(
            Vector2 p0, Vector2 p1,
            Vector2 p2, Vector2 p3)
        {
            //find cut point of 2 line 
            //y = mx + b
            //from line equation
            //y = mx + b ... (1)
            //from (1)
            //b = y- mx ... (2) 
            //----------------------------------
            //line1:
            //y1 = (m1 * x1) + b1 ...(3)            
            //line2:
            //y2 = (m2 * x2) + b2 ...(4)
            //----------------------------------
            //from (3),
            //b1 = y1 - (m1 * x1) ...(5)
            //b2 = y2 - (m2 * x2) ...(6)
            //----------------------------------
            //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
            //or find (x,y) where (3)==(4)
            //---------------------------------- 
            //at cutpoint, find x
            // (m1 * x1) + b1 = (m2 * x1) + b2  ...(11), replace x2 with x1
            // (m1 * x1) - (m2 * x1) = b2 - b1  ...(12)
            //  x1 * (m1-m2) = b2 - b1          ...(13)
            //  x1 = (b2-b1)/(m1-m2)            ...(14), now we know x1
            //---------------------------------- 
            //at cutpoint, find y
            //  y1 = (m1 * x1) + b1 ... (15), replace x1 with value from (14)
            //Ans: (x1,y1)
            //----------------------------------

            double y1diff = p1.Y - p0.Y;
            double x1diff = p1.X - p0.X;


            if (x1diff == 0)
            {
                //90 or 180 degree
                return new Vector2(p1.X, p2.Y);
            }
            //------------------------------
            //
            //find slope 
            double m1 = y1diff / x1diff;
            //from (2) b = y-mx, and (5)
            //so ...
            double b1 = p0.Y - (m1 * p0.X);

            //------------------------------
            double y2diff = p3.Y - p2.Y;
            double x2diff = p3.X - p2.X;
            double m2 = y2diff / x2diff;

            // 
            //from (6)
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point

            //check if (m1-m2 !=0)
            double cutx = (b2 - b1) / (m1 - m2); //from  (14)
            double cuty = (m1 * cutx) + b1;  //from (15)
            return new Vector2((float)cutx, (float)cuty);

        }
        static Vector2 FindCutPoint(Vector2 p0, Vector2 p1, Vector2 p2, float cutAngle)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1

            //from line equation
            //y = mx + b ... (1)
            //from (1)
            //b = y- mx ... (2) 
            //----------------------------------
            //line1:
            //y1 = (m1 * x1) + b1 ...(3)            
            //line2:
            //y2 = (m2 * x2) + b2 ...(4)
            //----------------------------------
            //from (3),
            //b1 = y1 - (m1 * x1) ...(5)
            //b2 = y2 - (m2 * x2) ...(6)
            //----------------------------------
            //y1diff = p1.Y-p0.Y  ...(7)
            //x1diff = p1.X-p0.X  ...(8)
            //
            //m1 = (y1diff/x1diff) ...(9)
            //m2 = cutAngle of m1 ...(10)
            //
            //replace value (x1,y1) and (x2,y2)
            //we know b1 and b2         
            //----------------------------------              
            //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
            //or find (x,y) where (3)==(4)
            //---------------------------------- 
            //at cutpoint, find x
            // (m1 * x1) + b1 = (m2 * x1) + b2  ...(11), replace x2 with x1
            // (m1 * x1) - (m2 * x1) = b2 - b1  ...(12)
            //  x1 * (m1-m2) = b2 - b1          ...(13)
            //  x1 = (b2-b1)/(m1-m2)            ...(14), now we know x1
            //---------------------------------- 
            //at cutpoint, find y
            //  y1 = (m1 * x1) + b1 ... (15), replace x1 with value from (14)
            //Ans: (x1,y1)
            //---------------------------------- 

            double y1diff = p1.Y - p0.Y;
            double x1diff = p1.X - p0.X;

            if (x1diff == 0)
            {
                //90 or 180 degree
                return new Vector2(p1.X, p2.Y);
            }
            //------------------------------
            //
            //find slope 
            double m1 = y1diff / x1diff;
            //from (2) b = y-mx, and (5)
            //so ...
            double b1 = p0.Y - (m1 * p0.X);
            // 
            //from (10)
            //double invert_m = -(1 / slope_m);
            //double m2 = -1 / m1;   //rotate m1
            //---------------------
            double angle = Math.Atan2(y1diff, x1diff); //rad in degree 
                                                       //double m2 = -1 / m1;

            double m2 = cutAngle == 90 ?
                //short cut
                (-1 / m1) :
                //or 
                Math.Tan(
                //radial_angle of original line + radial of cutAngle
                //return new line slope
                Math.Atan2(y1diff, x1diff) +
                MyMath.DegreesToRadians(cutAngle)); //new m 
            //---------------------


            //from (6)
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point

            //check if (m1-m2 !=0)
            double cutx = (b2 - b1) / (m1 - m2); //from  (14)
            double cuty = (m1 * cutx) + b1;  //from (15)
            return new Vector2((float)cutx, (float)cuty);
            //------
            //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
            //or find (x,y) where (3)==(4)
            //-----
            //if (3)==(4)
            //(m1 * x1) + b1 = (m2 * x2) + b2;
            //from given p0 and p1,
            //now we know m1 and b1, ( from (2),  b1 = y1-(m1*x1) )
            //and we now m2 since => it is a 90 degree of m1.
            //and we also know x2, since at the cut point x2 also =x1
            //now we can find b2...
            // (m1 * x1) + b1 = (m2 * x1) + b2  ...(5), replace x2 with x1
            // b2 = (m1 * x1) + b1 - (m2 * x1)  ...(6), move  (m2 * x1)
            // b2 = ((m1 - m2) * x1) + b1       ...(7), we can find b2
            //---------------------------------------------
        }

        public Vector2 CutPoint_P
        {
            get { return new Vector2(_P.newX, _P.newY); }
        }
        public Vector2 CutPoint_Q
        {
            get { return new Vector2(_Q.newX, _Q.newY); }
        }
    }

}