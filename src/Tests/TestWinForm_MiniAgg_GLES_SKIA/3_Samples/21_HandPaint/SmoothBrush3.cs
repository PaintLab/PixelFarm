//BSD, 2014-present, WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;

using Mini;

namespace PixelFarm.CpuBlit.Samples
{
    [Info(OrderCode = "22")]
    [Info("SmoothBrush3")]
    public class SmoothBrush3 : DemoBase
    {
        PixelFarm.Drawing.Point _latestMousePoint;
        List<MyBrushPath> _myBrushPathList = new List<MyBrushPath>();

        MyBrushPath _currentBrushPath;
        MyBrushPath _currentSelectedPath;

        int _lastMousePosX;
        int _lastMousePosY;
        Stroke _stroke1;
        public SmoothBrush3()
        {
            _stroke1 = new Stroke(10);
            _stroke1.LineCap = LineCap.Round;
            _stroke1.LineJoin = LineJoin.Round;
        }
        public override void Init()
        {

        }

        [DemoConfig]
        public SmoothBrushMode BrushMode
        {
            get;
            set;
        }
        [DemoConfig]
        public EditMode EditMode
        {
            get;
            set;
        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.FillColor = Drawing.Color.Black;
            int j = _myBrushPathList.Count;
            for (int n = 0; n < j; ++n)
            {
                _myBrushPathList[n].PaintLatest(p);

                //                MyBrushPath brushPath = myBrushPathList[n];
                //                if (brushPath.Vxs != null)
                //                {
                //                    switch (brushPath.BrushMode)
                //                    {
                //                        case SmoothBrushMode.CutBrush:
                //                            {
                //                            }
                //                            break;
                //                        default:
                //                            {
                //                                //TODO: review PixelCache here
                //                                p.FillColor = brushPath.FillColor;
                //                                p.Fill(brushPath.Vxs);

                //#if DEBUG
                //                                //if (brushPath.StrokeColor.alpha > 0)
                //                                //{
                //                                //    p.StrokeColor = Drawing.Color.Red;
                //                                //    p.Draw(brushPath.Vxs);
                //                                //}
                //#endif
                //                            }
                //                            break;
                //                    }
                //                }
                //                else
                //                {
                //                    //current drawing brush
                //                    var contPoints = brushPath.contPoints;
                //                    int pcount = contPoints.Count;
                //                    for (int i = 1; i < pcount; ++i)
                //                    {
                //                        var p0 = contPoints[i - 1];
                //                        var p1 = contPoints[i];
                //                        p.DrawLine(p0.x, p0.y, p1.x, p1.y);
                //                    }
                //                }
            }
        }


        public override void MouseUp(int x, int y)
        {
            if (_currentSelectedPath != null)
            {
                _currentSelectedPath.FillColor = Drawing.Color.Black;
            }
            _currentSelectedPath = null;
            if (EditMode == Samples.EditMode.Select)
            {
                return;
            }

            if (_currentBrushPath != null)
            {
                //1. close current path 
                switch (_currentBrushPath.BrushMode)
                {
                    case SmoothBrushMode.CutBrush:
                        {
                            _currentBrushPath.MakeRegularPath(5);
                            if (_myBrushPathList.Count > 0)
                            {
                                //1. remove 
                                _myBrushPathList.RemoveAt(_myBrushPathList.Count - 1);
                                //

                                if (_myBrushPathList.Count > 0)
                                {

                                    int j = _myBrushPathList.Count - 1;
                                    for (int i = j; i >= 0; --i)
                                    {
                                        //cut each path
                                        MyBrushPath lastPath = _myBrushPathList[i];
                                        //do path clip***
                                        List<VertexStore> paths = new List<VertexStore>();
                                        PixelFarm.CpuBlit.VertexProcessing.VxsClipper.CombinePaths(
                                                lastPath.GetMergedVxs(),
                                                _currentBrushPath.GetMergedVxs(),
                                                VertexProcessing.VxsClipperType.Difference,
                                                true,
                                                paths);

                                        _myBrushPathList.RemoveAt(i);

                                        if (i == j)
                                        {
                                            //the last one                                              
                                            for (int s = paths.Count - 1; s >= 0; --s)
                                            {
                                                MyBrushPath newBrushPath = new MyBrushPath();
                                                newBrushPath.BrushMode = lastPath.BrushMode;
                                                newBrushPath.StrokeColor = lastPath.StrokeColor;
                                                newBrushPath.FillColor = lastPath.FillColor;
                                                newBrushPath.SetVxs(paths[s]);
                                                _myBrushPathList.Add(newBrushPath); //add last
                                            }
                                        }
                                        else
                                        {
                                            for (int s = paths.Count - 1; s >= 0; --s)
                                            {
                                                MyBrushPath newBrushPath = new MyBrushPath();
                                                newBrushPath.BrushMode = lastPath.BrushMode;
                                                newBrushPath.StrokeColor = lastPath.StrokeColor;
                                                newBrushPath.FillColor = lastPath.FillColor;
                                                newBrushPath.SetVxs(paths[s]);
                                                _myBrushPathList.Insert(i, newBrushPath);
                                            }

                                        }
                                    }


                                }
                            }
                        }
                        break;
                    case SmoothBrushMode.SolidBrush:
                        {
                            //create close point
                            _currentBrushPath.AddPointAtLast(x, y);
                            _currentBrushPath.MakeRegularPath(5);
                        }
                        break;
                }
                _currentBrushPath = null;
            }

            base.MouseUp(x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            switch (this.EditMode)
            {
                case Samples.EditMode.Select:
                    {
                        if (_currentSelectedPath != null)
                        {
                            //find xdiff,ydiff
                            int xdiff = x - _lastMousePosX;
                            int ydiff = y - _lastMousePosY;
                            _lastMousePosX = x;
                            _lastMousePosY = y;
                            //move 
                            _currentSelectedPath.MoveBy(xdiff, ydiff);
                        }
                    }
                    break;
                case Samples.EditMode.Draw:
                    {
                        //find diff 
                        Vector newPoint = new Vector(x, y);
                        //find distance

                        _currentBrushPath.AddPointAtLast((int)newPoint.X, (int)newPoint.Y);
                        _latestMousePoint = new PixelFarm.Drawing.Point(x, y);
                        //
                        // currentBrushPath.MakeSmoothPath();
                    }
                    break;
            }
        }


        public override void MouseDown(int x, int y, bool isRightButton)
        {
            _lastMousePosX = x;
            _lastMousePosY = y;
            switch (this.EditMode)
            {
                case Samples.EditMode.Select:
                    {
                        //hit test find path object
                        HandleMouseDownOnSelectMode(x, y);
                    }
                    break;
                case Samples.EditMode.Draw:
                    {
                        _latestMousePoint = new PixelFarm.Drawing.Point(x, y);
                        _currentBrushPath = new MyBrushPath();
                        switch (BrushMode)
                        {
                            case SmoothBrushMode.SolidBrush:
                                _currentBrushPath.FillColor = Drawing.Color.Black;
                                _currentBrushPath.StrokeColor = Drawing.Color.Red;
                                break;
                            case SmoothBrushMode.EraseBrush:
                                _currentBrushPath.FillColor = Drawing.Color.White;
                                _currentBrushPath.StrokeColor = Drawing.Color.Transparent;
                                break;
                            case SmoothBrushMode.CutBrush:

                                break;
                        }
                        _currentBrushPath.BrushMode = this.BrushMode;
                        _myBrushPathList.Add(_currentBrushPath);
                        _currentBrushPath.AddPointAtFirst(x, y);
                    }
                    break;
            }

            base.MouseDown(x, y, isRightButton);
        }


        void HandleMouseDownOnSelectMode(int x, int y)
        {
            //hit test ...

            MyBrushPath selectedPath = null;
            int j = _myBrushPathList.Count;
            for (int i = _myBrushPathList.Count - 1; i >= 0; --i)
            {
                MyBrushPath mypath = _myBrushPathList[i];
                if (mypath.HitTest(x, y))
                {
                    //found 
                    //then check fill color
                    selectedPath = mypath;
                    break;
                }
            }

            if (selectedPath == _currentSelectedPath)
            {
                return;
            }

            if (_currentSelectedPath != null && selectedPath != _currentSelectedPath)
            {
                //clear prev
                _currentSelectedPath.FillColor = Drawing.Color.Black;
            }

            if (selectedPath != null)
            {
                selectedPath.FillColor = Drawing.Color.Red;
                _currentSelectedPath = selectedPath;
            }
        }
    }

}