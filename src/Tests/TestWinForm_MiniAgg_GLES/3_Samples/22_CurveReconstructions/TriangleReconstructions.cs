//MIT, 2018-present, WinterDev
//from http://www.antigrain.com/research/bezier_interpolation/index.html#PAGE_BEZIER_INTERPOLATION

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.PathReconstruction;

using Mini;


namespace PixelFarm.CpuBlit.Samples
{

    
    [Info(OrderCode = "03")]
    public class TriangleCurveReconstruction1 : DemoBase
    {
        VertexStore _triangleVxs;
        CurveFlattener _curveflattener = new CurveFlattener();

        Vector2 v0;
        Vector2 v1;
        Vector2 v2;

        Vector2 a01;
        Vector2 a12;
        Vector2 a20;

        public override void Init()
        {
            ShowReconstructionCurve = true;
            SmoothCoefficientValue = 1;
            using (VxsTemp.Borrow(out var vxs))
            {
                int xoffset = 50;
                int yoffset = 50;

                v0 = new Vector2(0 + xoffset, 0 + yoffset);
                v1 = new Vector2(300 + xoffset, 100 + yoffset);
                v2 = new Vector2(100 + xoffset, 120 + yoffset);

                vxs.AddMoveTo(v0.x, v0.y);
                vxs.AddLineTo(v1.x, v1.y);
                vxs.AddLineTo(v2.x, v2.y);
                vxs.AddCloseFigure();
                _triangleVxs = vxs.CreateTrim();
                //
                //mid point of each edge

                a01 = (v0 + v1) / 2;
                a12 = (v1 + v2) / 2;
                a20 = (v2 + v0) / 2;
                //----- 
            }
        }
        void DrawPoint(PixelFarm.Drawing.Painter p, Vector2 v)
        {
            Color prev = p.FillColor;
            p.FillColor = Color.Red;

            p.FillRect(v.X, v.Y, 4, 4);

            p.FillColor = prev;
        }
        void DrawLine(PixelFarm.Drawing.Painter p, Vector2 v0, Vector2 v1)
        {
            p.Line(v0.x, v0.y, v1.x, v1.Y, p.StrokeColor);
        }
        void DrawControllerPair(PixelFarm.Drawing.Painter p, BezierControllerArmPair c)
        {
            DrawLine(p, c.left, c.mid);
            DrawLine(p, c.mid, c.right);
            DrawPoint(p, c.mid);
        }

        [DemoConfig(MinValue = -10, MaxValue = 20)] //just a sample!, -10=> 20
        public float SmoothCoefficientValue { get; set; }

        [DemoConfig]
        public bool ShowReconstructionCurve { get; set; }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.StrokeColor = Color.Black;
            p.StrokeWidth = 2;
            p.StrokeColor = Color.Green;

            p.Draw(_triangleVxs);


            if (!ShowReconstructionCurve) return;

            //draw Ci line
            p.StrokeColor = Color.OrangeRed;
            DrawLine(p, a01, a12);
            DrawLine(p, a12, a20);
            DrawLine(p, a20, a01);
            //find B

            //DrawPoint(p, a01);
            //DrawPoint(p, a12);
            //DrawPoint(p, a20);

            BezierControllerArmPair c1 = BezierControllerArmPair.ReconstructControllerArms(v0, v1, v2);
            BezierControllerArmPair c2 = BezierControllerArmPair.ReconstructControllerArms(v1, v2, v0);
            BezierControllerArmPair c0 = BezierControllerArmPair.ReconstructControllerArms(v2, v0, v1);



            c0.UniformSmoothCoefficient = SmoothCoefficientValue;
            c1.UniformSmoothCoefficient = SmoothCoefficientValue;
            c2.UniformSmoothCoefficient = SmoothCoefficientValue;

            //DrawPoint(p, c0 = FindB(v0, v1, v2, out c1));
            //DrawPoint(p, b2 = FindB(v1, v2, v0, out c2));
            //DrawPoint(p, b0 = FindB(v2, v0, v1, out c0));

            p.StrokeColor = Color.Red;
            DrawControllerPair(p, c0);
            DrawControllerPair(p, c1);
            DrawControllerPair(p, c2);


            p.StrokeColor = Color.Blue;
            using (VxsTemp.Borrow(out var tmpVxs1, out var tmpVxs2))
            using (VectorToolBox.Borrow(tmpVxs1, out PathWriter pw))
            {
                pw.MoveTo(c0.mid.X, c0.mid.y);

                pw.Curve4(c0.right.x, c0.right.y, c1.left.x, c1.left.y, c1.mid.x, c1.mid.y); //1st curve

                pw.Curve4(c1.right.x, c1.right.y, c2.left.x, c2.left.y, c2.mid.x, c2.mid.y); //2nd curve

                pw.Curve4(c2.right.x, c2.right.y, c0.left.x, c0.left.y, c0.mid.x, c0.mid.y); //3rd curve

                _curveflattener.MakeVxs(tmpVxs1, tmpVxs2);
                p.Draw(tmpVxs2);
            }
        }

    }

}