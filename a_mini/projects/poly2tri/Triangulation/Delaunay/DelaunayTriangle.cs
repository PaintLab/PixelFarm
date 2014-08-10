//BSD 2014, WinterDev

/* Poly2Tri
 * Copyright (c) 2009-2010, Poly2Tri Contributors
 * http://code.google.com/p/poly2tri/
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Poly2Tri nor the names of its contributors may be
 *   used to endorse or promote products derived from this software without specific
 *   prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

/// Changes from the Java version
///   attributification
/// Future possibilities
///   Flattening out the number of indirections
///     Replacing arrays of 3 with fixed-length arrays?
///     Replacing bool[3] with a bit array of some sort?
///     Bundling everything into an AoS mess?
///     Hardcode them all as ABC ?

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Poly2Tri
{



    public class DelaunayTriangle
    {

        public TriangulationPoint P0, P1, P2;
        public DelaunayTriangle N0, N1, N2;

        //edge Delaunay  mark
        bool D0, D1, D2;
        //edge Constraint mark
        public bool C0, C1, C2;


        //lower 4 bits for EdgeIsDelaunay
        //next 4 bits for EdgeIsConstrained
        //int edgedNoteFlags;

        public bool IsInterior { get; set; }
        public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3)
        {

            this.P0 = p1;
            this.P1 = p2;
            this.P2 = p3;
        }
        public int IndexOf(TriangulationPoint p)
        {

            //if (TriangulationPoint.IsEqualPointCoord(P0, p)) return 0;
            //if (TriangulationPoint.IsEqualPointCoord(P1, p)) return 1;
            //if (TriangulationPoint.IsEqualPointCoord(P2, p)) return 2;

            if (P0 == p) return 0;
            if (P1 == p) return 1;
            if (P2 == p) return 2;

            throw new Exception("Calling index with a point that doesn't exist in triangle");

            return -1;
            //return -1;
            //int i = Points.IndexOf(p);
            //if (i == -1) throw new Exception("Calling index with a point that doesn't exist in triangle");
            //return i;
        }
        int InternalIndexOf(TriangulationPoint p)
        {

            if (P0 == p) return 0;
            if (P1 == p) return 1;
            if (P2 == p) return 2;

            //if (TriangulationPoint.IsEqualPointCoord(P0, p)) return 0;
            //if (TriangulationPoint.IsEqualPointCoord(P1, p)) return 1;
            //if (TriangulationPoint.IsEqualPointCoord(P2, p)) return 2;

            return -1;
            //return -1;
            //int i = Points.IndexOf(p);
            //if (i == -1) throw new Exception("Calling index with a point that doesn't exist in triangle");
            //return i;
        }
        public int IndexOf2(TriangulationPoint p)
        {
            if (P0 == p) return 0;
            if (P1 == p) return 1;
            if (P2 == p) return 2;
            //if (TriangulationPoint.IsEqualPointCoord(P0, p)) return 0;
            //if (TriangulationPoint.IsEqualPointCoord(P1, p)) return 1;
            //if (TriangulationPoint.IsEqualPointCoord(P2, p)) return 2;

            return -1;
        }
        public bool ContainsPoint(TriangulationPoint p)
        {
            if (P0 == p) return true;
            if (P1 == p) return true;
            if (P2 == p) return true;
            //if (TriangulationPoint.IsEqualPointCoord(P0, p)) return true;
            //if (TriangulationPoint.IsEqualPointCoord(P1, p)) return true;
            //if (TriangulationPoint.IsEqualPointCoord(P2, p)) return true;
            return false;
        }
        public bool EdgeIsDelaunay(int index)
        {
            //lower 4 bits for EdgeIsDelaunay
            //return ((edgedNoteFlags >> (int)index) & 0x7) != 0;
            switch (index)
            {
                case 0:
                    return this.D0;
                case 1:
                    return this.D1;
                default:
                    return this.D2;
            }
        }
        public void MarkEdgeDelunay(int index, bool value)
        {
            //clear old flags 
            //and then flags new value
            switch (index)
            {
                case 0:
                    this.D0 = value;
                    break;
                case 1:
                    this.D1 = value;
                    break;
                default:
                    this.D2 = value;
                    break;
            }
            //if (value)
            //{
            //    edgedNoteFlags |= (1 << index);
            //}
            //else
            //{
            //    edgedNoteFlags &= ~(1 << index);
            //}
        }

        public bool EdgeIsConstrained(int index)
        {
            switch (index)
            {
                case 0:
                    return this.C0;
                case 1:
                    return this.C1;
                default:
                    return this.C2;
            }
        }
        public void MarkEdgeConstraint(int index, bool value)
        {
            //clear old flags 
            //and then flags new value
            switch (index)
            {
                case 0:
                    this.C0 = value;
                    break;
                case 1:
                    this.C1 = value;
                    break;
                default:
                    this.C2 = value;
                    break;
            }
        }
        public void ClearAllEdgeDelaunayMarks()
        {
            this.D0 = this.D1 = this.D2 = false;
            //this.edgedNoteFlags &= ~0x7;
        }
        public int IndexCWFrom(TriangulationPoint p)
        {
            return (IndexOf(p) + 2) % 3;
        }
        public int IndexCCWFrom(TriangulationPoint p)
        {
            return (IndexOf(p) + 1) % 3;
        }

        public bool Contains(TriangulationPoint p)
        {
            return InternalIndexOf(p) >= 0;
        }

        /// <summary>
        /// Update neighbor pointers
        /// </summary>
        /// <param name="p1">Point 1 of the shared edge</param>
        /// <param name="p2">Point 2 of the shared edge</param>
        /// <param name="t">This triangle's new neighbor</param>
        private void MarkNeighbor(TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t)
        {
            //int i =;
            //if (i == -1) throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");


            switch (EdgeIndex(p1, p2))
            {
                case 0:
                    {
                        this.N0 = t;
                    } break;
                case 1:
                    {
                        this.N1 = t;
                    } break;
                case 2:
                    {
                        this.N2 = t;
                    } break;
                default:
                    {   //may be -1


                        throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
                    }
            }
            //Neighbors[i] = t;
        }
        public void ClearAllNBs()
        {
            N0 = N1 = N2 = null;
        }

        /// <summary>
        /// Exhaustive search to update neighbor pointers
        /// </summary>
        public void MarkNeighbor(DelaunayTriangle t)
        {
            // Points of this triangle also belonging to t
            bool a = t.Contains(P0);
            bool b = t.Contains(P1);
            bool c = t.Contains(P2);

            if (b && c) { N0 = t; t.MarkNeighbor(P1, P2, this); }
            else if (a && c) { N1 = t; t.MarkNeighbor(P0, P2, this); }
            else if (a && b) { N2 = t; t.MarkNeighbor(P0, P1, this); }
            else throw new Exception("Failed to mark neighbor, doesn't share an edge!");
        }

        /// <param name="t">Opposite triangle</param>
        /// <param name="p">The point in t that isn't shared between the triangles</param>
        public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p)
        {
            Debug.Assert(t != this, "self-pointer error");
            return PointCWFrom(t.PointCWFrom(p));
        }

        public DelaunayTriangle NeighborCWFrom(TriangulationPoint point)
        {
            switch ((IndexOf(point) + 1) % 3)
            {
                case 0:
                    return N0;
                case 1:
                    return N1;
                default:
                    return N2;
            }
            //return Neighbors[(IndexOf(point) + 1) % 3];
        }
        public DelaunayTriangle NeighborCCWFrom(TriangulationPoint point)
        {
            // return Neighbors[(InternalIndexOf(point) + 2) % 3];
            switch ((IndexOf(point) + 2) % 3)
            {
                case 0:
                    return N0;
                case 1:
                    return N1;
                default:
                    return N2;
            }
        }
        public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point)
        {
            // return Neighbors[InternalIndexOf(point)];
            switch (InternalIndexOf(point))
            {
                case 0:
                    return N0;
                case 1:
                    return N1;
                default:
                    return N2;
            }
        }

        public TriangulationPoint PointCCWFrom(TriangulationPoint point)
        {
            switch ((InternalIndexOf(point) + 1) % 3)
            {
                case 0:
                    {
                        return this.P0;
                    }
                case 1:
                    {
                        return this.P1;
                    }
                case 2:
                default:
                    {
                        return this.P2;
                    }
            }
            //return Points[(IndexOf(point) + 1) % 3];
        }
        public TriangulationPoint PointCWFrom(TriangulationPoint point)
        {
            //return Points[(IndexOf(point) + 2) % 3];

            switch ((InternalIndexOf(point) + 2) % 3)
            {
                case 0:
                    {
                        return this.P0;
                    }
                case 1:
                    {
                        return this.P1;
                    }
                case 2:
                default:
                    {
                        return this.P2;
                    }
            }

        }

        private void RotateCW()
        {
            var t = P2;
            P2 = P1;
            P1 = P0;
            P0 = t;
        }

        /// <summary>
        /// Legalize triangle by rotating clockwise around oPoint
        /// </summary>
        /// <param name="oPoint">The origin point to rotate around</param>
        /// <param name="nPoint">???</param>
        public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint)
        {
            RotateCW();
            switch (IndexCCWFrom(oPoint))
            {
                case 0:
                    {
                        P0 = nPoint;
                    } break;
                case 1:
                    {
                        P1 = nPoint;
                    } break;
                case 2:
                default:
                    {
                        P2 = nPoint;
                    } break;

            }
            //Points[IndexCCWFrom(oPoint)] = nPoint;
        }

        public override string ToString() { return P0 + "," + P1 + "," + P2; }

        /// <summary>
        /// Finalize edge marking
        /// </summary>
        public void MarkNeighborEdges()
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    if (EdgeIsConstrained[i] && Neighbors[i] != null)
            //    {
            //        Neighbors[i].MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
            //    }

            //}

            //-----------------
            //0
            if (this.C0 && N0 != null)
            {
                //(0 + 1) % 3 => 2
                //(0 + 2) % 3 => 1
                N0.SelectAndMarkConstrainedEdge(P2, P1);
            }
            //-----------------
            //1
            if (this.C1 && N1 != null)
            {
                //(1 + 1) % 3 => 1
                //(1 + 2) % 3 => 0
                N1.SelectAndMarkConstrainedEdge(P1, P0);
            }
            //-----------------
            //2
            if (this.C2 && N2 != null)
            {
                //(2 + 1) % 3 => 0
                //(2 + 2) % 3 => 1

                N2.SelectAndMarkConstrainedEdge(P0, P1);
            }
        }

        public void MarkEdge(DelaunayTriangle triangle)
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    if (EdgeIsConstrained[i])
            //    {
            //        triangle.MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
            //    }
            //}
            if (this.C0)
            {    //(0 + 1) % 3 => 2
                //(0 + 2) % 3 => 1
                triangle.SelectAndMarkConstrainedEdge(P2, P1);
            }
            if (this.C1)
            {   //(1 + 1) % 3 => 1
                //(1 + 2) % 3 => 0
                triangle.SelectAndMarkConstrainedEdge(P1, P0);
            }
            if (this.C2)
            {
                //(2 + 1) % 3 => 0
                //(2 + 2) % 3 => 1
                triangle.SelectAndMarkConstrainedEdge(P0, P1);
            }
        }

        public void MarkEdge(List<DelaunayTriangle> tList)
        {
            foreach (DelaunayTriangle t in tList)
            {
                //for (int i = 0; i < 3; i++)
                //{
                //    if (t.EdgeIsConstrained[i])
                //    {
                //        MarkConstrainedEdge(t.Points[(i + 1) % 3], t.Points[(i + 2) % 3]);
                //    }
                //}
                //-----------------------------
                //0
                if (t.C0)
                {
                    //(0 + 1) % 3 => 2;
                    //(0 + 2) % 3 => 1;
                    SelectAndMarkConstrainedEdge(t.P2, t.P1);
                }
                //-----------------------------
                //1
                if (t.C1)
                {
                    //(1 + 1) % 3 => 1;
                    //(1 + 2) % 3 => 0;
                    SelectAndMarkConstrainedEdge(t.P1, t.P0);
                }
                //-----------------------------
                //2
                if (t.C2)
                {
                    //(2 + 1) % 3 => 0;
                    //(2 + 2) % 3 => 1;
                    SelectAndMarkConstrainedEdge(t.P0, t.P1);
                }
            }
        }
        public void MarkConstrainedEdge(int index)
        {
            MarkEdgeConstraint(index, true);
        }

        public void SelectAndMarkConstrainedEdge(DTSweepConstraint edge)
        {
            SelectAndMarkConstrainedEdge(edge.P, edge.Q);
        }

        /// <summary>
        /// Mark edge as constrained
        /// </summary>
        public void SelectAndMarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q)
        {
            int i = EdgeIndex(p, q);
            if (i != -1)
            {
                //MarkConstrainedEdge(i, true);
                MarkEdgeConstraint(i, true);
            }
        }

        public double Area()
        {
            double b = P0.X - P1.X;
            double h = P2.Y - P1.Y;

            return Math.Abs((b * h * 0.5f));
        }

        public TriangulationPoint Centroid()
        {
            double cx = (P0.X + P1.X + P2.X) / 3f;
            double cy = (P0.Y + P1.Y + P2.Y) / 3f;
            return new TriangulationPoint(cx, cy);
        }

        /// <summary>
        /// Get the index of the neighbor that shares this edge (or -1 if it isn't shared)
        /// </summary>
        /// <returns>index of the shared edge or -1 if edge isn't shared</returns>
        public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2)
        {
            int i1 = InternalIndexOf(p1);// Points.IndexOf(p1);
            int i2 = InternalIndexOf(p2);

            // Points of this triangle in the edge p1-p2
            bool a = (i1 == 0 || i2 == 0);
            bool b = (i1 == 1 || i2 == 1);
            bool c = (i1 == 2 || i2 == 2);

            if (b && c) return 0;
            if (a && c) return 1;
            if (a && b) return 2;
            return -1;
        }

        public bool GetConstrainedEdgeCCW(TriangulationPoint p) { return EdgeIsConstrained((IndexOf(p) + 2) % 3); }
        public bool GetConstrainedEdgeCW(TriangulationPoint p) { return EdgeIsConstrained((IndexOf(p) + 1) % 3); }
        public bool GetConstrainedEdgeAcross(TriangulationPoint p) { return EdgeIsConstrained(IndexOf(p)); }
        public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce) { MarkEdgeConstraint((IndexOf(p) + 2) % 3, ce); }
        public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce) { MarkEdgeConstraint((IndexOf(p) + 1) % 3, ce); }
        public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce) { MarkEdgeConstraint(IndexOf(p), ce); }

        public bool GetDelaunayEdgeCCW(TriangulationPoint p) { return EdgeIsDelaunay((IndexOf(p) + 2) % 3); }
        public bool GetDelaunayEdgeCW(TriangulationPoint p) { return EdgeIsDelaunay((IndexOf(p) + 1) % 3); }
        public bool GetDelaunayEdgeAcross(TriangulationPoint p) { return EdgeIsDelaunay(IndexOf(p)); }
        public void SetDelaunayEdgeCCW(TriangulationPoint p, bool ce) { MarkEdgeDelunay((IndexOf(p) + 2) % 3, ce); }
        public void SetDelaunayEdgeCW(TriangulationPoint p, bool ce) { MarkEdgeDelunay((IndexOf(p) + 1) % 3, ce); }
        public void SetDelaunayEdgeAcross(TriangulationPoint p, bool ce) { MarkEdgeDelunay(IndexOf(p), ce); }


    }
}
