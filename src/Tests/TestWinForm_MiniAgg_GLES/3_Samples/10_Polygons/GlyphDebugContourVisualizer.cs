//MIT, 2017-present, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Contours;
using PixelFarm.VectorMath;
using PixelFarm.CpuBlit;

namespace PixelFarm
{
    class GlyphDebugContourVisualizer : OutlineWalker
    {
        Painter _painter;
        float _pxscale = 1;
        public GlyphDebugContourVisualizer()
        {
            ShowBones = true;
        }
        public void SetPainter(Painter p)
        {
            _painter = p;
        }
        public bool ShowCentrolLines { get; set; }
        public bool ShowBones { get; set; }
        public float Scale
        {
            get => _pxscale;
            set
            {
                //_pxscale = value;
            }
        }
        void DrawBoneJoint(PixelFarm.Drawing.Painter painter, Joint joint)
        {
            //-------------- 
            EdgeLine p_contactEdge = joint.dbugGetEdge_P();
            //mid point
            Vector2f jointPos = joint.OriginalJointPos * _pxscale;//scaled joint pos
            painter.FillRect(jointPos.X, jointPos.Y, 4, 4, PixelFarm.Drawing.Color.Yellow);
            if (joint.TipEdgeP != null)
            {
                EdgeLine tipEdge = joint.TipEdgeP;
                tipEdge.dbugGetScaledXY(out double p_x, out double p_y, out double q_x, out double q_y, _pxscale);
                //
                painter.Line(
                   jointPos.X, jointPos.Y,
                   p_x, p_y,
                   PixelFarm.Drawing.Color.White);
                painter.FillRect(p_x, p_y, 3, 3, PixelFarm.Drawing.Color.Green); //marker

                //
                painter.Line(
                    jointPos.X, jointPos.Y,
                    q_x, q_y,
                    PixelFarm.Drawing.Color.White);
                painter.FillRect(q_x, q_y, 3, 3, PixelFarm.Drawing.Color.Green); //marker
            }
            if (joint.TipEdgeQ != null)
            {
                EdgeLine tipEdge = joint.TipEdgeQ;
                tipEdge.dbugGetScaledXY(out double p_x, out double p_y, out double q_x, out double q_y, _pxscale);
                //
                painter.Line(
                   jointPos.X, jointPos.Y,
                   p_x, p_y,
                   PixelFarm.Drawing.Color.White);
                painter.FillRect(p_x, p_y, 3, 3, PixelFarm.Drawing.Color.Green); //marker

                //
                painter.Line(
                    jointPos.X, jointPos.Y,
                    q_x, q_y,
                    PixelFarm.Drawing.Color.White);
                painter.FillRect(q_x, q_y, 3, 3, PixelFarm.Drawing.Color.Green); //marker
            }

        }
        Vector2f _branchHeadPos;
        protected override void OnBeginDrawingBoneLinks(Vector2f branchHeadPos, int startAt, int endAt)
        {
            _branchHeadPos = branchHeadPos;
        }

        protected override void OnBegingLineHub(float centerX, float centerY)
        {

        }

        protected override void OnBoneJoint(Joint joint)
        {
            DrawBoneJoint(_painter, joint);
            //_infoView.ShowJoint(joint);
        }

        protected override void OnCentroidLine(double px, double py, double qx, double qy)
        {
            if(ShowCentrolLines)
            {
                float pxscale = _pxscale;
                //red centroid line
                _painter.Line(
                    px * pxscale, py * pxscale,
                    qx * pxscale, qy * pxscale,
                    PixelFarm.Drawing.Color.Red);
                ///small yellow marker at p and q point of centroid
                _painter.FillRect(px * pxscale, py * pxscale, 2, 2, PixelFarm.Drawing.Color.Yellow);
                _painter.FillRect(qx * pxscale, qy * pxscale, 2, 2, PixelFarm.Drawing.Color.Yellow);
            }
          
        }

        protected override void OnCentroidLineTip_P(double px, double py, double tip_px, double tip_py)
        {

        }

        protected override void OnCentroidLineTip_Q(double qx, double qy, double tip_qx, double tip_qy)
        {

        }

        protected override void OnDrawBone(Bone bone, int boneIndex)
        {
            if (!ShowBones) return;

            float pxscale = _pxscale;
            Joint jointA = bone.JointA;
            Joint jointB = bone.JointB;

            bool valid = false;
            if (jointA != null && jointB != null)
            {

                Vector2f jointAPoint = jointA.OriginalJointPos;
                Vector2f jointBPoint = jointB.OriginalJointPos;

                _painter.Line(
                      jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                      jointBPoint.X * pxscale, jointBPoint.Y * pxscale,
                      bone.IsLongBone ? PixelFarm.Drawing.Color.Yellow : PixelFarm.Drawing.Color.Magenta);

                //if (this.DrawDynamicOutline)
                //{
                //    //****
                //    _painter.Line(
                //        jointA.FitX * pxscale, jointA.FitY * pxscale,
                //        jointB.FitX * pxscale, jointB.FitY * pxscale,
                //        PixelFarm.Drawing.Color.White);
                //}

                valid = true;

                //_infoView.ShowBone(bone, jointA, jointB);
            }
            if (jointA != null && bone.TipEdge != null)
            {
                Vector2f jointAPoint = jointA.OriginalJointPos;
                Vector2f mid = bone.TipEdge.GetMidPoint();

                _painter.Line(
                    jointAPoint.X * pxscale, jointAPoint.Y * pxscale,
                    mid.X * pxscale, mid.Y * pxscale,
                    bone.IsLongBone ? PixelFarm.Drawing.Color.Yellow : PixelFarm.Drawing.Color.Magenta);

                valid = true;
                //_infoView.ShowBone(bone, jointA, bone.TipEdge);
            }

            if (boneIndex == 0)
            {
                //for first bone 
                _painter.FillRect(_branchHeadPos.X * pxscale, _branchHeadPos.Y * pxscale, 5, 5, PixelFarm.Drawing.Color.DeepPink);
            }
            if (!valid)
            {
                throw new NotSupportedException();
            }
        }

        protected override void OnEndDrawingBoneLinks()
        {

        }

        protected override void OnEndLineHub(float centerX, float centerY, Joint joint)
        {

        }

        protected override void OnGlyphEdgeN(EdgeLine edge)
        {

        }

        protected override void OnTriangle(int triAngleId, EdgeLine e0, EdgeLine e1, EdgeLine e2, double centroidX, double centroidY)
        {

        }
    }
}