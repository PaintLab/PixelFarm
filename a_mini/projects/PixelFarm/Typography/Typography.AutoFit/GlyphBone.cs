//MIT, 2017, WinterDev
using System;
using System.Numerics;

namespace Typography.Rendering
{

    /// <summary>
    /// line link between 2 centroid contact point
    /// </summary>
    public class GlyphVirtualBone
    {
        //TODO: rename to glyph bone

    }

    public class GlyphBoneJoint
    {
        //Bone joint is create by 2 connected (contact) 'inside' EdgeLines
        //(_p_contact_edge, _q_contact_edge)


        public EdgeLine _p_contact_edge;
        public EdgeLine _q_contact_edge;
        GlyphCentroidLine _owner;
        public GlyphBoneJoint(GlyphCentroidLine owner,
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
        public Vector2 GetPos()
        {
            //mid point of the edge line
            return _p_contact_edge.GetMidPoint();
        }
        public GlyphCentroidLine OwnerCentroidLine
        {
            get { return _owner; }
        }
        /// <summary>
        /// calculate distance^2 from contact point to specific point v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double CalculateSqrDistance(Vector2 v)
        {

            Vector2 contactPoint = this.GetPos();
            float xdiff = contactPoint.X - v.X;
            float ydiff = contactPoint.Y - v.Y;

            return (xdiff * xdiff) + (ydiff * ydiff);
        }


        short _selEdgePointCount;
        Vector2 _selectedEdgePoint_A, _selectedEdgePoint_B, _tip;
        EdgeLine _selectedEdgeA, _selectedEdgeB, _selectedTipEdge;

        public void AddSelectedEdgePoint(EdgeLine edgeLine, Vector2 vec)
        {   
            
            switch (_selEdgePointCount)
            {
                //not more thar2
                default: throw new NotSupportedException();
                case 0:
                    _selectedEdgeA = edgeLine;
                    _selectedEdgePoint_A = vec;
                    break;
                case 1:
                    _selectedEdgeB = edgeLine;
                    _selectedEdgePoint_B = vec;
                    break;
            }
            _selEdgePointCount++;
        }
        public void SetTipEdge(EdgeLine tipEdge)
        {
            this._selectedTipEdge = tipEdge;
            this._tip = tipEdge.GetMidPoint();
        }

        public short SelectedEdgePointCount { get { return _selEdgePointCount; } }
        public Vector2 SelectedEdgeA { get { return _selectedEdgePoint_A; } }
        public Vector2 SelectedEdgeB { get { return _selectedEdgePoint_B; } }
        public Vector2 TipPoint { get { return _tip; } }
    }



}