//MIT, 2018-present, WinterDev
//from http://www.antigrain.com/research/bezier_interpolation/index.html#PAGE_BEZIER_INTERPOLATION

using System;
using System.Collections.Generic;
using PaintLab.Svg;
using PixelFarm.Drawing;
using PixelFarm.PathReconstruction;
using PixelFarm.CpuBlit.VertexProcessing;
using Mini;


namespace PixelFarm.CpuBlit.Samples
{

    [Info(OrderCode = "03")]
    public class LionCurveReconstruction : DemoBase
    {
        float _smoothCoeff;
        bool _needUpdate;
        MyTestSprite _testSprite;
        VgVisualElement _vgVisualElem;
        CurveFlattener _curveflattener = new CurveFlattener();

        BezireControllerArmBuilder _builder = new BezireControllerArmBuilder();
        List<ReconstructedFigure> _output = new List<ReconstructedFigure>();
        public override void Init()
        {
            //eg. lion
            _vgVisualElem = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem;
            var spriteShape = new SpriteShape(_vgVisualElem);
            _testSprite = new MyTestSprite(spriteShape);
            _needUpdate = true;
            _smoothCoeff = 0;
        }


        [DemoConfig(MinValue = -10, MaxValue = 20)]
        public float SmoothCoefficientValue
        {
            get => _smoothCoeff;
            set
            {
                _smoothCoeff = value;
                _needUpdate = true;
            }
        }
        [DemoAction]
        public void ResetSmoothCoefficentToZero()
        {
            SmoothCoefficientValue = 0;
        }
        void PaintApproximateCurves(Painter p)
        {
            if (SmoothCoefficientValue == 0)
            {
                _testSprite.Render(p);
                return;
            }

            using (Tools.More.Borrow(p, out VgPaintArgs paintArgs))
            {
                paintArgs.PaintVisitHandler = (vxs, arg) =>
                {
                    //use external painter handler
                    //draw only outline with its fill-color.
                    Drawing.Painter m_painter = arg.P;
                    Drawing.Color prevFillColor = m_painter.FillColor;
                    m_painter.FillColor = m_painter.FillColor;

                    //do other transform first

                    _output.Clear();

                    _builder.SmoothCoefficiency = this.SmoothCoefficientValue;
                    _builder.ReconstructionControllerArms(vxs, _output);

                    using (VxsTemp.Borrow(out var tmpVxs1, out var tmpVxs2))
                    using (VectorToolBox.Borrow(tmpVxs1, out PathWriter pw))
                    {

                        int fig_count = _output.Count;
                        for (int f = 0; f < fig_count; ++f)
                        {
                            ReconstructedFigure fig = _output[f];
                            List<BezierControllerArmPair> arms = fig._arms;
                            int count = arms.Count;

                            int index = 0;
                            BezierControllerArmPair arm0 = arms[index];
                            BezierControllerArmPair arm1 = arms[index + 1];
                            //start 

                            pw.MoveTo(arm0.mid.X, arm0.mid.y);

                            pw.Curve4(
                                arm0.right.x, arm0.right.y,
                                arm1.left.x, arm1.left.y,
                                arm1.mid.x, arm1.mid.y);

                            index++;
                            arm0 = arm1;
                            for (; index < count - 1; ++index)
                            {
                                arm1 = arms[index];
                                //
                                pw.Curve4(
                                  arm0.right.x, arm0.right.y,
                                  arm1.left.x, arm1.left.y,
                                  arm1.mid.x, arm1.mid.y);
                                //
                                arm0 = arm1;
                            }
                            //the last curve
                            arm1 = arms[0];
                            pw.Curve4(
                                arm0.right.x, arm0.right.y,
                                arm1.left.x, arm1.left.y,
                                arm1.mid.x, arm1.mid.y);

                            pw.CloseFigure();

                            _curveflattener.MakeVxs(tmpVxs1, tmpVxs2);

                            //
                            m_painter.Fill(tmpVxs2);  //draw to output
                            //

                            //clear before reuse
                            tmpVxs1.Clear();
                            tmpVxs2.Clear();
                            pw.Clear();
                        }
                    }

                    //m_painter.Fill(vxs);

                    m_painter.FillColor = prevFillColor;
                };
                _vgVisualElem.Paint(paintArgs);
            }


        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.RenderQuality = Drawing.RenderQuality.HighQuality;
            //
            PaintApproximateCurves(p);
            //_testSprite.Render(p);
        }
        public override void MouseDrag(int x, int y)
        {
            //move to specific position
            _testSprite.Move(x, y);
        }
    }


    [Info(OrderCode = "03")]
    public class StripCurveReconstruction : DemoBase
    {
        float _smoothCoeff;
        bool _needUpdate;
        CurveFlattener _curveflattener = new CurveFlattener();
        BezireControllerArmBuilder _builder = new BezireControllerArmBuilder();
        List<ReconstructedFigure> _output = new List<ReconstructedFigure>();
        VertexStore _simpleStrip = new VertexStore();
        public override void Init()
        {
            //eg. lion
            using (VxsTemp.Borrow(out var vxs1))
            {
                vxs1.AddMoveTo(10, 10);
                vxs1.AddLineTo(30, 40);
                vxs1.AddLineTo(60, 80);
                vxs1.AddLineTo(80, 120);
                vxs1.AddLineTo(20, 80);
                vxs1.AddLineTo(10, 50);
                vxs1.AddCloseFigure();
                vxs1.ConfirmNoMore();
                _simpleStrip = vxs1.CreateTrim();
            }

            _needUpdate = true;
            _smoothCoeff = 0;
        }


        [DemoConfig(MinValue = -10, MaxValue = 20)]
        public float SmoothCoefficientValue
        {
            get => _smoothCoeff;
            set
            {
                _smoothCoeff = value;
                _needUpdate = true;
            }
        }
        [DemoAction]
        public void ResetSmoothCoefficentToZero()
        {
            SmoothCoefficientValue = 0;
        }
        void PaintApproximateCurves(Painter p)
        {
            if (SmoothCoefficientValue == 0)
            {
                p.FillColor = Color.Black;
                p.Fill(_simpleStrip);
                return;
            }


            //use external painter handler
            //draw only outline with its fill-color.
            Drawing.Painter m_painter = p;
            Drawing.Color prevFillColor = m_painter.FillColor;
            m_painter.FillColor = m_painter.FillColor;

            //do other transform first

            _output.Clear();
            _builder.SmoothCoefficiency = this.SmoothCoefficientValue;
            _builder.ReconstructionControllerArms(_simpleStrip, _output);

            using (VxsTemp.Borrow(out var tmpVxs1, out var tmpVxs2))
            using (VectorToolBox.Borrow(tmpVxs1, out PathWriter pw))
            {

                int fig_count = _output.Count;
                for (int f = 0; f < fig_count; ++f)
                {
                    ReconstructedFigure fig = _output[f];
                    List<BezierControllerArmPair> arms = fig._arms;
                    int count = arms.Count;

                    int index = 0;
                    BezierControllerArmPair arm0 = arms[index];
                    BezierControllerArmPair arm1 = arms[index + 1];
                    //start 

                    pw.MoveTo(arm0.mid.X, arm0.mid.y);

                    pw.Curve4(
                        arm0.right.x, arm0.right.y,
                        arm1.left.x, arm1.left.y,
                        arm1.mid.x, arm1.mid.y);

                    index++;
                    arm0 = arm1;
                    for (; index < count; ++index)
                    {
                        arm1 = arms[index];
                        //
                        pw.Curve4(
                          arm0.right.x, arm0.right.y,
                          arm1.left.x, arm1.left.y,
                          arm1.mid.x, arm1.mid.y);
                        //
                        arm0 = arm1;
                    }

                    //the last curve

                    arm1 = arms[0];
                    pw.Curve4(
                        arm0.right.x, arm0.right.y,
                        arm1.left.x, arm1.left.y,
                        arm1.mid.x, arm1.mid.y);

                    pw.CloseFigure();

                    _curveflattener.MakeVxs(tmpVxs1, tmpVxs2);

                    //
                    m_painter.Fill(tmpVxs2);  //draw to output
                                              //

                    //clear before reuse
                    tmpVxs1.Clear();
                    tmpVxs2.Clear();
                    pw.Clear();
                }
            }
            //m_painter.Fill(vxs); 
            m_painter.FillColor = prevFillColor;




        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.RenderQuality = Drawing.RenderQuality.HighQuality;
            //
            PaintApproximateCurves(p);
            //_testSprite.Render(p);
        }
        public override void MouseDrag(int x, int y)
        {
            //move to specific position

        }
    }

}