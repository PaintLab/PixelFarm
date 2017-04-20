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

        public EdgeLine _p_contact_edge;
        public EdgeLine _q_contact_edge;
        GlyphCentroidPair _owner;
        /// <summary>
        /// tip point (mid of tip edge)
        /// </summary>
        Vector2 _tipPoint;
        //one bone joint can have up to 2 tips  
        EdgeLine _selectedTipEdge;

#if DEBUG
        public readonly int dbugId = dbugTotalId++;
        public static int dbugTotalId;
#endif
        internal GlyphBoneJoint(GlyphCentroidPair owner,
            EdgeLine p_contact_edge,
            EdgeLine q_contact_edge)
        {
            this._p_contact_edge = p_contact_edge;
            this._q_contact_edge = q_contact_edge;
            this._owner = owner;
        }

        /// <summary>
        /// get position of this bone joint (mid point of the edge)
        /// </summary>
        /// <returns></returns>
        public Vector2 Position
        {
            get
            {
                //mid point of the edge line
                return _p_contact_edge.GetMidPoint();
            }
        }
        internal GlyphCentroidPair OwnerCentrodPair
        {
            get { return _owner; }
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
        public void SetTipEdge(EdgeLine tipEdge)
        {
            this._selectedTipEdge = tipEdge;
            this._tipPoint = tipEdge.GetMidPoint();
        }
        public Vector2 TipPoint { get { return _tipPoint; } }
        public EdgeLine TipEdge { get { return _selectedTipEdge; } } 
#if DEBUG
        public override string ToString()
        {
            return "id:" + dbugId + " " + this.Position.ToString();
        }
#endif

    }

}
