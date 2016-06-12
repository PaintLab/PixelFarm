using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg.Image;
using System.Text;
using System.Windows.Forms;
namespace Mini
{
    public partial class FormGdiTest : Form
    {
        //for test only
        bool _isMouseDown;
        Graphics _g;
        PixelFarm.Agg.Image.Point _latestMousePoint;
        PixelFarm.Agg.Samples.MyBrushPath _myBrushPath;
        GraphicsPath _latestBrushPathCache = null;
        List<System.Drawing.Point> _points = new List<System.Drawing.Point>();
        public FormGdiTest()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _g = this.CreateGraphics();
        }
        void UpdateOutput(Graphics g)
        {
            g.Clear(Color.White);
            DrawLines(g);
            DrawBrushPath(g);
        }
        void DrawLines(Graphics g)
        {
            if (_points.Count > 1)
            {
                g.DrawLines(Pens.Black, _points.ToArray());
            }
        }
        void DrawBrushPath(Graphics g)
        {
            if (_latestBrushPathCache != null)
            {
                g.FillPath(Brushes.Red, _latestBrushPathCache);
                return;
            }

            if (_myBrushPath.vxs != null)
            {
                //create new path 
                PixelFarm.Agg.VertexStore vxs = _myBrushPath.vxs;
                //render vertice in store
                int vcount = vxs.Count;
                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;
                //for (int i = 0; i < vcount; ++i)
                //{
                //    double x, y;
                //    PixelFarm.Agg.VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                //    switch (cmd)
                //    {
                //        case PixelFarm.Agg.VertexCmd.MoveTo:
                //            prevMoveToX = prevX = x;
                //            prevMoveToY = prevY = y;
                //            break;
                //        case PixelFarm.Agg.VertexCmd.LineTo:
                //            g.DrawLine(Pens.Red, (float)prevX, (float)prevY, (float)x, (float)y);
                //            prevX = x;
                //            prevY = y;
                //            break;
                //        case PixelFarm.Agg.VertexCmd.EndAndCloseFigure:
                //            g.DrawLine(Pens.Red, (float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                //            prevMoveToX = prevX = x;
                //            prevMoveToY = prevY = y;
                //            break;
                //        case PixelFarm.Agg.VertexCmd.EndFigure:
                //            break;
                //        case PixelFarm.Agg.VertexCmd.HasMore:
                //            break;
                //        case PixelFarm.Agg.VertexCmd.Stop:
                //            i = vcount + 1;//exit from loop
                //            break;
                //        default:
                //            break;
                //    }
                //}
                var brush_path = new GraphicsPath(FillMode.Winding);//*** winding for overlapped path
                for (int i = 0; i < vcount; ++i)
                {
                    double x, y;
                    PixelFarm.Agg.VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                    switch (cmd)
                    {
                        case PixelFarm.Agg.VertexCmd.MoveTo:
                            prevMoveToX = prevX = x;
                            prevMoveToY = prevY = y;
                            break;
                        case PixelFarm.Agg.VertexCmd.LineTo:
                            brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                            prevX = x;
                            prevY = y;
                            break;
                        case PixelFarm.Agg.VertexCmd.EndAndCloseFigure:
                            brush_path.AddLine((float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                            prevMoveToX = prevX = x;
                            prevMoveToY = prevY = y;
                            break;
                        case PixelFarm.Agg.VertexCmd.EndFigure:
                            break;
                        case PixelFarm.Agg.VertexCmd.HasMore:
                            break;
                        case PixelFarm.Agg.VertexCmd.Stop:
                            i = vcount + 1;//exit from loop
                            break;
                        default:
                            break;
                    }
                }

                _latestBrushPathCache = brush_path;
                g.FillPath(Brushes.Red, _latestBrushPathCache);
            }
            else
            {
                var contPoints = _myBrushPath.contPoints;
                int pcount = contPoints.Count;
                for (int i = 1; i < pcount; ++i)
                {
                    var p0 = contPoints[i - 1];
                    var p1 = contPoints[i];
                    g.DrawLine(Pens.Red, (float)p0.x, (float)p0.y, (float)p1.x, (float)p1.y);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //clear
            _latestBrushPathCache = null;
            _latestMousePoint = new PixelFarm.Agg.Image.Point(e.X, e.Y);
            _points.Clear();
            _myBrushPath = new PixelFarm.Agg.Samples.MyBrushPath();
            _points.Add(new System.Drawing.Point(e.X, e.Y));
            _myBrushPath.AddPointAtFirst(e.X, e.Y);
            _isMouseDown = true;
            //
            UpdateOutput(_g);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isMouseDown = false;
            _points.Add(new System.Drawing.Point(e.X, e.Y));
            _myBrushPath.AddPointAtLast(e.X, e.Y);
            _myBrushPath.MakeSmoothPath();
            UpdateOutput(_g);
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                int x = e.X;
                int y = e.Y;
                _points.Add(new System.Drawing.Point(x, y));
                //dragging
                //---------
                //find diff 

                Vector newPoint = new Vector(x, y);
                //find distance
                Vector oldPoint = new Vector(_latestMousePoint.x, _latestMousePoint.y);
                Vector delta = (newPoint - oldPoint) / 2; // 2,4 etc 
                                                          //midpoint
                Vector midPoint = (newPoint + oldPoint) / 2;
                delta = delta.NewLength(5);
                delta.Rotate(90);
                Vector newTopPoint = midPoint + delta;
                Vector newBottomPoint = midPoint - delta;
                //bottom point
                _myBrushPath.AddPointAtFirst((int)newBottomPoint.X, (int)newBottomPoint.Y);
                _myBrushPath.AddPointAtLast((int)newTopPoint.X, (int)newTopPoint.Y);
                _latestMousePoint = new PixelFarm.Agg.Image.Point(x, y);
                //---------

                UpdateOutput(_g);
            }
            base.OnMouseMove(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_points != null && _points.Count > 1)
            {
                UpdateOutput(e.Graphics);
            }
            base.OnPaint(e);
        }
    }
}
