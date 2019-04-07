//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

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
        PathRenderVxBuilder2 _pathRenderVxBuilder2;

        RequestFont _requestFont;
        ITextPrinter _textPrinter;
        RenderQuality _renderQuality;

        TargetBuffer _targetBuffer;

        public GLPainter()
        {

            //TODO: config default font
#if __MOBILE__
            //CurrentFont = new RequestFont("Droid Sans", 24);
            CurrentFont = new RequestFont("tahoma", 24);
#else
            CurrentFont = new RequestFont("tahoma", 14);
#endif             
            UseVertexBufferObjectForRenderVx = true;
            //tools
            _pathRenderVxBuilder = new PathRenderVxBuilder();
            _defaultBrush = _currentBrush = new SolidBrush(Color.Black); //default brush 
            _pathRenderVxBuilder2 = new PathRenderVxBuilder2();
        }


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
        public void UpdatePainterContext()
        {
            _width = _pcx.CanvasWidth;
            _height = _pcx.CanvasHeight;
            _clipBox = new RectInt(0, 0, _width, _height);
        }
        public override ICoordTransformer CoordTransformer
        {
            get => _pcx.CoordTransformer;
            set => _pcx.CoordTransformer = value;
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
        public GLPainterContext PainterContext => _pcx;
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

        class PathRenderVxBuilder
        {
            //helper struct

            List<float> _xylist = new List<float>();
            List<Figure> _figs = new List<Figure>();
            public PathRenderVxBuilder()
            {
            }


            public PathRenderVx CreatePathRenderVx(VertexStore vxs)
            {

                double prevX = 0;
                double prevY = 0;
                double prevMoveToX = 0;
                double prevMoveToY = 0;

                _xylist.Clear();
                _figs.Clear();
                //TODO: reivew here 
                //about how to reuse this list  
                //result...


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

                                _figs.Add(newfig);
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
                                _figs.Add(newfig);
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

                if (_figs.Count == 0)
                {
                    Figure newfig = new Figure(_xylist.ToArray());
                    newfig.IsClosedFigure = false;
                    return new PathRenderVx(newfig);
                }
                //
                if (_xylist.Count > 1)
                {
                    _xylist.Add((float)prevMoveToX);
                    _xylist.Add((float)prevMoveToY);
                    prevX = prevMoveToX;
                    prevY = prevMoveToY;
                    //
                    Figure newfig = new Figure(_xylist.ToArray());
                    newfig.IsClosedFigure = true; //? 
                    _figs.Add(newfig);
                }

                if (_figs.Count == 1)
                {
                    Figure fig = _figs[0];
                    _figs.Clear();
                    return new PathRenderVx(fig);
                }
                else
                {
                    MultiFigures multiFig = new MultiFigures(_figs.ToArray());
                    _figs.Clear();
                    return new PathRenderVx(multiFig);
                }
            }
        }


        class PathRenderVxBuilder2
        {
            ExtMsdfGen.MsdfGen3 _msdfGen;
            public PathRenderVxBuilder2()
            {
                _msdfGen = new ExtMsdfGen.MsdfGen3();
                _msdfGen.MsdfGenParams = new ExtMsdfGen.MsdfGenParams();
            }
            public TextureRenderVx CreateRenderVx(VertexStore vxs)
            {
#if DEBUG
               //_msdfGen.dbugWriteMsdfTexture = true;
#endif
                ExtMsdfGen.SpriteTextureMapData<MemBitmap> spriteTextureMap = _msdfGen.GenerateMsdfTexture(vxs);
                TextureRenderVx textureRenderVx = new TextureRenderVx(spriteTextureMap);
                return textureRenderVx;
            }
        }
    }
}