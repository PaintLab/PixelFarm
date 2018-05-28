//BSD, 2014-2018, WinterDev 
//adapt from Paper.js

using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;
using Mini;
using PixelFarm.Drawing;
namespace PixelFarm.Agg.Samples
{
    [Info(OrderCode = "22")]
    [Info("SmoothBrush3")]
    public class SmoothBrush3 : DemoBase
    {
        PixelFarm.Drawing.Point latestMousePoint;
        List<MyBrushPath> myBrushPathList = new List<MyBrushPath>();

        MyBrushPath currentBrushPath;
        MyBrushPath currentSelectedPath;

        int lastMousePosX;
        int lastMousePosY;
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
            int j = myBrushPathList.Count;
            for (int n = 0; n < j; ++n)
            {
                var brushPath = myBrushPathList[n];
                if (brushPath.Vxs != null)
                {
                    switch (brushPath.BrushMode)
                    {
                        case SmoothBrushMode.CutBrush:
                            {
                            }
                            break;
                        default:
                            {
                                //TODO: review PixelCache here
                                p.FillColor = brushPath.FillColor;
                                p.Fill(brushPath.Vxs);

#if DEBUG
                                //if (brushPath.StrokeColor.alpha > 0)
                                //{
                                //    p.StrokeColor = Drawing.Color.Red;
                                //    p.Draw(brushPath.Vxs);
                                //}
#endif
                            }
                            break;
                    }
                }
                else
                {
                    //current drawing brush
                    var contPoints = brushPath.contPoints;
                    int pcount = contPoints.Count;
                    for (int i = 1; i < pcount; ++i)
                    {
                        var p0 = contPoints[i - 1];
                        var p1 = contPoints[i];
                        p.DrawLine(p0.x, p0.y, p1.x, p1.y);
                    }
                }
            }
        }


        public override void MouseUp(int x, int y)
        {
            if (currentSelectedPath != null)
            {
                this.currentSelectedPath.FillColor = Drawing.Color.Black;
            }
            this.currentSelectedPath = null;
            if (EditMode == Samples.EditMode.Select)
            {
                return;
            }

            if (currentBrushPath != null)
            {
                //1. close current path 
                switch (currentBrushPath.BrushMode)
                {
                    case SmoothBrushMode.CutBrush:
                        {
                            currentBrushPath.MakeRegularPath(5);
                            if (myBrushPathList.Count > 0)
                            {
                                //1. remove 
                                myBrushPathList.RemoveAt(myBrushPathList.Count - 1);
                                //

                                if (myBrushPathList.Count > 0)
                                {

                                    int j = myBrushPathList.Count - 1;
                                    for (int i = j; i >= 0; --i)
                                    {
                                        //cut each path
                                        var lastPath = myBrushPathList[i];
                                        //do path clip***
                                        List<VertexStore> paths = PixelFarm.Agg.VertexSource.VxsClipper.CombinePaths(
                                                new VertexStoreSnap(lastPath.Vxs),
                                                new VertexStoreSnap(currentBrushPath.Vxs), VertexSource.VxsClipperType.Difference,
                                                true);

                                        myBrushPathList.RemoveAt(i);

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
                                                myBrushPathList.Add(newBrushPath); //add last
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
                                                myBrushPathList.Insert(i, newBrushPath);
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
                            currentBrushPath.AddPointAtLast(x, y);
                            currentBrushPath.MakeRegularPath(5);
                        }
                        break;
                }
                currentBrushPath = null;
            }

            base.MouseUp(x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            switch (this.EditMode)
            {
                case Samples.EditMode.Select:
                    {
                        if (this.currentSelectedPath != null)
                        {
                            //find xdiff,ydiff
                            int xdiff = x - this.lastMousePosX;
                            int ydiff = y - this.lastMousePosY;
                            this.lastMousePosX = x;
                            this.lastMousePosY = y;
                            //move 
                            currentSelectedPath.MoveBy(xdiff, ydiff);
                        }
                    }
                    break;
                case Samples.EditMode.Draw:
                    {
                        //find diff 
                        Vector newPoint = new Vector(x, y);
                        //find distance

                        currentBrushPath.AddPointAtLast((int)newPoint.X, (int)newPoint.Y);
                        latestMousePoint = new PixelFarm.Drawing.Point(x, y);
                        //
                        // currentBrushPath.MakeSmoothPath();
                    }
                    break;
            }
        }


        public override void MouseDown(int x, int y, bool isRightButton)
        {
            this.lastMousePosX = x;
            this.lastMousePosY = y;
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
                        latestMousePoint = new PixelFarm.Drawing.Point(x, y);
                        currentBrushPath = new MyBrushPath();
                        switch (BrushMode)
                        {
                            case SmoothBrushMode.SolidBrush:
                                currentBrushPath.FillColor = Drawing.Color.Black;
                                currentBrushPath.StrokeColor = Drawing.Color.Red;
                                break;
                            case SmoothBrushMode.EraseBrush:
                                currentBrushPath.FillColor = Drawing.Color.White;
                                currentBrushPath.StrokeColor = Drawing.Color.Transparent;
                                break;
                            case SmoothBrushMode.CutBrush:

                                break;
                        }
                        currentBrushPath.BrushMode = this.BrushMode;
                        this.myBrushPathList.Add(currentBrushPath);
                        currentBrushPath.AddPointAtFirst(x, y);
                    }
                    break;
            }

            base.MouseDown(x, y, isRightButton);
        }


        void HandleMouseDownOnSelectMode(int x, int y)
        {
            //hit test ...

            MyBrushPath selectedPath = null;
            int j = this.myBrushPathList.Count;
            for (int i = this.myBrushPathList.Count - 1; i >= 0; --i)
            {
                MyBrushPath mypath = myBrushPathList[i];
                if (mypath.HitTest(x, y))
                {
                    //found 
                    //then check fill color
                    selectedPath = mypath;
                    break;
                }
            }

            if (selectedPath == this.currentSelectedPath)
            {
                return;
            }

            if (this.currentSelectedPath != null && selectedPath != this.currentSelectedPath)
            {
                //clear prev
                currentSelectedPath.FillColor = Drawing.Color.Black;
            }

            if (selectedPath != null)
            {
                selectedPath.FillColor = Drawing.Color.Red;
                this.currentSelectedPath = selectedPath;
            }
        }
    }

}