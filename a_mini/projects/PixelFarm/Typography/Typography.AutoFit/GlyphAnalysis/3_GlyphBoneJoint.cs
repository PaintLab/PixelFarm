//MIT, 2017, WinterDev

using System.Numerics;

namespace Typography.Rendering
{

    public class GlyphBoneJoint
    {

        //A GlyphBoneJoint is on a midpoint of two 'inside' adjacent edges.
        //(2 contact edges)
        //of 2 triangles,      
        //(_p_contact_edge, _q_contact_edge)

        internal readonly EdgeLine _p_contact_edge;
        internal readonly EdgeLine _q_contact_edge;
        //one bone joint can have up to 2 tips  
        EdgeLine _tipEdge_p;
        EdgeLine _tipEdge_q;

        //temp 
        float _fitX, _fitY;

#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        public static int dbugTotalId;
#endif
        internal GlyphBoneJoint(
            EdgeLine p_contact_edge,
            EdgeLine q_contact_edge)
        {

            //both p and q is INSIDE, contact edge
            this._p_contact_edge = p_contact_edge;
            this._q_contact_edge = q_contact_edge;
            //this is original x,y
            Vector2 midpos = p_contact_edge.GetMidPoint();
            this._fitX = midpos.X;
            this._fitY = midpos.Y;

#if DEBUG
            if (p_contact_edge.inside_joint != null ||
                q_contact_edge.inside_joint != null)
            {
                throw new System.NotSupportedException();
            }
#endif
            p_contact_edge.inside_joint = this;
            q_contact_edge.inside_joint = this;
        }
        /// <summary>
        /// dynamic fit x
        /// </summary>
        public float FitX
        {
            get { return _fitX; }
        }
        /// <summary>
        /// dynamic fit y
        /// </summary>
        public float FitY
        {
            get { return _fitY; }
        }
        internal void SetFitXY(float newx, float newy)
        {
            this._fitX = newx;
            this._fitY = newy;
        }
        internal GlyphTriangle P_Tri
        {
            get
            {
                return _p_contact_edge.OwnerTriangle;
            }
        }
        internal GlyphTriangle Q_Tri
        {
            get
            {
                return _q_contact_edge.OwnerTriangle;
            }
        }


        /// <summary>
        /// get position of this bone joint (mid point of the edge)
        /// </summary>
        /// <returns></returns>
        public Vector2 OriginalJointPos
        {
            get
            {
                //mid point of the contact edge line
                return _p_contact_edge.GetMidPoint();
            }
        }
        public Vector2 DynamicFitPos
        {
            get
            {
                //mid point of the contact edge line
                return new Vector2(_fitX, _fitY);
            }
        }
        public float GetLeftMostRib()
        {
            //TODO: revisit this again

            return 0;
        }
        /// <summary>
        /// calculate distance^2 from contact point to specific point v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double CalculateSqrDistance(Vector2 v)
        {

            Vector2 contactPoint = this.OriginalJointPos;
            float xdiff = contactPoint.X - v.X;
            float ydiff = contactPoint.Y - v.Y;

            return (xdiff * xdiff) + (ydiff * ydiff);
        }
        internal void SetTipEdge_P(EdgeLine e)
        {
#if DEBUG
            if (_tipEdge_p != null)
            {
                throw new System.NotSupportedException();
            }
#endif
            e.IsTip = true;
            this._tipEdge_p = e;
        }
        internal void SetTipEdge_Q(EdgeLine e)
        {
#if DEBUG
            if (_tipEdge_q != null)
            {
                throw new System.NotSupportedException();
            }
            if (_tipEdge_q != null && _tipEdge_q == _tipEdge_p)
            {
                throw new System.NotSupportedException();
            }
#endif
            e.IsTip = true;
            this._tipEdge_q = e;

        }
        public bool HasTipP
        {
            get { return this._tipEdge_p != null; }
        }
        public bool HasTipQ
        {
            get { return this._tipEdge_q != null; }
        }
        public Vector2 TipPointP { get { return _tipEdge_p.GetMidPoint(); } }
        public EdgeLine TipEdgeP { get { return _tipEdge_p; } }

        public Vector2 TipPointQ { get { return _tipEdge_q.GetMidPoint(); } }
        public EdgeLine TipEdgeQ { get { return _tipEdge_q; } }

        internal bool ComposeOf(GlyphTriangle tri)
        {
            return this.P_Tri == tri || this.Q_Tri == tri;
        }

#if DEBUG
        public override string ToString()
        {
            return "id:" + dbugId + " " + this.OriginalJointPos.ToString();
        }

        public EdgeLine dbugGetEdge_P() { return _p_contact_edge; }
        public EdgeLine dbugGetEdge_Q() { return _q_contact_edge; }


        public void dbugGetCentroidBoneCenters(out double cx0, out double cy0, out double cx1, out double cy1)
        {

            //for debug
            GlyphTriangle p_tri = this.P_Tri;
            cx0 = p_tri.CentroidX;
            cy0 = p_tri.CentroidY;
            GlyphTriangle q_tri = this.Q_Tri;
            cx1 = q_tri.CentroidX;
            cy1 = q_tri.CentroidY;
        }
#endif

    }



    static class GlyphBoneJointExtensions
    {
        /// <summary>
        /// distribute associate glyph bone to end point of this joint
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="bone"></param>
        public static void AddAssociateGlyphBoneToEndPoint(this GlyphBoneJoint joint, GlyphBone bone)
        {
            //_p_contact_edge and _q_contact_edge share glyph end (glyph) points
            //so we select only 1 (to p)            

            AddAssociateGlyphBoneToEndPoint(joint._p_contact_edge, bone);
        }
        public static void AddAssociateGlyphBoneToEndPoint(this EdgeLine edge, GlyphBone bone)
        {
            //_p_contact_edge and _q_contact_edge share glyph end (glyph) points
            //so we select only 1 (to p)
            edge.GlyphPoint_P.AddAssociateBone(bone);
            edge.GlyphPoint_Q.AddAssociateBone(bone);
        }
    }
}
