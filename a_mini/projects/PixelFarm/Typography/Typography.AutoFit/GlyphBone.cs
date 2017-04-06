//MIT, 2017, WinterDev
using System;
using System.Numerics;

namespace Typography.Rendering
{

    /// <summary>
    /// link between 2 GlyphBoneJoint
    /// </summary>
    public class GlyphBone
    {
        public readonly EdgeLine TipEdge;
        public readonly GlyphBoneJoint JointA;
        public readonly GlyphBoneJoint JointB;

        public GlyphBone(GlyphBoneJoint a, GlyphBoneJoint b)
        {
            if (a == b)
            {

            }
            JointA = a;
            JointB = b;
        }
        public GlyphBone(GlyphBoneJoint a, EdgeLine tipEdge)
        {
            JointA = a;
            TipEdge = tipEdge;
        }
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
        public Vector2 Position
        {
            get
            {
                //mid point of the edge line
                return _p_contact_edge.GetMidPoint();
            }
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

            Vector2 contactPoint = this.Position;
            float xdiff = contactPoint.X - v.X;
            float ydiff = contactPoint.Y - v.Y;

            return (xdiff * xdiff) + (ydiff * ydiff);
        }


        short _ribCount;

        Vector2 _ribEndPoint_A, _ribEndPoint_B;
        /// <summary>
        /// tip point (mid of tip edge)
        /// </summary>
        Vector2 _tipPoint;

        EdgeLine _selectedEdgeA, _selectedEdgeB, _selectedTipEdge;

        public void AddRibEndAt(EdgeLine edgeLine, Vector2 vec)
        {
            switch (_ribCount)
            {
                //not more thar2
                default: throw new NotSupportedException();
                case 0:
                    _selectedEdgeA = edgeLine;
                    _ribEndPoint_A = vec;
                    break;
                case 1:
                    _selectedEdgeB = edgeLine;
                    _ribEndPoint_B = vec;
                    break;
            }
            _ribCount++;
        }
        public void SetTipEdge(EdgeLine tipEdge)
        {
            this._selectedTipEdge = tipEdge;
            this._tipPoint = tipEdge.GetMidPoint();
        }

        public short SelectedEdgePointCount { get { return _ribCount; } }
        public Vector2 RibEndPointA { get { return _ribEndPoint_A; } }
        public Vector2 RibEndPointB { get { return _ribEndPoint_B; } }
        public Vector2 TipPoint { get { return _tipPoint; } }
        public EdgeLine RibEndEdgeA { get { return _selectedEdgeA; } }
        public EdgeLine RibEndEdgeB { get { return _selectedEdgeB; } }
        public EdgeLine TipEdge { get { return _selectedTipEdge; } }
    }



}