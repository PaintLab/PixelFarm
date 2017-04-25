//MIT, 2017, WinterDev
using System;
using System.Numerics;
using System.Collections.Generic;


namespace Typography.Rendering
{

    public abstract class GlyphOutlineWalker
    {
        GlyphDynamicOutline _dynamicOutline;
        public GlyphOutlineWalker()
        {
            //default
            WalkCentroidBone = WalkGlyphBone = true;
        }

        public bool WalkCentroidBone { get; set; }
        public bool WalkGlyphBone { get; set; }
        //
        public void Walk(GlyphDynamicOutline dynamicOutline)
        {
            this._dynamicOutline = dynamicOutline;

            int triNumber = 0;
            foreach (GlyphTriangle tri in _dynamicOutline.dbugGetGlyphTriangles())
            {
                OnTriangle(triNumber++, tri.e0, tri.e1, tri.e2, tri.CentroidX, tri.CentroidY);
            }
            //--------------- 
#if DEBUG
            List<CentroidLineHub> centroidLineHubs = _dynamicOutline.dbugGetCentroidLineHubs();
            foreach (CentroidLineHub lineHub in centroidLineHubs)
            {
                Dictionary<GlyphTriangle, GlyphCentroidLine> lines = lineHub.GetAllCentroidLines();
                Vector2 hubCenter = lineHub.CalculateAvgHeadPosition();

                OnBegingLineHub(hubCenter.X, hubCenter.Y);
                foreach (GlyphCentroidLine line in lines.Values)
                {
                    int pairCount = line.pairs.Count;
                    for (int i = 0; i < pairCount; ++i)
                    {
                        GlyphCentroidPair pair = line.pairs[i];
                        if (WalkCentroidBone)
                        {
                            double px, py, qx, qy;
                            pair.GetLineCoords(out px, out py, out qx, out qy);
                            OnCentroidLine(px, py, qx, qy);
                            //--------------------------------------------------
                            if (pair.BoneJoint.TipEdgeP != null)
                            {
                                Vector2 pos = pair.BoneJoint.TipPointP;
                                OnCentroidLineTip_P(px, py, pos.X, pos.Y);
                            }
                            if (pair.BoneJoint.TipEdgeQ != null)
                            {
                                Vector2 pos = pair.BoneJoint.TipPointQ;
                                OnCentroidLineTip_Q(qx, qy, pos.X, pos.Y);
                            } 
                        }
                        if (WalkGlyphBone)
                        {
                            OnBoneJoint(pair.BoneJoint);
                        }
                    }
                    if (WalkGlyphBone)
                    {
                        //draw bone list
                        DrawBoneLinks(line);
                    }
                }
                //
                 OnEndLineHub(hubCenter.X, hubCenter.Y, lineHub.GetHeadConnectedJoint());
            }


            //----------------

            List<GlyphContour> cnts = _dynamicOutline._contours;
            int j = cnts.Count;
            for (int i = 0; i < j; ++i)
            {
                GlyphContour cnt = cnts[i];
                List<GlyphEdge> edgeLines = cnt.edges;
                if(edgeLines != null)
                {
                    int n = edgeLines.Count;
                    for (int m = 0; m < n; ++m)
                    {
                        GlyphEdge e = edgeLines[m];
                        Vector2 cut_p = e.Pos_P;
                        Vector2 cut_q = e.Pos_Q;
                        OnGlyphEdgeN(cut_p.X, cut_p.Y, cut_q.X, cut_p.Y);
                    }
                } 

                //List<GlyphPoint> pnts = cnt.flattenPoints;
                //int lim = pnts.Count - 1;
                //for (int m = 0; m < lim; ++m)
                //{
                //    GlyphPoint p = pnts[m];
                //    GlyphPoint q = pnts[m + 1]; 
                //    OnGlyphEdge(p.x, p.y, q.x, q.y); 
                //}


            }
            //----------------
#endif

        }
        void DrawBoneLinks(GlyphCentroidLine branch)
        {
            List<GlyphBone> glyphBones = branch.bones;
            int glyphBoneCount = glyphBones.Count;
            int startAt = 0;
            int endAt = startAt + glyphBoneCount;
            OnBeginDrawingBoneLinks(branch.GetHeadPosition(), startAt, endAt);
            int nn = 0;
            for (int i = startAt; i < endAt; ++i)
            {
                //draw line
                OnDrawBone(glyphBones[i], nn);
                nn++;
            }
            OnEndDrawingBoneLinks();

            ////draw link between each branch to center of hub
            //var brHead = branch.GetHeadPosition();
            //painter.Line(
            //    hubCenter.X * pxscale, hubCenter.Y * pxscale,
            //    brHead.X * pxscale, brHead.Y * pxscale);

            ////draw  a line link to centroid of target triangle

            //painter.Line(
            //    (float)brHead.X * pxscale, (float)brHead.Y * pxscale,
            //     hubCenter.X * pxscale, hubCenter.Y * pxscale,
            //     PixelFarm.Drawing.Color.Red);

        }
        //
        protected abstract void OnTriangle(int triAngleId, EdgeLine e0, EdgeLine e1, EdgeLine e2, double centroidX, double centroidY);

        protected abstract void OnCentroidLine(double px, double py, double qx, double qy);
        protected abstract void OnCentroidLineTip_P(double px, double py, double tip_px, double tip_py);
        protected abstract void OnCentroidLineTip_Q(double qx, double qy, double tip_qx, double tip_qy);
        protected abstract void OnBoneJoint(GlyphBoneJoint joint);
        protected abstract void OnBeginDrawingBoneLinks(Vector2 branchHeadPos, int startAt, int endAt);
        protected abstract void OnEndDrawingBoneLinks();
        protected abstract void OnDrawBone(GlyphBone bone, int boneIndex);
        protected abstract void OnBegingLineHub(float centerX, float centerY);
        protected abstract void OnEndLineHub(float centerX, float centerY, GlyphBoneJoint joint);

        protected abstract void OnGlyphEdge(float x0, float y0, float x1, float y1);
        protected abstract void OnGlyphEdgeN(float x0, float y0, float x1, float y1);
        //
    }
}