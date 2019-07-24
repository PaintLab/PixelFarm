//BSD, 2014-present, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// classes conv_curve
//
//----------------------------------------------------------------------------
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.VertexProcessing
{
    //---------------------------------------------------------------conv_curve
    // Curve converter class. Any path storage can have Bezier curves defined 
    // by their control points. There're two types of curves supported: curve3 
    // and curve4. Curve3 is a conic Bezier curve with 2 endpoints and 1 control
    // point. Curve4 has 2 control points (4 points in total) and can be used
    // to interpolate more complicated curves. Curve4, unlike curve3 can be used 
    // to approximate arcs, both circular and elliptical. Curves are approximated 
    // with straight lines and one of the approaches is just to store the whole 
    // sequence of vertices that approximate our curve. It takes additional 
    // memory, and at the same time the consecutive vertices can be calculated 
    // on demand. 
    //
    // Initially, path storages are not suppose to keep all the vertices of the
    // curves (although, nothing prevents us from doing so). Instead, path_storage
    // keeps only vertices, needed to calculate a curve on demand. Those vertices
    // are marked with special commands. So, if the path_storage contains curves 
    // (which are not real curves yet), and we render this storage directly, 
    // all we will see is only 2 or 3 straight line segments (for curve3 and 
    // curve4 respectively). If we need to see real curves drawn we need to 
    // include this class into the conversion pipeline. 
    //
    // Class conv_curve recognizes commands path_cmd_curve3 and path_cmd_curve4 
    // and converts these vertices into a move_to/line_to sequence. 
    //-----------------------------------------------------------------------

    public enum CurveApproximationMethod
    {
        Unknow,
        Div,
        Inc,
    }

    public class CurveFlattener
    {
        CurveApproximationMethod _selectedApproximationMethod = CurveApproximationMethod.Div;

        //tools , curve producer 
        readonly CurveSubdivisionFlattener _div_curveFlattener = new CurveSubdivisionFlattener();
        readonly CurveIncFlattener _inc_curveFlattener = new CurveIncFlattener();
        ArrayList<VectorMath.Vector2> _tmpFlattenPoints = new ArrayList<VectorMath.Vector2>();

        double _approximateScale = 1;//default
        public CurveFlattener()
        {
        }
        public double ApproximationScale
        {
            //default 1
            get => _approximateScale;
            set
            {
                _inc_curveFlattener.ApproximationScale =
                    _div_curveFlattener.ApproximationScale =
                    _approximateScale = value;
            }
        }

        public CurveApproximationMethod ApproximationMethod
        {
            get => _selectedApproximationMethod;
            set => _selectedApproximationMethod = value;
        }
        /// <summary>
        ///  curve incremental flattener, use specific step count
        /// </summary>
        public bool IncUseFixedStep
        {
            get => _inc_curveFlattener.UseFixedStepCount;
            set => _inc_curveFlattener.UseFixedStepCount = value;
        }
        /// <summary>
        /// curve incremental flattener, incremental step count
        /// </summary>
        public int IncStepCount
        {
            get => _inc_curveFlattener.FixedStepCount;
            set => _inc_curveFlattener.FixedStepCount = value;
        }
        /// <summary>
        /// curve subdivision flattener , angle tolerance
        /// </summary>
        public double AngleTolerance
        {
            //default 0
            get => _div_curveFlattener.AngleTolerance;
            set => _div_curveFlattener.AngleTolerance = value;
        }
        /// <summary>
        /// curve subdivision flattener, recursive limit
        /// </summary>
        public byte RecursiveLimit
        {
            get => _div_curveFlattener.RecursiveLimit;
            set => _div_curveFlattener.RecursiveLimit = value;
        }
        /// <summary>
        /// curve subdivision flattener, cusp limit
        /// </summary>
        public double CuspLimit
        {
            //default 0
            get => _div_curveFlattener.CuspLimit;
            set => _div_curveFlattener.CuspLimit = value;
        }

        enum CurvePointMode
        {
            NotCurve,
            P2,
            P3
        }

        public void Reset()
        {
            ApproximationScale = 1;
            ApproximationMethod = CurveApproximationMethod.Div;
            AngleTolerance = 0;
            CuspLimit = 0;
            _tmpFlattenPoints.Clear();

            _inc_curveFlattener.Reset();
            _div_curveFlattener.Reset();
        }

        public VertexStore MakeVxs(VertexStore vxs, VertexStore output)
        {
            return MakeVxs(vxs, null, output);
        }


        public VertexStore MakeVxs(VertexStore vxs, ICoordTransformer tx, VertexStore output)
        {

            CurvePointMode latestCurveMode = CurvePointMode.NotCurve;
            double x, y;
            VertexCmd cmd;
            VectorMath.Vector2 c3p2 = new VectorMath.Vector2();
            VectorMath.Vector2 c4p2 = new VectorMath.Vector2();
            VectorMath.Vector2 c4p3 = new VectorMath.Vector2();
            double lastX = 0;
            double lasty = 0;
            double lastMoveX = 0;
            double lastMoveY = 0;


            int index = 0;
            bool hasTx = tx != null;

            while ((cmd = vxs.GetVertex(index++, out x, out y)) != VertexCmd.NoMore)
            {
#if DEBUG
                if (VertexStore.dbugCheckNANs(x, y))
                {

                }
#endif

                //-----------------
                if (hasTx)
                {
                    tx.Transform(ref x, ref y);
                }

                //-----------------
                switch (cmd)
                {

                    case VertexCmd.C3:
                        {
                            switch (latestCurveMode)
                            {
                                case CurvePointMode.P2:
                                    {
                                    }
                                    break;
                                case CurvePointMode.P3:
                                    {
                                    }
                                    break;
                                case CurvePointMode.NotCurve:
                                    {
                                        c3p2.x = x;
                                        c3p2.y = y;
                                    }
                                    break;
                                default:
                                    {
                                    }
                                    break;
                            }
                            latestCurveMode = CurvePointMode.P2;
                        }
                        break;
                    case VertexCmd.C4:
                        {

                            switch (latestCurveMode)
                            {
                                case CurvePointMode.P2:
                                    {
                                        c3p2.x = x;
                                        c3p2.y = y;
                                    }
                                    break;
                                case CurvePointMode.P3:
                                    {
                                        c4p3.x = x;
                                        c4p3.y = y;
                                    }
                                    break;
                                case CurvePointMode.NotCurve:
                                    {
                                        c4p2.x = x;
                                        c4p2.y = y;
                                    }
                                    break;
                            }
                            latestCurveMode = CurvePointMode.P3;
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            switch (latestCurveMode)
                            {
                                case CurvePointMode.P2:
                                    {

                                        if (_selectedApproximationMethod == CurveApproximationMethod.Inc)
                                        {

                                            _inc_curveFlattener.Flatten(
                                               lastX, lasty,
                                               c3p2.X, c3p2.Y,
                                               x, y,
                                               _tmpFlattenPoints, true);

                                        }
                                        else
                                        {
                                            _div_curveFlattener.Flatten(lastX, lasty,
                                                c3p2.X, c3p2.Y,
                                                x, y,
                                                _tmpFlattenPoints,
                                                true
                                                );
                                        }
                                        //copy flatten curve to vxs
                                        int count = _tmpFlattenPoints.Count;
                                        for (int i = 0; i < count; ++i)
                                        {
                                            var vec = _tmpFlattenPoints[i];
                                            output.AddLineTo(vec.x, vec.y);
                                        }
                                        _tmpFlattenPoints.Clear();

                                    }
                                    break;
                                case CurvePointMode.P3:
                                    {

                                        if (_selectedApproximationMethod == CurveApproximationMethod.Inc)
                                        {
                                            _inc_curveFlattener.Flatten(
                                                lastX, lasty,
                                                c4p2.x, c4p2.y,
                                                c4p3.x, c4p3.y,
                                                x, y,
                                                _tmpFlattenPoints, true
                                                );
                                        }
                                        else
                                        {
                                            _div_curveFlattener.Flatten(
                                                 lastX, lasty,
                                                 c4p2.x, c4p2.y,
                                                 c4p3.x, c4p3.y,
                                                 x, y, _tmpFlattenPoints, true
                                                );
                                        }
                                        //copy flatten curve to vxs
                                        int count = _tmpFlattenPoints.Count;
                                        for (int i = 0; i < count; ++i)
                                        {
                                            var vec = _tmpFlattenPoints[i];
                                            output.AddLineTo(vec.x, vec.y);
                                        }

                                        _tmpFlattenPoints.Clear();
                                    }
                                    break;
                                default:
                                    {
                                        output.AddVertex(x, y, cmd);
                                    }
                                    break;
                            }
                            //-----------
                            latestCurveMode = CurvePointMode.NotCurve;
                            lastX = x;
                            lasty = y;
                            //-----------
                        }
                        break;
                    case VertexCmd.MoveTo:
                        {
                            //move to, and end command
                            output.AddVertex(x, y, cmd);
                            //-----------
                            latestCurveMode = CurvePointMode.NotCurve;
                            lastMoveX = lastX = x;
                            lastMoveY = lasty = y;
                            //-----------
                        }
                        break;

                    case VertexCmd.Close:
                    case VertexCmd.CloseAndEndFigure:
                        {
                            latestCurveMode = CurvePointMode.NotCurve;
                            output.AddVertex(lastMoveX, lastMoveY, cmd);
                            //move to begin 
                            lastX = lastMoveX;
                            lasty = lastMoveY;
                        }
                        break;

                    default:
                        {
                            //move to, and end command
                            output.AddVertex(x, y, cmd);
                            //-----------
                            latestCurveMode = CurvePointMode.NotCurve;
                            lastX = x;
                            lasty = y;
                            //-----------
                        }
                        break;
                }
            }
            return output;
        }
    }
}