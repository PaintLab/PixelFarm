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
            Dictionary<GlyphTriangle, CentroidLineHub> centroidLineHub = _dynamicOutline.dbugGetCentroidLineHubs();
            //--------------- 

            foreach (CentroidLineHub lineHub in centroidLineHub.Values)
            {
                Dictionary<GlyphTriangle, GlyphCentroidBranch> branches = lineHub.GetAllBranches();
                System.Numerics.Vector2 hubCenter = lineHub.GetCenterPos();

                OnBegingLineHub(hubCenter.X, hubCenter.Y);
                foreach (GlyphCentroidBranch branch in branches.Values)
                {
                    int lineCount = branch.lines.Count;
                    for (int i = 0; i < lineCount; ++i)
                    {
                        GlyphCentroidLine line = branch.lines[i];
                        if (WalkCentroidBone)
                        {
                            double px, py, qx, qy;
                            line.GetLineCoords(out px, out py, out qx, out qy);
                            OnCentroidLine(px, py, qx, qy);


                            if (line.P_Tip != null)
                            {
                                var pos = line.P_Tip.Pos;
                                OnCentroidLineTip_P(px, py, pos.X, pos.Y);
                            }
                            if (line.Q_Tip != null)
                            {
                                var pos = line.Q_Tip.Pos;
                                OnCentroidLineTip_Q(qx, qy, pos.X, pos.Y);
                            }

                        }
                        if (WalkGlyphBone)
                        {
                            OnBoneJoint(line.BoneJoint);
                        }
                    }
                    if (WalkGlyphBone)
                    {
                        //draw bone list
                        DrawBoneLinks(branch);
                    }
                }
                //
                OnEndLineHub(hubCenter.X, hubCenter.Y, lineHub.GetHeadConnectedJoint());

            }
        }
        void DrawBoneLinks(GlyphCentroidBranch branch)
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

        //
    }
}