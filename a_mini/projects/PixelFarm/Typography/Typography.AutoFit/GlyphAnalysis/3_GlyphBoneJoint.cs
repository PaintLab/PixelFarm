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

        public readonly EdgeLine _p_contact_edge;
        public readonly EdgeLine _q_contact_edge;
        GlyphCentroidPair _owner;

        //one bone joint can have up to 2 tips  
        EdgeLine _tipEdge_p;
        EdgeLine _tipEdge_q;
#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        public static int dbugTotalId;
#endif
        internal GlyphBoneJoint(GlyphCentroidPair owner,
            EdgeLine p_contact_edge,
            EdgeLine q_contact_edge)
        {
            //both p and q is INSIDE, contact edge
            this._p_contact_edge = p_contact_edge;
            this._q_contact_edge = q_contact_edge;
            this._owner = owner;
            //---------------------------

            

            //---------------------------
        }

        /// <summary>
        /// get position of this bone joint (mid point of the edge)
        /// </summary>
        /// <returns></returns>
        public Vector2 Position
        {
            get
            {
                //mid point of the contact edge line
                return _p_contact_edge.GetMidPoint();
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

            Vector2 contactPoint = this.Position;
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
        //
        internal GlyphCentroidPair OwnerCentrodPair
        {
            get { return _owner; }
        }
        internal bool ComposeOf(GlyphTriangle tri)
        {
            return this._owner.p == tri || this._owner.q == tri;
        }

#if DEBUG
        public override string ToString()
        {
            return "id:" + dbugId + " " + this.Position.ToString();
        }
#endif

    }

}
