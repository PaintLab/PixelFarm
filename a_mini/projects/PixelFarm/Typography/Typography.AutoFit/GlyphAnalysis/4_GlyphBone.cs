//MIT, 2017, WinterDev
using System;
using System.Numerics;

namespace Typography.Rendering
{

    /// <summary>
    /// link between 2 GlyphBoneJoint or Joint and tipEdge
    /// </summary>
    public class GlyphBone
    {
        public readonly EdgeLine TipEdge;
        public readonly GlyphBoneJoint JointA;
        public readonly GlyphBoneJoint JointB;
        double _len;

#if DEBUG 
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public GlyphBone(GlyphBoneJoint a, GlyphBoneJoint b)
        {
#if DEBUG
            if (a == b)
            {
                throw new NotSupportedException();
            }
#endif

            JointA = a;
            JointB = b;
            Vector2 bpos = b.Position;
            _len = Math.Sqrt(a.CalculateSqrDistance(bpos));
            EvaluteSlope(a.Position, bpos);

            a.AddAssociateGlyphBoneToEndPoint(this);
            b.AddAssociateGlyphBoneToEndPoint(this);
        }
        public GlyphBone(GlyphBoneJoint a, EdgeLine tipEdge)
        {
            JointA = a;
            TipEdge = tipEdge;

            this.IsTipBone = true;
            var midPoint = tipEdge.GetMidPoint();
            _len = Math.Sqrt(a.CalculateSqrDistance(midPoint));
            EvaluteSlope(a.Position, midPoint);

            a.AddAssociateGlyphBoneToEndPoint(this);
        }
        public bool IsTipBone
        {
            get;
            private set;
        }
        public bool IsLinkBack
        {
            get;
            set;
        }

        void EvaluteSlope(Vector2 p, Vector2 q)
        {

            double x0 = p.X;
            double y0 = p.Y;
            //q
            double x1 = q.X;
            double y1 = q.Y;

            SlopeAngleNoDirection = Math.Abs(Math.Atan2(Math.Abs(y1 - y0), Math.Abs(x1 - x0)));
            if (x1 == x0)
            {
                this.SlopeKind = LineSlopeKind.Vertical;
            }
            else
            {
                if (SlopeAngleNoDirection > MyMath._85degreeToRad)
                {
                    SlopeKind = LineSlopeKind.Vertical;
                }
                else if (SlopeAngleNoDirection < MyMath._03degreeToRad) //_15degreeToRad
                {
                    SlopeKind = LineSlopeKind.Horizontal;
                }
                else
                {
                    SlopeKind = LineSlopeKind.Other;
                }
            }
        }
        internal double SlopeAngleNoDirection { get; set; }
        public LineSlopeKind SlopeKind { get; set; }
        internal double Length
        {
            get
            {
                return _len;
            }
        }
        public bool IsLongBone { get; internal set; }

        //--------
        public float LeftMostPoint()
        {
            if (JointB != null)
            {
                //compare joint A and B 
                if (JointA.Position.X < JointB.Position.X)
                {
                    return JointA.GetLeftMostRib();
                }
                else
                {
                    return JointB.GetLeftMostRib();
                }
            }
            else
            {
                return JointA.GetLeftMostRib();
            }
        }


#if DEBUG
        public override string ToString()
        {
            if (TipEdge != null)
            {
                return dbugId + ":" + JointA.ToString() + "->" + TipEdge.GetMidPoint().ToString();
            }
            else
            {
                return dbugId + ":" + JointA.ToString() + "->" + JointB.ToString();
            }
        }
#endif
    }


    public static class GlyphBoneExtensions
    {

        //utils for glyph bones
        public static Vector2 GetMidPoint(this GlyphBone bone)
        {
            if (bone.JointB != null)
            {
                return (bone.JointA.Position + bone.JointB.Position) / 2;
            }
            else if (bone.TipEdge != null)
            {
                Vector2 edge = bone.TipEdge.GetMidPoint();
                return (edge + bone.JointA.Position) / 2;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        public static Vector2 GetBoneVector(this GlyphBone bone)
        {
            if (bone.JointB != null)
            {
                return bone.JointB.Position - bone.JointA.Position;
            }
            else if (bone.TipEdge != null)
            {
                return bone.TipEdge.GetMidPoint() - bone.JointA.Position;
            }
            else
            {
                return Vector2.Zero;
            }
        }
        public static EdgeLine FindOutsideEdge(this GlyphBone bone)
        {
            if (bone.JointB != null)
            {
                GlyphTriangle commonTri = FindCommonTriangle(bone.JointA, bone.JointB);
                if (commonTri != null)
                {
                    return GetFirstFoundOutsidEdge(commonTri);
                }
            }
            else if (bone.TipEdge != null)
            {
                return FindOutsideEdge(bone.JointA, bone.TipEdge);
            }

            return null;

        }
        static EdgeLine FindOutsideEdge(GlyphBoneJoint a, EdgeLine tipEdge)
        {
            GlyphCentroidPair ownerCentroid_A = a.OwnerCentrodPair;
            if (ContainsEdge(ownerCentroid_A.p, tipEdge))
            {
                return FindAnotherOutsideEdge(ownerCentroid_A.p, tipEdge);
            }
            else if (ContainsEdge(ownerCentroid_A.q, tipEdge))
            {
                return FindAnotherOutsideEdge(ownerCentroid_A.q, tipEdge);
            }
            return null;
        }
        static EdgeLine FindAnotherOutsideEdge(GlyphTriangle tri, EdgeLine knownOutsideEdge)
        {
            if (tri.e0.IsOutside && tri.e0 != knownOutsideEdge) { return tri.e0; }
            if (tri.e1.IsOutside && tri.e1 != knownOutsideEdge) { return tri.e1; }
            if (tri.e2.IsOutside && tri.e2 != knownOutsideEdge) { return tri.e2; }
            return null;
        }
        static bool ContainsEdge(GlyphTriangle tri, EdgeLine edge)
        {
            return tri.e0 == edge || tri.e1 == edge || tri.e2 == edge;
        }
        static GlyphTriangle FindCommonTriangle(GlyphBoneJoint a, GlyphBoneJoint b)
        {
            GlyphCentroidPair centroid_pair_A = a.OwnerCentrodPair;
            GlyphCentroidPair centroid_pair_B = b.OwnerCentrodPair;
            if (centroid_pair_A.p == centroid_pair_B.p || centroid_pair_A.p == centroid_pair_B.q)
            {
                return centroid_pair_A.p;
            }
            else if (centroid_pair_A.q == centroid_pair_B.p || centroid_pair_A.q == centroid_pair_B.q)
            {
                return centroid_pair_A.q;
            }
            else
            {
                return null;
            }
        }

        static EdgeLine GetFirstFoundOutsidEdge(GlyphTriangle tri)
        {
            if (tri.e0.IsOutside) { return tri.e0; }
            if (tri.e1.IsOutside) { return tri.e1; }
            if (tri.e2.IsOutside) { return tri.e2; }
            return null; //not found               
        }
    }

}