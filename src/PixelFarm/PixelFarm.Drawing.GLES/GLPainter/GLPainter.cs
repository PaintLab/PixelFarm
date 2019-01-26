//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

namespace PixelFarm.DrawingGL
{
    public sealed partial class GLPainter : Painter
    {
        GLPainterContext _pcx;
        SmoothingMode _smoothingMode; //smoothing mode of this  painter
        RenderSurfaceOrientation _orientation = RenderSurfaceOrientation.LeftTop;

        int _width;
        int _height;

        PathRenderVxBuilder _pathRenderVxBuilder;
        RequestFont _requestFont;
        ITextPrinter _textPrinter;
        RenderQuality _renderQuality;

        TargetBuffer _targetBuffer;

        public GLPainter()
        {

            CurrentFont = new RequestFont("tahoma", 14);
            UseVertexBufferObjectForRenderVx = true;
            //tools
            _pathRenderVxBuilder = PathRenderVxBuilder.CreateNew();
            _defaultBrush = _currentBrush = new SolidBrush(Color.Black); //default brush

        }
    
        public GLPainterContext PainterContext => _pcx;
        public void BindToPainterContext(GLPainterContext pcx)
        {
            if (_pcx == pcx)
            {
                return;
            }
            //
            _pcx = pcx;
            _width = pcx.CanvasWidth;
            _height = pcx.CanvasHeight;
            _clipBox = new RectInt(0, 0, _width, _height);
        }

        public override int Width => _width;
        public override int Height => _height;
        public override float OriginX => _pcx.OriginX;
        public override float OriginY => _pcx.OriginY;

        public override void Render(RenderVx renderVx)
        {
            throw new NotImplementedException();
        }

        public void DetachCurrentShader() => _pcx.DetachCurrentShader();

        public override RenderSurfaceOrientation Orientation
        {
            get => _orientation;
            set => _orientation = value;
        }

        public bool UseVertexBufferObjectForRenderVx { get; set; }

        public override void SetOrigin(float ox, float oy)
        {
            _pcx.SetCanvasOrigin((int)ox, (int)oy);
        }
        //
        public GLPainterContext Canvas => _pcx;
        //
        public override RenderQuality RenderQuality
        {
            get => _renderQuality;
            set => _renderQuality = value;
        }


        public override SmoothingMode SmoothingMode
        {
            get => _smoothingMode;
            set
            {
                switch (_smoothingMode = value)
                {
                    case SmoothingMode.HighQuality:
                    case SmoothingMode.AntiAlias:
                        _pcx.SmoothMode = SmoothMode.Smooth;
                        break;
                    default:
                        _pcx.SmoothMode = SmoothMode.No;
                        break;
                }

            }
        }


        public override bool UseSubPixelLcdEffect
        {
            get => _pcx.SmoothMode == SmoothMode.Smooth;
            set => _pcx.SmoothMode = value ? SmoothMode.Smooth : SmoothMode.No;
        }

        public override void Clear(Color color)
        {
            _pcx.Clear(color);
        }

        public override TargetBuffer TargetBuffer
        {
            get => _targetBuffer;
            set
            {
                if (_targetBuffer == value) return;
                //change target buffer
                _targetBuffer = value;

            }
        }


        //-----------------------------------------------------------------------------------------------------------------
        public override RenderVx CreateRenderVx(VertexStore vxs)
        {
            //store internal gfx path inside render vx  
            return _pathRenderVxBuilder.CreatePathRenderVx(vxs);
        }
        public RenderVx CreatePolygonRenderVx(float[] xycoords)
        {
            //store internal gfx path inside render vx

            return new PathRenderVx(new Figure(xycoords));
        }

        struct PathRenderVxBuilder
        {
            //helper struct

            List<float> _xylist;
            public static PathRenderVxBuilder CreateNew()
            {
                PathRenderVxBuilder builder = new PathRenderVxBuilder();
                builder._xylist = new List<float>();
                return builder;
            }

            public PathRenderVx CreatePathRenderVx(VertexStore vxs)
            {

                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;

                _xylist.Clear();
                //TODO: reivew here 
                //about how to reuse this list  
                //result...

                MultiFigures figures = new MultiFigures();
                int index = 0;
                VertexCmd cmd;

                double x, y;
                while ((cmd = vxs.GetVertex(index++, out x, out y)) != VertexCmd.NoMore)
                {
                    switch (cmd)
                    {
                        case PixelFarm.CpuBlit.VertexCmd.MoveTo:

                            prevMoveToX = prevX = x;
                            prevMoveToY = prevY = y;
                            _xylist.Add((float)x);
                            _xylist.Add((float)y);
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.LineTo:
                            _xylist.Add((float)x);
                            _xylist.Add((float)y);
                            prevX = x;
                            prevY = y;
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.Close:
                            {
                                //from current point 
                                _xylist.Add((float)prevMoveToX);
                                _xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                //-----------
                                Figure newfig = new Figure(_xylist.ToArray());
                                newfig.IsClosedFigure = true;
                                figures.AddFigure(newfig);
                                //-----------
                                _xylist.Clear(); //clear temp list

                            }
                            break;
                        case VertexCmd.CloseAndEndFigure:
                            {
                                //from current point 
                                _xylist.Add((float)prevMoveToX);
                                _xylist.Add((float)prevMoveToY);
                                prevX = prevMoveToX;
                                prevY = prevMoveToY;
                                // 
                                Figure newfig = new Figure(_xylist.ToArray());
                                newfig.IsClosedFigure = true;
                                figures.AddFigure(newfig);
                                //-----------
                                _xylist.Clear();//clear temp list
                            }
                            break;
                        case PixelFarm.CpuBlit.VertexCmd.NoMore:
                            goto EXIT_LOOP;
                        default:
                            throw new System.NotSupportedException();
                    }
                }
                EXIT_LOOP:

                if (figures.FigureCount == 0)
                {
                    Figure newfig = new Figure(_xylist.ToArray());
                    newfig.IsClosedFigure = false;

                    return new PathRenderVx(newfig);
                }
                else if (_xylist.Count > 1)
                {
                    _xylist.Add((float)prevMoveToX);
                    _xylist.Add((float)prevMoveToY);
                    prevX = prevMoveToX;
                    prevY = prevMoveToY;
                    //
                    Figure newfig = new Figure(_xylist.ToArray());
                    newfig.IsClosedFigure = true; //? 
                    figures.AddFigure(newfig);
                }
                return new PathRenderVx(figures);
            }

        }
    }
}